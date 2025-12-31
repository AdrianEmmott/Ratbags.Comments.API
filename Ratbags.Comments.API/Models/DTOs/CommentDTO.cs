namespace Ratbags.Comments.API.Models.DTOs;

public sealed record CommentDTO(    
    Guid Id,
    string? CommenterName,
    string Content,
    DateTimeOffset Published
);
