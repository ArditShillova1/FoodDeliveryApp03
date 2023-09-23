using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDeliveryApp03.Migrations
{
    public partial class Review1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_ProfilePicture_ProfilePictureId1",
                table: "Reviews",
                column: "ProfilePictureId1",
                principalTable: "ProfilePicture",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_ProfilePicture_ProfilePictureId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_ProfilePicture_ProfilePictureId1",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ProfilePictureId1",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ProfilePictureId1",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "ProfilePictureId",
                table: "Reviews",
                newName: "RestaurantId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_ProfilePictureId",
                table: "Reviews",
                newName: "IX_Reviews_RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_ProfilePicture_RestaurantId",
                table: "Reviews",
                column: "RestaurantId",
                principalTable: "ProfilePicture",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
