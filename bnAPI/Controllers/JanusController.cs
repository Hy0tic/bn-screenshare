using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Web;
using bnAPI.Hubs;
using Newtonsoft.Json;
using JsonElement = System.Text.Json.JsonElement;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace bnAPI.Controllers;

public class JanusController : Controller
{
    private readonly string _janusServerUrl = "http://localhost:8088/janus";

    private readonly HttpClient _httpClient;

    public JanusController()
    {
        _httpClient = new HttpClient();
    }

    [HttpPost("/initialize")]
    public async Task<IActionResult> InitializeJanusSession()
    {
        using var client = new HttpClient();
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, _janusServerUrl);
    
        var transactionId = Guid.NewGuid().ToString();  // Generating a unique transaction ID

        requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
        {
            janus = "create",
            transaction = transactionId
        }), Encoding.UTF8, "application/json");  // Specified encoding and content type
    
        var response = await client.SendAsync(requestMessage);
        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);
            
            if (jsonResponse.TryGetProperty("data", out var data) &&
                data.TryGetProperty("id", out var sessionId))
            {
                return Ok(sessionId.ToString());
            }
            else
            {
                // This will give you a more detailed reason for the failure
                return BadRequest($"Failed to initialize Janus session. Response: {responseBody}");
            }
        }
        else
        {
            // This will also provide more insights into what went wrong.
            var errorResponse = await response.Content.ReadAsStringAsync();
            return BadRequest($"HTTP Call failed. Response: {errorResponse}");
        }
    }
    
    [HttpPost("/attachPlugin")]
    public async Task<IActionResult> AttachPlugin(string sessionId)
    {
        using var client = new HttpClient();
        var transactionId = GenerateTransactionId();
        var requestUrl = $"{_janusServerUrl}/{sessionId}";
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl);
        requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
        {
            janus = "attach",
            plugin = "janus.plugin.videoroom",
            transaction = transactionId
        }), Encoding.UTF8, "application/json");
        var response = await client.SendAsync(requestMessage);
        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();

            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);

            if (jsonResponse.TryGetProperty("data", out var data) &&
                data.TryGetProperty("id", out var pluginId))
            {
                return Ok(pluginId.ToString());
            }
            else
            {
                return BadRequest("Failed to attach plugin. Unexpected response from Janus.");
            }
        }
        else
        {
            return BadRequest($"Error when communicating with Janus: {response.ReasonPhrase}");
        }
    }

    private static string GenerateTransactionId()
    {
        return Guid.NewGuid().ToString();
    }
    
    [HttpPost("/SendOfferToJanus")]
    public async Task<IActionResult> SendOfferToJanus([FromBody] JanusOffer offerDetails)
    {
        var offer = offerDetails.Offer;
        var sessionId = offerDetails.SessionId;
        var pluginId = offerDetails.PluginId;
        
        var transactionId = GenerateTransactionId();
        var requestPayload = new
        {
            janus = "message",
            transaction = transactionId, 
            body = new
            {
                // request = "configure",
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
        var response = await _httpClient.PostAsync($"{_janusServerUrl}/{sessionId}/{pluginId}", content); // sessionId and pluginId are values you would've gotten when initially setting up the session and attaching a plugin

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var janusResponse = JsonSerializer.Deserialize<JanusHub.JanusResponse>(jsonResponse);
            Console.WriteLine(janusResponse.Jsep.Sdp);
            return Ok(janusResponse.Jsep.Sdp);
        }

        return Ok();
    }
    
    [HttpPost("/CreateVideoRoom")]
    public async Task<IActionResult> CreateVideoRoom(string sessionId, string pluginId)
    {
        var roomDescription = "the cave";
        var rnd = new Random();
        var roomId = rnd.Next();
        
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
            return BadRequest($"Failed to create room: {jsonResponse["error"]["reason"]}");
        }
        Console.WriteLine("Successfully created the video room!");
        return Ok(responseContent);
    }

    [HttpPost("/JoinRoomAsPublisher")]
    public async Task<IActionResult> JoinVideoRoomAsPublisher(string sessionId, string pluginId, int roomId, string displayName)
    {
        // Construct the body of the request to Janus
        var janusRequestBody = new
        {
            janus = "message",
            body = new
            {
                request = "join",
                ptype = "publisher",
                room = roomId,
                display = displayName
            },
            transaction = GenerateTransactionId() // This should be a unique ID for each request
        };

        var content = new StringContent(JsonSerializer.Serialize(janusRequestBody), Encoding.UTF8, "application/json");

        // Send the request to the Janus server
        var response = await _httpClient.PostAsync($"{_janusServerUrl}/{sessionId}/{pluginId}", content);

        if (!response.IsSuccessStatusCode) return BadRequest(response);
        var responseBody = await response.Content.ReadAsStringAsync();
        Console.Write(responseBody);
        
        return Ok(response); // Return the Janus server response to the client

    }
    
    [HttpPost("/SendICECandidatesToJanus")]
    public async Task<IActionResult> SendICECandidatesToJanus(string ICEcandidates)
    {
        
        return Ok();
    }
    public class JanusOffer
    {
        public string Offer { get; set; }
        public string SessionId { get; set; }
        public string PluginId { get; set; }
    }
}