using System;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows;

namespace ToDoManager
{
    
    // Handle the drag and drop functionalities here.

    public class DragDropManager
    {
        public static Point dragStartPoint;

        public static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                dragStartPoint = e.GetPosition(null);
        }

        public static void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                // Handle a left mouse button up event

                if (sender is ListBox listBox && listBox.SelectedItem is Task task && task != null)
                {
                    // If a task is selected then start a single click operation on that task
                    // Open the task editor popup
                    MessageBox.Show("Performing single click function.");

                    // Make the selected item null so the task is deselected
                    listBox.SelectedItem = null;
                }
            }
        }

        public static void OnItemMouseMove(object sender, MouseEventArgs e)
        {
            // Check if the left mouse button is pressed
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                // Get current mouse position
                Point mousePosition = e.GetPosition(null);
                Vector diff = dragStartPoint - mousePosition;

                // Check if the mouse has moved far enough for dragging to initiate
                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    // Get the task being dragged and the listbox it belongs to
                    ListBox listBox = (ListBox)sender;
                    Task task = (Task)listBox.SelectedItem;

                    if (task != null)
                    {
                        // Start drag-and-drop operation
                        DragDrop.DoDragDrop(listBox, task, DragDropEffects.Move);
                    }
                }
            }
        }

        public static void OnItemDrop(object sender, DragEventArgs e, ListBox tasks, ListBox progress, ListBox done)
        {
            // Get the task and the listbox it is being dropped into
            ListBox listBox = (ListBox)sender;
            Task task = (Task)e.Data.GetData(typeof(Task));

            var sourceListBox = TaskManager.FindSourceListBox(task, tasks, progress, done);
            Point dropPosition = e.GetPosition(listBox);
            int dropIndex = TaskManager.GetDropIndex(listBox, dropPosition);

            if (sourceListBox != listBox || sourceListBox != null)
                TaskManager.RemoveTaskFromSourceList(sourceListBox, task);

            TaskManager.InsertTaskToList(listBox, task, dropIndex);
        }
    }
}
