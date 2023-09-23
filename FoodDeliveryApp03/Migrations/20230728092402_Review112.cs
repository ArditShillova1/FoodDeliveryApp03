using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDeliveryApp03.Migrations
{
    public partial class Review112 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

          

            migrationBuilder.DropColumn(
                name: "ProfilePictureId1",
                table: "Reviews");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProfilePictureId1",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProfilePictureId1",
                table: "Reviews",
                column: "ProfilePictureId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_ProfilePicture_ProfilePictureId1",
                table: "Reviews",
                column: "ProfilePictureId1",
                principalTable: "ProfilePicture",
                principalColumn: "Id");
        }
    }
}
