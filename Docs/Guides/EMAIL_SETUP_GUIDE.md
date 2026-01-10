# Email Service Setup Guide

## ✅ What's Already Done
- MailKit package installed
- EmailService implemented with real SMTP email sending
- Configuration structure ready in `appsettings.json`

## 🔧 How to Configure Email (Simple 3 Steps)

### Option 1: Using Gmail (Recommended for Beginners)

#### Step 1: Enable 2-Factor Authentication on Your Gmail Account
1. Go to your Google Account: https://myaccount.google.com/
2. Click on **Security** (left sidebar)
3. Under "Signing in to Google", enable **2-Step Verification**
4. Follow the prompts to set it up

#### Step 2: Generate an App Password
1. After enabling 2FA, go back to **Security**
2. Under "Signing in to Google", click **App passwords**
3. Select app: **Mail**
4. Select device: **Windows Computer** (or Other)
5. Click **Generate**
6. **Copy the 16-character password** (it looks like: `abcd efgh ijkl mnop`)

#### Step 3: Update appsettings.json
Open `RecruitmentPlatformAPI/appsettings.json` and update the EmailSettings section:

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "youremail@gmail.com",
    "SenderPassword": "your-16-char-app-password",
    "SenderName": "Recruitment Platform"
  }
}
```

Replace:
- `youremail@gmail.com` with your actual Gmail address
- `your-16-char-app-password` with the password from Step 2 (remove spaces)

**Example:**
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "john.doe@gmail.com",
    "SenderPassword": "abcdefghijklmnop",
    "SenderName": "Recruitment Platform"
  }
}
```

### Option 2: Using Outlook/Hotmail

Update `appsettings.json`:
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp-mail.outlook.com",
    "SmtpPort": 587,
    "SenderEmail": "youremail@outlook.com",
    "SenderPassword": "your-outlook-password",
    "SenderName": "Recruitment Platform"
  }
}
```

### Option 3: Using Other Email Services

| Service | SMTP Server | Port | SSL |
|---------|-------------|------|-----|
| Gmail | smtp.gmail.com | 587 | Yes |
| Outlook | smtp-mail.outlook.com | 587 | Yes |
| Yahoo | smtp.mail.yahoo.com | 587 | Yes |
| Mailgun | smtp.mailgun.org | 587 | Yes |
| SendGrid | smtp.sendgrid.net | 587 | Yes |

## 🧪 Testing Your Email Setup

1. **Start your API** (if not already running):
   ```powershell
   cd RecruitmentPlatformAPI
   dotnet run
   ```

2. **Register a new user** via Swagger or Postman:
   ```
   POST https://localhost:5217/api/auth/register
   ```
   Use a **real email address** that you can check!

3. **Check your email inbox** - you should receive a verification code

4. **Check the logs** in your terminal - you'll see:
   ```
   Verification email sent successfully to: user@example.com
   ```

## 🚨 Troubleshooting

### Problem: "Authentication failed"
**Solution:** 
- For Gmail: Make sure you're using the **App Password**, not your regular Gmail password
- Check that 2FA is enabled
- Make sure there are no spaces in the password

### Problem: "Could not connect to SMTP server"
**Solution:**
- Check your internet connection
- Verify the SMTP server address and port
- Some networks block port 587 - try using port 465 and change `EnableSsl` to `true`

### Problem: Emails not arriving
**Solution:**
- Check your **spam/junk folder**
- Verify the email address is correct
- Check API logs for error messages
- Try using a different email service (e.g., switch from Gmail to Outlook)

### Problem: "Less secure app access"
**Solution:**
- Gmail no longer supports "less secure apps"
- You **MUST** use App Passwords (see Step 2 above)

## 🔒 Security Best Practices

### For Development:
The current setup in `appsettings.json` is fine for development and testing.

### For Production:
**NEVER commit your real email credentials to Git!**

Instead, use environment variables or Azure Key Vault:

1. **Using User Secrets (Development):**
   ```powershell
   cd RecruitmentPlatformAPI
   dotnet user-secrets init
   dotnet user-secrets set "EmailSettings:SenderEmail" "your-email@gmail.com"
   dotnet user-secrets set "EmailSettings:SenderPassword" "your-app-password"
   ```

2. **Using Environment Variables (Production):**
   Set these in your hosting environment (Azure, AWS, etc.):
   - `EmailSettings__SenderEmail`
   - `EmailSettings__SenderPassword`

## 📝 What the Email Service Does

### When a user registers:
1. ✉️ Sends a verification email with a 6-digit code
2. 📧 Beautiful HTML email with your platform branding
3. ⏰ Code expires in 15 minutes

### When email is verified:
1. 🎉 Sends a welcome email
2. 📨 Confirms successful verification
3. 🔗 Includes link to get started

## 📧 Email Templates

The service sends **two types of emails**:

### 1. Verification Email
- Subject: "Verify Your Email - Recruitment Platform"
- Contains: 6-digit verification code
- Styled with HTML (professional look)
- Falls back to plain text if HTML not supported

### 2. Welcome Email
- Subject: "Welcome to Recruitment Platform!"
- Sent after successful verification
- Includes "Get Started" button
- Professional HTML design

## 🎯 Next Steps

1. ✅ Configure your email credentials (see Step 3 above)
2. ✅ Test with a real email address
3. ✅ Your frontend can now integrate with the authentication endpoints
4. ✅ Users will receive real emails for verification!

## ❓ Still Need Help?

If you encounter any issues:
1. Check the **console logs** when you run the API
2. Look for error messages in the terminal
3. Verify your email credentials are correct
4. Try with a different email service (Gmail, Outlook, etc.)

**Common Success Signs:**
- Console logs: `Verification email sent successfully to: user@example.com`
- Email arrives in inbox within 1-2 minutes
- 6-digit code displays correctly in email

---

🎓 **Beginner Tip:** Start with Gmail using the steps above. It's the easiest and most reliable option for development!
