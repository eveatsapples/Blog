using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Models.Domain
{
    public class BlogPost
    {
        public string Slug { get; set; }
        public int ID { get; set; }

        public virtual ApplicationUser User { get; set; }
        public string UserId { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }
        public bool Published { get; set; }
        public DateTime Date_created { get; set; }
        public DateTime? Date_updated { get; set; }

        public string MediaUrl { get; set; }
    }
}
