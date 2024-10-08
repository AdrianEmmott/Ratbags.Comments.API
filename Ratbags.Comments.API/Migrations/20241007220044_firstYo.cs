using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ratbags.Comments.API.Migrations
{
    /// <inheritdoc />
    public partial class firstYo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Comments__C3B4DFCA3BB14DDE", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");
        }
    }
}
