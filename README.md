## Table Of Contents

- [Description](#Exelor)
- [Tech Stack](#Tech-Stack)
- [Local Building](#Local-Building)
- [Config](#Config)
- [How to Contribute](#How-to-Contribute)
- [License](#License)

# Exelor
Exelor is a lightweight .net core api framework that includes jwt authentication with refresh token support, permission authorisation, auditing, logging, error handling, fluent validation, swagger, caching and rate limiting

# Tech Stack

- [.NET Core 3.0](https://github.com/dotnet/core)
- [Fluent Validation](https://github.com/JeremySkinner/FluentValidation)
- [MediatR](https://github.com/jbogard/MediatR)
- [Entity Framework Core](https://github.com/aspnet/EntityFrameworkCore).
- [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) for Swagger
- [ASP.NET Core JWT Bearer Authentication](https://github.com/aspnet/Security) for [JWT](https://jwt.io/) authentication with support for [refresh tokens](https://tools.ietf.org/html/rfc6749#section-1.5).
- [Serilog](https://github.com/serilog/serilog) for logging
- [Marvin.Cache.Headers](https://github.com/KevinDockx/HttpCacheHeaders) to add caching headers to responses
- [AspNetCoreRateLimit](https://github.com/stefanprodan/AspNetCoreRateLimit) to add rate limiting functionality
- [Sieve](https://github.com/Biarity/Sieve) to add paging, sorting and filtering functionality 

# Local Building

- Install [.NET Core SDK](https://dotnet.microsoft.com/download)
- Go to exelor folder and run `dotnet restore` and `dotnet build`
- Add and run migrations 
  - Install `ef` tool by running `dotnet tool install --global dotnet-ef`
  - Run `dotnet ef migrations add Init` and then `dotnet ef database update`
- Run `dotnet run` to start the server at `http://localhost:5000/`
- You can view the API reference at `http://localhost:5000/swagger`

# Config

TODO

# How to Contribute

1. Clone repo `git clone https://github.com/simplyvinay/exelor.git`
2. Create a new branch: `git checkout -b new_branch_name`
3. Make changes and test
4. Submit Pull Request with description of changes

## License

[MIT](https://github.com/simplyvinay/exelor/blob/master/LICENSE)


