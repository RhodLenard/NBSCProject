using System;
using UnityEngine;

[Serializable]
public class Task
{
    public int ID;
    public string Title;
    public string Description;
    public string Priority;
    public float EstimatedTime;

    // Store the due date as a string for serialization
    [NonSerialized] private DateTime dueDate;
    [SerializeField] private string dueDateString;

    public DateTime DueDate
    {
        get => dueDate;
        set
        {
            dueDate = value;
            dueDateString = dueDate.ToString("MM/dd/yyyy");
        }
    }

    public string DueDateString
    {
        get => dueDateString;
        set
        {
            dueDateString = value;

            if (DateTime.TryParseExact(value, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsedDate))
            {
                dueDate = parsedDate;
            }
            else
            {
                dueDate = DateTime.MinValue;
                Debug.LogError($"Failed to parse DueDateString: {value}. Defaulting to {DateTime.MinValue}");
            }
        }
    }

    public Task(string title, string description, string priority, DateTime dueDate, float estimatedTime)
    {
        Title = title;
        Description = description;
        Priority = priority;
        DueDate = dueDate;
        EstimatedTime = estimatedTime;
    }

    public Task() { }




// Priority values to allow sorting or comparison (1 for High, 2 for Medium, 3 for Low)
public int PriorityValue
    {
        get
        {
            return Priority switch
            {
                "High" => 1,
                "Medium" => 2,
                "Low" => 3,
                _ => 4, // Default for undefined priority
            };
        }
    }
}
