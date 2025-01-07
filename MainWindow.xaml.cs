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

namespace ToDoManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Point _dragStartPoint;
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
            DataContext = this;
            LoadData();

            Tasks.PreviewMouseLeftButtonDown += OnItemMouseLeftButtonDown;
            Progress.PreviewMouseLeftButtonDown += OnItemMouseLeftButtonDown;
            Done.PreviewMouseLeftButtonDown += OnItemMouseLeftButtonDown;

            Tasks.MouseMove += OnItemMouseMove;
            Progress.MouseMove += OnItemMouseMove;
            Done.MouseMove += OnItemMouseMove;

            Tasks.Drop += OnItemDrop;
            Progress.Drop += OnItemDrop;
            Done.Drop += OnItemDrop;
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
                AddTaskToList(Tasks, new Task 
                                            { 
                                                Id = Guid.NewGuid().ToString(), 
                                                Description = taskDescription,
                                                Priority = taskPriority,
                                                Difficulty = taskDifficulty,
                                                Points = taskPriority * taskPriority + taskDifficulty * taskDifficulty
                                            }
                );
        }

        private void ClosePopupButton_Click(object sender, RoutedEventArgs e) 
        { 
            IsNewTaskPopupOpen = false; 
        }

        private void AddTaskToList(ListBox listBox, Task task) 
        { 
            var tasks = new List<Task>((List<Task>)listBox.ItemsSource) { task }; 
            listBox.ItemsSource = tasks; 
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

        private void OnItemMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
        }

        private void OnItemMouseMove(object sender, MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(null);
            Vector diff = _dragStartPoint - mousePosition;
            
            if (e.LeftButton == MouseButtonState.Pressed && 
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || 
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                if (sender is ListBox listBox && listBox.SelectedItem is Task task)
                    DragDrop.DoDragDrop(listBox, task, DragDropEffects.Move);
            }
                
        }

        private void OnItemDrop(object sender, DragEventArgs e)
        {
            if (sender is ListBox listBox && e.Data.GetData(typeof(Task)) is Task task)
            {
                var sourceListBox = FindSourceListBox(task);
                Point dropPosition = e.GetPosition(listBox);
                int dropIndex = GetDropIndex(listBox, dropPosition);

                if (sourceListBox != listBox || sourceListBox != null)
                    RemoveTaskFromSourceList(sourceListBox, task);

                InsertTaskToList(listBox, task, dropIndex);
            }
        }

        private int GetDropIndex(ListBox listBox, Point dropPosition)
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

        private ListBox FindSourceListBox(Task task)
        {
            if (((List<Task>)Tasks.ItemsSource).Any(t => t.Id == task.Id))
                return Tasks;
            if (((List<Task>)Progress.ItemsSource).Any(t => t.Id == task.Id))
                return Progress;
            if (((List<Task>)Done.ItemsSource).Any(t => t.Id == task.Id))
                return Done;
            return null;
        }

        private void RemoveTaskFromSourceList(ListBox listBox,  Task task)
        {
            var tasks = new List<Task>((List<Task>)listBox.ItemsSource); 
            tasks.RemoveAll(t => t.Id == task.Id); 
            listBox.ItemsSource = tasks;
        }

        private void InsertTaskToList(ListBox listBox, Task task, int index)
        {
            var tasks = new List<Task>((List<Task>)listBox.ItemsSource);

            if (index >= 0 && index <= tasks.Count)
                tasks.Insert(index, task);
            else
                tasks.Add(task);
                
            listBox.ItemsSource = tasks;
        }
    }
}