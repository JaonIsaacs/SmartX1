# SmartX1

SmartX1 is a .NET 9 IoT gateway and dashboard for registering sensors, ingesting telemetry, managing device attachments, and viewing live device data through a Razor Pages web UI and REST API.

## What it does

- Registers IoT sensors with metadata such as device ID, category, location, unit, and configuration
- Accepts telemetry payloads from devices
- Shows latest readings in a dashboard
- Stores sensor profiles in LiteDB
- Keeps per-device telemetry history in memory
- Uploads device files such as configs, photos, and logs
- Encrypts uploaded files at rest with AES-256
- Provides an admin page for generating mock test data

## Tech stack

- ASP.NET Core Razor Pages + Controllers
- .NET 9
- LiteDB
- Bootstrap 5

## Project structure

- `SmartX1.1/Program.cs` - app startup and service registration
- `SmartX1.1/Controller` - API controllers
- `SmartX1.1/Services` - sensor and telemetry services
- `SmartX1.1/Models` - domain models and file storage logic
- `SmartX1.1/Pages` - Razor Pages UI

## Getting started

### Prerequisites

- .NET 9 SDK

### Run locally

```bash
dotnet restore SmartX1.sln
dotnet run --project SmartX1.1/SmartX1.csproj
```

Then open the local URL shown by ASP.NET Core in your browser.

## Main UI pages

- `/` - dashboard for live telemetry, registered devices, and file uploads
- `/Admin/TestData` - generates sample devices and telemetry for testing

## API overview

### Sensors

- `GET /api/sensors/all` - list registered sensors
- `GET /api/sensors/{deviceId}` - get one sensor
- `POST /api/sensors/register` - register a sensor
- `PUT /api/sensors` - update a sensor

### Telemetry

- `GET /api/telemetry/latest` - latest reading per device
- `GET /api/telemetry/device/{deviceId}` - recent readings for a device
- `GET /api/telemetry/device/{deviceId}/history` - full in-memory history for a device
- `POST /api/telemetry/ingest` - ingest telemetry data

### Aggregation

- `GET /api/aggregation/zone-total/{category}` - aggregate latest numeric readings by category

### Files

- `POST /api/files/upload/{deviceId}?fileType={type}` - upload a single file
- `POST /api/files/upload-batch/{deviceId}?fileType={type}` - upload multiple files
- `GET /api/files/device/{deviceId}` - list files for a device
- `GET /api/files/download/{attachmentId}` - download a file
- `DELETE /api/files/{attachmentId}` - delete a file

Valid file types are `config`, `photo`, and `log`.

## Data notes

- Sensor profiles are stored in `smartx.db`
- Telemetry records are currently stored in memory
- File metadata is currently stored in memory
- Uploaded file contents are written to the `uploads` directory as encrypted `.enc` files

## Sample telemetry payload

```json
{
  "deviceId": "ESP32:001A2B3C4D5E",
  "value": "23.7",
  "category": 0,
  "deploymentLocation": "Sub-Zone A > Zone 1 > Hydroponic Farm",
  "unit": "°C",
  "collectionTime": "2026-09-14T09:00:00Z"
}
```

## Notes

- The app allows all CORS origins, methods, and headers in its current configuration
- Deployment locations must follow a hierarchy format like `Child > Parent > Facility`
- The default file encryption key is development-oriented and should be replaced for production use