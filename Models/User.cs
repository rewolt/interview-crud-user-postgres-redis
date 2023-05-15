using System.ComponentModel.DataAnnotations;

namespace CrudUsers.Models
{
    public class User
    {
        public Guid Id { get; init; }
        [EmailAddress]
        public string Email { get; set; }
        [StringLength(50)]
        public string FirstName { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }
    }
}
