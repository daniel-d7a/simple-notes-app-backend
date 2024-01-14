using Microsoft.EntityFrameworkCore.Migrations;
using todo_app.core.Constants;

#nullable disable

namespace todo_app.EF.Migrations
{
    /// <inheritdoc />
    public partial class seedDbWithRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new[]
                {
                    Guid.NewGuid().ToString(),
                    RoleNames.ADMIN,
                    RoleNames.ADMIN.ToUpper(),
                    Guid.NewGuid().ToString(),
            });
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new[]
                {
                Guid.NewGuid().ToString(),
                RoleNames.USER,
                RoleNames.USER.ToUpper(),
                Guid.NewGuid().ToString(),
            });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Delete from [AspNetRoles]");
        }
    }
}
