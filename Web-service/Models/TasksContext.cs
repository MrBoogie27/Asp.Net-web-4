using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Web_service.Models
{
    public class TasksContext : DbContext
    {
        public TasksContext() :
        base("DefaultConnection")
        { }

        public DbSet<Tasks> Tasks { get; set; }
    }
}