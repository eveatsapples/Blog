﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Models.ViewModels
{
    public class IndexBlogPostViewModel
    {
        public string Slug { get; set; }

        public int ID { get; set; }

        public string User { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string MediaUrl { get; set; }

        public bool Published { get; set; }

        public string Date_created { get; set; }

        public string Date_updated { get; set; }
    }
}
