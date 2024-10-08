using Microsoft.EntityFrameworkCore;

namespace Ratbags.Comments.API.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comments__C3B4DFCA3BB14DDE");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.PublishDate).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
