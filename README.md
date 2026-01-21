# AiFlowAgentWorkflow

A sample **vertical-slice** .NET solution demonstrating **Microsoft Agent Framework (preview)** with **3 specialized agents**:

1. **CsvReader** (reads CSV metadata + preview)
2. **DataAnalyzer** (computes numeric stats + outliers + ML.NET spike detection)
3. **Emailer** (writes a summary email + sends via SMTP)

The agents are orchestrated as a **sequential workflow**:

```
CsvReader -> DataAnalyzer -> Emailer
```

## Prereqs

- .NET SDK (net8.0 or later)
- An OpenAI API key
- (Optional) SMTP credentials for real email sending

## Configure

Edit `src/Aiflow.Orchestrator.Api/appsettings.json`:

- `OpenAI:ApiKey` + `OpenAI:Model`
- `Smtp:*` (set `DryRun: true` to avoid sending during testing)
- `Storage:RootPath` (where uploaded CSVs are stored)

### Recommended: use environment variables

```
OpenAI__ApiKey=...
OpenAI__Model=gpt-4o-mini
Smtp__Host=smtp.example.com
Smtp__Port=587
Smtp__Username=...
Smtp__Password=...
Smtp__From=sender@example.com
Smtp__To=recipient@example.com
Smtp__DryRun=true
```

## Run (API)

From the repo root:

```
dotnet run --project src/Aiflow.Orchestrator.Api
```

Open Swagger:

- `http://localhost:5000/swagger` (or the port printed in console)

## Call the workflow

Endpoint:

- `POST /workflows/csv-analysis-email`

Multipart form fields:

- `csv` (file) **required**
- `to` (string) optional override recipient
- `dryRun` (bool) optional override dry run
- `spikeColumn` (string) optional numeric column name to run spike detection on
- `previewRows` (int) optional (default 20)

Example using curl:

```
curl -X POST "http://localhost:5000/workflows/csv-analysis-email" \
  -F "csv=@./sample.csv" \
  -F "dryRun=true"
```

## Notes

- Agents are **separate projects** so you can version and deploy them independently.
- `Aiflow.Core` contains shared **Options**, **Utilities**, and **Abstractions for utilities** (now grouped under `src/Models/*`).
- The vertical slice is `Features/CsvAnalysisEmail/Run` (request + handler + response in one folder).

