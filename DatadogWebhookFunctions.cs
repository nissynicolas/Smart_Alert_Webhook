using System.Net;
using System.Text.Json;
using DatadogWebhookFunction.Models;
using DatadogWebhookFunction.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

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

            // Read and deserialize the request body
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            
            if (string.IsNullOrEmpty(requestBody))
            {
                _logger.LogWarning("Received empty webhook payload");
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(FunctionResponse<string>.ErrorResponse("Empty payload"));
                return badResponse;
            }

            // Parse the webhook payload
            var payload = JsonSerializer.Deserialize<DatadogWebhookPayload>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (payload == null)
            {
                _logger.LogWarning("Failed to deserialize webhook payload");
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(FunctionResponse<string>.ErrorResponse("Invalid payload format"));
                return badResponse;
            }

            _logger.LogInformation("Processing Datadog webhook: AlertId={AlertId}, Type={AlertType}", 
                payload.AlertId, payload.AlertType);

            // Log the raw payload for debugging (be careful with sensitive data in production)
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Webhook payload: {Payload}", requestBody);
            }

            // Process the webhook
            var result = await _webhookService.ProcessDatadogWebhookAsync(payload);

            // Return success response
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(FunctionResponse<WebhookProcessingResult>.SuccessResponse(result));
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
