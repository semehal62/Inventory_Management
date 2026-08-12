using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory_management_System.Migrations
{
    /// <inheritdoc />
    public partial class Foreignkeychange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Managers_Managerid",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Employees_EmployeeId",
                table: "Sales");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "Sales",
                newName: "BaseUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Sales_EmployeeId",
                table: "Sales",
                newName: "IX_Sales_BaseUserId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Managers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Managerid",
                table: "Items",
                newName: "ManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_Items_Managerid",
                table: "Items",
                newName: "IX_Items_ManagerId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Employees",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Audit_logs",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Managers_ManagerId",
                table: "Items",
                column: "ManagerId",
                principalTable: "Managers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Users_BaseUserId",
                table: "Sales",
                column: "BaseUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Managers_ManagerId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Users_BaseUserId",
                table: "Sales");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "BaseUserId",
                table: "Sales",
                newName: "EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_Sales_BaseUserId",
                table: "Sales",
                newName: "IX_Sales_EmployeeId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Managers",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ManagerId",
                table: "Items",
                newName: "Managerid");

            migrationBuilder.RenameIndex(
                name: "IX_Items_ManagerId",
                table: "Items",
                newName: "IX_Items_Managerid");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Employees",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Audit_logs",
                newName: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Managers_Managerid",
                table: "Items",
                column: "Managerid",
                principalTable: "Managers",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Employees_EmployeeId",
                table: "Sales",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
