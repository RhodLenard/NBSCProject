using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

public class UIManager : MonoBehaviour
{
    public InputField titleInput;
    public InputField descriptionInput;
    public Dropdown priorityDropdown;
    public InputField dueDateInput;
    public InputField estimatedTimeInput;
    public Text taskSummaryText;
    public Dropdown sortDropdown;  // Dropdown to choose sorting option
    public InputField searchInputField;  // InputField for search query
    public Button searchButton;          // Button to trigger search
    public GameObject taskItemPrefab;    // The prefab for task UI items
    public Transform taskListParent;     // Parent container for task UI elements


    public GameObject noTasksText;  // Reference to the "No tasks added" text GameObject


    private TaskManager taskManager;

    public static UIManager Instance;

    void Start()
    {
        taskManager = GetComponent<TaskManager>();

        dueDateInput.onValueChanged.AddListener(OnDueDateInputChanged);

        UIManager.Instance?.UpdateTaskSummary();

        // Default sorting by priority
        sortDropdown.value = 0;  // Default to 'Priority' option
        sortDropdown.onValueChanged.AddListener(OnSortOptionChanged); // Add listener for sorting changes
        sortDropdown.onValueChanged.Invoke(sortDropdown.value);  // Trigger the sorting immediately

        // Add listener to search button click
        searchButton.onClick.AddListener(OnSearchButtonClick);

        // Initially, show "No tasks added" text if there are no tasks
        UpdateNoTasksText();  // Ensure the "No tasks added" text is properly set on start
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;   
        } 
        else
        {
            Destroy(gameObject); // Prevent multiple instances
        }
    }
    private void OnDueDateInputChanged(string text)
    {
        // Remove non-numeric characters
        string cleanedInput = System.Text.RegularExpressions.Regex.Replace(text, @"[^0-9]", "");

        if (cleanedInput.Length > 8)
        {
            cleanedInput = cleanedInput.Substring(0, 8);
        }

        // Format the input as MM/dd/yyyy
        if (cleanedInput.Length >= 2)
        {
            cleanedInput = cleanedInput.Insert(2, "/"); // Add slash after month
        }
        if (cleanedInput.Length >= 5)
        {
            cleanedInput = cleanedInput.Insert(5, "/"); // Add slash after day
        }

        // Update the input field only if it has changed
        if (cleanedInput != dueDateInput.text)
        {
            dueDateInput.text = cleanedInput;
            // Set the caret position to the end of the text
            dueDateInput.caretPosition = cleanedInput.Length;
        }
    }

    public void AddTask()
    {
        string title = titleInput.text;
        string description = descriptionInput.text;
        string priority = priorityDropdown.options[priorityDropdown.value].text;

        DateTime dueDate;
        string dateInput = dueDateInput.text;

        if (string.IsNullOrWhiteSpace(dateInput))
        {
            Debug.LogError("Due date cannot be empty. Please enter a valid date.");
            return;
        }

        if (!DateTime.TryParseExact(dateInput, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dueDate))
        {
            Debug.LogError($"Invalid date format: '{dateInput}'. Please use the format MM/dd/yyyy.");
            return;
        }

        float estimatedTime;
        if (!float.TryParse(estimatedTimeInput.text, out estimatedTime))
        {
            Debug.LogError("Invalid estimated time format. Please enter a valid number.");
            return;
        }

        Task newTask = new Task(title, description, priority, dueDate, estimatedTime);

        // Add the task to the TaskManager
        taskManager.AddTask(newTask);

        // Update the task list UI after adding the task
        UpdateTaskListUI(taskManager.GetAllTasks());

        // Update the task summary
        UpdateTaskSummary();

        // Clear input fields
        titleInput.text = "";
        descriptionInput.text = "";
        dueDateInput.text = "";
        estimatedTimeInput.text = "";

        // Update the "No tasks added" visibility
        UpdateNoTasksText();
    }

    // Other methods like UpdateTaskListUI, UpdateTaskSummary, UpdateNoTasksText...


    // Removes a task
    public void RemoveTask(Task taskToRemove)
    {
        // Remove the task from the TaskManager
        taskManager.RemoveTask(taskToRemove);

        // Log to debug if the task removal is working as expected
        Debug.Log("Task removed: " + taskToRemove.Title);

        // Update the task list UI after removal
        UpdateTaskListUI(taskManager.GetAllTasks());

        // Update the task summary (overdue, today, current tasks)
        UpdateTaskSummary();

        // Update the "No tasks added" visibility (if applicable)
        UpdateNoTasksText();
    }


    // Adds the task to the UI (instantiates the prefab)
    public void AddTaskToUI(Task task)
    {
        Debug.Log("Instantiating Task: " + task.Title);  // Log task details

        GameObject taskItem = Instantiate(taskItemPrefab, taskListParent);

        // Get the TaskItem component from the instantiated prefab
        TaskItem taskItemComponent = taskItem.GetComponent<TaskItem>();

        if (taskItemComponent != null)
        {
            // Initialize the TaskItem with the task data
            taskItemComponent.Initialize(taskManager, task);
        }
        else
        {
            Debug.LogError("TaskItem component is null in the prefab!");
        }

        // Force the layout to rebuild and adjust based on the new content
        LayoutRebuilder.ForceRebuildLayoutImmediate(taskListParent.GetComponent<RectTransform>());
    }

    // Updates task summary (overdue, today, and current tasks)
    // Updates task summary (overdue, today, and current tasks)
    public void UpdateTaskSummary()
    {
        if (taskManager == null)
        {
            taskManager = TaskManager.Instance;
        }

        var overdueTasks = taskManager.GetOverdueTasks();
        var tasksDueToday = taskManager.GetTasksDueToday();
        var currentTasks = taskManager.GetCurrentTasks();

        string summary = $"Overdue Tasks: {overdueTasks.Count}\n" +
                         $"Tasks Due Today: {tasksDueToday.Count}\n" +
                         $"Upcoming Tasks: {currentTasks.Count}";

        taskSummaryText.text = summary;

        Debug.Log($"Task Summary Updated: {summary}");
    }


    // This method is called when the search button is clicked
    public void OnSearchButtonClick()
    {
        string searchText = searchInputField.text;

        // Get the search results based on the query text
        List<Task> searchResults = taskManager.SearchTasks(searchText);

        // Update the task list UI with the search results
        UpdateTaskListUI(searchResults);
    }

    public void OnSortOptionChanged(int optionIndex)
    {
        Debug.Log("Sort Option Changed: " + optionIndex);  // Debug log to confirm the method is called

        List<Task> sortedTasks = new List<Task>();

        // Sort tasks based on the selected option
        switch (optionIndex)
        {
            case 0:  // Priority
                sortedTasks = taskManager.SortTasksByPriority();
                break;
            case 1:  // Due Date
                sortedTasks = taskManager.SortTasksByDueDate();
                break;
            case 2:  // Estimated Time
                sortedTasks = taskManager.SortTasksByEstimatedTime();
                break;
            default:
                sortedTasks = taskManager.GetAllTasks();  // Default to no sorting
                break;
        }

        // Log the sorted list to confirm sorting
        Debug.Log("Sorted Tasks: " + string.Join(", ", sortedTasks.Select(t => t.Title)));

        // Update the UI with the sorted tasks
        UpdateTaskListUI(sortedTasks);
    }

    public void UpdateTaskListUI(List<Task> tasks)
    {
        Debug.Log("Updating Task List UI with " + tasks.Count + " tasks...");

        // Clear the current task UI
        foreach (Transform child in taskListParent)
        {
            Destroy(child.gameObject); // Destroy existing UI elements
        }

        // If no tasks are left, show the "No tasks added" text
        if (tasks.Count == 0)
        {
            noTasksText.SetActive(true); // Show the "No tasks added" message
        }
        else
        {
            noTasksText.SetActive(false); // Hide the "No tasks added" message
        }

        // Add the updated tasks to the UI
        foreach (var task in tasks)
        {
            AddTaskToUI(task); // Re-add each task to the UI
        }

        // Force the layout to rebuild and adjust based on the new content
        LayoutRebuilder.ForceRebuildLayoutImmediate(taskListParent.GetComponent<RectTransform>());
    }


    // Helper function to update the "No tasks added" text visibility
    public void UpdateNoTasksText()
    {
        // If there are no tasks, display the "No tasks added" message
        if (taskManager.GetAllTasks().Count == 0)
        {
            noTasksText.SetActive(true);
        }
        else
        {
            noTasksText.SetActive(false);
        }
    }


}
