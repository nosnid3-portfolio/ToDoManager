using System;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows;

namespace ToDoManager
{

    // Handle the drag and drop functionalities here.

    /* When user selects a task by pressing the left mouse button on it and releasing it
     * if the required amount of time of holding the left mouse button was not met
     * the selection is a quick press, if the time required if met then the selection is press-and-hold
     * 
     * quick press activates an event where there is a popup to edit the settings of that task
     * 
     * press-and-hold activates an event where the cursor icon is changed to a move icon, 
     * and the task can now be drag-and-dropped
     * 
     * When the user releases the left mouse button after a press-and-hold event
     * nothing happens
     */

    public class DragDropManager
    {
        public static DispatcherTimer pressAndHoldTimer;
        public static Point dragStartPoint;
        public static bool isPressAndHold;
        public const int holdDuration = 500; // duration in milliseconds

        public DragDropManager()
        {
            // Constructor method
            pressAndHoldTimer = new DispatcherTimer();
            pressAndHoldTimer.Interval = TimeSpan.FromMilliseconds(holdDuration);
            pressAndHoldTimer.Tick += OnpressAndHoldTimerTick;
        }

        public static void OnpressAndHoldTimerTick(object sender, EventArgs e)
        {
            isPressAndHold = true;
            pressAndHoldTimer.Stop();

            // Handle press-and-hold event
            // Change the pointer to the move icon, 
        }

        public static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragStartPoint = e.GetPosition(null);
            isPressAndHold = false;
            pressAndHoldTimer.Start();
        }

        public static void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!isPressAndHold)
            {
                // Handle quick press event
                pressAndHoldTimer.Stop();
                // Open the task editor popup

                
            }
        }

        public static void OnItemMouseMove(object sender, MouseEventArgs e)
        {
            // Ensure left mouse button isPressAndHold
            if (isPressAndHold)
            {
                // Get current mouse position
                Point mousePosition = e.GetPosition(null);
                Vector diff = dragStartPoint - mousePosition;

                // Check if the mouse has moved far enough for a drag-and-drop
                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)


                    // Check if sender is listbox and selecteditem is task
                    if (sender is ListBox listBox && listBox.SelectedItem is Task task)
                    {
                        // start drag-and-drop operation
                        DragDrop.DoDragDrop(listBox, task, DragDropEffects.Move);
                    }
            }
        }

        public static void OnItemDrop(object sender, DragEventArgs e, ListBox tasks, ListBox progress, ListBox done)
        {
            if (sender is ListBox listBox && e.Data.GetData(typeof(Task)) is Task task)
            {
                var sourceListBox = TaskManager.FindSourceListBox(task, tasks, progress, done);
                Point dropPosition = e.GetPosition(listBox);
                int dropIndex = TaskManager.GetDropIndex(listBox, dropPosition);

                if (sourceListBox != listBox || sourceListBox != null)
                    TaskManager.RemoveTaskFromSourceList(sourceListBox, task);

                TaskManager.InsertTaskToList(listBox, task, dropIndex);
            }
        }
    }
}
