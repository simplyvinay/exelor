## Table Of Contents

- [Description](#Exelor)
- [Tech Stack](#Tech-Stack)
- [Features](#Features)
- [Local Building](#Local-Building)
- [Config](#Config)
- [How to Contribute](#How-to-Contribute)
- [License](#License)

# Exelor

Exelor is a lightweight .net core api framework that includes jwt authentication with refresh token support, permission authorisation, auditing, logging, error handling, fluent validation, swagger, caching, data shaping and rate limiting

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

# Features

#### Jwt and Refresh Tokens

Authentication in Exelor is done by issuing an access token with the users claims in it. You'll need to login to the application with a username and password and if successful, you'll get an access token that is valid
for 15 minutes along with a refresh token that is valid for 2 days. You'll get a 401 response with a `Token-Expired` header when the Jwt token is no longer valid. You can ask for a new token from the refreshtoken endpoint.

#### Authorisation

By default all routes in Exelor needs to be authorised. If you don't want a specific route to authorised, say registering a new user, you need to add `[AllowAnonymous]` attribute to that route. Exelor supports permissions based authorisation, the access token that is issued contains the permissions claims which is used for authorisation.

You can authorise an action in 3 different ways
- Attribute based authorisation: You can add `HasPermission` attribute to controllers/actions and provide a list of permission which has access to the controllers/actions

<pre lang="csharp">
[HttpGet]
<b>[HasPermission(Permissions.ReadUsers, Permissions.EditUsers)]</b>
public async Task<List<UserDetailsDto>> Get(
	SieveModel sieveModel)
{
	return await _mediator.Send(new UserList.Query(sieveModel));
}
</pre>

- By checking if the user has a permission by calling `IsAllowed`

<pre lang="csharp">
[HttpPut]
public async Task<UserDetailsDto> Edit(
	[FromBody] UpdateUser.Command command)
{
	if (<b>!_currentUser.IsAllowed(Permissions.EditUsers)</b>)
		throw new HttpException(HttpStatusCode.Forbidden);
	return await _mediator.Send(command);
}
</pre>

- By validating a permission against the user (this throws an exception if the user doesn't have the permission in question)
<pre lang="csharp">
[HttpDelete("{id}")]
public async Task Delete(
	int id)
{
	<b>_currentUser.Authorize(Permissions.EditUsers);</b>
	await _mediator.Send(new DeleteUser.Command(id));
}
</pre>


#### Audit Logs

- You can enable auditing of entities which will log changes done on entities to the database. This table will store who made the changes, the table on which the change was made, the key of the entity that was changed, old values before the change and new values after

<img src="/assets/images/Audit.PNG" alt="Audit" width="100%" />


#### Paging, Sorting and Filtering

- You can use paging, sorting and filtering by using the Sieve model on Get endpoints which supports the following params (you can read more about Sieve [here](https://github.com/Biarity/Sieve))
```curl
GET /GetPosts

?sorts=     LikeCount,CommentCount,-created         // sort by likes, then comments, then descendingly by date created 
&filters=   LikeCount>10, Title@=awesome title,     // filter to posts with more than 10 likes, and a title that contains the phrase "awesome title"
&page=      1                                       // get the first page...
&pageSize=  10                                      // ...which contains 10 posts

```

#### Data Shaping
- You can request the fields that you are interested in and only those fields are returned in the response
```curl
GET /Roles

?fields=     Id, Name         // Only returns the Id and Name values
```
<pre lang="JSON">
[
  {
    "Id": 1,
    "Name": "HR"
  },
  {
    "Id": 2,
    "Name": "Project Manager"
  }
]
</pre>

# Local Building

- Install [.NET Core SDK](https://dotnet.microsoft.com/download)
- Go to exelor folder and run `dotnet restore` and `dotnet build`
- Add and run migrations 
  - Install `ef` tool by running `dotnet tool install --global dotnet-ef`
  - Run `dotnet ef migrations add Init` and then `dotnet ef database update`
- Run `dotnet run` to start the server at `http://localhost:5000/`
- You can view the API reference at `http://localhost:5000/swagger`
- Login using `{ "userName": "john",  "password": "test" }` for ReadUsers permission and `{  "userName": "jane",  "password": "test" }` for SuperUser permission

# Config

#### ConnectionStrings
- `DefaultConnection`: `Server=localhost;Database=starter;Trusted_Connection=True;MultipleActiveResultSets=true`
  - SQL connection string, a default database called starter is created when you run migrations

#### JwtSettings

- `SecurityKey`: `A super secret long key to encrypt and decrypt the token`
- `Issuer`: `Issuer`
- `Audience`: `Audience`
  - The key, issuer and audience values to generate a jwt token

#### PasswordHasher

- `Key`: `Secret key to encrypt passwords`
  - The key to encrypt the passwords

#### AuditSettings

- `Enabled`: `true`
  - This is to enable or disable logging changes on the entities to Audits table

#### Serilog

- `MinimumLevel`: `"Default": "Information"`
- `WriteTo`: `"Name": "Console"`
  - Configure serilog options. Check [wiki](https://github.com/serilog/serilog/wiki) for more options and modify `appsettings.json` 

#### IpRateLimiting

- `IpWhitelist`: `[ "127.0.0.1", "::1/10", "192.168.0.0/24" ]`
- `GeneralRules`: `...`
  - Configure IpRateLimiting options. Localhost is whitelisted by default. Check [wiki](https://github.com/stefanprodan/AspNetCoreRateLimit/wiki) for more options and modify `appsettings.json`

#### Sieve

- `Sieve`: `"CaseSensitive": false, "DefaultPageSize": 50, "MaxPageSize": 100, "ThrowExceptions": true`
  - Default options for Sieve which is used for paging, sorting and filtering

# How to Contribute

1. Clone repo `git clone https://github.com/simplyvinay/exelor.git`
2. Create a new branch: `git checkout -b new_branch_name`
3. Make changes and test
4. Submit Pull Request with description of changes

## License

[MIT](https://github.com/simplyvinay/exelor/blob/master/LICENSE)

