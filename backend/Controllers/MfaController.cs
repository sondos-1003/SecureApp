using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureWebApp.backend.Services;
using SecureWebApp.Services;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class MfaController : ControllerBase
{
    private readonly IMfaService _totpMfaService;

    // Inject the service through constructor
    public MfaController(IMfaService totpMfaService)
    {
        _totpMfaService = totpMfaService;
    }

    /// <summary>
    /// Generates MFA setup code for the authenticated user
    /// </summary>
    /// <returns>Returns QR code data for MFA setup</returns>
    [HttpPost("setup")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MfaSetupResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SetupMfa()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var setupCode = await _totpMfaService.GenerateSetupCode(userId, email);

        return Ok(new MfaSetupResponse
        {
            SetupCode = setupCode,
            ManualEntryKey = ExtractManualEntryKey(setupCode) // Helper method
        });
    }

    private string ExtractManualEntryKey(string setupCode)
    {
        // Extract the secret key from the otpauth URL
        var secretStart = setupCode.IndexOf("secret=") + 7;
        var secretEnd = setupCode.IndexOf('&', secretStart);
        return secretEnd == -1
            ? setupCode[secretStart..]
            : setupCode[secretStart..secretEnd];
    }
}
public class MfaSetupResponse
{
    public string SetupCode { get; set; }  // otpauth:// URL for QR code
    public string ManualEntryKey { get; set; }  // The actual secret key for manual entry
}