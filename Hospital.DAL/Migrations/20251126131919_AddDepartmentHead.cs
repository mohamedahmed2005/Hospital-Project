using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentHead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DoctorId",
                table: "Departments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_DoctorId",
                table: "Departments",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Doctors_DoctorId",
                table: "Departments",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Doctors_DoctorId",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_DoctorId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "Departments");
        }
    }
}
