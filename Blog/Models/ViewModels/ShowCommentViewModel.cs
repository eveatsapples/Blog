using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Models.ViewModels
{
    public class ShowCommentViewModel
    {
        public int ID { get; set; }

        public string UserID { get; set; }
        public int PostID { get; set; }

        public string Body { get; set; }
        public string Date_created { get; set; }
        public string Date_updated { get; set; }
        public string Updated_reason { get; set; }
    }
}