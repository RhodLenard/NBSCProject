using UnityEngine;
using UnityEngine.UI;

public class TaskItem : MonoBehaviour
{
    public Text taskText;
    public Button removeButton;
    public Button editButton;

    private TaskManager taskManager;
    private Task task;

    public void Initialize(TaskManager manager, Task task)
    {
        this.task = task;
        taskManager = manager;

        // Set the task details in the UI components
        taskText.text = $"Title: {task.Title}\n" +
                        $"Description: {task.Description}\n" +
                        $"Priority: {task.Priority}\n" +
                        $"Due Date: {task.DueDate:MM/dd/yyyy}\n" +
                        $"Time: {task.EstimatedTime}h";

        // Set up button listeners
        removeButton.onClick.AddListener(RemoveTask);
        editButton.onClick.AddListener(EditTask);
    }

    private void RemoveTask()
    {
        taskManager.RemoveTask(task); // Remove the task from the manager
        UIManager.Instance.UpdateTaskSummary(); // Update the summary immediately
        UIManager.Instance.UpdateTaskListUI(taskManager.GetAllTasks()); // Refresh the UI
        Destroy(gameObject); // Remove the task item from the UI
    }


    private void EditTask()
    {
        // Show the task edit UI and populate the fields
        TaskEditPanel.Instance.Show(task);
    }
}
