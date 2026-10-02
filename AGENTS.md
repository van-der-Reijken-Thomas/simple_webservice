# AGENTS.md

## Project overview

This repository contains an Azure Functions v4 application written in C# using the .NET isolated worker model.

## Project structure

- `Program.cs`: configures and starts the Functions worker.
- `HelloWorldTrigger.cs`: contains the HTTP-triggered function.
- `host.json`: contains Azure Functions host configuration.
- `local.settings.json`: contains local-only settings and must not contain committed secrets.
- `simple_webservice.csproj`: defines the target framework and NuGet dependencies.

## Development commands

Run all commands from the repository root.

```powershell
dotnet restore
dotnet build
dotnet run
```

Prefer `dotnet run` over `func start` for this .NET isolated project. Azure Functions Core Tools may otherwise start from the wrong output directory and fail to load extensions correctly.

After startup, use the URL printed by the Functions host to test `HelloWorldTrigger`. The port may be defined in `Properties/launchSettings.json`.

## Implementation guidelines

- Keep the application on the .NET isolated worker model.
- Use constructor injection for services and logging.
- Keep function classes and function entry methods public.
- Give each function a unique `[Function("...")]` name.
- Keep HTTP routes, methods, and authorization levels explicit.
- Use nullable reference types and address compiler warnings rather than suppressing them without justification.
- Never commit credentials, connection strings, access tokens, or other secrets.
- Do not edit generated files under `bin/` or `obj/`.

## Verification

For code changes, run:

```powershell
dotnet build
```

When runtime behavior changes, also start the host with `dotnet run` and exercise the affected endpoint. Report any verification that could not be completed because of local machine policies or unavailable external services.

## Change discipline

- Make focused changes and preserve unrelated user work.
- Add or update tests when behavior changes and a test project is available.
- Avoid introducing dependencies unless they provide a clear benefit.
- Update this file when the project's build or runtime workflow changes.

