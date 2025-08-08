# 🚀 **How to Use Your Datadog Webhook Functions**

## ✅ **Build Status: SUCCESS**
Your Azure Functions project has been successfully built and is ready to run!

---

## 📡 **Available Webhook Endpoints**

### **1. 🎯 Main Datadog Webhook** 
**URL**: `POST /api/datadog/webhook`  
**Purpose**: Primary endpoint for receiving Datadog alerts  
**Content-Type**: `application/json`

### **2. 📊 Event Webhook**
**URL**: `POST /api/datadog/webhook/events`  
**Purpose**: Specialized endpoint for Datadog events  
**Content-Type**: `application/json`

### **3. 🧪 Test Endpoint**
**URL**: `GET|POST /api/datadog/test`  
**Purpose**: Test that your function is working  
**Content-Type**: None required

### **4. ❤️ Health Check**
**URL**: `GET /api/health`  
**Purpose**: Monitor function health  
**Content-Type**: None required

### **5. ℹ️ Function Info**
**URL**: `GET /api/info`  
**Purpose**: Get function details and endpoint list  
**Content-Type**: None required

---

## 🧪 **Testing Your Functions**

### **Option 1: PowerShell Testing (Recommended)**

```powershell
# Test the function locally (after running func start)
$testUrl = "http://localhost:7071/api/datadog/test"

# Simple GET test
Invoke-RestMethod -Uri $testUrl -Method GET

# Test main webhook with sample Datadog payload
$webhookUrl = "http://localhost:7071/api/datadog/webhook"
$payload = @{
    alert_id = "test-123"
    title = "High CPU Usage Alert"
    body = "CPU usage has exceeded 90% threshold"
    alert_type = "error"
    alert_transition = "Triggered"
    hostname = "web-server-01"
    priority = "high"
    tags = @("environment:production", "service:web", "team:platform")
} | ConvertTo-Json

Invoke-RestMethod -Uri $webhookUrl -Method POST -Body $payload -ContentType "application/json"
```

### **Option 2: cURL Testing**

```bash
# Test endpoint
curl -X GET http://localhost:7071/api/datadog/test

# Main webhook test
curl -X POST http://localhost:7071/api/datadog/webhook \
  -H "Content-Type: application/json" \
  -d '{
    "alert_id": "test-456",
    "title": "Database Connection Alert",
    "body": "Database connection pool exhausted",
    "alert_type": "error",
    "alert_transition": "Triggered",
    "hostname": "db-server-02",
    "priority": "critical",
    "tags": ["environment:production", "service:database"]
  }'

# Health check
curl -X GET http://localhost:7071/api/health
```

### **Option 3: Postman Collection**

**Create a new collection with these requests:**

1. **Test Function** - `GET http://localhost:7071/api/datadog/test`
2. **Health Check** - `GET http://localhost:7071/api/health`
3. **Main Webhook** - `POST http://localhost:7071/api/datadog/webhook`
   ```json
   {
     "alert_id": "postman-test-789",
     "title": "Memory Usage Critical",
     "body": "System memory usage is at 95%",
     "alert_type": "error", 
     "alert_transition": "Triggered",
     "hostname": "app-server-03",
     "priority": "high",
     "tags": ["environment:production", "service:app", "type:memory"]
   }
   ```

---

## 🎯 **Datadog Webhook Payload Format**

Your functions accept the standard Datadog webhook format:

```json
{
  "alert_id": "12345678",
  "title": "Alert Title",
  "body": "Detailed alert description",
  "alert_type": "error|warning|info|success",
  "alert_transition": "Triggered|Recovered|No Data|Warn",
  "hostname": "server-name",
  "priority": "low|normal|high|critical",
  "tags": ["tag1", "tag2", "tag3"]
}
```

### **Alert Types with Emoji Logging:**
- ❌ **error** - Critical errors requiring immediate attention
- ⚠️ **warning** - Warning conditions that need monitoring  
- ℹ️ **info** - Informational alerts and notifications
- ✅ **success** - Success notifications and confirmations

