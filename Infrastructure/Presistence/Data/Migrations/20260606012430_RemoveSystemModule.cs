using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSystemModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contents_Users_UserId",
                table: "Contents");

            migrationBuilder.DropForeignKey(
                name: "FK_Histories_SystemRoots_SystemId",
                table: "Histories");

            migrationBuilder.DropTable(
                name: "AIModels");

            migrationBuilder.DropTable(
                name: "SystemRoots");

            migrationBuilder.DropIndex(
                name: "IX_Histories_SystemId",
                table: "Histories");

            migrationBuilder.DropIndex(
                name: "IX_Contents_UserId",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "SystemId",
                table: "Histories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Contents");

            migrationBuilder.AlterColumn<string>(
                name: "AttachedUserEmail",
                table: "Histories",
                type: "nvarchar(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UserEmail",
                table: "Contents",
                type: "nvarchar(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Contents",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateIndex(
                name: "IX_Histories_AttachedUserEmail",
                table: "Histories",
                column: "AttachedUserEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_UserEmail",
                table: "Contents",
                column: "UserEmail");

            migrationBuilder.Sql("""
                DELETE FROM Histories
                WHERE AttachedUserEmail NOT IN (SELECT Email FROM Users);

                DELETE FROM Contents
                WHERE UserEmail NOT IN (SELECT Email FROM Users);

                DELETE FROM Users
                WHERE Email NOT IN ('admin@vioguard.com', 'analyst@vioguard.com');
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_Contents_Users_UserEmail",
                table: "Contents",
                column: "UserEmail",
                principalTable: "Users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Histories_Users_AttachedUserEmail",
                table: "Histories",
                column: "AttachedUserEmail",
                principalTable: "Users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contents_Users_UserEmail",
                table: "Contents");

            migrationBuilder.DropForeignKey(
                name: "FK_Histories_Users_AttachedUserEmail",
                table: "Histories");

            migrationBuilder.DropIndex(
                name: "IX_Histories_AttachedUserEmail",
                table: "Histories");

            migrationBuilder.DropIndex(
                name: "IX_Contents_UserEmail",
                table: "Contents");

            migrationBuilder.AlterColumn<string>(
                name: "AttachedUserEmail",
                table: "Histories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)");

            migrationBuilder.AddColumn<string>(
                name: "SystemId",
                table: "Histories",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "UserEmail",
                table: "Contents",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Contents",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Contents",
                type: "nvarchar(256)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SystemRoots",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SystemName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemRoots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AIModels",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SystemId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccuracyThreshold = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Framework = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModelType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AIModels_SystemRoots_SystemId",
                        column: x => x.SystemId,
                        principalTable: "SystemRoots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Histories_SystemId",
                table: "Histories",
                column: "SystemId");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_UserId",
                table: "Contents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AIModels_SystemId",
                table: "AIModels",
                column: "SystemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contents_Users_UserId",
                table: "Contents",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Email");

            migrationBuilder.AddForeignKey(
                name: "FK_Histories_SystemRoots_SystemId",
                table: "Histories",
                column: "SystemId",
                principalTable: "SystemRoots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
