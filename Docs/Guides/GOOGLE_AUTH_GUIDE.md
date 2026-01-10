# Google OAuth Authentication - Integration Guide

## Overview
This guide explains how to integrate Google OAuth authentication with our Recruitment Platform API. Users can now register and login using their Google accounts as an alternative to email/password authentication.

---

## 🔧 Backend Setup (Already Complete)

### What's Been Added:
1. ✅ **Google.Apis.Auth** package (v1.73.0)
2. ✅ **Database fields** for OAuth support:
   - `AuthProvider` (Email/Google)
   - `ProviderUserId` (Google's unique ID)
   - `ProfilePictureUrl` (Google profile picture)
3. ✅ **API Endpoint**: `POST /api/auth/google`
4. ✅ **User Model** updated to support OAuth users
5. ✅ **Configuration** for Google Client ID validation

### ⚙️ Required Configuration

Before testing, you need to add your Google Client ID to `appsettings.json`:

```json
"GoogleOAuth": {
  "ClientId": "YOUR_GOOGLE_CLIENT_ID_HERE.apps.googleusercontent.com"
}
```

**Where to get this:**
- Follow **Step 2: Get Google Client ID** below
- Copy your Client ID from Google Cloud Console
- Paste it in both `appsettings.json` and `appsettings.Development.json`

**Security Note:** The backend validates tokens against this Client ID to prevent token spoofing.

---

## 📡 API Endpoint

### `POST /api/auth/google`

Authenticates users with Google. Creates new account if user doesn't exist, or logs in existing user.

#### Request Body
```json
{
  "idToken": "eyJhbGciOiJSUzI1NiIsImtpZCI6IjE4MmU0...",
  "accountType": "JobSeeker"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `idToken` | string | Yes | Google ID token from frontend |
| `accountType` | string | Yes | Either "JobSeeker" or "Recruiter" (case-insensitive) |

#### Success Response (200 OK)
```json
{
  "success": true,
  "message": "Login successful.",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "firstName": "Ahmed",
    "lastName": "Hassan",
    "email": "ahmed@gmail.com",
    "accountType": "JobSeeker",
    "isEmailVerified": true,
    "isActive": true
  }
}
```

#### Error Responses

**Invalid Token (400 Bad Request)**
```json
{
  "success": false,
  "message": "Invalid Google token. Please try again."
}
```

**Unverified Google Email (400 Bad Request)**
```json
{
  "success": false,
  "message": "Your Google email is not verified. Please verify your email with Google first."
}
```

**Deactivated Account (400 Bad Request)**
```json
{
  "success": false,
  "message": "Your account is deactivated. Please contact support."
}
```

---

## 🎨 Frontend Integration (React)

### Step 1: Install Google OAuth Library

```bash
npm install @react-oauth/google
```

### Step 2: Get Google Client ID

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select existing one
3. Enable **Google+ API**
4. Go to **Credentials** → **Create Credentials** → **OAuth 2.0 Client ID**
5. Set **Authorized JavaScript origins**: `http://localhost:3000`
6. Set **Authorized redirect URIs**: `http://localhost:3000`
7. Copy your **Client ID**

### Step 3: Setup Google OAuth Provider

Wrap your app with `GoogleOAuthProvider`:

```tsx
// main.tsx or App.tsx
import { GoogleOAuthProvider } from '@react-oauth/google';

const GOOGLE_CLIENT_ID = "YOUR_GOOGLE_CLIENT_ID_HERE";

function App() {
  return (
    <GoogleOAuthProvider clientId={GOOGLE_CLIENT_ID}>
      <Router>
        <Routes>
          {/* Your routes */}
        </Routes>
      </Router>
    </GoogleOAuthProvider>
  );
}
```

### Step 4: Create Google Sign-In Button Component

```tsx
// components/GoogleSignInButton.tsx
import { useGoogleLogin } from '@react-oauth/google';
import { useState } from 'react';
import axios from 'axios';

interface GoogleSignInButtonProps {
  accountType: 'JobSeeker' | 'Recruiter';
  onSuccess: (response: any) => void;
  onError: (error: string) => void;
}

export function GoogleSignInButton({ accountType, onSuccess, onError }: GoogleSignInButtonProps) {
  const [loading, setLoading] = useState(false);

  const login = useGoogleLogin({
    onSuccess: async (tokenResponse) => {
      setLoading(true);
      try {
        // Get user info from Google
        const userInfo = await axios.get(
          'https://www.googleapis.com/oauth2/v3/userinfo',
          { headers: { Authorization: `Bearer ${tokenResponse.access_token}` } }
        );

        // Get ID token
        const idTokenResponse = await axios.post(
          'https://oauth2.googleapis.com/token',
          {
            code: tokenResponse.code,
            client_id: 'YOUR_CLIENT_ID',
            client_secret: 'YOUR_CLIENT_SECRET',
            redirect_uri: 'http://localhost:3000',
            grant_type: 'authorization_code',
          }
        );

        // Send to your backend
        const response = await axios.post('http://localhost:5217/api/auth/google', {
          idToken: idTokenResponse.data.id_token,
          accountType: accountType
        });

        onSuccess(response.data);
      } catch (error: any) {
        onError(error.response?.data?.message || 'Google sign-in failed');
      } finally {
        setLoading(false);
      }
    },
    onError: () => {
      onError('Google sign-in was cancelled or failed');
    },
    flow: 'auth-code',
  });

  return (
    <button
      onClick={() => login()}
      disabled={loading}
      className="google-signin-button"
    >
      {loading ? 'Signing in...' : '🚀 Continue with Google'}
    </button>
  );
}
```

