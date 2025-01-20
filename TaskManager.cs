using Syncfusion.ProjIO;
using System;
using System.Diagnostics;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;

namespace ToDoManager
{

    // Move the task management methods here.

    public class TaskManager
	{
        // Add the listboxitem to the listbox
        public static void AddTaskToList(ListBox listBox, Task task)
        {
            var tasks = new List<Task>((List<Task>)listBox.ItemsSource) { task };
            listBox.ItemsSource = tasks;
        }

        public static void RemoveTaskFromSourceList(ListBox listBox, Task task)
        {
            var tasks = new List<Task>((List<Task>)listBox.ItemsSource);
            tasks.RemoveAll(t => t.Id == task.Id);
            listBox.ItemsSource = tasks;
        }

        public static ListBox? FindSourceListBox(Task task, ListBox Tasks, ListBox Progress, ListBox Done)
        {
            if (((List<Task>)Tasks.ItemsSource).Any(t => t.Id == task.Id))
                return Tasks;
            if (((List<Task>)Progress.ItemsSource).Any(t => t.Id == task.Id))
                return Progress;
            if (((List<Task>)Done.ItemsSource).Any(t => t.Id == task.Id))
                return Done;
            return null;
        }

        public static void InsertTaskToList(ListBox listBox, Task task, int index)
        {
            var tasks = new List<Task>((List<Task>)listBox.ItemsSource);

            if (index >= 0 && index <= tasks.Count)
                tasks.Insert(index, task);
            else
                tasks.Add(task);

            listBox.ItemsSource = tasks;
        }

        public static int GetDropIndex(ListBox listBox, Point dropPosition)
        {
            for (int i = 0; i < listBox.Items.Count; i++)
            {
                ListBoxItem item = (ListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(i);

                if (item != null)
                {
                    Point itemPosition = item.TransformToAncestor(listBox).Transform(new Point(0, 0));

                    if (dropPosition.Y < itemPosition.Y + item.ActualHeight / 2)
                        return i;
                }
            }
            return listBox.Items.Count;
        }


    }
}

