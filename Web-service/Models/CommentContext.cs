using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Web_service.Models
{
    public class CommentContext : DbContext
    {
        public CommentContext() :
        base("DefaultConnection")
        { }

        public DbSet<Comment> Comment { get; set; }
    }
}