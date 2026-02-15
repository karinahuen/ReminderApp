using System.ComponentModel.DataAnnotations;

namespace ReminderApp.Models
{
    public class Reminder
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        // This handles the "Website" or "Mobile" tag
        [Required]
        public string Platform { get; set; } = "Website"; // Values: "Website" or "Mobile"

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Links the reminder to the logged-in user so users only see their own data
        public string? OwnerId { get; set; } 
    }
}