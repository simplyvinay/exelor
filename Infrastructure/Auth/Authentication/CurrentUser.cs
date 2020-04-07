﻿using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Exelor.Infrastructure.Auth.Authentication
{
    public interface ICurrentUser
    {
        string Id { get; }
    }

    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Id => _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}