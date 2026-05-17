# Step 4: API Client

### Current Implementation (GNOME)
The GNOME version uses a custom `ApiClient` based on `libsoup` (via `gjsFetch`).
-   **Service**: `app/src/services/ApiClient.ts`.
-   **Authentication**: Uses a custom header `X-Respatch-Token` and sometimes a query parameter `_token` for POST actions.
-   **Endpoints**:
    -   `GET /status`: Verify project connectivity.
    -   `GET /transports`: Fetch status of all message transports.
    -   `GET /recent-messages`: Fetch latest processed messages.
    -   `GET /transport/{name}`: Fetch failed messages for a specific transport.
    -   `POST /transport/{name}/{id}/retry`: Retry a failed message.
    -   `POST /transport/{name}/{id}/remove`: Remove a failed message.

### Windows Port Strategy
The Windows version will use the standard **HttpClient** from `System.Net.Http`.

-   **Service**: Create an `ApiClient` service in C#.
-   **Models (DTOs)**: Define C# classes that match the JSON structure of the API responses.
    -   `TransportInfo`, `RecentMessage`, `FailedMessage`, `ApiResponse`.
-   **JSON Serialization**: Use `System.Text.Json` for deserializing responses and serializing requests.
-   **Error Handling**: Implement robust error handling for network issues, timeouts, and API-specific errors (e.g., unauthorized access).

### Migration Tasks
1.  **Define DTO Models**: Port the TypeScript interfaces from `app/src/models/` to C# classes/records.
2.  **Implement HttpClient Service**:
    -   Configure a single `HttpClient` instance (using `IHttpClientFactory` if using DI).
    -   Implement methods for GET and POST requests, handling the `X-Respatch-Token` header.
3.  **Authentication Logic**: Implement the token-based authentication used by the Respatch server.
4.  **Async/Await**: Ensure all API calls are asynchronous to keep the UI responsive.
