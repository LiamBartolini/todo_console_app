using System.ComponentModel.DataAnnotations;

namespace todo_console_app.Models
{
    public partial class Todo  
    {
        [Key]
        public long ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string CreationDate { get; set; }
        public int Checked { get; set; } = 0;

        public override string ToString()
        {
            bool isChecked = Checked == 0 ? false : true;
            return $"{ID} - {Title} - {isChecked}\n\r{Content}\n\r{CreationDate}\n";
        }
    }
}