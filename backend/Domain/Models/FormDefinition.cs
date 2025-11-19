namespace backend.Domain.Models;

public class FormDefinition
{
    public Guid Id { get; set; }
    public string FormKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

