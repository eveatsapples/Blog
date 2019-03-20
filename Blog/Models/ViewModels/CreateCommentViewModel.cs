using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models.ViewModels
{
    public class CreateCommentViewModel
    {
        [Key]
        public int ID { get; set; }

        public virtual ApplicationUser User { get; set; }
        public int UserID { get; set; }
        public int PostID { get; set; }

        [Required]
        public string Body { get; set; }
        public DateTime Date_created { get; set; }
        public DateTime? Date_updated { get; set; }
        public string Updated_reason { get; set; }

    }
}