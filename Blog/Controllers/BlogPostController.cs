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



namespace Blog.Controllers
{
    public class BlogPostController : Controller
    {
        private ApplicationDbContext DbContext;

        public BlogPostController()
        {
            DbContext = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            var model = DbContext.BlogPosts
              .Select(p => new IndexBlogPostViewModel
              {
                  Id = p.Id,
                  Title = p.Title,
                  Body = p.Body,
                  Date_created = p.Date_created.ToString(),
                  Date_updated = p.Date_updated.ToString(),
                  MediaUrl = p.MediaUrl
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

            if (DbContext.BlogPosts.Any(p => p.UserId == userId &&
                  p.Title == formData.Title &&
                  (!id.HasValue || p.Id != id.Value)))
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
                post = new BlogPost();
                post.UserId = userId;
                DbContext.BlogPosts.Add(post);
            }
            else
            {
                post = DbContext.BlogPosts.FirstOrDefault(
                    p => p.Id == id);

                if (post == null)
                {
                    return RedirectToAction(nameof(BlogPostController.Index));
                }
            }

            post.Title = formData.Title;
            post.Body = formData.Body;
            post.Date_created = DateTime.Now;
            post.Published = true;

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
                p => p.Id == id && p.UserId == userId);

            if (post == null)
            {
                return RedirectToAction(nameof(BlogPostController.Index));
            }

            var model = new EditPostViewModel();
            model.Title = post.Title;
            model.Body = post.Body;
            post.Date_updated = DateTime.Now;
            post.Published = true;

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

            var post = DbContext.BlogPosts.FirstOrDefault(p => p.Id == id);

            if (post != null)
            {
                DbContext.BlogPosts.Remove(post);
                DbContext.SaveChanges();
            }

            return RedirectToAction(nameof(BlogPostController.Index));
        }

        [HttpGet]
        public ActionResult FullArticle(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction(nameof(BlogPostController.Index));

            var userId = User.Identity.GetUserId();

            var post = DbContext.BlogPosts.FirstOrDefault(p =>
                p.Id == id.Value);

            if (post == null)
                return RedirectToAction(nameof(BlogPostController.Index));

            var model = new FullArticleViewModel();
            model.Title = post.Title;
            model.Body = post.Body;
            model.MediaUrl = post.MediaUrl;

            return View(model);
        }
    }
}
