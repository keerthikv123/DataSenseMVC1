-- declare variables used in cursor
DECLARE @moduleId Int;
DECLARE @roleId INT;
declare @add bit;
declare @edit bit;
declare @delete bit;
declare @view bit;
declare @rolePermissionId int;

 
-- declare cursor
DECLARE roleModuleCheck CURSOR FOR
  SELECT RolePermissions.RoleId, RolePermissions.ModuleId,RolePermissions.RolePermissionId
  FROM RolePermissions
 
-- open cursor
OPEN roleModuleCheck;
 
-- loop through a cursor
FETCH NEXT FROM roleModuleCheck INTO @moduleId, @roleId,@rolePermissionId;
WHILE @@FETCH_STATUS = 0
    BEGIN

     update RolePermissions
     set Add=@add,Delete=@delete,Edit=@edit,View=@view,
     where RolePermissionId=@rolePermissionId;

   FETCH NEXT FROM roleModuleCheck INTO @moduleId, @roleId;
    END;
 
-- close and deallocate cursor
CLOSE roleModuleCheck;