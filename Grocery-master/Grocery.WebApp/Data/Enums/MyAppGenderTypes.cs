using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Grocery.WebApp.Data.Enums
{
    public enum MyAppGenderTypes
    {
        [Display(Name = "Male")]
        Male,
        [Display(Name = "Female")]
        Female
    }
}
