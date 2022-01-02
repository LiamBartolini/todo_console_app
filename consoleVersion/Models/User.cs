using System;
using System.Collections.Generic;

#nullable disable

namespace todo_console_app.Models
{
    public partial class User
    {
        public User()
        {
            Todos = new HashSet<Todo>();
        }

        public long Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public virtual ICollection<Todo> Todos { get; set; }
    }
}
