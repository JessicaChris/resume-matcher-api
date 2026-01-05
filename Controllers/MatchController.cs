using Microsoft.AspNetCore.Mvc;
using ResumeMatcherAPI.Services;
using ResumeMatcherAPI.Models;

namespace ResumeMatcherAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchController : ControllerBase
{
    private readonly TextSimilarityService _service = new();
    private readonly PdfTextExtractor _pdfExtractor = new();

    // ✅ EXISTING TEXT MATCH (UNCHANGED)
    [HttpPost]
    public IActionResult Match([FromBody] MatchRequest req)
    {
        var score = _service.CalculateSimilarity(req.ResumeText, req.JobDescription);
        return Ok(new MatchResult { MatchPercentage = score });
    }

    // 🆕 NEW PDF UPLOAD MATCH
    [HttpPost("upload")]
    public IActionResult MatchWithPdf(
        IFormFile resumeFile,
        [FromForm] string jobDescription)
    {
        if (resumeFile == null || resumeFile.Length == 0)
            return BadRequest("Resume PDF is required");

        using var stream = resumeFile.OpenReadStream();
        var resumeText = _pdfExtractor.ExtractText(stream);

        var score = _service.CalculateSimilarity(resumeText, jobDescription);

        return Ok(new MatchResult { MatchPercentage = score });
    }
}

// DTO FOR TEXT MATCH
public class MatchRequest
{
    public string ResumeText { get; set; }
    public string JobDescription { get; set; }
}