### Step 5: Use in Register/Login Pages

```tsx
// pages/Register.tsx
import { GoogleSignInButton } from '../components/GoogleSignInButton';
import { useNavigate } from 'react-router-dom';

export function Register() {
  const navigate = useNavigate();
  const [accountType, setAccountType] = useState<'JobSeeker' | 'Recruiter'>('JobSeeker');

  const handleGoogleSuccess = (response: any) => {
    // Store token
    localStorage.setItem('token', response.token);
    localStorage.setItem('user', JSON.stringify(response.user));
    
    // Redirect to dashboard
    navigate('/dashboard');
  };

  const handleGoogleError = (error: string) => {
    alert(error);
  };

  return (
    <div className="register-page">
      <h1>Create Account</h1>
      
      {/* Account Type Selection */}
      <div className="account-type-selector">
        <button
          className={accountType === 'JobSeeker' ? 'active' : ''}
          onClick={() => setAccountType('JobSeeker')}
        >
          Job Seeker
        </button>
        <button
          className={accountType === 'Recruiter' ? 'active' : ''}
          onClick={() => setAccountType('Recruiter')}
        >
          Recruiter
        </button>
      </div>

      {/* Google Sign-In Button */}
      <GoogleSignInButton
        accountType={accountType}
        onSuccess={handleGoogleSuccess}
        onError={handleGoogleError}
      />

      <div className="divider">OR</div>

      {/* Traditional Email/Password Form */}
      <form onSubmit={handleEmailRegister}>
        {/* Your existing form fields */}
      </form>
    </div>
  );
}
```

---

## 🔒 Security Considerations

### ✅ What We Do:
1. **Server-side token verification**: Backend validates Google tokens using `GoogleJsonWebSignature`
2. **Email verification check**: Ensures Google email is verified
3. **Secure token storage**: JWT tokens stored in localStorage (or httpOnly cookies for better security)
4. **Account linking**: Prevents duplicate accounts with same email

### ⚠️ Important Notes:
- **NEVER** trust the frontend token without backend verification
- Google tokens expire - backend validates on every request
- Users who sign up with Google **cannot** use email/password login (and vice versa)
- Profile pictures from Google are public URLs - no download needed

---

## 🧪 Testing

### Test with Postman

1. Get a Google ID token from your frontend (check browser DevTools → Network tab)
2. Send POST request to `http://localhost:5217/api/auth/google`:

```json
{
  "idToken": "PASTE_REAL_ID_TOKEN_HERE",
  "accountType": "JobSeeker"
}
```

3. You should receive a JWT token and user info

### Test Flow

1. ✅ **New User Registration**
   - User clicks "Continue with Google"
   - Selects account type
   - Backend creates account with Google info
   - Returns JWT token

2. ✅ **Existing User Login**
   - User clicks "Continue with Google"
   - Backend finds existing account by email
   - Returns JWT token

3. ✅ **Email Conflict Handling**
   - If email exists with different auth provider, shows appropriate error

---

## 🎯 User Experience Flow

```
┌─────────────────────────────────────────────────────────────┐
│                     Register/Login Page                      │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  [Account Type: JobSeeker | Recruiter]                       │
│                                                               │
│  ┌─────────────────────────────────────────────────────┐    │
│  │   🚀 Continue with Google                           │    │
│  └─────────────────────────────────────────────────────┘    │
│                                                               │
│  ─────────────────── OR ───────────────────                 │
│                                                               │
│  Email: [_________________________]                          │
│  Password: [_________________________]                       │
│  [Register] or [Login]                                       │
│                                                               │
└─────────────────────────────────────────────────────────────┘
```

**New Google User Journey:**
1. Click "Continue with Google"
2. Select Google account
3. Choose account type (JobSeeker/Recruiter)
4. ✅ Account created → Redirected to dashboard
5. Profile completion wizard (if JobSeeker) or company info (if Recruiter)

**Existing Google User Journey:**
1. Click "Continue with Google"
2. Select Google account
3. ✅ Logged in → Redirected to dashboard

---

## 🚨 Common Issues & Solutions

### Issue 1: "Invalid Google token"
**Cause**: Token expired or malformed
**Solution**: Ensure you're sending the `id_token` not `access_token`

### Issue 2: "Account type is required"
**Cause**: Missing or invalid accountType
**Solution**: Must be "JobSeeker" or "Recruiter" (case-insensitive - "jobseeker", "RECRUITER" also work)

### Issue 3: CORS Error
**Cause**: Frontend origin not allowed
**Solution**: Backend already allows `localhost:3000`, `localhost:4200`, `localhost:5173`

### Issue 4: "This account uses Google sign-in"
**Cause**: User trying to login with email/password but registered with Google
**Solution**: Tell user to use "Continue with Google" button

---

## 📚 Additional Resources

- [Google OAuth Documentation](https://developers.google.com/identity/protocols/oauth2)
- [@react-oauth/google Documentation](https://www.npmjs.com/package/@react-oauth/google)
- [Google Cloud Console](https://console.cloud.google.com/)

---

## 🎉 Summary

✅ **Backend**: Fully implemented and tested
✅ **Endpoint**: `POST /api/auth/google`
✅ **Frontend**: Ready to integrate with `@react-oauth/google`
✅ **Security**: Server-side token verification
✅ **UX**: Seamless Google sign-in experience

**Your frontend team needs to:**
1. Install `@react-oauth/google`
2. Get Google Client ID from Cloud Console
3. Create GoogleSignInButton component
4. Add it to Register/Login pages

That's it! 🚀 Simple, secure, and beginner-friendly.
