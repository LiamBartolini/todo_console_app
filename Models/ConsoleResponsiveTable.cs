using System.Linq;
using static System.Console;

namespace todo_console_app.Models
{
    public static class ConsoleResponsiveTable
    {
        private const int TABLE_WIDTH = 60;

        public static void PrintSepartorLine()
        {
            WriteLine(new string('-', TABLE_WIDTH));   
        }

        public static void PrintRow(params string[] cols)
        {
            int colWidth = (TABLE_WIDTH - cols.Length) / cols.Length;
   
            const string SEED = "|";
   
            string row = cols.Aggregate(SEED, (separator, columnText) => separator + GetCenterAllignedText(columnText, colWidth) + SEED);

            WriteLine(row);
        }

        public static string GetCenterAllignedText(string columnText, int columnWidth)
        {
            columnText = columnText.Length > columnWidth ? columnText.Substring(0, columnWidth - 3) + "..." : columnText;

            return string.IsNullOrEmpty(columnText)
                    ? new string(' ', columnWidth)
                    : columnText.PadRight(columnWidth - ((columnWidth - columnText.Length) / 2)).PadLeft(columnWidth);
        }
    }
}