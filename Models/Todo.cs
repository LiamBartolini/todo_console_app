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
        public string Data { get; set; }
        public long? Checked { get; set; }
    }
}
