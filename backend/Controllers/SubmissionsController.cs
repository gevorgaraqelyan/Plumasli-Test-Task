using Microsoft.AspNetCore.Mvc;
using backend.Services;
using backend.Common;
using System.Text.Json;

namespace backend.Controllers;

[ApiController]
[Route("api")]
public class SubmissionsController : ControllerBase
{
    private readonly ISubmissionService _submissionService;

    public SubmissionsController(ISubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    [HttpPost("forms/{formKey}/submissions")]
    public async Task<IActionResult> CreateSubmission(string formKey, [FromBody] JsonElement data)
    {
        if (string.IsNullOrWhiteSpace(formKey))
        {
            return BadRequest(new ApiErrorResponse(ErrorMessages.FormKeyRequired));
        }

        var dataJson = data.ToString();
        if (string.IsNullOrWhiteSpace(dataJson) || dataJson == "{}")
        {
            return BadRequest(new ApiErrorResponse(ErrorMessages.SubmissionDataRequired));
        }

        try
        {
            var submission = await _submissionService.CreateSubmissionAsync(formKey, dataJson);
            return CreatedAtAction(nameof(GetSubmission), new { id = submission.Id }, submission);
        }
        catch
        {
            return StatusCode(500, new ApiErrorResponse(ErrorMessages.InternalServerError));
        }
    }

    [HttpGet("submissions")]
    public async Task<IActionResult> GetSubmissions(
        [FromQuery] string? formKey = null,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 50;

        var submissions = await _submissionService.SearchSubmissionsAsync(formKey, search, page, pageSize);
        return Ok(submissions);
    }

    [HttpGet("submissions/{id}")]
    public async Task<IActionResult> GetSubmission(Guid id)
    {
        var submission = await _submissionService.GetSubmissionByIdAsync(id);
        if (submission == null)
        {
            return NotFound(new ApiErrorResponse(ErrorMessages.SubmissionNotFound));
        }

        return Ok(submission);
    }
}

