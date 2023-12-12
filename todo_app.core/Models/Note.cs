using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace todo_app.core.Models
{
    public class Note : BaseModel
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string? Title { get; set; }

        public string? Body { get; set; }


    }
}
