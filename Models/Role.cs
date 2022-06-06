using System.ComponentModel.DataAnnotations;

namespace DataSenseMVC.Models
{
    public class Role
    {
       [Key]
        public int RoleId { get; set; }

        public string RoleName { get; set; }
    }
}
