using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Grocery.WebApp.Models
{
    public class Product
    {
        [Key]
        [Required]
        [Column(name:"ProjectId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Comment("The Unique ID of the Product")]
        public Guid ProductID { get; set; }

        [Required]
        [StringLength(80)]
        [Column(TypeName ="varchar")]
        [Comment("Name of the Product sold by Store")]
        public string ProductName { get; set; }

        [Required]
        [Comment("Quantity of Product currently Available in store")]
        public short Quantity { get; set; } //onModelCreating for default value

        [Required]
        [Column(TypeName ="decimal(8,2)")]
        public decimal SellingPricePerUnit { get; set; }

        [Comment("Image of the Product")]
        public byte[] Image { get; set; }

        #region Audit Fields

        [Required]
        public Guid CreatedByUserId { get; set; }
        public Guid? UpdatedByUserId { get; set; }

        [Required]
        //[DisplayFormat(DataFormatString ="{dd-MM-yyyy}")]
        public DateTime LastUpdateOn { get; set; }
        #endregion

        #region Establish relationship with MyIdentityUser
        public MyIdentityUser CreatedByUser { get; set; }
        public MyIdentityUser UpdatedByUser { get; set; }
        #endregion
    }
}
