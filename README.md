# SecureApp

## 📌 Project Description
SecureApp is a lightweight ASP.NET Core web application focused on demonstrating secure login and registration mechanisms. It is designed following security best practices by implementing encryption, hashing, secure token handling, and threat modeling techniques.

---

## 🛠️ Tech Stack

**Frontend:**
- Minimal Razor Pages / ASP.NET Core Views

**Backend:**
- ASP.NET Core (C#)

**Authentication & Security:**
- AES Encryption (for usernames)
- BCrypt Password Hashing
- JWT for secure session handling
- Multi-Factor Authentication (MFA/2FA) using TOTP (Time-Based One-Time Password)

**Storage:**
- In-memory user store (No database)

---

## ⚙️ Setup Instructions

### Prerequisites
- .NET SDK 7.0+
- Git
- Visual Studio or VS Code

---

## 🔐 Security Measures Implemented

- **AES-256 Encryption:** Used to encrypt usernames before storing them securely.
- **BCrypt Hashing:** Secure password hashing with salting.
- **JWT Tokens:** Used for authenticated sessions with proper claims and expiration handling.
- **HTTPS Only:** Enforced secure transport layer communication.
- **STRIDE Threat Modeling:** Full analysis performed using Microsoft Threat Modeling Tool.
- **No SQL Injection Risk:** No database layer used; logic designed to avoid injection attacks.
- **Authentication & Authorization:** Implemented with MFA/2FA using TOTP (Time-Based One-Time Password).
- **Security Headers:** Implemented via middleware for additional protection.

---

## 🧠 Threat Modeling (STRIDE) & DREAD Documentation

- STRIDE model created using Microsoft Threat Modeling Tool.
- The `.htm` report file is included in the `/docs` folder.
- Major threats identified and mitigated include:
  - Elevation of Privilege
  - Information Disclosure
  - Tampering
  - Spoofing
  - Repudiation

- DREAD ratings are documented within the threat report.

**Example Mitigations:**
- Input validation
- JWT expiration handling
- Logging for repudiation protection

---

## 🔎 Security Scanning Tools Used

- GitHub CodeQL (Static analysis for C# integrated in CI/CD)
- Snyk CLI (Dependency vulnerability scanning)
