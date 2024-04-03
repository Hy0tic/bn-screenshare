using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace bnAPI.Hubs;

public class JanusHub : Hub
{
    private readonly string _janusServerUrl = "http://localhost:8088/janus";
    private readonly HttpClient _httpClient;
    
    public JanusHub()
    {
        _httpClient = new HttpClient();
    }
    
    public async Task SendICECandidateAsync(string sessionId, string handleId, JObject iceCandidate)
    {
        var candidate = iceCandidate["candidate"].ToString();
        var sdpMid = iceCandidate["sdpMid"].ToString();
        var sdpMLineIndex = iceCandidate["sdpMLineIndex"].ToObject<int>();
        var usernameFragment = iceCandidate["usernameFragment"].ToString();

        var payload = new
        {
            janus = "trickle",
            transaction = Guid.NewGuid().ToString(),
            candidate = new
            {
                candidate = candidate,
                sdpMid = sdpMid,
                sdpMLineIndex = sdpMLineIndex,
                usernameFragment = usernameFragment
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json");
        await _httpClient.PostAsync($"{_janusServerUrl}/{sessionId}/{handleId}", content);

        // Ideally, you'd want to handle the HTTP response, perhaps logging any potential errors.
    }

    public async Task CreateVideoRoom(string sessionId, string pluginId)
    {
        var roomDescription = "the cave";
        var roomId = "sketch";
        
        // Construct the request payload
        var requestPayload = new
        {
            janus = "message",
            transaction = Guid.NewGuid().ToString(),
            body = new
            {
                request = "create",
                room = roomId,
                description = roomDescription,
                publishers = 6  // For example, allows 6 publishers.
            }
        };
        
        // Serialize the payload to JSON
        var content = new StringContent(
            JsonConvert.SerializeObject(requestPayload),
            Encoding.UTF8,
            "application/json");
        
        var response = await _httpClient.PostAsync($"{_janusServerUrl}/{sessionId}/{pluginId}", content);
        // Read the response content
        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonResponse = JObject.Parse(responseContent);

        if (jsonResponse["janus"].ToString() != "success")
        {
            Console.WriteLine($"Failed to create room: {jsonResponse["error"]["reason"]}");
        }
        else
        {
            Console.WriteLine("Successfully created the video room!");
        }
    }
    
    public async Task SendOfferToJanus(string offer, string sessionId, string pluginId)
    {
        // TODO: Forward the offer to the Janus server using Janus API.
        // This can be done via a REST API call, WebSocket, or any other method Janus supports.

        var janusAnswer = await ForwardOfferToJanusAndGetAnswer(offer, sessionId, pluginId);

        // Once you have the answer from Janus, send it back to the client.
        await Clients.Caller.SendAsync("ReceiveAnswerFromJanus", janusAnswer);
    }
    
    private async Task<string> ForwardOfferToJanusAndGetAnswer(string offer, string sessionId, string pluginId)
    {
        var requestPayload = new
        {
            janus = "message",
            // These are placeholders; adjust the body as required for your Janus configuration
            transaction = "some-unique-transaction-id", // Usually a random string
            body = new
            {
                request = "configure",
                audio = true,
                video = true
            },
            jsep = new
            {
                type = "offer",
                sdp = offer
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(requestPayload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{_janusServerUrl}/janus/{sessionId}/{pluginId}", content); // sessionId and pluginId are values you would've gotten when initially setting up the session and attaching a plugin

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var janusResponse = JsonSerializer.Deserialize<JanusResponse>(jsonResponse);
            
            return janusResponse?.Jsep?.Sdp;
        }
        // Handle error
            throw new HttpRequestException($"Failed to forward offer to Janus. Status code: {response.StatusCode}");
    }
    
    public class JanusResponse
    {
        public string Janus { get; set; }
        public string Transaction { get; set; }
        public JanusJsep Jsep { get; set; }
    }

    public class JanusJsep
    {
        public string Type { get; set; }
        public string Sdp { get; set; }
    }
    

}