using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using ToDo;

namespace toDoApplication
{
    public class TaskRepository
    {
        private readonly string _connectionString;

        public TaskRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool TestConnection()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Database connection error: {ex.Message}");
                return false;
            }
        }

        public void AddTask(ToDo.Task newTask)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO tasks (id, description, is_completed, importance, date_added) VALUES (@id, @description, @isCompleted, @importance, @dateAdded)";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", newTask.Id.ToString());
                        command.Parameters.AddWithValue("@description", newTask.Description);
                        command.Parameters.AddWithValue("@isCompleted", newTask.IsCompleted);
                        command.Parameters.AddWithValue("@importance", (int)newTask.Importance);
                        command.Parameters.AddWithValue("@dateAdded", newTask.DateAdded);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error adding task: {ex.Message}");
                throw;
            }
        }

        public List<ToDo.Task> GetAllTasks()
        {
            List<ToDo.Task> tasks = new List<ToDo.Task>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT id, description, is_completed, importance, date_added FROM tasks ORDER BY date_added ASC";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string description = reader.GetString("description");
                                ImportanceLevel importance = (ImportanceLevel)reader.GetInt32("importance");

                                ToDo.Task task = new ToDo.Task(description, importance);
                                task.Id = Guid.Parse(reader.GetString("id"));
                                task.IsCompleted = reader.GetBoolean("is_completed");
                                task.DateAdded = reader.GetDateTime("date_added");
                                tasks.Add(task);
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error listing tasks: {ex.Message}");
                throw;
            }
            return tasks;
        }

        public ToDo.Task? GetTaskByIdPrefix(string idPrefix)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string selectSql = "SELECT id, description, is_completed, importance, date_added FROM tasks WHERE id LIKE @idPrefix LIMIT 1";
                    using (MySqlCommand selectCommand = new MySqlCommand(selectSql, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@idPrefix", $"{idPrefix}%");
                        using (MySqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string description = reader.GetString("description");
                                ImportanceLevel importance = (ImportanceLevel)reader.GetInt32("importance");

                                ToDo.Task task = new ToDo.Task(description, importance);
                                task.Id = Guid.Parse(reader.GetString("id"));
                                task.IsCompleted = reader.GetBoolean("is_completed");
                                task.DateAdded = reader.GetDateTime("date_added");
                                return task;
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error retrieving task: {ex.Message}");
                throw;
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid GUID format encountered during retrieval.");
                throw;
            }
            return null;
        }

        public bool RemoveTask(Guid taskId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string deleteSql = "DELETE FROM tasks WHERE id = @id";
                    using (MySqlCommand deleteCommand = new MySqlCommand(deleteSql, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@id", taskId.ToString());
                        int rowsAffected = deleteCommand.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error removing task: {ex.Message}");
                return false;
            }
        }

        public bool UpdateTask(ToDo.Task taskToUpdate)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string updateSql = "UPDATE tasks SET description = @description, is_completed = @isCompleted, importance = @importance WHERE id = @id";
                    using (MySqlCommand updateCommand = new MySqlCommand(updateSql, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@description", taskToUpdate.Description);
                        updateCommand.Parameters.AddWithValue("@isCompleted", taskToUpdate.IsCompleted);
                        updateCommand.Parameters.AddWithValue("@importance", (int)taskToUpdate.Importance);
                        updateCommand.Parameters.AddWithValue("@id", taskToUpdate.Id.ToString());
                        int rowsAffected = updateCommand.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error updating task: {ex.Message}");
                return false;
            }
        }
    }
}
