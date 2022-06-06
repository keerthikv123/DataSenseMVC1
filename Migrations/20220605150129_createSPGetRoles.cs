using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataSenseMVC.Migrations
{
    public partial class createSPGetRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string storedProc = @"EXEC(N'CREATE PROCEDURE [dbo].[GetAllRoles]
                                AS	
                                BEGIN
	                                SELECT * FROM  ROLES
                                END')";
            migrationBuilder.Sql(storedProc);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROC [dbo].[GetAllRoles]");
        }
    }
}
