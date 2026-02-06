using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Grocery.WebApp.Models
{
    public class MyIdentityRole :IdentityRole <Guid>
    {
        [MaxLength(100)]
        [StringLength(100)]
        public string Description { get; set; }
    }
}
