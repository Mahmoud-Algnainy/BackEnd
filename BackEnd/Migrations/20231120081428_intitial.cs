using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    public partial class intitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sarf_Id = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sarf_Id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Esthkaks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Esthkaks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Estkta3s",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estkta3s", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSarfs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SarfDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSarfs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSarfs_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSarf_Esthkaks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EsthkakId = table.Column<int>(type: "int", nullable: false),
                    EsthkakValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmployeeSarfId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSarf_Esthkaks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSarf_Esthkaks_EmployeeSarfs_EmployeeSarfId",
                        column: x => x.EmployeeSarfId,
                        principalTable: "EmployeeSarfs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmployeeSarf_Esthkaks_Esthkaks_EsthkakId",
                        column: x => x.EsthkakId,
                        principalTable: "Esthkaks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSarf_Estkta3s",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Estkta3Id = table.Column<int>(type: "int", nullable: false),
                    Estkta3Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmployeeSarfId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSarf_Estkta3s", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSarf_Estkta3s_EmployeeSarfs_EmployeeSarfId",
                        column: x => x.EmployeeSarfId,
                        principalTable: "EmployeeSarfs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmployeeSarf_Estkta3s_Estkta3s_Estkta3Id",
                        column: x => x.Estkta3Id,
                        principalTable: "Estkta3s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSarf_Esthkaks_EmployeeSarfId",
                table: "EmployeeSarf_Esthkaks",
                column: "EmployeeSarfId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSarf_Esthkaks_EsthkakId",
                table: "EmployeeSarf_Esthkaks",
                column: "EsthkakId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSarf_Estkta3s_EmployeeSarfId",
                table: "EmployeeSarf_Estkta3s",
                column: "EmployeeSarfId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSarf_Estkta3s_Estkta3Id",
                table: "EmployeeSarf_Estkta3s",
                column: "Estkta3Id");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSarfs_EmployeeId",
                table: "EmployeeSarfs",
                column: "EmployeeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "EmployeeSarf_Esthkaks");

            migrationBuilder.DropTable(
                name: "EmployeeSarf_Estkta3s");

            migrationBuilder.DropTable(
                name: "Esthkaks");

            migrationBuilder.DropTable(
                name: "EmployeeSarfs");

            migrationBuilder.DropTable(
                name: "Estkta3s");

            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
