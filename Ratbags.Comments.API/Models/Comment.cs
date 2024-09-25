﻿namespace Ratbags.Comments.API.Models;

public partial class Comment
{
    public Guid Id { get; set; }

    public Guid ArticleId { get; set; }

    public Guid UserId { get; set; }

    public string CommentContent { get; set; } = null!;

    public DateTime PublishDate { get; set; }

    public virtual Article Article { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
