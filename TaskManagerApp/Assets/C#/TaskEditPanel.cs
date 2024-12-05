using UnityEngine;
using UnityEngine.UI;
using System;

public class TaskEditPanel : MonoBehaviour
{
    public InputField titleInput;
    public InputField descriptionInput;
    public Dropdown priorityDropdown;
    public InputField dueDateInput;
    public InputField estimatedTimeInput;

    private Task currentTask;

    // Singleton pattern to ensure only one instance of TaskEditPanel is accessible
    public static TaskEditPanel Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Prevent multiple instances of the edit panel
        }

        // Make sure the panel is hidden when the game starts
        gameObject.SetActive(false);
    }

    public void Show(Task task)
    {
        currentTask = task;

        titleInput.text = task.Title;
        descriptionInput.text = task.Description;
        priorityDropdown.value = GetPriorityIndex(task.Priority);

        // Display the due date
        dueDateInput.text = task.DueDate != DateTime.MinValue
            ? task.DueDate.ToString("MM/dd/yyyy")
            : ""; // Show empty if the date is invalid

        estimatedTimeInput.text = task.EstimatedTime.ToString();

        gameObject.SetActive(true);
    }


    public void Hide()
    {
        // Hide the task edit panel
        gameObject.SetActive(false);
    }

    private int GetPriorityIndex(string priority)
    {
        return priority switch
        {
            "High" => 0,
            "Medium" => 1,
            "Low" => 2,
            _ => 3 // Default for invalid priority
        };
    }

    public void SaveEdits()
    {
        if (currentTask != null)
        {
            // Update the task with the edited values
            currentTask.Title = titleInput.text;
            currentTask.Description = descriptionInput.text;
            currentTask.Priority = priorityDropdown.options[priorityDropdown.value].text;

            // Update the due date
            DateTime newDueDate;
            if (DateTime.TryParseExact(dueDateInput.text, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out newDueDate))
            {
                currentTask.DueDate = newDueDate;
            }

            // Update the estimated time
            float newEstimatedTime;
            if (float.TryParse(estimatedTimeInput.text, out newEstimatedTime))
            {
                currentTask.EstimatedTime = newEstimatedTime;
            }

            // Hide the panel after saving
            Hide();

            // Notify TaskManager to update the task
            TaskManager.Instance.UpdateTask(currentTask);

            // Now, update the UI immediately to reflect changes
            UIManager.Instance.UpdateTaskListUI(TaskManager.Instance.GetAllTasks()); // Refresh task list in UI
        }
    }


    public void CancelEdits()
    {
        // Simply hide the task edit panel without saving any changes
        Hide();
    }
}
