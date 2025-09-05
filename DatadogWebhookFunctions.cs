using DatadogWebhookFunction.Models;
using DatadogWebhookFunction.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using OpenAI;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace DatadogWebhookFunction;

/// <summary>
/// Azure Functions for handling Datadog webhooks
/// </summary>
public class DatadogWebhookFunctions
{
    private readonly ILogger<DatadogWebhookFunctions> _logger;
    private readonly IWebhookProcessingService _webhookService;

    public DatadogWebhookFunctions(ILogger<DatadogWebhookFunctions> logger, IWebhookProcessingService webhookService)
    {
        _logger = logger;
        _webhookService = webhookService;
    }

    /// <summary>
    /// Main Datadog webhook function
    /// </summary>
    [Function("DatadogWebhook")]
    public async Task<HttpResponseData> DatadogWebhook(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "datadog/webhook")] HttpRequestData req)
    {
        try
        {
            _logger.LogInformation("Received Datadog webhook request from {RemoteIp}",
                req.Headers.Contains("X-Forwarded-For") ? req.Headers.GetValues("X-Forwarded-For").FirstOrDefault() : "Unknown");

            // Read the request body
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            using JsonDocument doc = JsonDocument.Parse(requestBody);

            // 🔹 Extract only "alert" field (alert title)
            var title = doc.RootElement.GetProperty("alert").GetString();

            string prompt = string.IsNullOrWhiteSpace(title)
                    ? @"You are an assistant that retrieves Confluence documentation.
                Given an error message, your task is to search and return only the most relevant Confluence pages that contain Root Cause Analysis (RCA), root cause explanations, or resolution steps directly related to the error.

                Focus specifically on:
                - RCA documents
                - Postmortems
                - Resolution or guidance steps
                - Troubleshooting guides

                Do not include unrelated pages.
                If no RCA or resolution steps are available, clearly state that no relevant Confluence pages were found."
                    : $@"You are an assistant that retrieves Confluence documentation.
                Given the error message: ""{title}"", search and return only the most relevant Confluence pages that contain Root Cause Analysis (RCA), root cause explanations, or resolution steps directly related to this error.

                Focus specifically on:
                - RCA documents
                - Postmortems
                - Resolution or guidance steps
                - Troubleshooting guides

                Do not include unrelated pages.
                If no RCA or resolution steps are available, clearly state that no relevant Confluence pages were found.";

            if (string.IsNullOrEmpty(requestBody))
            {
                _logger.LogWarning("Received empty webhook payload");
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(FunctionResponse<string>.ErrorResponse("Empty payload"));
                return badResponse;
            }

            IChatClient client =
                new ChatClientBuilder(
                    new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
                    .GetChatClient("gpt-4.1")
                    .AsIChatClient())
                .UseFunctionInvocation()
                .Build();

            var mcpSseEndpoint = new Uri("https://jira-server-fthpf3hqa5bpeefx.eastus2-01.azurewebsites.net/sse");

            IMcpClient mcpClient = await McpClientFactory.CreateAsync(
                new SseClientTransport(new SseClientTransportOptions
                {
                    Endpoint = mcpSseEndpoint
                })
            );

            // List available tools from MCP (optional, if you still want to use them)
            Console.WriteLine("Available tools:");
            IList<McpClientTool> tools = await mcpClient.ListToolsAsync();
            foreach (McpClientTool tool in tools)
            {
                Console.WriteLine($"{tool}");
            }
            Console.WriteLine();

            // Prepare conversation history
            List<ChatMessage> messages = new List<ChatMessage>
            {
                new(ChatRole.User, prompt)
            };

            var aiTools = tools.Cast<AITool>().ToList();

            StringBuilder fullResponse = new();

            await foreach (ChatResponseUpdate update in client
                .GetStreamingResponseAsync(messages, new() { Tools = aiTools }))
            {
                Console.Write(update);
                fullResponse.Append(update);
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync(fullResponse.ToString(), Encoding.UTF8);

            return response;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse webhook JSON payload");
            var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await errorResponse.WriteAsJsonAsync(FunctionResponse<string>.ErrorResponse("Invalid JSON format"));
            return errorResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Datadog webhook");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(FunctionResponse<string>.ErrorResponse("Internal server error"));
            return errorResponse;
        }
    }

    /// <summary>
    /// Datadog event webhook function
    /// </summary>
    [Function("DatadogEventWebhook")]
    public async Task<HttpResponseData> DatadogEventWebhook(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "datadog/webhook/events")] HttpRequestData req)
    {
        try
        {
            _logger.LogInformation("Received Datadog event webhook");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var payload = JsonSerializer.Deserialize<DatadogWebhookPayload>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (payload == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(FunctionResponse<string>.ErrorResponse("Invalid payload"));
                return badResponse;
            }

            _logger.LogInformation("Processing Datadog event: EventType={EventType}, Title={EventTitle}",
                payload.EventType, payload.EventTitle);

            var result = await _webhookService.ProcessDatadogWebhookAsync(payload);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(FunctionResponse<WebhookProcessingResult>.SuccessResponse(result, "Event webhook processed successfully"));
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Datadog event webhook");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(FunctionResponse<string>.ErrorResponse("Failed to process event webhook"));
            return errorResponse;
        }
    }

    /// <summary>
    /// Test function to verify the webhook receiver is working
    /// </summary>
    [Function("TestWebhook")]
    public async Task<HttpResponseData> TestWebhook(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", "get", Route = "datadog/test")] HttpRequestData req)
    {
        _logger.LogInformation("Test webhook function called via {Method}", req.Method);

        var testResult = new
        {
            Message = "🚀 Datadog Webhook Function is working!",
            Timestamp = DateTime.UtcNow,
            FunctionApp = "Azure Functions",
            Version = "1.0.0",
            Environment = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT") ?? "Development",
            Endpoints = new[]
            {
                "POST /api/datadog/webhook",
                "POST /api/datadog/webhook/events",
                "GET|POST /api/datadog/test",
                "GET /api/health"
            }
        };

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(FunctionResponse<object>.SuccessResponse(testResult, "Test successful"));
        return response;
    }

    /// <summary>
    /// Health check function
    /// </summary>
    [Function("HealthCheck")]
    public async Task<HttpResponseData> HealthCheck(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequestData req)
    {
        _logger.LogDebug("Health check requested");

        var healthData = new
        {
            Status = "Healthy",
            Service = "Datadog Webhook Function",
            Version = "1.0.0",
            Timestamp = DateTime.UtcNow,
            Environment = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT") ?? "Development",
            FunctionApp = Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME") ?? "Local",
            Region = Environment.GetEnvironmentVariable("REGION_NAME") ?? "Unknown"
        };

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(FunctionResponse<object>.SuccessResponse(healthData, "Health check passed"));
        return response;
    }

    /// <summary>
    /// Get function information and available endpoints
    /// </summary>
    [Function("GetInfo")]
    public async Task<HttpResponseData> GetInfo(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "info")] HttpRequestData req)
    {
        var info = new
        {
            Service = "Datadog Webhook Receiver",
            Type = "Azure Functions",
            Version = "1.0.0",
            Status = "Running",
            Endpoints = new
            {
                Webhooks = new[]
                {
                    "POST /api/datadog/webhook - Main webhook endpoint",
                    "POST /api/datadog/webhook/events - Event-specific webhook",
                    "POST|GET /api/datadog/test - Test endpoint"
                },
                Utility = new[]
                {
                    "GET /api/health - Health check",
                    "GET /api/info - This endpoint"
                }
            },
            Timestamp = DateTime.UtcNow
        };

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(info);
        return response;
    }
}
