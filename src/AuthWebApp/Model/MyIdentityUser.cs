using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AuthWebApp.Model
{
    public class MyIdentityUser: IdentityUser
    {
        [Display(Name = "Registration Time"), DataType(DataType.DateTime)]
        public DateTime RegistrationTime { get; set; }

        [Display(Name = "Last login time"), DataType(DataType.DateTime)]
        public DateTime LastLoginTime { get; set; }

        [Display(Name = "Status")]
        public bool BlockStatus { get; set; }

        [NotMapped]
        public bool IsSelected { get; set; }
    }
}
