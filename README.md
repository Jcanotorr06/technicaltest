# Technical Test: Full-Stack Task Management App with Azure Integration

![logo](./client/public/text%20logo.png)

This project is a full-stack task management application built with React, TypeScript, and Azure Functions.

It allows users to create, read, update, and delete tasks, with a focus on integrating with Azure services for authentication and data storage.

## Features

- User authentication with Azure Entra ID
- CRUD operations for lists and tasks
- Integration with Azure SQL Database for data storage

## Tech Stack

- Frontend: React, Typescript, Tailwind, Tanstack Router, Tanstack Query
- Backend: Azure Functions, C#, Entity Framework Core
- Database: Azure SQL Server Database
- Authentication: Microsoft identity platform, OpenID Connect
- Devops: GitHub Actions
- Testing: xUnit

## Quick Start

Clone the repository with the following command:

```bash
  git clone https://github.com/Jcanotorr06/technicaltest.git
```

Navigate to the client directory and install the dependencies:

```bash
  cd technicaltest/client
  pnpm install
```

Create a `.env` file in the `client` directory and add the necessary environment variables.

```env
  ENV = Environment setting (e.g., LOCAL, PROD)
  VITE_FUNCTIONS_KEY = Azure functions key
  VITE_APIM_KEY = Azure API Management key
  VITE_API_URL = API URL
  VITE_CLIENT_ID = Azure AD Application (client) ID
  VITE_CLIENT_AUTHORITY = Azure AD Authority URL
  VITE_REDIRECT_URI = Redirect URI for Azure AD
  VITE_LOGOUT_URI = Logout URI for Azure AD
```

Navigate to the API directory and add the necessary environment variables to the `local.settings.json` file.

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "Environment": "DEV", // Change to Local to bypass API authentication for testing purposes
    "SQLConnectionString": "",
    "DatabaseName": "",
    "APIMSubscriptionKey": "",
    "OpenIdConfiguration": "",
    "TokenIssuer": "",
    "TokenAudience": ""
  }
}
```

Install the Entity Framework Core CLI

```bash
dotnet tool install --global dotnet-ef
```

Perform database migrations

```bash
dotnet ef database update --connection <your_connection_string>
```

Install the Azure SWA CLI tool globally

```bash
npm install -g @azure/static-web-apps-cli
```

In the `root` directory, run the following command to start the Azure Static Web Apps CLI:

```bash
swa start
```

This will start the development server and the API, allowing you to access the application at [http://localhost:4280](http://localhost:4280).

## Authentication Setup

Follow the official Microsoft documentation to create and configure your Tenant and Azure AD application and User Flow [here](https://learn.microsoft.com/en-us/entra/identity-platform/quickstart-create-new-tenant).

Once done, update the `.env` file in the `client` directory and the `local.settings.json` file in the `API` directory with the appropriate values.

```env
  VITE_CLIENT_ID = <your_application_client_id>
  VITE_CLIENT_AUTHORITY = https.//<your_tenant_subdomain>.ciamlogin.com/
  VITE_REDIRECT_URI = <application_redirect_uri>
  VITE_LOGOUT_URI = <application_logout_uri>
```

```json
{
    "OpenIdConfiguration": "https://<your_tenant_subdomain>.ciamlogin.com/<your_tenant_subdomain>.onmicrosoft.com/v2.0/.well-known/openid-configuration",
    "TokenIssuer": "https://<your_tenant_id>.ciamlogin.com/<your_tenant_id>/v2.0",
    "TokenAudience": "<your_application_client_id>"
}
```

Optionally, you can set the `Environment` variable in `local.settings.json` to `Local` to bypass API authentication for testing purposes.

## Api Documentation

See the generated [API documentation](https://jcanotorr06.github.io/technicaltest/Swagger/) for more details on the available endpoints and their usage.

## Architecture

[Architecture](./docs/Architecture/README.md)

## What Could Be Improved

- Implement dynamic imports in the client application to reduce bundle size and improve loading times.
- Improve error handling and user feedback in the client application.
- Implement rate limiting in the API to prevent abuse and ensure fair usage.
- Automate Swagger documentation generation and deployment.
- Improve API response times through caching and optimization techniques.
- Implement proper pagination in the API to support large datasets.
- Add comprehensive unit and integration tests to ensure code quality and prevent regressions.
- Automate dependency updates and vulnerability scanning to maintain a secure and up-to-date codebase.
- Implement scrapped features from the initial project scope e.g. Tags, Re-Ordering, Task Assignment.
- Add support for additional authentication methods (e.g. social logins) to improve user experience.
- Improve accessibility features to ensure the application is usable by all individuals.

## Conclusion

This project demonstrates the implementation of a modern web application using Azure Static Web Apps, Azure Functions, and a client-side framework. By following the steps outlined in this guide, you can set up a secure and scalable application that leverages the power of Azure services.

It has been designed with best practices in mind, ensuring a robust and maintainable codebase. The use of Azure services allows for seamless integration and deployment, making it easier to manage and scale your application as needed.

Thank you for taking the time to review this project. I hope it meets your expectations and provides a solid foundation for your own development efforts.
