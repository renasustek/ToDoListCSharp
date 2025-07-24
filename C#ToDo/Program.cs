
using System; 
using ToDo;   

namespace toDoApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            
            ToDo.Task myNewTask = new ToDo.Task("Learn C# Namespaces", ImportanceLevel.Important);

            Console.WriteLine(myNewTask.ToString());
        }
    }
}
