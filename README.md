# eReferrals API

## Description
This project is an ASP.NET Core API that provides endpoints for handling Referrals.

## Prerequisites
Make sure you have the following installed and set up:
- [.NET SDK](https://dotnet.microsoft.com/download) version 8.0
- `az login --tenant <YOUR_TENNANT>`

## Required configuration for local development
To configure the project, follow these steps:
1. Open [appsettings.Development.json](appsettings.Development.json) or user secrets file and configure BaseUrl for PAS Referrals API.
```
"PasReferralsApi": {
    "BaseUrl": "<YOUR_URL>"
```

## Project Structure
The core project structure is organized as follows:
```
WCCG.PAS.Referrals.API/
│
├── Properties
│   └── launchSettings.json
|
├── ApiClient
│   └── Clients for external APIs
|
├── Configuration
│   └── Configuration files and their validation
│
├── Controllers
│   └── v1
|       └── Controllers for API of version 1
|
├── Errors
│   └── FHIR HTTP error models
|
├── Middleware
│   └── Response finalisation and error handling process
|
├── Services
│   └── Service classes
|
├── Swagger
│   └── Helper classes for Swagger
│
├── Validators
│   └── Validation classes
|
├── appsettings.json
|   └── appsettings.Development.json
|
└── Program.cs
```

## Running the Project
To run the project locally, follow these steps:
1. Clone the repository.
2. Don't forget `az login --tenant <YOUR_TENNANT>`
3. Setup local configuration according to `Required configuration for local development` section
2. Rebuild and run the project.
6. Open your web browser and navigate to `https://localhost:xxxxx/swagger/index.html` to access the SwaggerUI with API endpoints.

## API Endpoints
Example payloads for POST endpoints can be found in the `Examples` folder. 

### POST /api/v1/Referrals/$process-message
- Description: Creates a referral and returns enriched response 
- Request body should be a valid FHIR Bundle JSON object. [Example Payload](./src/WCCG.PAS.Referrals.API/Examples/createReferral-example-payload.json)
- Response is also a FHIR Bundle but enriched with new values generated while the creation process:
