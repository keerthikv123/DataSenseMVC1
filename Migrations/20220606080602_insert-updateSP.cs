using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataSenseMVC.Migrations
{
    public partial class insertupdateSP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string storedProc = @"EXEC(N'CREATE PROCEDURE [dbo].[RolesInsertUpdate] (@id            INTEGER,
                                          @roleName  VARCHAR(50))
AS	
BEGIN
	                               
	IF @id=0
		begin
			insert into Roles(RoleName) values (@roleName)
		end
	else
		begin
			update Roles set RoleName=@roleName where RoleId=@id
		end

end
')";
            migrationBuilder.Sql(storedProc);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
