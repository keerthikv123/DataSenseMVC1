using DataSenseMVC.Data;
using DataSenseMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataSenseMVC.Controllers
{
    public class RoleController : Controller
    {
        private readonly DataSenseMVCAppContext _context;

        public RoleController(DataSenseMVCAppContext context)
        {
            _context = context;
        }
        // GET: Role
        public async Task<IActionResult> Index()
        {
            var roles = await _context.Roles.FromSqlInterpolated($"exec GetAllRoles").ToListAsync();
            return View(roles);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
            }
            RolePermissionViewModel model = new RolePermissionViewModel();
            var role = await _context.Roles.Where(r => r.RoleId == id).FirstOrDefaultAsync();
            var rolepermissions = await _context.RolePermissions.Include(r => r.Module).Where(r => r.RoleId == id).ToListAsync();

            model.RoleName = role.RoleName;
            model.RoleId = role.RoleId;
            if (rolepermissions != null && rolepermissions.Count > 0)
            {
                model.ModulePermissionList = rolepermissions.Select(s => new ModulePermission
                {
                    Add = s.Add,
                    Delete = s.Delete,
                    Edit = s.Edit,
                    View = s.View,
                    Module = s.Module.ModuleName

                }).ToList();
            }

            if (role == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // GET: Role/Create
        public IActionResult Create()
        {
            var modules = _context.Modules.Select(m => new ModulePermission { ModuleId = m.ModuleId, Module = m.ModuleName }).ToList();
            RolePermissionViewModel model = new RolePermissionViewModel
            {
                ModulePermissionList = modules
            };
            return View(model);
        }

        // POST: Role/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RolePermissionViewModel role)
        {
            //  if (ModelState.IsValid)
            {
                var isValid = IsValidWithSpaceConstraint(role.RoleName);

                if (isValid)
                {
                    var checkRoleExists = _context.Roles.Where(r => r.RoleName == role.RoleName).FirstOrDefault();
                    if (checkRoleExists == null)
                    {
                        Role newRole = new Role { RoleName = role.RoleName };
                        //  _context.Add(newRole);
                        _context.Roles.FromSqlInterpolated($"exec RolesInsertUpdate {0},{role.RoleName}");

                        List<RolePermission> rolePermissions = new List<RolePermission>();
                        if (role.ModulePermissionList != null)
                        {
                            foreach (var rolePermission in role.ModulePermissionList.Where(i => i.RolePermissionId==0).ToList())
                            {

                                if (rolePermission.Add || rolePermission.Edit || rolePermission.Delete || rolePermission.View)
                                {
                                    RolePermission newItem = new RolePermission
                                    {
                                        Role = newRole,
                                        ModuleId = rolePermission.ModuleId,
                                        View = rolePermission.View,
                                        Add = rolePermission.Add,
                                        Edit = rolePermission.Edit,
                                        Delete = rolePermission.Delete
                                    };
                                     rolePermissions.Add(newItem);
                                   

                                }
                            }
                        }
                        _context.AddRange(rolePermissions);
                        await _context.SaveChangesAsync();
                    }
                }


                return RedirectToAction(nameof(Index));
                //  }
                return View(role);
            }
        }

        // GET: Role/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
            }

            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            RolePermissionViewModel model = new RolePermissionViewModel();
            var modules = _context.Modules.Select(m => new ModulePermission { ModuleId = m.ModuleId, Module = m.ModuleName }).ToList();
            var rolepermissions = await _context.RolePermissions.Include(r => r.Module).Where(r => r.RoleId == id).ToListAsync();
            model.RoleName = role.RoleName;
            model.RoleId = role.RoleId;
            foreach (var module in modules)
            {
                if (rolepermissions != null && rolepermissions.Count > 0)
                {
                    var rp = rolepermissions.Where(rpp => rpp.Module.ModuleId == module.ModuleId).FirstOrDefault();
                    if (rp != null)
                    {
                        module.RolePermissionId = rp.RolePermissionId;
                        module.ModuleId = rp.ModuleId;
                        module.Module = rp.Module.ModuleName;
                        module.IsSelected = true;
                        module.Add = rp.Add;
                        module.View = rp.View;
                        module.Delete = rp.Delete;
                        module.Edit = rp.Edit;
                    }
                }
            }
            model.ModulePermissionList = modules;
            //if (rolepermissions != null && rolepermissions.Count > 0)
            //{
            //    model.ModulePermissionList = rolepermissions.Select(s => new ModulePermission
            //    {
            //        Add = s.Add,
            //        Delete = s.Delete,
            //        Edit = s.Edit,
            //        View = s.View,
            //        Module = s.Module.ModuleName,
            //        IsSelected=true

            //    }).ToList();
            //}
            return View(model);
        }

        // POST: Role/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RolePermissionViewModel role)
        {
                bool isValid = IsValidWithSpaceConstraint(role.RoleName);
            if (isValid)
            {
                var checkRoleExists = _context.Roles.Where(r => r.RoleId != role.RoleId).FirstOrDefault();
                if (checkRoleExists != null)
                {
                    var existingRole = _context.Roles.Where(r => r.RoleId == role.RoleId).FirstOrDefault();
                    if (existingRole.RoleName != role.RoleName)
                    {
                        existingRole.RoleName = role.RoleName;
                        // _context.Roles.Update(existingRole);
                       // _context.Roles.FromSqlInterpolated($"exec RolesInsertUpdate");

                        _context.Roles.FromSqlInterpolated($"exec RolesInsertUpdate {existingRole.RoleId},{existingRole.RoleName}");


                    }


                    foreach (var rolePermission in role.ModulePermissionList.ToList())
                    {
                        List<RolePermission> rolePermissionList = new List<RolePermission>();
                        rolePermissionList = _context.RolePermissions.Where(x => x.RoleId == role.RoleId).ToList();



                        //existing records of role permission
                        var rp = rolePermissionList.Where(x => x.RolePermissionId == rolePermission.RolePermissionId).FirstOrDefault();

                        if (rp != null)
                        {
                            rp.Add = rolePermission.Add;
                            rp.View = rolePermission.View;
                            rp.Delete = rolePermission.Delete;
                            rp.Edit = rolePermission.Edit;

                              _context.RolePermissions.Update(rp);
                          

                        }
                        //for new RolePermissions
                        else if(rp==null)
                        {
                            if (rolePermission.RolePermissionId == 0)
                            {
                                if (rolePermission.Add || rolePermission.Edit || rolePermission.Delete || rolePermission.View)
                                {
                                    rp = new RolePermission();
                                    rp.RoleId = role.RoleId;
                                    rp.ModuleId = rolePermission.ModuleId;
                                    rp.Add = rolePermission.Add;
                                    rp.View = rolePermission.View;
                                    rp.Delete = rolePermission.Delete;
                                    rp.Edit = rolePermission.Edit;

                                    _context.RolePermissions.Add(rp);
                                }
                            }
                        }

                      
                        
                    }
                   
                    await _context.SaveChangesAsync();
                }


                return RedirectToAction(nameof(Index));
            }
            
            return View(role);
        }

        // GET: Role/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(m => m.RoleId == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: Role/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Roles == null)
            {
                return Problem("Entity set 'DataSenseMVC.Roles'  is null.");
            }
            var role = await _context.Roles.FindAsync(id);
            if (role != null)
            {
                  _context.Roles.Remove(role);


              //  _context.Roles.FromSqlInterpolated($"exec RolesDelete {role.RoleId}");

            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.RoleId == id);
        }

        private bool IsValidWithSpaceConstraint(string inputString)
        {
            bool result = false;
            int stringLength = inputString.Length;
            //Checking whether first/last characte is white space
            char firstChar = inputString[0];
            char lastChar = inputString[stringLength - 1];
            bool isValid = char.IsWhiteSpace(firstChar) && char.IsWhiteSpace(lastChar);

            int numberOfSpaces = 0;
            for (int i = 0; i < stringLength; i++)
            {
                char currentChar = inputString[i];
                if (char.IsWhiteSpace(currentChar))
                  {
                    numberOfSpaces++;
                  }
            }

            if (numberOfSpaces < 2 && isValid == false)
                result = true;
            else
                result = false;

            return result;
        }

    }
}
