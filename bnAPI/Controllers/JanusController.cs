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
    private const string JanusServerUrl = "http://localhost:8088/janus";

    private readonly HttpClient _httpClient;

    public JanusController()
    {
        _httpClient = new HttpClient();
    }

    [HttpPost("/initialize")]
    public async Task<IActionResult> InitializeJanusSession()
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, JanusServerUrl);
        var transactionId = Guid.NewGuid().ToString();  // Generating a unique transaction ID
        requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
        {
            janus = "create",
            transaction = transactionId
        }), Encoding.UTF8, "application/json");  // Specified encoding and content type
        var response = await _httpClient.SendAsync(requestMessage);
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
        var transactionId = GenerateTransactionId();
        var requestUrl = $"{JanusServerUrl}/{sessionId}";
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl);
        requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
        {
            janus = "attach",
            plugin = "janus.plugin.streaming",
            transaction = transactionId
        }), Encoding.UTF8, "application/json");
        var response = await _httpClient.SendAsync(requestMessage);
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
                // request = '',
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
        var response = await _httpClient.PostAsync($"{JanusServerUrl}/{sessionId}/{pluginId}", content); // sessionId and pluginId are values you would've gotten when initially setting up the session and attaching a plugin

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var janusResponse = JsonSerializer.Deserialize<JanusHub.JanusResponse>(jsonResponse);
            Console.WriteLine(janusResponse.Jsep.Sdp);
            return Ok(janusResponse.Jsep.Sdp);
        }

        return Ok();
    }
    
    [HttpPost("create-stream")]
    public async Task<IActionResult> CreateStream()
    {
        // Create the session with Janus first
        var sessionId = await CreateJanusSession();

        // Attach to the streaming plugin
        var handleId = await AttachToStreamingPlugin(sessionId);

        // Send the create request to the streaming plugin
        var config = new StreamConfig();
        var response = await SendCreateStreamRequest(sessionId, handleId, config);
        return Ok(response);
    }
    private async Task<string> CreateJanusSession()
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, JanusServerUrl);
        var transactionId = Guid.NewGuid().ToString();  // Generating a unique transaction ID
        requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
        {
            janus = "create",
            transaction = transactionId
        }), Encoding.UTF8, "application/json");  // Specified encoding and content type
        var response = await _httpClient.SendAsync(requestMessage);
        
        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);
            
            if (jsonResponse.TryGetProperty("data", out var data) &&
                data.TryGetProperty("id", out var sessionId))
            {
                return sessionId.ToString();
            }
            else
            {
                throw new Exception();
            }
        }
        else
        {
            throw new Exception();
        }
    }
    
    private async Task<string> AttachToStreamingPlugin(string sessionId)
    {
        var transactionId = GenerateTransactionId();
        var requestUrl = $"{JanusServerUrl}/{sessionId}";
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl);
        
        requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
        {
            janus = "attach",
            plugin = "janus.plugin.streaming",
            transaction = transactionId
        }), Encoding.UTF8, "application/json");
        
        var response = await _httpClient.SendAsync(requestMessage);
        
        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();

            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);

            if (jsonResponse.TryGetProperty("data", out var data) &&
                data.TryGetProperty("id", out var pluginId))
            {
                return pluginId.ToString();
            }
            else
            {
                throw new Exception();
            }
        }
        else
        {
            throw new Exception();
        }
    }
    
    private async Task<object> SendCreateStreamRequest(string sessionId, string handleId, StreamConfig config)
    {
        var requestUrl = $"{JanusServerUrl}/{sessionId}/{handleId}";
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl);
        var createStreamRequest = new
        {
            janus = "message",
            transaction = Guid.NewGuid().ToString(), // Generate a new transaction ID
            body = new
            {
                request = "list",
                // type = config.Type,
                // description = config.Description,
                // audio = config.IsAudioActive,
                // video = config.IsVideoActive,
                // audioport = 5002,
                // audioopt = 111
                // Add more fields based on your StreamConfig
            },
            session_id = sessionId,
            handle_id = handleId
        };

        var jsonRequest = JsonSerializer.Serialize(createStreamRequest);
        requestMessage.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        

        // Send the request to the Janus server
        var response = await _httpClient.SendAsync(requestMessage);
        if (!response.IsSuccessStatusCode)
        {
            return null; // or handle the error as appropriate
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<object>(jsonResponse); // Deserialize to a dynamic object or a specific class

        return responseObject;
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
    
    public class StreamConfig
    {
        public string Description { get; set; } = "Test Stream";
        public bool IsAudioActive { get; set; } = true;
        public bool IsVideoActive { get; set; } = true;
        public string Type { get; set; } = "rtp";
        // Add more properties as needed for your stream configuration
    }

}