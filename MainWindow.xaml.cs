using Syncfusion.Windows.Controls.PivotGrid;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ToDoManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// 
    /// Keep the code related to the window and its basic initialization here.
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        
        private bool _isNewTaskPopupOpen;
        
        public bool IsNewTaskPopupOpen
        { 
            get { return _isNewTaskPopupOpen; } 
            set 
            { 
                _isNewTaskPopupOpen = value; 
                OnPropertyChanged("IsNewTaskPopupOpen"); 
            } 
        }
        
        public event PropertyChangedEventHandler PropertyChanged; 
        
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            InitializeComponent();

            // Set data context for data binding
            DataContext = this;

            // Load the initial data
            LoadData();
        }

        // Methods below are connecting the MainWindow to the DragDropManager class
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragDropManager.OnMouseLeftButtonDown(sender, e);
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DragDropManager.OnMouseLeftButtonUp(sender, e);
        }

        private void OnItemMouseMove( object sender, MouseEventArgs e )
        {
            DragDropManager.OnItemMouseMove(sender, e);
        }

        private void OnItemDrop(object sender, DragEventArgs e)
        {
            DragDropManager.OnItemDrop(sender, e, Tasks, Progress, Done);
        }
        //


        // When a listboxitem is selected
        public void MyListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if an item is selected
            if (Tasks != null && Tasks.SelectedItem != null)
            {
                // Do something with the selected item
                Task selectedItem = (Task)Tasks.SelectedItem;
            }
        }

        // When the user presses the "Add A New Task" button -> show the popup menu
        private void OpenNewTaskPopupButton_Click(object sender, RoutedEventArgs e) { IsNewTaskPopupOpen = true; }

        // Confirm button in the popup
        private void AddTaskButton_Click(object sender, RoutedEventArgs e) 
        { 
            string taskDescription = TaskDescriptionTextBox.Text;
            int taskPriority = (int)PrioritySlider.Value; 
            int taskDifficulty = (int)DifficultySlider.Value;

            // If the user typed some text into the description on the popup
            // then add the task to the board
            if (!string.IsNullOrEmpty(taskDescription))
                TaskManager.AddTaskToList(Tasks, new Task 
                    { 
                        Id = Guid.NewGuid().ToString(), 
                        Description = taskDescription,
                        Priority = taskPriority,
                        Difficulty = taskDifficulty,
                        Points = taskPriority * taskPriority + taskDifficulty * taskDifficulty
                    });
        }

        // 'X' button in the popup
        private void ClosePopupButton_Click(object sender, RoutedEventArgs e) 
        { 
            IsNewTaskPopupOpen = false; 
        }

        private void LoadData()
        {
            Tasks.ItemsSource = new List<Task> { };
            Tasks.ItemTemplate = (DataTemplate)Resources["ItemTemplate"];

            Progress.ItemsSource = new List<Task> { };
            Progress.ItemTemplate = (DataTemplate)Resources["ItemTemplate"];

            Done.ItemsSource = new List<Task> { };
            Done.ItemTemplate = (DataTemplate)Resources["ItemTemplate"];
        }
    }
}