using System;
using System.Collections.Generic;

#nullable disable

namespace todo_console_app.Models
{
    public partial class Todo
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string CreationDate { get; set; }
        public long Checked { get; set; }
        public long FkUserId { get; set; }

        public virtual User FkUser { get; set; }
    }
}
