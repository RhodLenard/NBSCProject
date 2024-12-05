using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

public class TaskManager : MonoBehaviour
{
    // Singleton instance
    public static TaskManager Instance;
    private List<Task> tasks = new List<Task>();

    private string saveFilePath;



    // Awake method to ensure the Singleton pattern is applied
    void Awake()
    {

        if (Instance == null)
        {
            Instance = this; // Set the static instance to this instance of TaskManager

            saveFilePath = Path.Combine(Application.persistentDataPath, "task.json");
            LoadTasks();
        }
        else
        {
            Destroy(gameObject); // Destroy if there's already an existing instance
        }
        
    }

    public void SaveTasks()
    {
        foreach (var task in tasks)
        {
            Debug.Log($"Saving Task: {task.Title}, DueDate: {task.DueDate}, DueDateString: {task.DueDateString}");
        }

        string json = JsonUtility.ToJson(new TaskListWrapper(tasks), true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log($"Tasks saved to {saveFilePath}");
    }

    public void LoadTasks()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            TaskListWrapper loadedData = JsonUtility.FromJson<TaskListWrapper>(json);

            foreach (var task in loadedData.tasks)
            {
                if (!string.IsNullOrEmpty(task.DueDateString))
                {
                    task.DueDateString = task.DueDateString; // Trigger the setter to update DueDate
                }
                else
                {
                    task.DueDate = DateTime.MinValue;
                }
            }

            tasks = loadedData.tasks;
            Debug.Log($"Tasks loaded from {saveFilePath}. Count: {tasks.Count}");
        }
        else
        {
            Debug.Log("No saved tasks found, starting fresh.");
            tasks = new List<Task>();
        }
    }

    [Serializable]
    private class TaskListWrapper
    {
        public List<Task> tasks;

        public TaskListWrapper(List<Task> tasks)
        {
            this.tasks = tasks;
        }
    }

    // Add a task to the list
    public void AddTask(Task newTask)
    {
        tasks.Add(newTask);
        SaveTasks();  // Save tasks after adding
    }

    // Get all tasks
    public List<Task> GetAllTasks()
    {
        return tasks;
    }

    // Get overdue tasks
    public List<Task> GetOverdueTasks()
    {
        return tasks.FindAll(t => t.DueDate.Date < DateTime.Now.Date);
    }

    // Get tasks due today
    public List<Task> GetTasksDueToday()
    {
        return tasks.FindAll(t => t.DueDate.Date == DateTime.Now.Date);
    }

    // Get tasks due in the future (not today and not overdue)
    public List<Task> GetCurrentTasks()
    {
        return tasks.FindAll(t => t.DueDate > DateTime.Now && t.DueDate.Date != DateTime.Now.Date);
    }

    // Sort tasks by priority
    public List<Task> SortTasksByPriority()
    {
        List<Task> sortedTasks = tasks.OrderBy(t => t.PriorityValue).ToList();
        Debug.Log("Sorted by Priority: " + string.Join(", ", sortedTasks.Select(t => t.Title)));
        return sortedTasks;
    }

    // Sort tasks by due date
    public List<Task> SortTasksByDueDate()
    {
        return tasks.OrderBy(t => t.DueDate).ToList();
    }

    // Sort tasks by estimated time (ascending)
    public List<Task> SortTasksByEstimatedTime()
    {
        return tasks.OrderBy(t => t.EstimatedTime).ToList();
    }

    // Remove a task
    public void RemoveTask(Task task)
    {
        tasks.Remove(task);
        SaveTasks(); // Save tasks after removal

        // Notify the UI to update
        UIManager.Instance?.UpdateTaskListUI(tasks);
        UIManager.Instance?.UpdateTaskSummary();
    }

    // Search tasks by keyword
    public List<Task> SearchTasks(string keyword)
    {
        return tasks.FindAll(task => task.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                                      task.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }

    // Update a task with new details
    public void UpdateTask(Task updatedTask)
    {
        // Find the task to update using a unique identifier (e.g., Title or ID)
        Task taskToUpdate = tasks.Find(t => t.Title == updatedTask.Title); // You might want to use an ID if available.

        if (taskToUpdate != null)
        {
            // Update the task details
            taskToUpdate.Title = updatedTask.Title;
            taskToUpdate.Description = updatedTask.Description;
            taskToUpdate.Priority = updatedTask.Priority;
            taskToUpdate.DueDate = updatedTask.DueDate;
            taskToUpdate.EstimatedTime = updatedTask.EstimatedTime;

            Debug.Log("Task updated: " + taskToUpdate.Title);
        }
    }

   
}
