// Program.cs
// This file implements a simple command-line interface for managing tasks,
// now using a dedicated TaskService class for business logic.

using System;
using System.Collections.Generic;
using ToDo; // Required to access the Task class and ImportanceLevel enum

namespace toDoApplication
{
    class Program
    {
        // Connection string for the MySQL database.
        // IMPORTANT: Replace 'your_strong_password' with the actual password you set in docker-compose.yml.
        private const string ConnectionString = "Server=127.0.0.1;Port=3306;Database=todo_db;Uid=root;Pwd=your_strong_password;";

        // Instance of the TaskService to handle all business operations.
        private static TaskService? _taskService;

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the ToDo Application (Service Layer Version)!");
            Console.WriteLine("---");

            // Initialize the TaskRepository and inject it into the TaskService.
            // This is where we wire up the dependencies.
            TaskRepository taskRepository = new TaskRepository(ConnectionString);
            _taskService = new TaskService(taskRepository);

            // Test database connection on startup
            if (!_taskService.TestDatabaseConnection())
            {
                Console.WriteLine("Failed to connect to the database. Please ensure MySQL Docker container is running and accessible.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Successfully connected to the database!");

            // Main application loop
            while (true)
            {
                DisplayMenu();
                string? command = Console.ReadLine()?.ToLower().Trim();

                switch (command)
                {
                    case "add":
                        AddTask();
                        break;
                    case "list":
                        ListTasks();
                        break;
                    case "remove":
                        RemoveTask();
                        break;
                    case "alter":
                        AlterTask();
                        break;
                    case "exit":
                        Console.WriteLine("Exiting ToDo Application. Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid command. Please try again.");
                        break;
                }
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        // Displays the main menu options to the user.
        static void DisplayMenu()
        {
            Console.WriteLine("Commands:");
            Console.WriteLine("  add    - Add a new task");
            Console.WriteLine("  list   - List all tasks");
            Console.WriteLine("  remove - Remove a task by ID");
            Console.WriteLine("  alter  - Alter an existing task (description, importance, or completion status)");
            Console.WriteLine("  exit   - Exit the application");
            Console.Write("Enter command: ");
        }

        // Handles adding a new task using the service layer.
        static void AddTask()
        {
            Console.Write("Enter task description: ");
            string? description = Console.ReadLine();

            Console.WriteLine("Select importance level:");
            foreach (ImportanceLevel level in Enum.GetValues(typeof(ImportanceLevel)))
            {
                Console.WriteLine($"  {(int)level}. {level}");
            }
            Console.Write($"Enter importance (1-5): ");
            string? importanceInput = Console.ReadLine();

            ImportanceLevel importance = ImportanceLevel.WasteOfTime;
            if (int.TryParse(importanceInput, out int importanceValue) && Enum.IsDefined(typeof(ImportanceLevel), importanceValue))
            {
                importance = (ImportanceLevel)importanceValue;
            }
            else
            {
                Console.WriteLine("Invalid importance level. Defaulting to 'WasteOfTime'.");
            }

            _taskService?.AddTask(description, importance);
        }

        // Lists all tasks using the service layer.
        static void ListTasks()
        {
            List<ToDo.Task> tasks = _taskService?.GetAllTasks() ?? new List<ToDo.Task>();

            if (tasks.Count == 0)
            {
                Console.WriteLine("No tasks found in the database.");
                return;
            }

            Console.WriteLine("\n--- Your Tasks ---");
            foreach (var task in tasks)
            {
                Console.WriteLine(task.ToString());
            }
            Console.WriteLine("------------------");
        }

        // Handles removing a task using the service layer.
        static void RemoveTask()
        {
            Console.Write("Enter the ID (first 8 chars of GUID) of the task to remove: ");
            string? idInput = Console.ReadLine()?.Trim();

            _taskService?.RemoveTask(idInput);
        }

        // Handles altering an existing task using the service layer.
        static void AlterTask()
        {
            Console.Write("Enter the ID (first 8 chars of GUID) of the task to alter: ");
            string? idInput = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(idInput))
            {
                Console.WriteLine("Task ID cannot be empty.");
                return;
            }

            Console.WriteLine($"\n--- Altering Task with ID prefix: {idInput} ---");
            Console.WriteLine("What do you want to alter?");
            Console.WriteLine("  1. Description");
            Console.WriteLine("  2. Importance");
            Console.WriteLine("  3. Completion Status");
            Console.Write("Enter choice (1-3): ");
            string? choice = Console.ReadLine();

            string? newDescription = null;
            ImportanceLevel? newImportance = null;
            bool? newIsCompleted = null;

            switch (choice)
            {
                case "1":
                    Console.Write("Enter new description: ");
                    newDescription = Console.ReadLine();
                    break;
                case "2":
                    Console.WriteLine("Select new importance level:");
                    foreach (ImportanceLevel level in Enum.GetValues(typeof(ImportanceLevel)))
                    {
                        Console.WriteLine($"  {(int)level}. {level}");
                    }
                    Console.Write("Enter new importance (1-5): ");
                    string? importanceInput = Console.ReadLine();
                    if (int.TryParse(importanceInput, out int importanceValue) && Enum.IsDefined(typeof(ImportanceLevel), importanceValue))
                    {
                        newImportance = (ImportanceLevel)importanceValue;
                    }
                    else
                    {
                        Console.WriteLine("Invalid importance level. No change made.");
                    }
                    break;
                case "3":
                    Console.Write("Mark as completed? (yes/no): ");
                    string? completedInput = Console.ReadLine()?.ToLower();
                    if (completedInput == "yes")
                    {
                        newIsCompleted = true;
                    }
                    else if (completedInput == "no")
                    {
                        newIsCompleted = false;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. No change made to completion status.");
                    }
                    break;
                default:
                    Console.WriteLine("Invalid choice. No changes made.");
                    break;
            }

            _taskService?.AlterTask(idInput, newDescription, newImportance, newIsCompleted);
        }
    }
}
