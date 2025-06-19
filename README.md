# SecureApp
#Project Description
SecureApp is a lightweight ASP.NET Core web application focused on demonstrating secure login and registration mechanisms. It is designed to follow best security practices by implementing encryption, hashing, secure token handling, and threat modeling techniques.
#Tech Stack
Frontend: Minimal Razor Pages / ASP.NET Core Views
Backend: ASP.NET Core (C#)
Authentication:
AES Encryption (for usernames)
BCrypt Password Hashing
JWT for secure session handling
Storage: In-memory user store (no database)
#⚙️ Setup Instructions
Prerequisites
.NET SDK 7.0+
Git
Visual Studio or VS Code
#Security Measures Implemented
• AES-256 Encryption: Used to encrypt usernames before storing them. • BCrypt Hashing: Secure password hashing with salting. • JWT Tokens: Tokens generated for authenticated sessions with proper claims. • HTTPS Only: Enforced secure transport. • STRIDE Threat Modeling: Full analysis using Microsoft Threat Modeling Tool. • No SQL Injection risk: No database layer used; logic secured against code injection. • Authentication & Authorization: Multi-Factor Authentication (MFA/2FA):Method: Implemented TOTP (Time-Based One-Time Password) • headers using middleware
Threat Modeling (STRIDE) & DREAD Documentation
• STRIDE model created using Microsoft Threat Modeling Tool. • The .htm report file is included in the ((/docs folder)). • Major threats identified and mitigated: o **Elevation of privilege o Information disclosure o Tampering o Spoofing • DREAD ratings are recorded within the threat report. • Example mitigations include: o Input validation o JWT expiration o Logging (repudiation protection)
Security Scanning Tools Used • GitHub CodeQL: Static analysis for C# (CI/CD integrated) • Snyk CLI : For dependency vulnerability scanning
