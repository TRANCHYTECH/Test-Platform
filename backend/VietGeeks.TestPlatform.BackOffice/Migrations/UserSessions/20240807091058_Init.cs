#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace VietGeeks.TestPlatform.BackOffice.Migrations.UserSessions
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "session");

            migrationBuilder.CreateTable(
                name: "UserSessions",
                schema: "session",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SubjectId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SessionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Renewed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Expires = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ticket = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_ApplicationName_Key",
                schema: "session",
                table: "UserSessions",
                columns: new[] { "ApplicationName", "Key" },
                unique: true,
                filter: "[ApplicationName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_ApplicationName_SessionId",
                schema: "session",
                table: "UserSessions",
                columns: new[] { "ApplicationName", "SessionId" },
                unique: true,
                filter: "[ApplicationName] IS NOT NULL AND [SessionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_ApplicationName_SubjectId_SessionId",
                schema: "session",
                table: "UserSessions",
                columns: new[] { "ApplicationName", "SubjectId", "SessionId" },
                unique: true,
                filter: "[ApplicationName] IS NOT NULL AND [SessionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_Expires",
                schema: "session",
                table: "UserSessions",
                column: "Expires");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSessions",
                schema: "session");
        }
    }
}
