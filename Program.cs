﻿using Pastel;
using System;
using System.Net;
using System.Text;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Collections.Generic;
using todo_console_app.Models;

namespace todo_console_app
{
    class Program
    {
        static User currentUser = null;

        static void Main()
        {
            PrintTitle("Todo-App");
            ConsoleKey keyPressed;
            Todos db = new();
            IEnumerable<Todo> query;
            string strId;

            Console.WriteLine("Are you signed in? [y/n]");
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.Y:
                    Login();
                    break;

                case ConsoleKey.N:
                default:
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
                        query = from todo in db.Db
                                where todo.Checked == 0 && todo.FkUserId == currentUser.Id // if checked == 0 is like unchecked; otherwise (value == 1) is checked 
                                select todo; 

                        PrintFormattedToDo(query);
                        VisualizeAllContent();
                        break;

                    case ConsoleKey.D3:
                        query = (from todo in db.Db
                                where todo.FkUserId == currentUser.Id
                                select todo).ToList<Todo>(); 

                        PrintFormattedToDo(query);
                        VisualizeAllContent();
                        break;

                    case ConsoleKey.D4:
                        query = from todo in db.Db
                                where todo.Checked == 1 && todo.FkUserId == currentUser.Id
                                select todo;

                        PrintFormattedToDo(query);    
                        break;

                    case ConsoleKey.D5:
                        Console.Write("Insert ID: ");
                        strId = Console.ReadLine();
                        
                        try { CheckTodo(long.Parse(strId)); }
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
                        Console.Write("Insert new password: ");
                        string strNewPassword = Console.ReadLine();
                        
                        if (ResetPassword(strNewPassword)) {
                            Console.WriteLine("Password changed!".Pastel("#00ff00"));
                            Login();
                        } else {
                            PrintError("Passwords cannot be the same!");
                            continue;
                        }
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

        static void SignOut()
        {
            currentUser = null;
            Main();
        }

        static void SignIn()
        {
            string username, password, email;
            Console.WriteLine("Sing in".Pastel("#00fff0"));

            Console.Write("Insert username: ");
            username = Console.ReadLine();

            Console.Write("Insert password: ");
            password = Console.ReadLine();

            Console.Write("Insert email: ");
            email = Console.ReadLine();

            Todos db = new();
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

                Console.Write("Insert password: ");
                password = Console.ReadLine();

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

        static void VisualizeAllContent()
        {
            Console.Write("Enter the id for visualize all the content:");
            long idTodo = 0;
            try
            {
                idTodo = long.Parse(Console.ReadLine());
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

        static void PrintFormattedToDo(Todo todo)
        {
            ConsoleResponsiveTable.PrintSepartorLine();
            ConsoleResponsiveTable.PrintRow("ID", "Title", "Content", "Checked");
            ConsoleResponsiveTable.PrintSepartorLine();
            ConsoleResponsiveTable.PrintRow(todo.Id.ToString(), todo.Title, todo.Content, todo.Checked.ToString());
        }

        static void PrintFormattedToDo(IEnumerable<Todo> todos)
        {
            ConsoleResponsiveTable.PrintSepartorLine();
            ConsoleResponsiveTable.PrintRow("ID", "Title", "Content", "Checked");
            foreach (Todo todo in todos)
            {
                ConsoleResponsiveTable.PrintSepartorLine();
                ConsoleResponsiveTable.PrintRow(todo.Id.ToString(), todo.Title, todo.Content, todo.Checked.ToString());
            }
            ConsoleResponsiveTable.PrintSepartorLine();
        }
    }
}