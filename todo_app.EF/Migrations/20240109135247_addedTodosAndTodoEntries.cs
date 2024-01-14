using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace todo_app.EF.Migrations
{
    /// <inheritdoc />
    public partial class addedTodosAndTodoEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Todos",
                columns: table =>
                    new
                    {
                        Id = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        Title = table.Column<string>(
                            type: "nvarchar(100)",
                            maxLength: 100,
                            nullable: true
                        ),
                        IsDone = table.Column<bool>(type: "bit", nullable: false),
                        UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                        UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Todos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Todos_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "TodoEntries",
                columns: table =>
                    new
                    {
                        Id = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        Text = table.Column<string>(
                            type: "nvarchar(200)",
                            maxLength: 200,
                            nullable: false
                        ),
                        IsDone = table.Column<bool>(type: "bit", nullable: false),
                        Priority = table.Column<int>(type: "int", nullable: false),
                        Position = table.Column<int>(type: "int", nullable: false),
                        TodoId = table.Column<int>(type: "int", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TodoEntries_Todos_TodoId",
                        column: x => x.TodoId,
                        principalTable: "Todos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_TodoEntries_TodoId",
                table: "TodoEntries",
                column: "TodoId"
            );

            migrationBuilder.CreateIndex(name: "IX_Todos_UserId", table: "Todos", column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "TodoEntries");

            migrationBuilder.DropTable(name: "Todos");
        }
    }
}
