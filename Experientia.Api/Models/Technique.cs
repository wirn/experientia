using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Experientia.Api.Models
{
    [ApiController]
    [Route("api/[controller]")]
    public class Technique
    {
        public int Id { get; set; }
        [Required,MaxLength(100)]
        public string Name { get; set; } = "";
    }
}
