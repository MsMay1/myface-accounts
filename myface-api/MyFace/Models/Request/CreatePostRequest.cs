using System.ComponentModel.DataAnnotations;

namespace MyFace.Models.Request
{
    public class CreatePostRequest
    {
        [Required]
        [StringLength(140)]
        public string Message { get; set; }
        
        public string ImageUrl { get; set; }
        
        // [Required]
        // [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        // public int UserId { get; set; }
    }
}