### **Alert Transitions:**
- 🚨 **Triggered** - Alert condition has been triggered
- ✅ **Recovered** - Alert condition has recovered
- 📊 **No Data** - No data received for the monitored metric
- ⚠️ **Warn** - Warning threshold has been reached

---

## 🔥 **Enhanced Logging Features**

Your functions include emoji-enhanced logging for better visibility:

```
🚨 ALERT TRIGGERED abc12345: High CPU Usage on web-server-01
✅ ALERT RECOVERED def67890: Database Connection Restored on db-server-02
⚠️ WARNING ALERT ghi24680: Memory Usage High on app-server-03
ℹ️ INFO ALERT jkl13579: Deployment Completed on build-server-04
```

---

## 🎯 **Expected Response Format**

All functions return a consistent JSON response:

```json
{
  "success": true,
  "message": "Webhook processed successfully",
  "data": {
    "processingId": "abc12345",
    "alertType": "error",
    "alertTransition": "Triggered",
    "summary": "Processed error alert 'High CPU Usage' - Triggered"
  },
  "timestamp": "2025-08-07T10:30:00Z"
}
```

---

## 🚀 **Running Locally (Azure Functions Core Tools)**

If you have Azure Functions Core Tools installed:

```bash
# Navigate to the project folder
cd AzureFunctions

# Start the function host
func start

# Your functions will be available at:
# http://localhost:7071/api/datadog/webhook
# http://localhost:7071/api/datadog/webhook/events  
# http://localhost:7071/api/datadog/test
# http://localhost:7071/api/health
# http://localhost:7071/api/info
```

---

## 🎯 **Production Usage (After Deployment)**

Once deployed to Azure, your URLs will be:

```
https://YOUR-FUNCTION-APP.azurewebsites.net/api/datadog/webhook
https://YOUR-FUNCTION-APP.azurewebsites.net/api/datadog/webhook/events
https://YOUR-FUNCTION-APP.azurewebsites.net/api/datadog/test
https://YOUR-FUNCTION-APP.azurewebsites.net/api/health
https://YOUR-FUNCTION-APP.azurewebsites.net/api/info
```

### **Configure in Datadog:**
1. Go to **Integrations → Webhooks**
2. Click **New Webhook**
3. **URL**: `https://YOUR-FUNCTION-APP.azurewebsites.net/api/datadog/webhook`
4. **Name**: `Azure Function Webhook`
5. Test and attach to monitors

---

## 🔍 **Monitoring Your Functions**

### **Real-time Logs:**
- **Azure Portal** → Function App → **Monitor** → **Live Logs**
- **Application Insights** → **Live Metrics**

### **Function Execution History:**
- **Azure Portal** → Function App → **Functions** → Select function → **Monitor**

### **Log Examples:**
```
[2025-08-07T10:30:00Z] Processing Datadog webhook abc12345 - Alert: test-123, Type: error, Transition: Triggered
[2025-08-07T10:30:01Z] 🚨 ALERT TRIGGERED abc12345: High CPU Usage Alert on web-server-01
[2025-08-07T10:30:01Z] Successfully processed webhook abc12345
```

---

## 🎯 **Key Features of Your Solution**

✅ **Multiple Endpoints** - Different webhook types supported  
✅ **Enhanced Logging** - Emoji-based alerts for better visibility  
✅ **Error Handling** - Comprehensive error handling and responses  
✅ **Health Monitoring** - Built-in health check endpoint  
✅ **JSON Processing** - Automatic Datadog payload processing  
✅ **Serverless** - Auto-scaling, pay-per-use Azure Functions  
✅ **Production Ready** - Application Insights integration  

---

## 🔧 **Next Steps**

1. **Test locally** using the examples above
2. **Deploy to Azure** using the deployment guide
3. **Configure Datadog** to send webhooks to your function
4. **Monitor** webhook processing in Azure Portal
5. **Extend** the business logic in `WebhookProcessingService.cs`

Your webhook solution is ready to handle production Datadog alerts! 🎉
