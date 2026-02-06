using Grocery.WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Grocery.WebApp.Areas.Admin.ViewModels
{
    public class ProductViewModel
    {
        
        [Required]
        [Display(Name = "Product ID")]
        public Guid ProductID { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(80)]
        public string ProductName { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        public short Quantity { get; set; } //onModelCreating for default value

        [Required]
        [Display(Name = "Selling Price per Unit")]
        public decimal SellingPricePerUnit { get; set; }

        // Property to store the image received from the database
        public byte[] Image { get; set; }

        // Property to receive the file uploaded for the Image
        public IFormFile ImageFile { get; set; }

        #region Audit Fields

        [Required]
        [Display(Name ="Created by")]
        public Guid CreatedByUserId { get; set; }

        [Display(Name = "Updated by")]
        public Guid? UpdatedByUserId { get; set; }

        [Required]
        [Display(Name = "Last updated on")]
        public DateTime LastUpdateOn { get; set; }
        #endregion

        #region Establish relationship with MyIdentityUser
        public MyIdentityUser CreatedByUser { get; set; }
        public MyIdentityUser UpdatedByUser { get; set; }
        #endregion
    }
}
