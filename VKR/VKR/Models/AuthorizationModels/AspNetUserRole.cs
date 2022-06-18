using System.ComponentModel.DataAnnotations;

namespace VKR.Models.AuthorizationModels
{
    public class AspNetUserRole
    {
        [Key]
        public int UserId { get; set; }
        [Key]
        public int RoleId { get; set; }
    }
}
