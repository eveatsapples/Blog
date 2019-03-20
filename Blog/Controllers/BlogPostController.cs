using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Blog.Models;
using Blog.Models.Domain;
using Blog.Models.ViewModels;
using System.IO;
using System.Text.RegularExpressions;

namespace Blog.Controllers
{
    [RoutePrefix("blog")]
    public class BlogPostController : Controller
    {
        private ApplicationDbContext DbContext;

        public BlogPostController()
        {
            DbContext = new ApplicationDbContext();
        }

        [Route]
        public ActionResult Index()
        {
            var model = DbContext.BlogPosts
              .Select(p => new IndexBlogPostViewModel
              {
                  Slug = p.Slug,
                  ID = p.ID,
                  Title = p.Title,
                  Body = p.Body,
                  Date_created = p.Date_created.ToString(),
                  Date_updated = p.Date_updated.ToString(),
                  MediaUrl = p.MediaUrl,
                  Published = p.Published
              }).ToList();

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult NewPost()
        {
            var userId = User.Identity.GetUserId();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult NewPost(EditPostViewModel formData)
        {
            return SavePost(null, formData);
        }

        [Authorize(Roles = "Admin")]
        private ActionResult SavePost(int? id, EditPostViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var userId = User.Identity.GetUserId();

            if (DbContext.BlogPosts.Any(p => p.Title == formData.Title &&
                  (!id.HasValue || p.ID != id.Value)))
            {
                ModelState.AddModelError(nameof(EditPostViewModel.Title),
                    "Article title should be unique");

                return View();
            }
            string fileExtension;

            if (formData.Media != null)
            {
                fileExtension = Path.GetExtension(formData.Media.FileName);

                if (!Constants.AllowedFileExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("", "File extension is not allowed.");
                    return View();
                }
            }

            BlogPost post;

            if (!id.HasValue)
            {
                post = new BlogPost
                {
                    UserId = userId
                };
                DbContext.BlogPosts.Add(post);
                post.Slug = Slugify(formData.Title);
                post.Date_created = DateTime.Now;
            }
            else
            {
                post = DbContext.BlogPosts.FirstOrDefault(
                    p => p.ID == id);

                if (post == null)
                {
                    return RedirectToAction(nameof(BlogPostController.Index));
                }
                post.Date_updated = DateTime.Now;
            }

            post.Title = formData.Title;
            post.Body = formData.Body;
            post.Published = formData.Published;

            if (formData.Media != null)
            {
                if (!Directory.Exists(Constants.MappedUploadFolder))
                {
                    Directory.CreateDirectory(Constants.MappedUploadFolder);
                }

                var fileName = formData.Media.FileName;
                var fullPathWithName = Constants.MappedUploadFolder + fileName;

                formData.Media.SaveAs(fullPathWithName);

                post.MediaUrl = Constants.UploadFolder + fileName;
            }

            DbContext.SaveChanges();

            return RedirectToAction(nameof(BlogPostController.Index));
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(BlogPostController.Index));
            }

            var userId = User.Identity.GetUserId();

            var post = DbContext.BlogPosts.FirstOrDefault(
                p => p.ID == id && p.UserId == userId);

            if (post == null)
            {
                return RedirectToAction(nameof(BlogPostController.Index));
            }

            var model = new EditPostViewModel
            {
                Title = post.Title,
                Body = post.Body
            };
            post.Date_updated = DateTime.Now;
            post.Published = post.Published;

            DbContext.SaveChanges();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, EditPostViewModel formData)
        {
            return SavePost(id, formData);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(BlogPostController.Index));
            }

            var post = DbContext.BlogPosts.FirstOrDefault(p => p.ID == id);

            if (post != null)
            {
                DbContext.BlogPosts.Remove(post);
                DbContext.SaveChanges();
            }

            return RedirectToAction(nameof(BlogPostController.Index));
        }

        [HttpGet]
        [Route("{slug}")]
        public ActionResult FullArticleBySlug(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return RedirectToAction(nameof(BlogPostController.Index));
            }

            var userId = User.Identity.GetUserId();

            var post = DbContext.BlogPosts.FirstOrDefault(p =>
            p.Slug.ToString() == slug);

            if (post == null)
            {
                return RedirectToAction(nameof(BlogPostController.Index));
            }

            var model = new FullArticleViewModel
            {
                ID = post.ID,
                Title = post.Title,
                Body = post.Body,
                MediaUrl = post.MediaUrl
            };

            return View("FullArticle", model);
        }

        [HttpGet]
        [Route("Search")]
        public ActionResult Search(string query)
        {
            var model = DbContext.BlogPosts
              .Where(p => p.Title.Contains(query) || p.Slug.Contains(query) || p.Body.Contains(query))
              .Select(p => new IndexBlogPostViewModel
              {
                  Slug = p.Slug,
                  ID = p.ID,
                  Title = p.Title,
                  Body = p.Body,
                  Date_created = p.Date_created.ToString(),
                  Date_updated = p.Date_updated.ToString(),
                  MediaUrl = p.MediaUrl,
                  Published = p.Published
              }).ToList();

            return View("index", model);
        }

        public static string Slugify(string str)
        {
          BlogPostController bpc = new BlogPostController();  
          str = str.ToLower();
          str = Regex.Replace(str, @"[^a-z0-9\s, %*]", "");
          str = Regex.Replace(str, @"\s+", " ").Trim();
          str = Regex.Replace(str, @"\s", "-");
          string result = str;
          if (bpc.DbContext.BlogPosts.Any(p => p.Slug == result))
          {
            int i = 1;
            str = str + "-";
            while (bpc.DbContext.BlogPosts.Any(p => p.Slug == result))
            {
              string numStr = i.ToString();
              result = str + numStr;
              i++;
            }
          }
          return result;
        }

    }
}
