using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using Web_service.Models;

namespace Web_service.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            User user = null;
            if (User == null || User.Identity == null)
                return RedirectToAction("Login", "Account");
            using (UserContext db = new UserContext())
            {
                user = db.Users.FirstOrDefault(u => u.Email == User.Identity.Name);
            }
            if (user == null)
                return RedirectToAction("Login", "Account");

            List<Tasks> tasks = new List<Tasks>();
            using (TasksContext db = new TasksContext()) 
            {
                var query = db.Tasks.Where(u => u.IdUser == user.Id).ToList();
                foreach (var item in query)
                {
                    if (item.Name == null)
                        item.Name = "";
                    if (item.Description == null)
                        item.Description = "";
                    if (item.Name.Length > 30)
                    {
                        string str = "";
                        string str1 = item.Name;
                        while (str1.Length > 30)
                        {
                            str += str1.Substring(0, 30) + " ";
                            str1 = str1.Remove(0, 30);
                        }
                        str += str1;
                        item.Name = str;
                    }
                    if (item.Description.Length > 30)
                    {
                        string str = "";
                        string str1 = item.Description;
                        while (str1.Length > 30)
                        {
                            str += str1.Substring(0, 30) + " ";
                            str1 = str1.Remove(0, 30);
                        }
                        str += str1;
                        item.Description = str;
                    }
                    tasks.Add(item);
                }
            }

            ViewBag.User = user;
            ViewBag.Tasks = tasks;

            return View();
        }

        public ActionResult AjaxFind()
        {
            var condition = Convert.ToInt32(Request["sel"]);
            var find = Request["data"];

            User user = null;
            if (User == null || User.Identity == null)
                return RedirectToAction("/Index", "Home");

            using (UserContext db = new UserContext())
            {
                user = db.Users.FirstOrDefault(u => u.Email == User.Identity.Name);
            }

            if (user == null)
                return RedirectToAction("/Index", "Home");

            List<Tasks> tasks = new List<Tasks>();
            using (TasksContext db = new TasksContext())
            {
                var query = db.Tasks.Where(u => u.IdUser == user.Id).ToList();
                foreach (var item in query)
                {
                    if (find == "")
                    {
                        if (condition == 1)
                            tasks.Add(item);
                        if (condition == 2 && item.condition == false)
                            tasks.Add(item);
                        if (condition == 3 && item.condition == true)
                            tasks.Add(item);
                    }
                    else
                    {
                        if (condition == 1 && (item.Name.Contains(find) || item.Description.Contains(find)))
                            tasks.Add(item);
                        if (condition == 2 && item.condition == false && (item.Name.Contains(find) || item.Description.Contains(find)))
                            tasks.Add(item);
                        if (condition == 3 && item.condition == true && (item.Name.Contains(find) || item.Description.Contains(find)))
                            tasks.Add(item);
                    }
                }
            }

            foreach (var item in tasks)
            {
                if (item.Name.Length > 30)
                {
                    string str = "";
                    string str1 = item.Name;
                    while (str1.Length > 30)
                    {
                        str += str1.Substring(0, 30) + " ";
                        str1 = str1.Remove(0, 30);
                    }
                    str += str1;
                    item.Name = str;
                }
                if (item.Description.Length > 30)
                {
                    string str = "";
                    string str1 = item.Description;
                    while (str1.Length > 30)
                    {
                        str += str1.Substring(0, 30) + " ";
                        str1 = str1.Remove(0, 30);
                    }
                    str += str1;
                    item.Description = str;
                }
            }

            ViewBag.User = user;
            ViewBag.Tasks = tasks;

            return View();
        }

        public ActionResult Insert()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Insert(string name, string description)
        {
            User user = null;
            if (User == null || User.Identity == null)
                return RedirectToAction("/Index", "Home");

            using (UserContext db = new UserContext())
            {
                user = db.Users.FirstOrDefault(u => u.Email == User.Identity.Name);
            }

            if (user == null)
                return RedirectToAction("/Index", "Home");
            using (TasksContext db = new TasksContext())
            {
                db.Tasks.Add(new Tasks { Name = name, Description = description, IdUser = user.Id });
                db.SaveChanges();
            }

            return RedirectToAction("/Index", "Home");
        }
        
        //добавление коментария через аджакс
        public ActionResult AjaxInsertComment()
        { 
            return View();
        }

        public ActionResult AjaxInsertComments()
        {
            int id = Convert.ToInt32(Session["id"]);

            User user = null;
            if (User == null || User.Identity == null)
                return RedirectToAction("/Index", "Home");
            using (UserContext db = new UserContext())
            {
                user = db.Users.FirstOrDefault(u => u.Email == User.Identity.Name);
            }
            if (user == null)
                return RedirectToAction("/Index", "Home");

            string description = Request["data"];
            using (CommentContext db = new CommentContext())
            {
                db.Comment.Add(new Comment { Description = description, IdUser = id });
                db.SaveChanges();
            }

            return RedirectToAction("Change", "Home", new { id = id });
        }

        public ActionResult AjaxDelete()
        {
            var id = Convert.ToInt32(Request["data"]);
            int userId;
            if (User == null || User.Identity == null)
                return RedirectToAction("/Index", "Home");

            using (UserContext db = new UserContext())
            {
                userId = db.Users.First(u => u.Email == User.Identity.Name).Id;
            }

            using (TasksContext db = new TasksContext())
            {
                var delete = db.Tasks.Where(u => u.Id == id && u.IdUser == userId).ToList();
                foreach (var item in delete)
                {
                    db.Tasks.Remove(item);
                }
                db.SaveChanges();
            }

            return RedirectToAction("/Index", "Home");
        }

        public ActionResult AjaxDeleteComment()
        {
            var id = Convert.ToInt32(Request["data"]);
            var idTasks = Convert.ToInt32(Request["idTask"]);

            User user = null;
            if (User == null || User.Identity == null)
                return RedirectToAction("/Index", "Home");
            using (UserContext db = new UserContext())
            {
                user = db.Users.FirstOrDefault(u => u.Email == User.Identity.Name);
            }
            if (user == null)
                return RedirectToAction("/Index", "Home");

            using (CommentContext db = new CommentContext())
            {
                var delete = db.Comment.Where(u => u.Id == id && u.IdUser == user.Id).ToList();
                foreach(var item in delete)
                {
                    db.Comment.Remove(item);
                }
                db.SaveChanges();
            }

            return RedirectToAction("Change", "Home", new { id = idTasks });
        }

        public ActionResult AjaxChangeComment()
        {
            var id = Convert.ToInt32(Request["data"]);
            Session["idComment"] = id;
            User user = null;
            if (User == null || User.Identity == null)
                return RedirectToAction("/Index", "Home");
            using (UserContext db = new UserContext())
            {
                user = db.Users.FirstOrDefault(u => u.Email == User.Identity.Name);
            }
            if (user == null)
                return RedirectToAction("/Index", "Home");

            using (CommentContext db = new CommentContext())
            {
                var request = db.Comment.Where(u => u.Id == id && u.IdUser == user.Id).ToList();
                foreach (var item in request)
                {
                    ViewBag.Comment = item;
                }
            }
            return View();
        }

        public ActionResult AjaxChangeComments()
        {
            var id = Convert.ToInt32(Session["id"]);
            var idComment = Convert.ToInt32(Session["idComment"]);
            var value = Request["data"];

            using (CommentContext db = new CommentContext())
            {
                var request = db.Comment.Find(idComment);
                request.Description = value;
                if (TryUpdateModel(request))
                {
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Change", "Home", new { id = id });
        }

        [HttpGet]
        public ActionResult Change(int id)
        {
            Session["id"] = id;

            User user = null;
            if (User == null || User.Identity == null)
                return RedirectToAction("/Index", "Home");
            using (UserContext db = new UserContext())
            {
                user = db.Users.FirstOrDefault(u => u.Email == User.Identity.Name);
            }
            if (user == null)
                return RedirectToAction("/Index", "Home");

            using (TasksContext db = new TasksContext())
            {
                var query = db.Tasks.Where(u => u.Id == id && u.IdUser == user.Id).ToList();
                foreach (var item in query)
                {
                    ViewBag.Tasks = item;
                }
            }
            if (ViewBag.Tasks == null)
                return RedirectToAction("/Index", "Home");

            List<Comment> comment = new List<Comment>();
            using (CommentContext db = new CommentContext())
            {
                var query = db.Comment.Where(u => u.IdUser == id).ToList();
                foreach (var item in query)
                {
                    if (item.Description.Length > 30)
                    {
                        string str = "";
                        string str1 = item.Description;
                        while (str1.Length > 30)
                        {
                            str += str1.Substring(0, 30) + " ";
                            str1 = str1.Remove(0, 30);
                        }
                        str += str1;
                        item.Description = str;
                    }

                    comment.Add(item);
                }
            }
            ViewBag.Comment = comment;
            return View();
        }

        public ActionResult AjaxUpdateCondition()
        {
            var id = Convert.ToInt32(Request["data"]);

            using (TasksContext db = new TasksContext())
            {
                var update = db.Tasks.Find(id);
                if (update.condition)
                    update.condition = false;
                else
                    update.condition = true;
                if (TryUpdateModel(update))
                {
                    db.SaveChanges();
                }
            }

            return RedirectToAction("/Index", "Home");
        }

        [HttpPost]
        public ActionResult Change(Tasks task)
        {
            using (TasksContext db = new TasksContext())
            {
                var update = db.Tasks.Find(task.Id);
                update.Name = task.Name;
                update.Description = task.Description;
                if(TryUpdateModel(update))
                {
                    db.SaveChanges();
                }
            }

            return RedirectToAction("/Index", "Home");
        }
    }
}