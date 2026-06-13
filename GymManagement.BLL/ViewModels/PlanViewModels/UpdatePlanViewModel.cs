using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.ViewModels.PlanViewModel
{
    public class UpdatePlanViewModel
    {
        [Required]
        public int Id { get; set; }
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 3650, ErrorMessage = "Duration must be greater than 0")]
        public int DurationDays { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 1000000, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, MinimumLength = 5,
            ErrorMessage = "Description must be between 5 and 500 characters")]
        public string Description { get; set; } = default!;
  
    }
}
