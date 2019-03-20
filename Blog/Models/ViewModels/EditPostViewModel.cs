using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Blog.Models.ViewModels
{
    public class EditPostViewModel
    {
        [Required]
        public string Title { get; set; }

        [AllowHtml]
        [Required]
        public string Body { get; set; }

        public HttpPostedFileBase Media { get; set; }

        public bool Published { get; set; }
    }
}
