using Microsoft.EntityFrameworkCore.Migrations;

namespace TheBookClub.Migrations
{
    public partial class addforeignkeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookId",
                table: "Messages",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_BookId",
                table: "Messages",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Books_BookId",
                table: "Messages",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Books_BookId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_BookId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "BookId",
                table: "Messages");
        }
    }
}
