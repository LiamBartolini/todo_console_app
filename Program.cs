using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using todo_console_app.Models;

namespace todo_console_app
{
    class Program
    {
        static void Main(string[] args)
        {
            PrintTitle("Todo-App");
            ConsoleKey keyPressed;
            Todos db = new();
            IEnumerable<Todo> query;
            string strId;
            do
            {
                PrintMenu();
                keyPressed = Console.ReadKey(true).Key;
                switch (keyPressed) 
                {
                    case ConsoleKey.D0:
                        Environment.Exit(0);
                        break;

                    case ConsoleKey.C:
                        Console.Clear();
                        PrintTitle("Todo-App");
                        break;

                    case ConsoleKey.D1:
                        Console.Write("Insert title: ");
                        string strTitle = Console.ReadLine();

                        Console.Write("Insert content: ");
                        string strContent = Console.ReadLine();
                        
                        AddTodo(
                            new Todo {
                                Title = strTitle,
                                Content = strContent,
                                CreationDate = $"{DateTime.Today:dd/MM}"
                            }
                        );

                        break;

                    case ConsoleKey.D2:
                        query = from todo in db.Db
                                where todo.Checked == 0 // if checked == 0 is like unchecked; otherwise (value == 1) is checked 
                                select todo; 

                        PrintFormattedToDo(query);
                        break;

                    case ConsoleKey.D3:
                        query = (from todo in db.Db
                                select todo).ToList<Todo>(); 

                        PrintFormattedToDo(query);
                        break;

                    case ConsoleKey.D4:
                        query = from todo in db.Db
                                where todo.Checked == 1
                                select todo;

                        PrintFormattedToDo(query);    
                        break;

                    case ConsoleKey.D5:
                        Console.Write("Insert ID: ");
                        strId = Console.ReadLine();
                        
                        CheckTodo(long.Parse(strId));
                        break;

                    case ConsoleKey.D6:
                        Console.Write("Insert ID: ");
                        strId = Console.ReadLine();

                        RemoveTodo(long.Parse(strId));                        
                        break;
                    
                    case ConsoleKey.D7:
                        Console.Write("Insert ID: ");
                        strId = Console.ReadLine();
                        
                        Console.Write("Insert content (it can be empty): ");
                        strContent = Console.ReadLine();
                        
                        Console.Write("Insert title (it can be empty): ");
                        strTitle = Console.ReadLine();
                        
                        ModifyTodo(long.Parse(strId), strContent, strTitle);
                    break;

                    default:
                        Environment.Exit(-1);
                        break;
                }
            } while (true);
        }

        static void RemoveTodo(long idTodo)
        {
            Todos db = new();
            var todo = (from record in db.Db
                        where record.ID == idTodo
                        select record).First();

            db.Db.Remove(todo);
            db.SaveChanges();
        }

        static void CheckTodo(long idTodo)
        {
            Todos db = new();
            var todo = (from record in db.Db
                       where record.ID == idTodo
                       select record).First();
            
            if (todo.Checked == 0)
                todo.Checked = 1;
            else 
                todo.Checked = 0;

            db.SaveChanges();
        }

        static void ModifyTodo(long idTodo, string content = "", string title = "")
        {
            Todos db = new();
            Todo todo = (from record in db.Db
                        where record.ID == idTodo
                        select record).First();   
            todo.Content = content != "" ? content : todo.Content;
            todo.Title = title != "" ? title : todo.Title;
            db.SaveChanges();
        }

        static void PrintMenu() 
        {
            StringBuilder menu = new();
            menu.AppendLine("[0] - Close program");
            menu.AppendLine("[C] - Clear window");
            menu.AppendLine("[1] - Add new Todo");
            menu.AppendLine("[2] - Visualize all un-checked Todo");
            menu.AppendLine("[3] - Visualize all Todo");
            menu.AppendLine("[4] - Visualize all checked Todo");
            menu.AppendLine("[5] - Check Todo");
            menu.AppendLine("[6] - Remove Todo");
            menu.AppendLine("[7] - Modify Todo");
            Console.WriteLine(menu.ToString());
        }

        static void AddTodo(Todo todo) 
        {
            Todos db = new();
            db.Db.Add(todo);
            db.SaveChanges();
        }

        static void PrintTitle(string title) 
        {
            (int, int) cursorPosition = Console.GetCursorPosition();
            Console.SetCursorPosition(Console.WindowWidth / 2, cursorPosition.Item2);
            Console.WriteLine(title);
        }

        static void PrintFormattedToDo(Todo todo)
        {
            ConsoleResponsiveTable.PrintSepartorLine();
            ConsoleResponsiveTable.PrintRow("ID", "Title", "Content", "Checked");
            ConsoleResponsiveTable.PrintSepartorLine();
            ConsoleResponsiveTable.PrintRow(todo.ID.ToString(), todo.Title, todo.Content, todo.Checked.ToString());
        }

        static void PrintFormattedToDo(IEnumerable<Todo> todos)
        {
            ConsoleResponsiveTable.PrintSepartorLine();
            ConsoleResponsiveTable.PrintRow("ID", "Title", "Content", "Checked");
            foreach (Todo todo in todos)
            {
                ConsoleResponsiveTable.PrintSepartorLine();
                ConsoleResponsiveTable.PrintRow(todo.ID.ToString(), todo.Title, todo.Content, todo.Checked.ToString());
            }
            ConsoleResponsiveTable.PrintSepartorLine();
        }
    }
}
