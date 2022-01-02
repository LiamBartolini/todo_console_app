using Pastel;
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using todo_console_app.Models;

namespace todo_console_app
{
    class Program
    {
        public enum Action {
            ResetPassword,
            SigIn,
            LogIn
        }

        static User currentUser = null;

        static void Main()
        {
            PrintTitle("Todo-App");
            ConsoleKey keyPressed;
            Todos db = new();
            IEnumerable<Todo> query;
            string strId;
            
            // value from 1 and the unique ID in db of the todo
            long gapValue;

            Console.WriteLine("Are you signed in? [y/n]");
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.Y:
                default:
                    Login();
                    break;

                case ConsoleKey.N:
                    SignIn();
                    Login();
                    break;
            }

            if (currentUser == null) 
            {
                PrintError("User not logged in!");
                Environment.Exit(0);
            }

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
                                CreationDate = $"{DateTime.Today:dd/MM}",
                                FkUserId = currentUser.Id
                            }
                        );
                        break;

                    case ConsoleKey.D2:
                        query = (from todo in db.Db
                                where todo.Checked == 0 && todo.FkUserId == currentUser.Id // if checked == 0 is like unchecked; otherwise (value == 1) is checked 
                                select todo).ToList<Todo>(); 

                        gapValue = PrintFormattedToDo(query as List<Todo>);
                        VisualizeAllContent(gapValue: gapValue);
                        break;

                    case ConsoleKey.D3:
                        query = (from todo in db.Db
                                where todo.FkUserId == currentUser.Id
                                select todo).ToList<Todo>(); 

                        gapValue = PrintFormattedToDo(query.ToList<Todo>());
                        VisualizeAllContent(gapValue: gapValue);
                        break;

                    case ConsoleKey.D4:
                        query = from todo in db.Db
                                where todo.Checked == 1 && todo.FkUserId == currentUser.Id
                                select todo;

                        PrintFormattedToDo(query.ToList<Todo>());
                        break;

                    case ConsoleKey.D5:
                        query = from todo in db.Db
                                where todo.FkUserId == currentUser.Id
                                select todo;

                        gapValue = PrintFormattedToDo(query.ToList<Todo>());
                        
                        Console.Write("Insert ID: ");
                        strId = Console.ReadLine();
                        
                        try { CheckTodo(long.Parse(strId) + gapValue); }
                        catch (Exception ex) { PrintError(ex.Message); }
                        
                        break;

                    case ConsoleKey.D6:
                        Console.Write("Insert ID: ");
                        strId = Console.ReadLine();
                        
                        try { RemoveTodo(long.Parse(strId)); }
                        catch (Exception ex) { PrintError(ex.Message); }

                        break;
                    
                    case ConsoleKey.D7:
                        Console.Write("Insert ID: ");
                        strId = Console.ReadLine();
                        
                        Console.Write("Insert content (it can be empty): ");
                        strContent = Console.ReadLine();
                        
                        Console.Write("Insert title (it can be empty): ");
                        strTitle = Console.ReadLine();
                        
                        try { ModifyTodo(long.Parse(strId), strContent, strTitle); }
                        catch (Exception ex) { PrintError(ex.Message); }
                    break;

                    case ConsoleKey.D8:
                        SignOut();
                        break;
                    
                    case ConsoleKey.R:
                        InsertPassword(Action.ResetPassword);
                        break;

                    default:
                        Environment.Exit(-1);
                        break;
                }
            } while (true);
        }

        static bool ResetPassword(string newPassword)
        {
            Todos db = new();
            string oldPassword = (from user in db.Users
                        where user.Id == currentUser.Id
                        select user.Password).First();

            if (newPassword != oldPassword) 
            {
                var query = (from user in db.Users
                        where user.Id == currentUser.Id
                        select user).First();

                query.Password = newPassword;
                db.SaveChanges();
                return true;
            }
            else
                return false;
        }

        // method for insert password
        static string InsertPassword(Action action) {
            ConsoleKey pressed;
            string strNewPassword = "";
            
            // hide the password while typing it
            Console.Write("Insert password: ");
            (int, int) initialPosition = Console.GetCursorPosition();
            do {
                pressed = Console.ReadKey(true).Key;

                // check for number
                if (((int)pressed) >= 48 && ((int)pressed) <= 57) {
                    strNewPassword += ((int)pressed) - 48;
                    Console.WriteLine(strNewPassword);
                    continue;
                }

                if (pressed == ConsoleKey.Backspace) {
                    if (strNewPassword.Length - 1 >= 0) {
                        strNewPassword = strNewPassword.Remove(strNewPassword.Length - 1);
                    }
                    continue;
                }
                
                if (pressed == ConsoleKey.Escape) {
                    strNewPassword = "";
                    continue;
                }

                if (pressed != ConsoleKey.Enter && pressed != ConsoleKey.Escape) {
                    if (Console.CapsLock) {
                        strNewPassword += pressed.ToString().ToUpper();
                    } else {
                        strNewPassword += pressed.ToString().ToLower();
                    }
                }

                Console.WriteLine(strNewPassword);
                Console.SetCursorPosition(initialPosition.Item1, initialPosition.Item2);
            } while(pressed != ConsoleKey.Enter || strNewPassword.Length <= 8);

            switch(action) {
                case Action.ResetPassword:
                    if (ResetPassword(strNewPassword)) {
                        Console.WriteLine("Password changed!".Pastel("#00ff00"));
                        Login();
                    } else {
                        PrintError("Passwords cannot be the same!");
                    }
                break;

                case Action.LogIn:
                case Action.SigIn:
                    return strNewPassword;

                default:
                    break;
            }
            return null;
        }

        static void SignOut()
        {
            currentUser = null;
            Main();
        }

        static void SignIn()
        {
            string username, password, email;
            Console.WriteLine("Sing in".Pastel("#00fff0"));
            Todos db = new();

            do
            {
                Console.Write("Insert username: ");
                username = Console.ReadLine();

                password = InsertPassword(Action.SigIn);

                Console.Write("\nInsert email: ");
                email = Console.ReadLine();

                var query = (from user in db.Users
                            where user.Username == username || user.Email == email
                            select user).ToList<User>();

                if (query.Count() > 0) {
                    PrintError("Cannot exists more then one account with the same username/email");
                } else {
                    break;
                }
                
            } while (true);

            db.Users.Add(
                new() {
                    Username = username,
                    Password = password,
                    Email = email
                }
            );

            db.SaveChanges();

            Console.Clear();
        }

        static void Login()
        {
            string username, password;
            Console.WriteLine("Log in".Pastel("#00fff0"));
            
            do 
            {
                Console.Write("Insert username: ");
                username = Console.ReadLine();

                password = InsertPassword(Action.LogIn);

                Console.Clear();
            } while (username == string.Empty && password == string.Empty);
            
            Todos db = new();
            
            var query = (from user in db.Users
                        where user.Username == username && user.Password == password
                        select user).ToList();

            if (query.Count <= 0)
            {
                PrintError("User doesn't exist!".Pastel("#ff0000"));
                Main();
            }

            currentUser = query.First();
            Console.Clear();
        }

        static void PrintError(string err)
        {
            Console.WriteLine(err.Pastel("#ff0000"));
        }

        static void VisualizeAllContent(long gapValue = 0)
        {
            Console.Write("Enter the id for visualize all the content:");
            long idTodo = 0;
            try
            {
                idTodo = long.Parse(Console.ReadLine()) + gapValue;
                Todos db = new();
                var todo = (from record in db.Db
                            where record.Id == idTodo && record.FkUserId == currentUser.Id
                            select record).First();

                Console.Write($"Content: `{todo.Content}`\n");

            }
            catch (Exception ex) { PrintError(ex.Message); return; }
        }

        static void RemoveTodo(long idTodo)
        {
            Todos db = new();
            var todo = (from record in db.Db
                        where record.Id == idTodo && record.FkUserId == currentUser.Id
                        select record).First();

            db.Db.Remove(todo);
            db.SaveChanges();
        }

        static void CheckTodo(long idTodo)
        {
            Todos db = new();
            var todo = (from record in db.Db
                       where record.Id == idTodo && record.FkUserId == currentUser.Id
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
                        where record.Id == idTodo && record.FkUserId == currentUser.Id
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
            menu.AppendLine("[8] - Sign out");
            menu.AppendLine("[r] - Reset account password");
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
        
        static long PrintFormattedToDo(List<Todo> todos)
        {
            ConsoleResponsiveTable.PrintSepartorLine();
            ConsoleResponsiveTable.PrintRow("ID", "Title", "Content", "Checked");
            foreach (Todo todo in todos)
            {
                ConsoleResponsiveTable.PrintSepartorLine();
                ConsoleResponsiveTable.PrintRow((todos.IndexOf(todo) + 1).ToString(), todo.Title, todo.Content, todo.Checked.ToString());
            }
            ConsoleResponsiveTable.PrintSepartorLine();
            return todos.First().Id - 1;
        }
    }
}