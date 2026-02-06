using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Capstone6.Models
{
    [Table("Departments")]
    public class Department
    {
        [Key]
        [JsonPropertyName("departmentId")]
        public int DepartmentID { get; set; }

        [DisplayName("Department Name")]
        [Required]
        [StringLength(50, ErrorMessage = "Name of Department cannot have more than 50 characters")]
        [MinLength(3, ErrorMessage = "Name of Department should have at least 3 characters")]
        public string DepartmentName { get; set; }

        // 1 Department many Subjects
        public virtual ICollection<Subject> Subjects { get; set; }
    }
}
