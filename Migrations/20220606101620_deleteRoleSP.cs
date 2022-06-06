using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataSenseMVC.Migrations
{
    public partial class deleteRoleSP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string storedProc = @"EXEC(N'CREATE PROCEDURE [dbo].[RolesDelete] (@id INTEGER)
AS	
BEGIN
	 
	
	delete  from Roles where RoleId=@id
	delete from RolePermissions where RoleId=@id

end')";
            migrationBuilder.Sql(storedProc);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
