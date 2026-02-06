using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Grocery.WebApp.Data.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace Grocery.WebApp.Models
{
    public class MyIdentityUser : IdentityUser<Guid>
    {
        [Required]                                  // Not NULL constraint for DB schema
        [Display(Name ="Display Name")]             // Constraint for UI Label
        [MinLength(2)]
        [MaxLength(60)]
        [StringLength(60)]
        public string DisplayName { get; set; }

        [Required]
        [Display(Name = "Gender")]
        [PersonalData]                          
        public MyAppGenderTypes Gender { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        [PersonalData]
        [Column(TypeName ="smalldatetime")]         //Tells DB to use smalldatetime datatype
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Display(Name ="Is Admin User?")]
        public bool IsAdminUser { get; set; }

        [Display(Name = "Photo")]
        [PersonalData]
        public byte[] Photo { get; set; }

        #region Establish relation with Product
        [ForeignKey(nameof(Product.CreatedByUserId))]
        public ICollection<Product> ProductCreatedByUser { get; set; }

        [ForeignKey(nameof(Product.UpdatedByUserId))]
        public ICollection<Product> ProductUpdatedByUser { get; set; }


        #endregion
    }
}
