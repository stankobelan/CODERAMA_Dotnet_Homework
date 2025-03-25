

# CoderamaInterview Project

A sample ASP.NET Core project demonstrating an extensible document management API with support for multiple serialization formats (JSON, XML, MessagePack) and repository types (in-memory, file system). The project also includes validation, caching (response caching), and a clean separation of concerns across various layers.

## Table of Contents

1. [Overview](#overview)  
2. [Features](#features)  
3. [Architecture](#architecture)  
4. [Prerequisites](#prerequisites)  
5. [Installation](#installation)  
6. [Usage](#usage)  
7. [Testing](#testing)  
8. [Configuration](#configuration)  
9. [Contributing](#contributing)  
10. [License](#license)

---

## Overview

This project provides a RESTful API for creating, retrieving, and updating documents with a flexible serialization mechanism (Strategy Pattern) and interchangeable storage repositories (Repository Pattern). It demonstrates best practices like:

- **SOLID principles**  
- **Dependency Injection**  
- **Validation** via FluentValidation  
- **Response Caching** for performance improvements  
- **Unit Tests** for controllers, services, and repositories

---

## Features

1. **Multiple Serialization Formats**  
   - **JSON** (`application/json`)  
   - **XML** (`application/xml`)  
   - **MessagePack** (`application/x-msgpack`)

2. **Different Underlying Storages**  
   - **InMemoryDocumentRepository** (default)  
   - Potential for **FileSystemDocumentRepository** or cloud-based repositories

3. **Validation**  
   - Uses **FluentValidation** to ensure required fields are present and correctly formatted

4. **Caching**  
   - **Response Caching** enabled for the GET endpoint to reduce load for frequently accessed documents

5. **Extensibility**  
   - Adding a new serializer or repository involves implementing straightforward interfaces

6. **API Endpoints**  
   - **POST** `/documents` - Creates a new document  
   - **PUT** `/documents` - Updates an existing document  
   - **GET** `/documents/{id}` - Retrieves a document in various formats, depending on `Accept` header

---

## Architecture

**Key Layers & Components**:

- **Controllers**  
  Contain HTTP endpoints and minimal logic. They delegate work to the Service layer.

- **Services** (e.g., `DocumentService`)  
  Orchestrate business logic: caching, repository calls, serialization selection based on `Accept` header.

- **Serialization** (e.g., `JsonDocumentSerializer`, `XmlDocumentSerializer`, `MessagePackDocumentSerializer`)  
  Implement the `IDocumentSerializer` interface to handle serialization/deserialization. Registered via a serializer factory for runtime selection.

- **Repositories** (e.g., `InMemoryDocumentRepository`)  
  Implement `IDocumentRepository` for storing and retrieving documents. Can be swapped out easily for a different storage mechanism.

- **Validation** (e.g., `DocumentValidator`)  
  Uses FluentValidation to ensure posted documents meet certain requirements (e.g., non-empty ID, tags, data).

- **Program.cs**  
  - Registers all services and repositories in the DI container  
  - Configures **Response Caching**, **Swagger**, and **Logging**  

---

## Prerequisites

- **.NET 8 SDK** (or whichever version the project targets)  
- **Visual Studio**, **Visual Studio Code**, or another C#-compatible IDE/text editor  
- **Postman** or **cURL** for testing the API (optional but recommended)

---

## Installation

1. **Clone this repository**:
   ```bash
   git clone https://github.com/your-organization/coderama-interview.git
   ```
2. **Navigate into the project folder**:
   ```bash
   cd coderama-interview
   ```
3. **Restore packages**:
   ```bash
   dotnet restore
   ```
4. **Build the solution**:
   ```bash
   dotnet build
   ```

---

## Usage

1. **Run the application**:
   ```bash
   dotnet run
   ```
   By default, it listens on `https://localhost:5001` (if using HTTPS) and `http://localhost:5000` (if using HTTP).

2. **View API Docs**:  
   - Navigate to `https://localhost:5001/swagger` in your browser (or HTTP variant).  
   - Test the endpoints directly from Swagger UI.

3. **Postman or cURL**:
   ```bash
   # Example: Create a JSON document
   curl -X POST "https://localhost:5001/documents" \
        -H "Content-Type: application/json" \
        -d "{\"id\":\"doc001\",\"tags\":[\"test\"],\"data\":\"{...}\",\"dataAsString\":\"{...}\"}"
   ```

---

## Testing

The solution includes **xUnit** tests in the `CoderamaInterview.Tests` project:

- **Controllers** (DocumentControllerTests)  
- **Services** (DocumentServiceTests)  
- **Repositories** (InMemoryDocumentRepositoryTests, FileSystemDocumentRepositoryTests)  
- **Serializers** (JsonDocumentSerializerTests, XmlDocumentSerializerTests, MessagePackDocumentSerializerTests)

To run the tests:

```bash
dotnet test
```

---

## Configuration

**Response Caching**:  
- Configured in `Program.cs`:
  ```csharp
  builder.Services.AddResponseCaching();
  ...
  app.UseResponseCaching();
  ```
- **[ResponseCache]** attribute on `GetDocument` method specifying duration and VaryByHeader.

**Swagger**:
- Configured in `Program.cs` under `app.Environment.IsDevelopment()`.

**Logging**:
- Uses built-in .NET logging, configured in `Program.cs`.

---

## Contributing

1. **Fork** this repo
2. **Create a feature branch** (`git checkout -b feature/new-serializer`)
3. **Commit changes** (`git commit -m 'Add new serializer'`)
4. **Push to the branch** (`git push origin feature/new-serializer`)
5. **Open a Pull Request** to the main branch

---

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) file for details.

---
