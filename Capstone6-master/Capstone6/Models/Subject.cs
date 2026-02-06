using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Capstone6.Models
{
    [Table("Subjects")]
    public class Subject
    {
        [Key]
        public int SubjectID { get; set; }

        [Required]
        [DisplayName("Subject Name")]
        [StringLength(50, ErrorMessage = "Name of Department cannot have more than 50 characters")]
        [MinLength(3, ErrorMessage = "Name of Subject should have at least 3 characters")]
        public string SubjectName { get; set; }

        [ForeignKey("Department")]
        [Required]
        [Display(Name = "Department Name")]
        [JsonPropertyName("DepartmentID")]
        public int DepartmentID { get; set; }

        // 1 Subject 1 Department
        [DisplayName("Department Name")]
        [JsonIgnore]
        public virtual Department Department { get; set; }
        

    }
}
