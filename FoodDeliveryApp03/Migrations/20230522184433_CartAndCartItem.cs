using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDeliveryApp03.Migrations
{
    public partial class CartAndCartItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_MenuItems_MenuItemID",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Cart");

            migrationBuilder.RenameColumn(
                name: "MenuItemID",
                table: "Cart",
                newName: "MenuItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Cart_MenuItemID",
                table: "Cart",
                newName: "IX_Cart_MenuItemId");

            migrationBuilder.AlterColumn<int>(
                name: "MenuItemId",
                table: "Cart",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "CartItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MenuItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CartId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItem_Cart_CartId",
                        column: x => x.CartId,
                        principalTable: "Cart",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CartItem_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_CartId",
                table: "CartItem",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_MenuItemId",
                table: "CartItem",
                column: "MenuItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_MenuItems_MenuItemId",
                table: "Cart",
                column: "MenuItemId",
                principalTable: "MenuItems",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_MenuItems_MenuItemId",
                table: "Cart");

            migrationBuilder.DropTable(
                name: "CartItem");

            migrationBuilder.RenameColumn(
                name: "MenuItemId",
                table: "Cart",
                newName: "MenuItemID");

            migrationBuilder.RenameIndex(
                name: "IX_Cart_MenuItemId",
                table: "Cart",
                newName: "IX_Cart_MenuItemID");

            migrationBuilder.AlterColumn<int>(
                name: "MenuItemID",
                table: "Cart",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Cart",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_MenuItems_MenuItemID",
                table: "Cart",
                column: "MenuItemID",
                principalTable: "MenuItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
