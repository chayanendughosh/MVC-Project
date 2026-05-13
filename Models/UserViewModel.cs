using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace WebApplication1.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile is required")]
        [RegularExpression(@"^\+?[0-9]{7,15}$", ErrorMessage = "Invalid mobile number")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "State is required")]
        public int StateId { get; set; }

        // For multi-select hobbies (like the working code pattern)
        public List<string> SelectedHobbies { get; set; }

        // Available hobbies for dropdown
        public List<SelectListItem> AvailableHobbies { get; set; }

        // Internal property to get/set comma-separated string for DB storage
        public string Hobbies
        {
            get
            {
                return SelectedHobbies != null && SelectedHobbies.Count > 0
                    ? string.Join(",", SelectedHobbies)
                    : string.Empty;
            }
            set
            {
                SelectedHobbies = !string.IsNullOrWhiteSpace(value)
                    ? value.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries).Select(h => h.Trim()).ToList()
                    : new List<string>();
            }
        }
    }
}
