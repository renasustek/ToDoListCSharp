using System;
using System.Collections.Generic;
using ToDo;

namespace toDoApplication
{
    public class TaskService
    {
        private readonly TaskRepository _taskRepository;

        public TaskService(TaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public bool TestDatabaseConnection()
        {
            return _taskRepository.TestConnection();
        }

        public void AddTask(string description, ImportanceLevel importance)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                Console.WriteLine("Description cannot be empty. Task not added.");
                return;
            }

            ToDo.Task newTask = new ToDo.Task(description, importance);
            _taskRepository.AddTask(newTask);
        }

        public List<ToDo.Task> GetAllTasks()
        {
            return _taskRepository.GetAllTasks();
        }

        public bool RemoveTask(string idPrefix)
        {
            if (string.IsNullOrWhiteSpace(idPrefix))
            {
                Console.WriteLine("Task ID cannot be empty.");
                return false;
            }

            ToDo.Task? taskToRemove = _taskRepository.GetTaskByIdPrefix(idPrefix);

            if (taskToRemove != null)
            {
                if (_taskRepository.RemoveTask(taskToRemove.Id))
                {
                    Console.WriteLine($"Task '{taskToRemove.Description}' (ID: {taskToRemove.Id.ToString().Substring(0, 8)}...) removed successfully.");
                    return true;
                }
                else
                {
                    Console.WriteLine("Failed to remove task from the database.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Task not found with that ID.");
                return false;
            }
        }

        public bool AlterTask(string idPrefix, string? newDescription, ImportanceLevel? newImportance, bool? newIsCompleted)
        {
            if (string.IsNullOrWhiteSpace(idPrefix))
            {
                Console.WriteLine("Task ID cannot be empty.");
                return false;
            }

            ToDo.Task? taskToAlter = _taskRepository.GetTaskByIdPrefix(idPrefix);

            if (taskToAlter == null)
            {
                Console.WriteLine("Task not found with that ID.");
                return false;
            }

            bool updateNeeded = false;

            if (newDescription != null)
            {
                if (!string.IsNullOrWhiteSpace(newDescription))
                {
                    taskToAlter.Description = newDescription;
                    updateNeeded = true;
                }
                else
                {
                    Console.WriteLine("Description cannot be empty. No change made.");
                }
            }

            if (newImportance.HasValue)
            {
                taskToAlter.Importance = newImportance.Value;
                updateNeeded = true;
            }

            if (newIsCompleted.HasValue)
            {
                taskToAlter.IsCompleted = newIsCompleted.Value;
                updateNeeded = true;
            }

            if (updateNeeded)
            {
                if (_taskRepository.UpdateTask(taskToAlter))
                {
                    Console.WriteLine("Task updated successfully.");
                    Console.WriteLine($"Updated Task: {taskToAlter.ToString()}");
                    return true;
                }
                else
                {
                    Console.WriteLine("Task not updated (no rows affected or error occurred).");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("No changes requested or invalid input. No update performed.");
                return false;
            }
        }
    }
}
