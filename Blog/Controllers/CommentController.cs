using Blog.Models;
using Blog.Models.Domain;
using Blog.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    [RoutePrefix("blog")]
    public class CommentController : Controller
    {
       private ApplicationDbContext DbContext;
       public CommentController()
       {
           DbContext = new ApplicationDbContext();
       }

       public ActionResult Create(int postID)
       {
            Comment comment = new Comment();
            comment.PostID = postID; // this will be sent from the ArticleDetails View, hold on :).
            return PartialView(comment);            
       }
        
       [HttpPost]
       [Authorize]
       public ActionResult Create(Comment comment)
       {
         return SaveComment(comment);
       }

       [HttpGet]
       [Authorize(Roles = "Admin, Moderator")]
       public ActionResult Edit(int? id)
       {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(BlogPostController.Index));
            }

            var comment = DbContext.Comments.FirstOrDefault(
                p => p.ID == id);

            if (comment == null)
            {
                return RedirectToAction(nameof(BlogPostController.Index));
            }

            comment.Date_updated = DateTime.Now;

            DbContext.SaveChanges();
            return View(comment);
       }
 
       [HttpPost]
       [Authorize(Roles = "Admin, Moderator")]
       public ActionResult Edit(Comment comment)
       {
         return SaveComment(comment);
       }

        [HttpPost]
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                //return RedirectToAction(nameof(BlogPostController.Index));
                return RedirectToAction("FullArticle", "Blog");
            }

            var comment = DbContext.Comments.FirstOrDefault(p => p.ID == id);
            var post = DbContext.BlogPosts.FirstOrDefault(p => p.ID == comment.PostID);
            string postSlug = post.Slug;

            if (comment != null)
            {
                DbContext.Comments.Remove(comment);
                DbContext.SaveChanges();
            }

            return RedirectToAction("FullArticleBySlug", "BlogPost", new { slug = postSlug });
        }



        [Authorize]
        private ActionResult SaveComment(Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var userId = User.Identity.GetUserId();

            var newOrUpdatedComment = DbContext.Comments.FirstOrDefault(
                p => p.ID == comment.ID);

            if (newOrUpdatedComment == null)
            {
                comment.UserID = userId;
                DbContext.Comments.Add(comment);
                comment.Date_created = DateTime.Now;
            }
            else
            {
                newOrUpdatedComment = DbContext.Comments.FirstOrDefault(
                    p => p.ID == comment.ID);

                if (newOrUpdatedComment == null)
                {
                    return RedirectToAction(nameof(BlogPostController.Index));
                }

                newOrUpdatedComment.Updated_reason = comment.Updated_reason;
                newOrUpdatedComment.Date_updated = DateTime.Now;
                newOrUpdatedComment.Body = comment.Body;
            }

            DbContext.SaveChanges();
            
            var currentPost = DbContext.BlogPosts.FirstOrDefault(
                p => p.ID == comment.PostID);

            //return RedirectToAction("FullArticle", "Blog");
            return RedirectToAction("FullArticleBySlug", "BlogPost", new { slug = currentPost.Slug });
            //return View();
        }

         public ActionResult Show(int postID)
         {
              var model = DbContext.Comments.ToList()
                .Where(p => p.PostID == postID)
                .Select(p => new ShowCommentViewModel
                {
                    Body = p.Body,
                    ID = p.ID,
                    UserID = p.UserID,
                    Date_created = p.Date_created.ToString(),
                    Date_updated = p.Date_updated.ToString(),
                    Updated_reason = p.Updated_reason,
                    PostID = p.PostID
                }).ToList();

              return PartialView(model);            
         }

    }
}
