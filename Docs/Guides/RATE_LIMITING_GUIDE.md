# Rate Limiting Implementation Guide

## Overview
Rate limiting has been successfully implemented to protect the authentication endpoints from abuse, brute force attacks, and resource exhaustion.

## Protected Endpoints

The following authentication endpoints are now protected with rate limiting:

| Endpoint | Rate Limit | Time Period | Purpose |
|----------|-----------|-------------|---------|
| `POST /api/auth/login` | 20 requests | 15 minutes | Prevents brute force password attacks |
| `POST /api/auth/verify-email` | 10 requests | 1 hour | Prevents OTP guessing attacks |
| `POST /api/auth/verify-reset-otp` | 10 requests | 1 hour | Prevents password reset OTP guessing |
| `POST /api/auth/resend-verification` | 3 requests | 1 hour | Prevents email flooding/spam |
| `POST /api/auth/forgot-password` | 3 requests | 1 hour | Prevents email flooding attacks |
| `POST /api/auth/register` | 10 requests | 1 hour | Prevents spam registrations |
| `POST /api/auth/google` | 10 requests | 1 hour | Prevents OAuth abuse and token replay attacks |

## Security Benefits

### 🛡️ Attack Prevention
- **Brute Force Protection**: Login endpoint limited to 20 attempts per 15 minutes
- **OAuth Abuse Prevention**: Google OAuth limited to 10 attempts per 1 hour
- **OTP Guessing Prevention**: OTP verification endpoints limited to 10 attempts per hour
- **Email Flooding Protection**: Email-sending endpoints limited to 3 requests per hour
- **Spam Registration Prevention**: Registration limited to 10 accounts per hour per IP

### 💰 Resource Protection
- Protects Gmail SMTP quota (2,000 emails/day)
- Reduces unnecessary database queries
- Prevents server resource exhaustion
- Improves performance for legitimate users

## Technical Implementation

### Package Used
- **AspNetCoreRateLimit** v5.0.0 - Production-ready rate limiting library

### Configuration Files Modified

1. **RecruitmentPlatformAPI.csproj**
   - Added AspNetCoreRateLimit package reference

2. **appsettings.json & appsettings.Development.json**
   - Added `IpRateLimiting` configuration section
   - Defined rate limits for each endpoint
   - Configured HTTP 429 response format

3. **Program.cs**
   - Added rate limiting services and middleware
   - Configured in-memory caching for rate limit counters
   - Positioned middleware correctly in the pipeline

## Rate Limit Response

When a rate limit is exceeded, the API returns:

**Status Code:** `429 Too Many Requests`

**Response Body:**
```json
API quota exceeded for [IP_ADDRESS]. Retry after [RETRY_TIME].
```

**Headers:**
- `Retry-After`: Time in seconds until the limit resets
- `X-Rate-Limit-Limit`: Maximum number of requests allowed in the period
- `X-Rate-Limit-Remaining`: Number of requests remaining in current period
- `X-Rate-Limit-Reset`: Timestamp when the limit resets (Unix epoch)

**Example Test Result:**
```
Request #1: 401 ✓
Request #2: 401 ✓
Request #3: 401 ✓
Request #4: 401 ✓
Request #5: 401 ✓
Request #6: 429 ✓ RATE LIMIT TRIGGERED!
```

## Testing Rate Limits

### Using cURL (PowerShell)

```powershell
# Test login rate limit (5 requests in 15 minutes)
for ($i=1; $i -le 6; $i++) {
    Write-Host "Request $i"
    curl -X POST http://localhost:5217/api/auth/login `
         -H "Content-Type: application/json" `
         -d '{"email":"test@example.com","password":"test123"}'
}
# The 6th request should return 429
```

### Using Postman

1. Create a request to any protected endpoint
2. Send it multiple times rapidly
3. After exceeding the limit, observe the 429 response
4. Check the `Retry-After` header for wait time

## Configuration Customization

To adjust rate limits, modify the `IpRateLimiting` section in `appsettings.json`:

```json
"GeneralRules": [
  {
    "Endpoint": "POST:/api/auth/login",
    "Period": "15m",  // Time period: s, m, h, d
    "Limit": 5        // Number of requests allowed
  }
]
```

### Period Format
- `s` = seconds
- `m` = minutes
- `h` = hours
- `d` = days

## Production Considerations

### For High-Traffic Applications
Consider using distributed caching (Redis) instead of in-memory storage:

```csharp
// In Program.cs, replace:
builder.Services.AddInMemoryRateLimiting();

// With:
builder.Services.AddDistributedRateLimiting<AsyncKeyLockProcessingStrategy>();
builder.Services.AddDistributedRateLimiting<RedisProcessingStrategy>();
builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
    ConnectionMultiplexer.Connect("your-redis-connection-string"));
```

### Behind Load Balancers/Proxies
The configuration uses `X-Real-IP` header to identify clients behind proxies. Ensure your load balancer/reverse proxy sets this header:

**nginx:**
```nginx
proxy_set_header X-Real-IP $remote_addr;
```

**IIS/Azure App Service:**
Already handled automatically

### IP Whitelisting
To exclude specific IPs from rate limiting (e.g., monitoring services):

```json
"IpRateLimiting": {
  "IpWhitelist": [ "127.0.0.1", "::1" ]
}
```

## Monitoring

### Logging
Rate limit violations are automatically logged. Check application logs for:
- `AspNetCoreRateLimit.Middleware.RateLimitMiddleware` entries
- HTTP 429 responses in access logs

### Metrics to Monitor
- Number of 429 responses per endpoint
- Rate limit hit ratio (blocked vs. allowed)
- Most frequently blocked IPs
- Peak traffic patterns

## Troubleshooting

### Issue: Legitimate users getting blocked
**Solution:** Increase the limit or time period for the specific endpoint

### Issue: Rate limits not working
**Checklist:**
1. Verify `app.UseIpRateLimiting()` is before `app.UseAuthentication()`
2. Check `appsettings.json` has `IpRateLimiting` section
3. Ensure `EnableEndpointRateLimiting` is `true`
4. Verify package is installed: `dotnet list package | Select-String "AspNetCoreRateLimit"`

### Issue: All requests blocked immediately
**Cause:** Incorrect endpoint path format
**Solution:** Ensure endpoints use format: `METHOD:/api/path` (case-sensitive)

## Future Enhancements

- [ ] Implement user-based rate limiting (in addition to IP-based)
- [ ] Add client ID rate limiting for mobile apps
- [ ] Integrate with Redis for distributed rate limiting
- [ ] Add rate limit metrics dashboard
- [ ] Implement exponential backoff for repeated violations
- [ ] Add email notifications for suspicious activity

## References

- [AspNetCoreRateLimit Documentation](https://github.com/stefanprodan/AspNetCoreRateLimit)
- [OWASP Rate Limiting Guidelines](https://owasp.org/www-community/controls/Blocking_Brute_Force_Attacks)
- [RFC 6585 - HTTP Status Code 429](https://tools.ietf.org/html/rfc6585)

---

**Implementation Date:** December 5, 2025  
**Version:** 1.0  
**Status:** ✅ Active & Production-Ready
