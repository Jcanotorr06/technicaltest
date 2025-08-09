using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TagModelTaskModel_TagModel_TagsId",
                table: "TagModelTaskModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TagModel",
                table: "TagModel");

            migrationBuilder.RenameTable(
                name: "TagModel",
                newName: "Tags");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tags",
                table: "Tags",
                column: "Id");

            migrationBuilder.InsertData(
                table: "TaskStatus",
                columns: new[] { "Id", "Description", "StatusName" },
                values: new object[,]
                {
                    { 1, "", "Pending" },
                    { 2, "", "In Progress" },
                    { 3, "", "Completed" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_TagModelTaskModel_Tags_TagsId",
                table: "TagModelTaskModel",
                column: "TagsId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TagModelTaskModel_Tags_TagsId",
                table: "TagModelTaskModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tags",
                table: "Tags");

            migrationBuilder.DeleteData(
                table: "TaskStatus",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TaskStatus",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TaskStatus",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.RenameTable(
                name: "Tags",
                newName: "TagModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TagModel",
                table: "TagModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TagModelTaskModel_TagModel_TagsId",
                table: "TagModelTaskModel",
                column: "TagsId",
                principalTable: "TagModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
