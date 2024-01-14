using System.ComponentModel.DataAnnotations;
using todo_app.core.Models.Auth;

namespace todo_app.core.Models
{
    public abstract class BaseModel
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public bool IsFavourite { get; set; }

        [Required]
        public string UserId { get; set; }
        public UserModel User { get; set; }
    }
}
