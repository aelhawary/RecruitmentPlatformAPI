# Authentication Module — Frontend Integration Guide

**Last Updated:** February 2026  
**Base URL:** `http://localhost:5217/api/auth`

---

## Table of Contents

1. [Overview](#overview)
2. [Auth Flows Summary](#auth-flows-summary)
3. [Response Format](#response-format)
4. [Endpoints Reference](#endpoints-reference)
   - [POST /register](#1-register)
   - [POST /verify-email](#2-verify-email)
   - [POST /resend-verification](#3-resend-verification)
   - [POST /login](#4-login)
   - [POST /google](#5-google-oauth)
   - [POST /forgot-password](#6-forgot-password)
   - [POST /validate-reset-token](#7-validate-reset-token)
   - [POST /reset-password](#8-reset-password)
   - [GET /me](#9-get-current-user)
5. [Security Features](#security-features)
6. [React Integration](#react-integration)
   - [Project Setup](#project-setup)
   - [Axios Instance](#axios-instance)
   - [Auth API Service](#auth-api-service)
   - [Auth Context & Provider](#auth-context--provider)
   - [Protected Route Component](#protected-route)
   - [Registration Form](#registration-form)
   - [Login Form](#login-form)
   - [Email Verification Page](#email-verification-page)
   - [Google OAuth Button](#google-oauth-button)
   - [Forgot Password Flow](#forgot-password-flow)
7. [Error Handling Patterns](#error-handling-patterns)
8. [Troubleshooting](#troubleshooting)

---

## Overview

The authentication module supports two methods:

| Method | Flow |
|--------|------|
| **Email/Password** | Register → Verify Email → Login |
| **Google OAuth 2.0** | Click "Sign in with Google" → Send ID token to backend → Done |

After successful authentication, the backend returns a **JWT token** (valid for 24 hours) that must be sent with all protected API requests:

```
Authorization: Bearer <token>
```

---

## Auth Flows Summary

### Flow 1: Email/Password Registration

```
1. User fills registration form
2. POST /register → returns success (NO token yet)
3. User receives 6-digit code via email (expires in 15 minutes)
4. POST /verify-email → returns success (NO token yet)
5. User is redirected to login page
6. POST /login → returns JWT token ✅
```

> **Important:** Neither `/register` nor `/verify-email` returns a JWT token. The user MUST log in after verification.

### Flow 2: Google OAuth

```
1. User clicks "Sign in with Google"
2. Google returns an ID token to the frontend
3. POST /google (idToken + accountType) → returns JWT token ✅
```

> For **new** Google users, `accountType` ("JobSeeker" or "Recruiter") is required.  
> For **existing** Google users, it is ignored (the server uses the stored value).

### Flow 3: Password Reset

```
1. POST /forgot-password (email) → always returns 200 OK
2. User clicks the link in email → frontend extracts token from URL
3. POST /validate-reset-token (token) → check if link is still valid
4. POST /reset-password (token + newPassword) → returns success
5. User is redirected to login page
```

---

## Response Format

Every auth endpoint returns the same `AuthResponseDto` shape:

### Success (with token — login/google only)

```json
{
  "success": true,
  "message": "Login successful.",
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "user": {
    "id": 1,
    "firstName": "John",
    "lastName": "Doe",
    "email": "john@example.com",
    "accountType": "JobSeeker",
    "isEmailVerified": true,
    "isActive": true
  }
}
```

### Success (without token — register/verify/reset)

```json
{
  "success": true,
  "message": "Registration successful. Please check your email to verify your account.",
  "token": null,
  "user": {
    "id": 1,
    "firstName": "John",
    "lastName": "Doe",
    "email": "john@example.com",
    "accountType": "JobSeeker",
    "isEmailVerified": false,
    "isActive": true
  }
}
```

### Error

```json
{
  "success": false,
  "message": "Invalid email or password. 3 attempts remaining before account lockout."
}
```

### Account Locked Error (HTTP 423)

```json
{
  "success": false,
  "message": "Account locked due to multiple failed login attempts...",
  "lockoutEnd": "2026-02-19T15:30:00Z",
  "remainingMinutes": 12
}
```

### Validation Error (HTTP 400 — invalid model)

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Email": ["Invalid email format"],
    "Password": ["Password must contain at least one uppercase letter, one lowercase letter, and one digit"]
  }
}
```

> **Note:** Validation errors (ModelState) use a **different shape** than business logic errors. Always check for both formats.

---

## Endpoints Reference

### 1. Register

Creates a new user account and sends a verification email.

```
POST /api/auth/register
```

**Request Body:**

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| `firstName` | string | ✅ | Max 50 chars. Letters, spaces, hyphens, apostrophes, periods only |
| `lastName` | string | ✅ | Max 50 chars. Same rules as firstName |
| `email` | string | ✅ | Valid email format. Max 255 chars |
| `password` | string | ✅ | 8–100 chars. Must contain: 1 uppercase, 1 lowercase, 1 digit |
| `confirmPassword` | string | ✅ | Must match `password` |
| `accountType` | string | ✅ | `"JobSeeker"` or `"Recruiter"` (case-insensitive) |

**Example Request:**

```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "password": "SecurePass123",
  "confirmPassword": "SecurePass123",
  "accountType": "JobSeeker"
}
```

**Responses:**

| Status | Condition |
|--------|-----------|
| `200` | Registration successful — verification email sent |
| `400` | Email already exists OR validation failed |

**Example Success Response:**

```json
{
  "success": true,
  "message": "Registration successful. Please check your email to verify your account.",
  "token": null,
  "user": {
    "id": 1,
    "firstName": "John",
    "lastName": "Doe",
    "email": "john@example.com",
    "accountType": "JobSeeker",
    "isEmailVerified": false,
    "isActive": true
  }
}
```

**Possible Error Messages:**

- `"An account with this email already exists. Try logging in or use 'Forgot password'."`
- `"Invalid accountType. Allowed values: JobSeeker, Recruiter."`

---

### 2. Verify Email

Verifies a user's email using the 6-digit code sent to their inbox.

```
POST /api/auth/verify-email
```

**Request Body:**

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| `email` | string | ✅ | Valid email format |
| `verificationCode` | string | ✅ | Exactly 6 digits |

**Example Request:**

```json
{
  "email": "john@example.com",
  "verificationCode": "482910"
}
```

**Responses:**

| Status | Condition |
|--------|-----------|
| `200` | Email verified — user must now log in |
| `400` | Invalid code, expired code, or user not found |

**Example Success Response:**

```json
{
  "success": true,
  "message": "Email verified successfully! Please log in to continue.",
  "token": null,
  "user": {
    "id": 1,
    "firstName": "John",
    "lastName": "Doe",
    "email": "john@example.com",
    "accountType": "JobSeeker",
    "isEmailVerified": true,
    "isActive": true
  }
}
```

> **Important:** This endpoint does NOT return a JWT token. Redirect the user to the login page after successful verification.

**Possible Error Messages:**

- `"User not found."`
- `"Email is already verified. You can log in now."`
- `"No valid verification code found. Please request a new one."`
- `"Verification code has expired. Please request a new one."`
- `"Invalid verification code. Please use the most recent code sent to your email."`

---

### 3. Resend Verification

Resends a new 6-digit verification code. Invalidates all previous codes.

```
POST /api/auth/resend-verification
```

**Request Body:**

| Field | Type | Required |
|-------|------|----------|
| `email` | string | ✅ |

**Example Request:**

```json
{
  "email": "john@example.com"
}
```

**Responses:**

| Status | Condition |
|--------|-----------|
| `200` | New code sent |
| `400` | User not found or already verified |

**Possible Error Messages:**

- `"User not found."`
- `"Email is already verified."`

---

### 4. Login

Authenticates a user and returns a JWT token.

```
POST /api/auth/login
```

**Request Body:**

| Field | Type | Required |
|-------|------|----------|
| `email` | string | ✅ |
| `password` | string | ✅ |

**Example Request:**

```json
{
  "email": "john@example.com",
  "password": "SecurePass123"
}
```

**Responses:**

| Status | Condition |
|--------|-----------|
| `200` | Login successful — token returned |
| `401` | Invalid credentials |
| `403` | Email not verified OR account deactivated |
| `423` | Account locked (too many failed attempts) |

**Example Success Response:**

```json
{
  "success": true,
  "message": "Login successful.",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "firstName": "John",
    "lastName": "Doe",
    "email": "john@example.com",
    "accountType": "JobSeeker",
    "isEmailVerified": true,
    "isActive": true
  }
}
```

**Possible Error Messages & Frontend Actions:**

| Message | HTTP | Frontend Action |
|---------|------|-----------------|
| `"Invalid email or password. X attempts remaining..."` | 401 | Show warning about lockout |
| `"This account uses Google sign-in..."` | 401 | Redirect to Google login |
| `"Your email address isn't verified yet..."` | 403 | Redirect to verification page |
| `"Your account is deactivated..."` | 403 | Show message, offer support contact |
| `"Account is locked due to multiple failed login attempts..."` | 423 | Show countdown timer using `remainingMinutes` |

---

### 5. Google OAuth

Authenticates (or registers) a user via Google ID token.

```
POST /api/auth/google
```

**Request Body:**

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `idToken` | string | ✅ | The ID token from Google Sign-In |
| `accountType` | string | ✅ | `"JobSeeker"` or `"Recruiter"`. Required for new users; ignored for existing |

**Example Request:**

```json
{
  "idToken": "eyJhbGciOiJSUzI1NiIsImtpZCI6IjE4...",
  "accountType": "JobSeeker"
}
```

**Responses:**

| Status | Condition |
|--------|-----------|
| `200` | Auth successful — token returned |
| `401` | Invalid token, unverified Google email, or account issue |

**Behavior for different scenarios:**

| Scenario | What Happens |
|----------|--------------|
| New user | Account created, JWT returned, welcome email sent |
| Existing email user (registered via email) | Account linked to Google, JWT returned |
| Existing Google user (same Google account) | Login successful, JWT returned |
| Existing Google user (different Google account) | **Rejected** — "associated with a different Google account" |
| Account locked | **Rejected** — lockout enforced same as email login |
| Account deactivated | **Rejected** |

---

### 6. Forgot Password

Initiates the password reset flow. Sends a reset link to the user's email.

```
POST /api/auth/forgot-password
```

**Request Body:**

| Field | Type | Required |
|-------|------|----------|
| `email` | string | ✅ |

**Example Request:**

```json
{
  "email": "john@example.com"
}
```

**Response (always 200 OK):**

```json
{
  "success": true,
  "message": "If your email is associated with an account, you'll receive a password reset link shortly."
}
```

> **Security:** This endpoint **always** returns `200 OK` regardless of whether the email exists. This prevents attackers from discovering which emails are registered.

**Email Contains:** A link like `http://localhost:3000/reset-password?token=<secure-token>`. The token expires in **15 minutes**.

---

### 7. Validate Reset Token

Checks if a password reset token (from the email link) is still valid. Call this when the user opens the reset page.

```
POST /api/auth/validate-reset-token
```

**Request Body:**

| Field | Type | Required |
|-------|------|----------|
| `token` | string | ✅ |

**Example Request:**

```json
{
  "token": "a1b2c3d4e5f6..."
}
```

**Responses:**

| Status | Condition |
|--------|-----------|
| `200` | Token valid — show the reset form |
| `400` | Token invalid, expired, or already used |

**Possible Error Messages:**

- `"Invalid or expired reset link. Please request a new password reset."`
- `"This reset link has expired. Please request a new password reset."`

---

### 8. Reset Password

Resets the user's password using the token from the email link.

```
POST /api/auth/reset-password
```

**Request Body:**

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| `token` | string | ✅ | Token from email link |
| `newPassword` | string | ✅ | 8–100 chars. 1 uppercase, 1 lowercase, 1 digit |
| `confirmNewPassword` | string | ✅ | Must match `newPassword` |

**Example Request:**

```json
{
  "token": "a1b2c3d4e5f6...",
  "newPassword": "NewSecure456",
  "confirmNewPassword": "NewSecure456"
}
```

**Responses:**

| Status | Condition |
|--------|-----------|
| `200` | Password reset successful — redirect to login |
| `400` | Invalid/expired token or validation failed |

> **Note:** A successful password reset automatically clears any account lockout.

---

### 9. Get Current User

Returns the authenticated user's info from the JWT token. Use this to verify a stored token is still valid (e.g., on page refresh).

```
GET /api/auth/me
Authorization: Bearer <token>
```

**No request body.**

**Responses:**

| Status | Condition |
|--------|-----------|
| `200` | Token valid — returns user info |
| `401` | Token invalid, expired, or missing |

**Example Success Response:**

```json
{
  "success": true,
  "user": {
    "id": "1",
    "email": "john@example.com",
    "name": "John Doe",
    "role": "JobSeeker",
    "firstName": "John",
    "lastName": "Doe"
  }
}
```

> **Note:** The `id` here is a string (extracted from JWT claims). The `role` corresponds to `accountType`.

---

## Security Features

### Password Policy

| Rule | Details |
|------|---------|
| Minimum length | 8 characters |
| Maximum length | 100 characters |
| Requirements | At least 1 uppercase, 1 lowercase, 1 digit |
| Hashing | BCrypt (server-side) |
| Frontend regex | `^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$` |

### Account Lockout

| Rule | Details |
|------|---------|
| Failed attempts allowed | 5 |
| Lockout duration | 15 minutes (auto-unlock) |
| Lockout notification | Email sent to user when account is locked |
| Lockout cleared by | Successful login, password reset, or waiting 15 min |
| Applies to | Both `/login` and `/google` endpoints |

When an account is locked, the response includes `lockoutEnd` (ISO date) and `remainingMinutes` (integer).

### Email Verification Codes

| Rule | Details |
|------|---------|
| Code format | 6 digits |
| Expiry | 15 minutes |
| Resend behavior | Invalidates all previous codes, sends new one |
| Comparison | Constant-time (prevents timing attacks) |

### Password Reset Tokens

| Rule | Details |
|------|---------|
| Expiry | 15 minutes |
| Single-use | Token is invalidated after one successful reset |
| New request | Invalidates all previous unused tokens |

### JWT Token

| Rule | Details |
|------|---------|
| Algorithm | HMAC-SHA256 |
| Expiry | 24 hours |
| Claims | `sub` (userId), `email`, `name`, `role`, `FirstName`, `LastName` |
| Storage | `localStorage` (or `httpOnly` cookie in production) |

### CORS

The backend allows requests from any `localhost` origin during development. For production, configure the specific frontend domain in the backend CORS policy.

---

## React Integration

### Project Setup

```bash
npm install axios @react-oauth/google react-router-dom
```

**Environment variables** (`.env`):

```env
REACT_APP_API_URL=http://localhost:5217/api
REACT_APP_GOOGLE_CLIENT_ID=1094518034372-ka3p6p1dc6ur5d9os4pula12d2u9e7jl.apps.googleusercontent.com
```

---

### Axios Instance

Create a shared Axios instance that automatically attaches the JWT token to every request.

```typescript
// src/api/axios.ts
import axios from "axios";

const api = axios.create({
  baseURL: process.env.REACT_APP_API_URL,
  headers: { "Content-Type": "application/json" },
});

// Attach JWT token to every request
api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Handle 401 globally (expired/invalid token)
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem("token");
      localStorage.removeItem("user");
      window.location.href = "/login";
    }
    return Promise.reject(error);
  }
);

export default api;
```

---

### Auth API Service

```typescript
// src/api/authService.ts
import api from "./axios";

// ─── Types ───────────────────────────────────────────────
export interface UserInfo {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  accountType: "JobSeeker" | "Recruiter";
  isEmailVerified: boolean;
  isActive: boolean;
}

export interface AuthResponse {
  success: boolean;
  message: string;
  token?: string;
  resetToken?: string;
  lockoutEnd?: string;
  remainingMinutes?: number;
  user?: UserInfo;
}

export interface CurrentUserResponse {
  success: boolean;
  user: {
    id: string;
    email: string;
    name: string;
    role: string;
    firstName: string;
    lastName: string;
  };
}

// ─── API Calls ───────────────────────────────────────────
export const authService = {
  register: (data: {
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    confirmPassword: string;
    accountType: "JobSeeker" | "Recruiter";
  }) => api.post<AuthResponse>("/auth/register", data),

  login: (email: string, password: string) =>
    api.post<AuthResponse>("/auth/login", { email, password }),

  googleAuth: (idToken: string, accountType: "JobSeeker" | "Recruiter") =>
    api.post<AuthResponse>("/auth/google", { idToken, accountType }),

  verifyEmail: (email: string, verificationCode: string) =>
    api.post<AuthResponse>("/auth/verify-email", { email, verificationCode }),

  resendVerification: (email: string) =>
    api.post<AuthResponse>("/auth/resend-verification", { email }),

  forgotPassword: (email: string) =>
    api.post<AuthResponse>("/auth/forgot-password", { email }),

  validateResetToken: (token: string) =>
    api.post<AuthResponse>("/auth/validate-reset-token", { token }),

  resetPassword: (
    token: string,
    newPassword: string,
    confirmNewPassword: string
  ) =>
    api.post<AuthResponse>("/auth/reset-password", {
      token,
      newPassword,
      confirmNewPassword,
    }),

  getCurrentUser: () => api.get<CurrentUserResponse>("/auth/me"),
};
```

---

### Auth Context & Provider

```tsx
// src/context/AuthContext.tsx
import React, {
  createContext,
  useContext,
  useState,
  useEffect,
  useCallback,
} from "react";
import { authService, UserInfo } from "../api/authService";

interface AuthContextType {
  user: UserInfo | null;
  token: string | null;
  isLoading: boolean;
  login: (email: string, password: string) => Promise<string>;
  googleLogin: (
    idToken: string,
    accountType: "JobSeeker" | "Recruiter"
  ) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType>(null!);
export const useAuth = () => useContext(AuthContext);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [user, setUser] = useState<UserInfo | null>(null);
  const [token, setToken] = useState<string | null>(
    localStorage.getItem("token")
  );
  const [isLoading, setIsLoading] = useState(true);

  // On mount: verify stored token is still valid
  useEffect(() => {
    const verifyToken = async () => {
      if (!token) {
        setIsLoading(false);
        return;
      }
      try {
        const { data } = await authService.getCurrentUser();
        if (data.success) {
          const storedUser = localStorage.getItem("user");
          if (storedUser) {
            setUser(JSON.parse(storedUser));
          }
        }
      } catch {
        localStorage.removeItem("token");
        localStorage.removeItem("user");
        setToken(null);
        setUser(null);
      } finally {
        setIsLoading(false);
      }
    };
    verifyToken();
  }, [token]);

  const login = useCallback(
    async (email: string, password: string): Promise<string> => {
      const { data } = await authService.login(email, password);
      if (!data.success) {
        throw { response: { data } };
      }
      localStorage.setItem("token", data.token!);
      localStorage.setItem("user", JSON.stringify(data.user));
      setToken(data.token!);
      setUser(data.user!);
      return data.user!.accountType;
    },
    []
  );

  const googleLogin = useCallback(
    async (idToken: string, accountType: "JobSeeker" | "Recruiter") => {
      const { data } = await authService.googleAuth(idToken, accountType);
      if (!data.success) {
        throw { response: { data } };
      }
      localStorage.setItem("token", data.token!);
      localStorage.setItem("user", JSON.stringify(data.user));
      setToken(data.token!);
      setUser(data.user!);
    },
    []
  );

  const logout = useCallback(() => {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    setToken(null);
    setUser(null);
  }, []);

  return (
    <AuthContext.Provider
      value={{ user, token, isLoading, login, googleLogin, logout }}
    >
      {children}
    </AuthContext.Provider>
  );
};
```

---

### Protected Route

```tsx
// src/components/ProtectedRoute.tsx
import { Navigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

interface Props {
  children: React.ReactNode;
  allowedRoles?: ("JobSeeker" | "Recruiter")[];
}

const ProtectedRoute: React.FC<Props> = ({ children, allowedRoles }) => {
  const { user, token, isLoading } = useAuth();

  if (isLoading) return <div>Loading...</div>;
  if (!token) return <Navigate to="/login" replace />;
  if (allowedRoles && user && !allowedRoles.includes(user.accountType)) {
    return <Navigate to="/unauthorized" replace />;
  }

  return <>{children}</>;
};

export default ProtectedRoute;
```

**Usage in Router:**

```tsx
<Route
  path="/dashboard"
  element={
    <ProtectedRoute allowedRoles={["JobSeeker"]}>
      <Dashboard />
    </ProtectedRoute>
  }
/>

<Route
  path="/recruiter/dashboard"
  element={
    <ProtectedRoute allowedRoles={["Recruiter"]}>
      <RecruiterDashboard />
    </ProtectedRoute>
  }
/>
```

---

### Registration Form

```tsx
// src/pages/RegisterPage.tsx
import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { authService } from "../api/authService";

const RegisterPage: React.FC = () => {
  const navigate = useNavigate();
  const [form, setForm] = useState({
    firstName: "",
    lastName: "",
    email: "",
    password: "",
    confirmPassword: "",
    accountType: "JobSeeker" as "JobSeeker" | "Recruiter",
  });
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setLoading(true);

    try {
      const { data } = await authService.register(form);
      if (data.success) {
        // Redirect to verification page, passing the email
        navigate("/verify-email", { state: { email: form.email } });
      } else {
        setError(data.message);
      }
    } catch (err: any) {
      // Handle validation errors (different shape from business errors)
      if (err.response?.data?.errors) {
        const messages = Object.values(err.response.data.errors).flat();
        setError((messages as string[]).join(" "));
      } else if (err.response?.data?.message) {
        setError(err.response.data.message);
      } else {
        setError("Registration failed. Please try again.");
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      {error && <div className="error">{error}</div>}

      <input
        placeholder="First Name"
        value={form.firstName}
        onChange={(e) => setForm({ ...form, firstName: e.target.value })}
        required
      />
      <input
        placeholder="Last Name"
        value={form.lastName}
        onChange={(e) => setForm({ ...form, lastName: e.target.value })}
        required
      />
      <input
        type="email"
        placeholder="Email"
        value={form.email}
        onChange={(e) => setForm({ ...form, email: e.target.value })}
        required
      />
      <input
        type="password"
        placeholder="Password (min 8 chars, A-Z, a-z, 0-9)"
        value={form.password}
        onChange={(e) => setForm({ ...form, password: e.target.value })}
        required
      />
      <input
        type="password"
        placeholder="Confirm Password"
        value={form.confirmPassword}
        onChange={(e) =>
          setForm({ ...form, confirmPassword: e.target.value })
        }
        required
      />
      <select
        value={form.accountType}
        onChange={(e) =>
          setForm({
            ...form,
            accountType: e.target.value as "JobSeeker" | "Recruiter",
          })
        }
      >
        <option value="JobSeeker">Job Seeker</option>
        <option value="Recruiter">Recruiter</option>
      </select>

      <button type="submit" disabled={loading}>
        {loading ? "Registering..." : "Register"}
      </button>
    </form>
  );
};

export default RegisterPage;
```

---

### Login Form

```tsx
// src/pages/LoginPage.tsx
import React, { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

const LoginPage: React.FC = () => {
  const navigate = useNavigate();
  const { login } = useAuth();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [lockout, setLockout] = useState<{
    end: string;
    minutes: number;
  } | null>(null);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setLockout(null);
    setLoading(true);

    try {
      const accountType = await login(email, password);
      navigate(
        accountType === "Recruiter" ? "/recruiter/dashboard" : "/dashboard"
      );
    } catch (err: any) {
      const data = err.response?.data;
      if (data) {
        setError(data.message);

        // Handle account lockout (HTTP 423)
        if (data.lockoutEnd) {
          setLockout({ end: data.lockoutEnd, minutes: data.remainingMinutes });
        }

        // Handle unverified email (HTTP 403)
        if (data.message?.includes("not verified")) {
          setError(data.message + " Redirecting to verification...");
          setTimeout(
            () => navigate("/verify-email", { state: { email } }),
            2000
          );
        }

        // Handle Google-only account
        if (data.message?.includes("Google sign-in")) {
          setError(
            "This account was created with Google. Please use the Google login button."
          );
        }
      } else {
        setError("Login failed. Please try again.");
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      {error && <div className="error">{error}</div>}
      {lockout && (
        <div className="lockout-warning">
          Account locked. Try again in {lockout.minutes} minute(s).
        </div>
      )}

      <input
        type="email"
        placeholder="Email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        required
      />
      <input
        type="password"
        placeholder="Password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        required
      />

      <button type="submit" disabled={loading || !!lockout}>
        {loading ? "Logging in..." : "Login"}
      </button>

      <Link to="/forgot-password">Forgot password?</Link>
      <Link to="/register">Don't have an account? Register</Link>
    </form>
  );
};

export default LoginPage;
```

---

### Email Verification Page

```tsx
// src/pages/VerifyEmailPage.tsx
import React, { useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { authService } from "../api/authService";

const VerifyEmailPage: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const passedEmail = (location.state as any)?.email || "";

  const [email] = useState(passedEmail);
  const [code, setCode] = useState("");
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const handleVerify = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setMessage("");
    setLoading(true);

    try {
      const { data } = await authService.verifyEmail(email, code);
      if (data.success) {
        setMessage("Email verified! Redirecting to login...");
        setTimeout(() => navigate("/login"), 2000);
      } else {
        setError(data.message);
      }
    } catch (err: any) {
      setError(err.response?.data?.message || "Verification failed.");
    } finally {
      setLoading(false);
    }
  };

  const handleResend = async () => {
    try {
      const { data } = await authService.resendVerification(email);
      if (data.success) {
        setMessage("New verification code sent! Check your email.");
        setError("");
      } else {
        setError(data.message);
      }
    } catch (err: any) {
      setError(err.response?.data?.message || "Failed to resend code.");
    }
  };

  return (
    <div>
      <h2>Verify Your Email</h2>
      <p>
        Enter the 6-digit code sent to <strong>{email}</strong>
      </p>

      {message && <div className="success">{message}</div>}
      {error && <div className="error">{error}</div>}

      <form onSubmit={handleVerify}>
        <input
          placeholder="6-digit code"
          value={code}
          onChange={(e) => setCode(e.target.value)}
          maxLength={6}
          pattern="\d{6}"
          required
        />
        <button type="submit" disabled={loading}>
          {loading ? "Verifying..." : "Verify"}
        </button>
      </form>

      <button onClick={handleResend} type="button">
        Resend Code
      </button>
    </div>
  );
};

export default VerifyEmailPage;
```

---

### Google OAuth Button

Install the Google OAuth library:

```bash
npm install @react-oauth/google
```

**Wrap your app** in `index.tsx` or `App.tsx`:

```tsx
import { GoogleOAuthProvider } from "@react-oauth/google";

<GoogleOAuthProvider clientId={process.env.REACT_APP_GOOGLE_CLIENT_ID!}>
  <App />
</GoogleOAuthProvider>;
```

**Google Login Button component:**

```tsx
// src/components/GoogleLoginButton.tsx
import React, { useState } from "react";
import { GoogleLogin, CredentialResponse } from "@react-oauth/google";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

interface Props {
  accountType: "JobSeeker" | "Recruiter";
}

const GoogleLoginButton: React.FC<Props> = ({ accountType }) => {
  const navigate = useNavigate();
  const { googleLogin } = useAuth();
  const [error, setError] = useState("");

  const handleSuccess = async (response: CredentialResponse) => {
    if (!response.credential) {
      setError("No credential received from Google.");
      return;
    }

    try {
      await googleLogin(response.credential, accountType);
      navigate("/dashboard");
    } catch (err: any) {
      setError(
        err.response?.data?.message || "Google authentication failed."
      );
    }
  };

  return (
    <div>
      {error && <div className="error">{error}</div>}
      <GoogleLogin
        onSuccess={handleSuccess}
        onError={() => setError("Google Sign-In failed.")}
        text="continue_with"
        shape="rectangular"
        width="300"
      />
    </div>
  );
};

export default GoogleLoginButton;
```

**Usage on login/register pages:**

```tsx
<GoogleLoginButton accountType="JobSeeker" />
```

> **Note:** When an existing user (who registered via email) signs in with Google using the same email, the backend automatically links the accounts. No special frontend handling is needed.

---

### Forgot Password Flow

**Step 1 — Request Reset Link:**

```tsx
// src/pages/ForgotPasswordPage.tsx
import React, { useState } from "react";
import { authService } from "../api/authService";

const ForgotPasswordPage: React.FC = () => {
  const [email, setEmail] = useState("");
  const [submitted, setSubmitted] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    await authService.forgotPassword(email);
    setSubmitted(true); // Always show success (backend always returns 200)
  };

  if (submitted) {
    return (
      <div>
        <h2>Check Your Email</h2>
        <p>
          If an account exists for <strong>{email}</strong>, a password reset
          link has been sent.
        </p>
        <p>The link expires in 15 minutes.</p>
      </div>
    );
  }

  return (
    <form onSubmit={handleSubmit}>
      <h2>Forgot Password</h2>
      <input
        type="email"
        placeholder="Enter your email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        required
      />
      <button type="submit">Send Reset Link</button>
    </form>
  );
};

export default ForgotPasswordPage;
```

**Step 2 — Reset Password Page** (user arrives from email link, e.g., `/reset-password?token=abc123`):

```tsx
// src/pages/ResetPasswordPage.tsx
import React, { useState, useEffect } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { authService } from "../api/authService";

const ResetPasswordPage: React.FC = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const token = searchParams.get("token") || "";

  const [isValidToken, setIsValidToken] = useState<boolean | null>(null);
  const [newPassword, setNewPassword] = useState("");
  const [confirmNewPassword, setConfirmNewPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  // Validate token on page load
  useEffect(() => {
    if (!token) {
      setIsValidToken(false);
      return;
    }
    authService
      .validateResetToken(token)
      .then(({ data }) => setIsValidToken(data.success))
      .catch(() => setIsValidToken(false));
  }, [token]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setLoading(true);

    try {
      const { data } = await authService.resetPassword(
        token,
        newPassword,
        confirmNewPassword
      );
      if (data.success) {
        navigate("/login", {
          state: { message: "Password reset successful! Please log in." },
        });
      } else {
        setError(data.message);
      }
    } catch (err: any) {
      if (err.response?.data?.errors) {
        const messages = Object.values(err.response.data.errors).flat();
        setError((messages as string[]).join(" "));
      } else {
        setError(
          err.response?.data?.message || "Password reset failed."
        );
      }
    } finally {
      setLoading(false);
    }
  };

  if (isValidToken === null) return <div>Validating link...</div>;
  if (isValidToken === false) {
    return (
      <div>
        <h2>Invalid or Expired Link</h2>
        <p>This password reset link is invalid or has expired.</p>
        <a href="/forgot-password">Request a new reset link</a>
      </div>
    );
  }

  return (
    <form onSubmit={handleSubmit}>
      <h2>Set New Password</h2>
      {error && <div className="error">{error}</div>}

      <input
        type="password"
        placeholder="New password (min 8 chars, A-Z, a-z, 0-9)"
        value={newPassword}
        onChange={(e) => setNewPassword(e.target.value)}
        required
      />
      <input
        type="password"
        placeholder="Confirm new password"
        value={confirmNewPassword}
        onChange={(e) => setConfirmNewPassword(e.target.value)}
        required
      />

      <button type="submit" disabled={loading}>
        {loading ? "Resetting..." : "Reset Password"}
      </button>
    </form>
  );
};

export default ResetPasswordPage;
```

---

## Error Handling Patterns

### Two Error Shapes to Handle

The backend returns errors in **two different formats**. Always handle both:

```typescript
// src/utils/extractError.ts
export function extractError(err: any): string {
  const data = err.response?.data;

  // Shape 1: Business logic error (AuthResponseDto)
  if (data?.message) {
    return data.message;
  }

  // Shape 2: Validation error (ASP.NET ModelState)
  if (data?.errors) {
    return Object.values(data.errors).flat().join(" ");
  }

  return "Something went wrong. Please try again.";
}

// Usage:
// catch (err) { setError(extractError(err)); }
```

### HTTP Status Code Reference

| Status | Meaning | Action |
|--------|---------|--------|
| `200` | Success | Process response normally |
| `400` | Validation or business error | Display `message` or `errors` |
| `401` | Unauthorized (bad credentials or expired JWT) | Redirect to login |
| `403` | Forbidden (unverified email or deactivated) | Show specific message |
| `423` | Account locked | Show countdown timer |

---

## Troubleshooting

| Issue | Cause | Fix |
|-------|-------|-----|
| CORS error in browser | Frontend origin not allowed | Backend allows all `localhost` origins during development. Make sure your URL is `http://localhost:XXXX` |
| 401 on all protected endpoints | Token expired or missing | Check `localStorage.getItem("token")` exists and the Axios interceptor is setting the `Authorization` header |
| "This account uses Google sign-in" | User registered via Google, trying email login | Show a message to use the Google button instead |
| Verification code not arriving | Email may be in spam | Check spam folder. Backend logs show email send status |
| Password validation rejected | Missing uppercase, lowercase, or digit | Validate on frontend before submitting: `/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/` |
| `accountType` rejected on register | Value not recognized | Use exact values: `"JobSeeker"` or `"Recruiter"` (case-insensitive on backend, but use exact casing for consistency) |
| Google sign-in popup blocked | Browser blocking popups | User must allow popups for the site |
| Reset link expired | Token is only valid 15 minutes | User must request a new link via `/forgot-password` |
| Login says "email not verified" | User registered but didn't verify | Redirect to `/verify-email` with the email in state |
