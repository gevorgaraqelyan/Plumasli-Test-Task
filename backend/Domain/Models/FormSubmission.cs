namespace backend.Domain.Models;

public class FormSubmission
{
    public Guid Id { get; set; }
    public string FormKey { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Data { get; set; } = string.Empty;
}

