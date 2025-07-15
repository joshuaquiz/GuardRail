# Test API Endpoint for NFC Card Emulator

## Overview
This document provides examples for creating a test API endpoint that the NFC Card Emulator can use to fetch data.

## Simple Node.js Express Server

```javascript
const express = require('express');
const app = express();
const port = 3000;

// Enable CORS for testing
app.use((req, res, next) => {
    res.header('Access-Control-Allow-Origin', '*');
    res.header('Access-Control-Allow-Headers', 'Origin, X-Requested-With, Content-Type, Accept');
    next();
});

// Test endpoint that returns NFC data
app.get('/api/getNfcData', (req, res) => {
    const response = {
        payload: "Hello from NFC Card Emulator!",
        timestamp: new Date().toISOString(),
        success: true,
        deviceId: req.headers['user-agent'] || 'unknown'
    };
    
    console.log('NFC data requested:', response);
    res.json(response);
});

// Health check endpoint
app.get('/health', (req, res) => {
    res.json({ status: 'OK', timestamp: new Date().toISOString() });
});

app.listen(port, () => {
    console.log(`Test API server running at http://localhost:${port}`);
    console.log(`NFC endpoint: http://localhost:${port}/api/getNfcData`);
});
```

## ASP.NET Core Minimal API

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();

// Test endpoint
app.MapGet("/api/getNfcData", () =>
{
    var response = new
    {
        payload = "Hello from NFC Card Emulator!",
        timestamp = DateTime.UtcNow.ToString("O"),
        success = true,
        requestId = Guid.NewGuid().ToString()
    };
    
    Console.WriteLine($"NFC data requested: {response}");
    return Results.Ok(response);
});

// Health check
app.MapGet("/health", () => Results.Ok(new { status = "OK", timestamp = DateTime.UtcNow }));

app.Run();
```

## Python Flask Server

```python
from flask import Flask, jsonify
from datetime import datetime
import uuid

app = Flask(__name__)

@app.route('/api/getNfcData', methods=['GET'])
def get_nfc_data():
    response = {
        'payload': 'Hello from NFC Card Emulator!',
        'timestamp': datetime.utcnow().isoformat() + 'Z',
        'success': True,
        'requestId': str(uuid.uuid4())
    }
    
    print(f'NFC data requested: {response}')
    return jsonify(response)

@app.route('/health', methods=['GET'])
def health_check():
    return jsonify({
        'status': 'OK',
        'timestamp': datetime.utcnow().isoformat() + 'Z'
    })

if __name__ == '__main__':
    print('Test API server running at http://localhost:5000')
    print('NFC endpoint: http://localhost:5000/api/getNfcData')
    app.run(debug=True, host='0.0.0.0', port=5000)
```

## Expected JSON Response Format

The API should return a JSON response with the following structure:

```json
{
    "payload": "This is the data to send via NFC",
    "timestamp": "2024-01-01T12:00:00.000Z",
    "success": true,
    "requestId": "12345678-1234-1234-1234-123456789012"
}
```

### Required Fields
- **payload**: The actual data that will be transmitted via NFC (configurable field name in app settings)

### Optional Fields
- **timestamp**: When the data was generated
- **success**: Boolean indicating if the request was successful
- **requestId**: Unique identifier for the request
- **deviceId**: Identifier for the requesting device
- **metadata**: Any additional information

## Testing with Different Data Types

### Simple Text Data
```json
{
    "payload": "Simple text message for NFC transmission"
}
```

### JSON String Data
```json
{
    "payload": "{\"userId\":\"12345\",\"accessLevel\":\"admin\",\"expires\":\"2024-12-31\"}"
}
```

### Base64 Encoded Data
```json
{
    "payload": "SGVsbG8gZnJvbSBORkMgQ2FyZCBFbXVsYXRvciE="
}
```

### Structured Data
```json
{
    "payload": "USER:12345|LEVEL:ADMIN|EXP:2024-12-31"
}
```

## Error Responses

### Authentication Error
```json
{
    "error": "Authentication required",
    "success": false,
    "code": 401
}
```

### Server Error
```json
{
    "error": "Internal server error",
    "success": false,
    "code": 500
}
```

### Data Not Available
```json
{
    "error": "No data available for this request",
    "success": false,
    "code": 404
}
```

## Configuration in NFC Card Emulator App

1. **API Endpoint URL**: Set to your test server URL (e.g., `http://192.168.1.100:3000/api/getNfcData`)
2. **JSON Field Name**: Set to `payload` (or whatever field contains your NFC data)
3. **HTTP Timeout**: Set to appropriate timeout (default: 10 seconds)

## Testing Steps

1. Start your test API server
2. Configure the NFC Card Emulator app with your server URL
3. Use an NFC reader tool to interact with the device
4. Monitor the API server logs to see requests
5. Verify data transmission in the NFC reader tool

## Security Considerations for Production

- Use HTTPS for all API communications
- Implement proper authentication (API keys, OAuth, etc.)
- Validate and sanitize all input data
- Implement rate limiting to prevent abuse
- Log all requests for audit purposes
- Use secure data transmission protocols
