# API Documentation Standards

**JobIntel API Documentation Guidelines**  
**Last Updated:** December 30, 2025

## Quick Reference

### Controller Template

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RecruitmentPlatformAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ExampleController : ControllerBase
    {
        // Controller implementation
    }
}
```

### Endpoint Documentation Template

```csharp
/// <summary>
/// [Verb] [what the endpoint does] ([additional context])
/// </summary>
/// <param name="paramName">Description of parameter</param>
/// <returns>Description of return value</returns>
[HttpPost("endpoint-name")]
[Authorize] // If authentication required
[ProducesResponseType(typeof(SuccessDto), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
public async Task<IActionResult> MethodName([FromBody] RequestDto dto)
{
    // Implementation
}
```

## Response Type Guidelines

### Standard Response Types by Scenario

| Scenario | Status Code | Response Type |
|----------|-------------|---------------|
| Success (data returned) | 200 OK | `typeof(ApiResponse<T>)` |
| Success (resource created) | 201 Created | `typeof(ResponseDto)` |
| Model validation failed | 400 Bad Request | `typeof(ValidationProblemDetails)` |
| Business rule violation | 400 Bad Request | `typeof(ErrorDto)` or `typeof(ApiResponse<T>)` |
| Not authenticated | 401 Unauthorized | `typeof(ApiErrorResponse)` |
| Resource not found | 404 Not Found | `typeof(ApiErrorResponse)` or `typeof(ApiResponse<T>)` |

### Example Combinations

**Create Operation:**
```csharp
[HttpPost]
[Authorize]
[ProducesResponseType(typeof(ProjectResponseDto), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ProjectResponseDto), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
```

**Read Operation (Single):**
```csharp
[HttpGet("{id}")]
[Authorize]
[ProducesResponseType(typeof(ApiResponse<DataDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
```

**Read Operation (List):**
```csharp
[HttpGet]
[ProducesResponseType(typeof(ApiResponse<List<DataDto>>), StatusCodes.Status200OK)]
```

**Update Operation:**
```csharp
[HttpPut("{id}")]
[Authorize]
[ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
```

**Delete Operation:**
```csharp
[HttpDelete("{id}")]
[Authorize]
[ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
```

## Summary Writing Guidelines

### Format
```
[Verb] [noun/description] ([additional context if needed])
```

### Examples

✅ **Good:**
- "Register a new user (JobSeeker or Recruiter)"
- "Get all active projects sorted by display order (excludes deleted projects)"
- "Delete a project (soft delete - auto-reorders remaining projects)"
- "Request password reset OTP via email (always returns success to prevent email enumeration)"

❌ **Bad:**
- "This endpoint registers users" (too verbose)
- "POST /api/auth/register" (repeats HTTP method)
- "Registers a user. The user can be either a job seeker or recruiter. Returns 201 if successful." (too detailed, belongs in other tags)

### Best Practices

1. **Start with action verb** (Get, Create, Update, Delete, Add, Remove, Verify, etc.)
2. **Keep it concise** (under 150 characters)
3. **Use parentheses for important context** (e.g., "(Step 2 of wizard)", "(soft delete)")
4. **Avoid repeating HTTP method** (it's already in the attribute)
5. **Don't include examples** (Swagger generates them)

## Parameter Documentation

### Format
```csharp
/// <param name="paramName">Clear description [format/range if applicable] (default: value)</param>
```

### Examples

✅ **Good:**
```csharp
/// <param name="lang">Language code: "en" for English, "ar" for Arabic (default: "en")</param>
/// <param name="projectId">Project ID to update</param>
/// <param name="dto">Project details (title, technologiesUsed, description, projectLink)</param>
```

❌ **Bad:**
```csharp
/// <param name="lang">lang parameter</param>
/// <param name="id">id</param>
```

## Returns Documentation

### Format
```csharp
/// <returns>Brief description of what is returned and any key conditions</returns>
```

### Examples

✅ **Good:**
```csharp
/// <returns>List of active projects with all project details</returns>
/// <returns>Success message (always 200 OK to prevent email enumeration)</returns>
/// <returns>Auth response with temporary reset token (valid for 15 minutes)</returns>
```

## What NOT to Include

### ❌ Avoid Manual JSON Examples

**Before (Don't do this):**
```csharp
/// <remarks>
/// Sample request:
///     POST /api/auth/login
///     {
///        "email": "user@example.com",
///        "password": "password"
///     }
/// </remarks>
```

**After (Do this):**
```csharp
/// <summary>
/// Login user and return JWT token
/// </summary>
/// <param name="loginDto">Login credentials</param>
/// <returns>Auth response with JWT token and user info. Email must be verified to login.</returns>
```

### Why?
- Swagger auto-generates examples from your DTOs
- Manual examples get outdated
- DTOs are the single source of truth
- Less code to maintain

## Adding New Endpoints Checklist

Before committing new endpoints, verify:

- [ ] Controller has `[Produces("application/json")]`
- [ ] Method has XML `<summary>` tag
- [ ] All parameters have `<param>` descriptions
- [ ] Return value has `<returns>` description
- [ ] Success response: `[ProducesResponseType(typeof(SuccessDto), StatusCodes.Status2XX)]`
- [ ] Bad request: `[ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]`
- [ ] Validation errors: `[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]`
- [ ] If `[Authorize]`: `[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]`
- [ ] If entity lookup: `[ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]`
- [ ] No manual JSON examples in `<remarks>`
- [ ] DTO properties have validation attributes

## Common Patterns

### Pattern 1: Authenticated CRUD Endpoint
```csharp
/// <summary>
/// [Action] [entity] ([context])
/// </summary>
/// <param name="dto">Entity details</param>
/// <returns>Created/updated entity with metadata</returns>
[HttpPost]
[Authorize]
[ProducesResponseType(typeof(EntityResponseDto), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(EntityResponseDto), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
public async Task<IActionResult> CreateEntity([FromBody] CreateEntityDto dto)
```

### Pattern 2: Public Reference Data
```csharp
/// <summary>
/// Get list of [entities] with [features]
/// </summary>
/// <param name="lang">Language code (default: "en")</param>
/// <returns>List of entities with localized names</returns>
[HttpGet]
[ProducesResponseType(typeof(ApiResponse<List<EntityDto>>), StatusCodes.Status200OK)]
public async Task<IActionResult> GetEntities([FromQuery] string lang = "en")
```

### Pattern 3: Authentication Endpoint
```csharp
/// <summary>
/// [Auth action] ([important security note])
/// </summary>
/// <param name="dto">Authentication credentials</param>
/// <returns>Auth response with JWT token</returns>
[HttpPost("endpoint")]
[ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
public async Task<ActionResult<AuthResponseDto>> AuthAction([FromBody] AuthDto dto)
```

## Swagger UI Testing

After adding/updating endpoints:

1. **Build the project**: `dotnet build`
2. **Run the application**: `dotnet run`
3. **Open Swagger UI**: Navigate to `/swagger`
4. **Verify each endpoint shows:**
   - Clear summary
   - All parameters with descriptions
   - Request body schema (for POST/PUT)
   - All response codes with schemas
   - "Try it out" button works
   - Authentication lock icon (if `[Authorize]`)

## Conclusion

Following these standards ensures:
- ✅ Consistent API documentation across all endpoints
- ✅ Professional Swagger UI experience
- ✅ Easy-to-consume API for frontend developers
- ✅ Auto-generated client SDKs work correctly
- ✅ Maintainable codebase (single source of truth)

**Remember:** Code + Attributes = Complete Documentation. No need for separate docs!
