# Swagger / OpenAPI 2.0 auto-generated files

The contents in this folder were generated with the `swagger-codegen` tool, using the OpenAPI (Swagger) specification defined in the `docs/Swagger/swagger.json` file.

## Installation

You can follow the official [swagger-codegen](https://github.com/swagger-api/swagger-codegen) documentation for installation instructions.

Or you can use the following command to install it via Homebrew:

```bash
brew install swagger-codegen
```

Or via chocolatey:

```bash
choco install swagger-codegen
```

## Server API documentation

See the generated [API documentation](https://jcanotorr06.github.io/technicaltest/Swagger/) for more details on the available endpoints and their usage.

### How to authenticate

To make requests to the API, you need to include a valid Bearer token in the Authorization header. You can obtain a token by authenticating with the identity provider.

```bash
Authorization: Bearer <your_token>
```

Additionally, you need to include the following environment variables for the API to validate the token:

```json
  {
    "OpenIdConfiguration": "<your_openid_configuration>", // URL to the OpenID configuration
    "TokenIssuer": "<your_token_issuer>", // Expected token issuer
    "TokenAudience": "<your_token_audience>" // Expected audience for the token
  }
```
