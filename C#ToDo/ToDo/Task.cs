using System;

namespace ToDo
{
    public enum ImportanceLevel
    {
        WasteOfTime = 1,
        NotImportant = 2,
        Meh = 3,
        Important = 4,
        DoMeOrDie = 5
    }

    public class Task
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public bool IsCompleted { get; set; }

        public ImportanceLevel Importance { get; set; }

        public DateTime DateAdded { get; set; }

    
        public Task(string description, ImportanceLevel importance)
        {
            Id = Guid.NewGuid();
            Description = description;
            IsCompleted = false; 
            Importance = importance;
            DateAdded = DateTime.Now;
        }

        public override string ToString()
        {
            return $"[{Id.ToString().Substring(0, 8)}...] {Description} (Importance: {Importance}) - {(IsCompleted ? "Completed" : "Pending")} (Added: {DateAdded.ToShortDateString()})";
        }
    }
}
