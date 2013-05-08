using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace www.Models
{
    [Table("Companies")]
    public class Company
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CompanyId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Required]
        [Display(Name = "Company Logo")]
        public string CompanyLogo { get; set; }
    }
}