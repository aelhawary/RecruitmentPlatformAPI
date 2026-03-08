# Profile Completion Wizard — Frontend Integration Guide

> How the backend tracks wizard progress and how to use it on the frontend.

---

## Overview

The backend tracks each user's wizard progress via `User.ProfileCompletionStep` (an integer stored in the DB). This value is returned in **login**, **register**, **Google auth**, **verify-email**, and **`/me`** responses so the frontend can route the user to the correct wizard step immediately after authentication.

---

## 1. Wizard Steps

### Job Seeker (4 steps)

| Step | Name | What the user fills in |
|------|------|------------------------|
| 0 | Not Started | *(initial state after registration)* |
| 1 | Personal Info and CV | Name, phone, country, city, job title, bio, optional picture and CV |
| 2 | Experience and Education | Work experience entries, education entries |
| 3 | Projects | Portfolio / project entries |
| 4 | Skills, Social Links and Certificates | Skills, social media links, certificates — **wizard complete** |

### Recruiter (1 step)

| Step | Name | What the user fills in |
|------|------|------------------------|
| 0 | Not Started | *(initial state)* |
| 1 | Company Information | Company name, industry, size, etc. — **wizard complete** |

---

## 2. Where `profileCompletionStep` Appears

### 2a. Login / Register / Google Auth / Verify Email

All auth responses share the same `UserInfoDto` shape:

```jsonc
{
  "success": true,
  "message": "Login successful.",
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "user": {
    "id": 1,
    "firstName": "Ahmed",
    "lastName": "Mohamed",
    "email": "user@example.com",
    "accountType": "JobSeeker",        // or "Recruiter"
    "isEmailVerified": true,
    "isActive": true,
    "profileCompletionStep": 0         // <-- wizard step (0-4 for JobSeeker, 0-1 for Recruiter)
  }
}
```

### 2b. GET `/api/auth/me` (JWT required)

```jsonc
{
  "success": true,
  "user": {
    "id": "1",
    "email": "user@example.com",
    "name": "Ahmed Mohamed",
    "role": "JobSeeker",
    "firstName": "Ahmed",
    "lastName": "Mohamed",
    "profileCompletionStep": 0         // <-- from JWT claim (snapshot at login time)
  }
}
```

> **Important:** The `/me` endpoint reads from the JWT token, so `profileCompletionStep` is a **snapshot from login time**. If the user completes steps mid-session, this value is stale. Use `/wizard-status` for the real-time value.

### 2c. GET `/api/jobseeker/wizard-status` or `/api/recruiter/wizard-status` (JWT required, real-time)

**Job Seeker example:**

```jsonc
{
  "success": true,
  "data": {
    "currentStep": 1,
    "isComplete": false,
    "stepName": "Personal Info & CV",
    "completedSteps": ["Personal Info & CV"]
  },
  "message": null
}
```

**Recruiter example:**

```jsonc
{
  "success": true,
  "data": {
    "currentStep": 0,
    "isComplete": false,
    "stepName": "Not Started",
    "completedSteps": []
  },
  "message": null
}
```

These are the **authoritative** sources of truth — they query the DB directly.

---

## 3. Frontend Routing Logic

### On Login / Page Refresh

```
profileCompletionStep === 0  →  Route to Step 1 (Personal Info)
profileCompletionStep === 1  →  Route to Step 2 (Experience & Education)
profileCompletionStep === 2  →  Route to Step 3 (Projects)
profileCompletionStep === 3  →  Route to Step 4 (Skills, Social, Certificates)
profileCompletionStep === 4  →  Route to Dashboard (wizard complete)
```

**Recommended flow:**

```
1. User logs in → read response.user.profileCompletionStep
2. If step < 4 (JobSeeker) or step < 1 (Recruiter) → route to wizard
3. If step === 4 (or 1 for Recruiter) → route to dashboard
```

### During the Wizard (mid-session navigation)

After saving each step, call the wizard-status endpoint for your account type to get the updated `currentStep`:

- **Job Seeker:** `GET /api/jobseeker/wizard-status`
- **Recruiter:** `GET /api/recruiter/wizard-status`

Do **not** rely on the JWT `/me` value mid-session.

---

## 4. Which Endpoints Advance the Step

The wizard step **only moves forward** (never decrements). Backend logic: `if (user.ProfileCompletionStep < targetStep) → set it`.

### Job Seeker

| Step | Advancing Endpoint | Notes |
|------|--------------------|-------|
| 0 → 1 | `POST /api/jobseeker/personal-info` | Returns `profileCompletionStep` in response |
| 1 → 2 | `POST /api/jobseeker/experience` or `POST /api/jobseeker/education` | Call wizard-status after save |
| 2 → 3 | `POST /api/jobseeker/projects` | Call wizard-status after save |
| 3 → 4 | `PUT /api/jobseeker/social-accounts` | Call wizard-status after save |

### Recruiter

| Step | Advancing Endpoint | Notes |
|------|--------------------|-------|
| 0 → 1 | `POST /api/recruiter/company-info` | Returns `profileCompletionStep` in response |

---

## 5. Typical Frontend Pseudocode

```javascript
// After login
const { token, user } = await api.login(email, password);
localStorage.setItem('token', token);

if (user.accountType === 'JobSeeker') {
  if (user.profileCompletionStep >= 4) {
    router.push('/dashboard');
  } else {
    // Route to the NEXT step (step after the last completed one)
    router.push(`/wizard/step-${user.profileCompletionStep + 1}`);
  }
} else if (user.accountType === 'Recruiter') {
  if (user.profileCompletionStep >= 1) {
    router.push('/dashboard');
  } else {
    router.push('/wizard/company-info');
  }
}
```

```javascript
// After saving a wizard step (Job Seeker example)
await api.savePersonalInfo(data);

// Get fresh wizard state from DB
const wizardUrl = user.accountType === 'JobSeeker'
  ? '/api/jobseeker/wizard-status'
  : '/api/recruiter/wizard-status';
const status = await api.get(wizardUrl);
const { currentStep, isComplete } = status.data;

if (isComplete) {
  router.push('/dashboard');
} else {
  router.push(`/wizard/step-${currentStep + 1}`);
}
```

```javascript
// On page refresh / app init — determine where user left off
const wizardUrl = user.accountType === 'JobSeeker'
  ? '/api/jobseeker/wizard-status'
  : '/api/recruiter/wizard-status';
const status = await api.get(wizardUrl);

if (status.data.isComplete) {
  router.push('/dashboard');
} else {
  router.push(`/wizard/step-${status.data.currentStep + 1}`);
}
```

---

## 6. Key Points Summary

| Topic | Detail |
|-------|--------|
| **DB column** | `User.ProfileCompletionStep` (int, default 0) |
| **JWT claim** | `ProfileCompletionStep` — snapshot at token issue time |
| **Real-time source** | `GET /api/jobseeker/wizard-status` (or `/api/recruiter/wizard-status`) |
| **Step direction** | Forward only — steps never go backward |
| **Save as Draft** | Each step saves independently to DB; user can leave and resume later |
| **Login routing** | Read `user.profileCompletionStep` from login response for immediate routing |
| **Mid-session routing** | Call `wizard-status` after each save for the authoritative current step |
