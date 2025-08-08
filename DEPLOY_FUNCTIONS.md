# Deploy Datadog Webhook as Azure Function App

## 🚀 **Why Azure Functions for Webhooks?**

✅ **Serverless** - Pay only when webhooks are received  
✅ **Auto-scaling** - Handles webhook bursts automatically  
✅ **Event-driven** - Perfect for webhook scenarios  
✅ **Cost-effective** - No idle server costs  
✅ **Managed infrastructure** - No server maintenance  

---

## 📦 **Manual Deployment Steps (15 minutes)**

### **Step 1: Build the Function App (2 minutes)**

1. **Navigate to the Functions folder**:
   ```bash
   cd AzureFunctions
   ```

2. **Build and publish**:
   ```bash
   dotnet publish -c Release -o publish
   ```

3. **Create deployment package**:
   - Right-click `publish` folder → "Send to" → "Compressed folder"
   - Rename to `datadog-webhook-function.zip`

### **Step 2: Create Function App in Azure Portal (8 minutes)**

1. **Go to Azure Portal**: https://portal.azure.com
2. **Create a resource** → Search "Function App"
3. **Click "Create"**

**Fill the form:**
- **Resource Group**: Create new → `webhook-functions-rg`
- **Function App name**: `datadog-webhook-func-[yourname]` (globally unique)
- **Publish**: **Code**
- **Runtime stack**: **.NET**
- **Version**: **8 (LTS) Isolated**
- **Region**: Choose closest to you
- **Operating System**: **Windows**
- **Plan type**: **Consumption (Serverless)** ← Perfect for webhooks!

4. **Click "Review + create"** → **"Create"**

### **Step 3: Deploy Your Functions (3 minutes)**

1. **Go to your Function App** in Azure Portal
2. **Deployment Center** → **Zip Deploy**
3. **Upload** your `datadog-webhook-function.zip`
4. **Wait for deployment** (1-2 minutes)

### **Step 4: Configure Application Settings (2 minutes)**

1. **Configuration** → **Application settings**
2. **Add these settings**:
   - `DatadogWebhook__EnableDetailedLogging` = `true`
   - `DatadogWebhook__MaxPayloadSize` = `1048576`

3. **Click "Save"**

---

## 🎯 **Your Function Endpoints**

After deployment, your webhook URLs will be:

| **Function** | **URL** | **Purpose** |
|--------------|---------|-------------|
| **Main Webhook** | `https://your-func-app.azurewebsites.net/api/datadog/webhook` | Primary Datadog webhook |
| **Event Webhook** | `https://your-func-app.azurewebsites.net/api/datadog/webhook/events` | Custom events |
| **Test Function** | `https://your-func-app.azurewebsites.net/api/datadog/test` | Test endpoint |
| **Health Check** | `https://your-func-app.azurewebsites.net/api/health` | Health monitoring |
| **Info** | `https://your-func-app.azurewebsites.net/api/info` | Function information |

---

## 🧪 **Testing Your Function**

### **Test via Browser**
Open: `https://your-func-app.azurewebsites.net/api/datadog/test`

### **Test via Postman**
- **URL**: `https://your-func-app.azurewebsites.net/api/datadog/webhook`
- **Method**: POST
- **Body**:
```json
{
  "alert_id": "func-test-123",
  "title": "Azure Function Test Alert",
  "body": "Testing Datadog webhook via Azure Functions",
  "alert_type": "info",
  "alert_transition": "Triggered",
  "hostname": "azure-function",
  "priority": "normal",
  "tags": ["azure", "functions", "test"]
}
```

---

## 📊 **Monitoring Your Functions**

### **Real-time Logs**
- **Azure Portal** → Your Function App → **Functions** → **Monitor**
- **Live Logs** to see webhook processing in real-time

### **Application Insights**
- Automatically enabled for performance monitoring
- **Application Insights** → **Live Metrics** for real-time monitoring

### **Function Execution History**
- See every webhook execution with timing and status
- **Functions** → **DatadogWebhook** → **Monitor**

---

## 🔧 **Configure in Datadog**

1. **Datadog Dashboard** → **Integrations** → **Webhooks**
2. **New Webhook**:
   - **URL**: `https://your-func-app.azurewebsites.net/api/datadog/webhook`
   - **Name**: `Azure Function Webhook`
3. **Test** using Datadog's test feature
4. **Attach to monitors**

---

## 💰 **Cost Benefits of Functions vs Web App**

| **Scenario** | **Function App (Consumption)** | **Web App (Always On)** |
|--------------|-------------------------------|-------------------------|
| **10 webhooks/day** | ~$0.01/month | ~$13/month |
| **100 webhooks/day** | ~$0.10/month | ~$13/month |
| **1000 webhooks/day** | ~$1.00/month | ~$13/month |
| **Burst traffic** | Auto-scales, pay per use | Fixed cost, may need scaling |

**Result**: Functions can be **99% cheaper** for typical webhook scenarios! 💵

---

## 🚀 **Advanced Features Available**

### **Auto-scaling**
- Handles 0-200+ concurrent webhooks automatically
- No configuration needed

### **Durable Functions** (Future Enhancement)
- For complex webhook workflows
- State management and orchestration

### **Event Grid Integration**
- Forward webhooks to other Azure services
- Fan-out to multiple processors

### **Logic Apps Integration**
- Visual workflow designer
- Connect to hundreds of services

---

## 🔄 **Comparison: Web App vs Function App**

| **Feature** | **Web App** | **Function App** |
|-------------|-------------|------------------|
| **Cost** | Always running | Pay per execution |
| **Scaling** | Manual/Auto | Automatic |
| **Cold Start** | None | ~2-3 seconds |
| **Complexity** | Full web server | Simple functions |
| **Best For** | High-frequency APIs | Event-driven webhooks |

**For Datadog webhooks**: **Functions are ideal** because webhooks are sporadic, event-driven, and don't need always-on servers! 🎯
