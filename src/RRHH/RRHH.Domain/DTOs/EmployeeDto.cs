using System.ComponentModel.DataAnnotations;

namespace RRHH.Domain.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }
        [Required]
        [StringLength(100)]
        public string Position { get; set; } = string.Empty;
        [Range(0.01, double.MaxValue)]
        public decimal Salary { get; set; }
        [Required]
        [StringLength(100)]
        public string Department { get; set; } = string.Empty;
    }
}
