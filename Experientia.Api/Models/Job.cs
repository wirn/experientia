using System.ComponentModel.DataAnnotations;

namespace Experientia.Api.Models
{
    public class Job
    {
        public long Id { get; set; }
        [Required]
        public long UserId { get; set; }

        public AppUser? User { get; set; }
        public string Place { get; set; } = "";
        public string Role { get; set; } = "";
        public string? Location { get; set; }
        [Required]
        public DateOnly FromDate { get; set; }       
        public DateOnly? ToDate { get; set; }          

        [Required]
        public List<Technique> Techniques { get; set; } = new(); 
    }
}
