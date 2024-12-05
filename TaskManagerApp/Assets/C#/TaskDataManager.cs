using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System;

public static class TaskDataManager
{
    private static readonly string FilePath = Path.Combine(Application.persistentDataPath, "task.json");

    public static void SaveTasks(List<Task> tasks)
    {
        string json = JsonUtility.ToJson(new TaskListWrapper(tasks), true);
        File.WriteAllText(FilePath, json);
        Debug.Log("Tasks saved to: " + FilePath);
    }

    public static List<Task> LoadTasks()
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            Debug.Log("Loaded JSON: " + json);
            var taskListWrapper = JsonUtility.FromJson<TaskListWrapper>(json);

            foreach (var task in taskListWrapper.tasks)
            {
                if (!string.IsNullOrEmpty(task.DueDateString))
                {
                    task.DueDateString = task.DueDateString; // Trigger setter to update DueDate
                }
                else
                {
                    task.DueDate = DateTime.MinValue;
                }
                Debug.Log($"Loaded Task: {task.Title}, DueDate: {task.DueDate}, DueDateString: {task.DueDateString}");
            }

            return taskListWrapper.tasks;
        }

        Debug.LogWarning("No task.json file found. Returning an empty task list.");
        return new List<Task>();
    }


    [Serializable]
    private class TaskListWrapper
    {
        public List<Task> tasks;
        public TaskListWrapper(List<Task> tasks) { this.tasks = tasks; }
    }
}