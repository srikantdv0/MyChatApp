using System;
using System.ComponentModel.DataAnnotations;

namespace ChatShared.Models
{
    public class EditFormUser
    {
        [Required(ErrorMessage = "You can't enter chat room without a UserName")]
        [MinLength(6, ErrorMessage = "UserName should be 6 to 20 character long")]
        [MaxLength(20, ErrorMessage = "UserName should be 6 to 20 character long")]
        public string Name { get; set; } = string.Empty;
    }
}

