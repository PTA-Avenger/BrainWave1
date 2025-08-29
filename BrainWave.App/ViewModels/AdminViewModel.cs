using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace BrainWave.App.ViewModels
{
    public class AdminViewModel : INotifyPropertyChanged
    {
        private readonly HttpClient _httpClient;
        private string _username = "";
        private string _password = "";
        private bool _isLoggedIn = false;
        private bool _isBusy = false;
        private string _errorMessage = "";
        private string _authToken = "";

        public AdminViewModel()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("https://your-api-url.com/") };
            
            Users = new ObservableCollection<AdminUserDto>();
            Tasks = new ObservableCollection<AdminTaskDto>();
            FilteredUsers = new ObservableCollection<AdminUserDto>();
            FilteredTasks = new ObservableCollection<AdminTaskDto>();

            LoginCommand = new Command(async () => await LoginAsync());
            LogoutCommand = new Command(Logout);
            RefreshUsersCommand = new Command(async () => await LoadUsersAsync());
            RefreshTasksCommand = new Command(async () => await LoadTasksAsync());
            EditUserCommand = new Command<AdminUserDto>(async (user) => await EditUserAsync(user));
            DeleteUserCommand = new Command<AdminUserDto>(async (user) => await DeleteUserAsync(user));
            EditTaskCommand = new Command<AdminTaskDto>(async (task) => await EditTaskAsync(task));
            DeleteTaskCommand = new Command<AdminTaskDto>(async (task) => await DeleteTaskAsync(task));
        }

        public ObservableCollection<AdminUserDto> Users { get; }
        public ObservableCollection<AdminTaskDto> Tasks { get; }
        public ObservableCollection<AdminUserDto> FilteredUsers { get; }
        public ObservableCollection<AdminTaskDto> FilteredTasks { get; }

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set { _isLoggedIn = value; OnPropertyChanged(); }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasError)); }
        }

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public ICommand LoginCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand RefreshUsersCommand { get; }
        public ICommand RefreshTasksCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand EditTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }

        private async Task LoginAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                ErrorMessage = "";

                var loginData = new { Username, Password };
                var json = JsonSerializer.Serialize(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/admin/login", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonSerializer.Deserialize<AdminLoginResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    _authToken = loginResponse?.Token ?? "";
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);
                    
                    IsLoggedIn = true;
                    await LoadUsersAsync();
                    await LoadTasksAsync();
                }
                else
                {
                    ErrorMessage = "Invalid credentials";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Login failed: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void Logout()
        {
            IsLoggedIn = false;
            _authToken = "";
            _httpClient.DefaultRequestHeaders.Authorization = null;
            Username = "";
            Password = "";
            ErrorMessage = "";
            
            Users.Clear();
            Tasks.Clear();
            FilteredUsers.Clear();
            FilteredTasks.Clear();
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/admin/users");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var users = JsonSerializer.Deserialize<List<AdminUserDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    Users.Clear();
                    FilteredUsers.Clear();
                    
                    foreach (var user in users ?? new List<AdminUserDto>())
                    {
                        Users.Add(user);
                        FilteredUsers.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load users: {ex.Message}";
            }
        }

        private async Task LoadTasksAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/admin/tasks");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var tasks = JsonSerializer.Deserialize<List<AdminTaskDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    Tasks.Clear();
                    FilteredTasks.Clear();
                    
                    foreach (var task in tasks ?? new List<AdminTaskDto>())
                    {
                        Tasks.Add(task);
                        FilteredTasks.Add(task);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load tasks: {ex.Message}";
            }
        }

        public void ApplyUserRoleFilter(string role)
        {
            FilteredUsers.Clear();
            var filtered = role == "All" ? Users : Users.Where(u => u.Role == role);
            foreach (var user in filtered)
            {
                FilteredUsers.Add(user);
            }
        }

        public void ApplyUserStatusFilter(string status)
        {
            FilteredUsers.Clear();
            var filtered = status == "All" ? Users : 
                          status == "Active" ? Users.Where(u => u.IsActive) : 
                          Users.Where(u => !u.IsActive);
            foreach (var user in filtered)
            {
                FilteredUsers.Add(user);
            }
        }

        public void ApplyTaskStatusFilter(string status)
        {
            FilteredTasks.Clear();
            var filtered = status == "All" ? Tasks : Tasks.Where(t => t.Task_Status.ToString() == status);
            foreach (var task in filtered)
            {
                FilteredTasks.Add(task);
            }
        }

        public void ApplyTaskPriorityFilter(string priority)
        {
            FilteredTasks.Clear();
            var filtered = priority == "All" ? Tasks : Tasks.Where(t => t.Priority_Level.ToString() == priority);
            foreach (var task in filtered)
            {
                FilteredTasks.Add(task);
            }
        }

        private async Task EditUserAsync(AdminUserDto user)
        {
            // Implement edit user functionality
            await Application.Current.MainPage.DisplayAlert("Edit User", $"Edit functionality for {user.F_Name} {user.L_Name}", "OK");
        }

        private async Task DeleteUserAsync(AdminUserDto user)
        {
            var confirm = await Application.Current.MainPage.DisplayAlert("Delete User", $"Are you sure you want to delete {user.F_Name} {user.L_Name}?", "Yes", "No");
            if (confirm)
            {
                try
                {
                    var response = await _httpClient.DeleteAsync($"api/admin/users/{user.UserID}");
                    if (response.IsSuccessStatusCode)
                    {
                        Users.Remove(user);
                        FilteredUsers.Remove(user);
                        await Application.Current.MainPage.DisplayAlert("Success", "User deleted successfully", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Failed to delete user: {ex.Message}", "OK");
                }
            }
        }

        private async Task EditTaskAsync(AdminTaskDto task)
        {
            // Implement edit task functionality
            await Application.Current.MainPage.DisplayAlert("Edit Task", $"Edit functionality for {task.Title}", "OK");
        }

        private async Task DeleteTaskAsync(AdminTaskDto task)
        {
            var confirm = await Application.Current.MainPage.DisplayAlert("Delete Task", $"Are you sure you want to delete '{task.Title}'?", "Yes", "No");
            if (confirm)
            {
                try
                {
                    var response = await _httpClient.DeleteAsync($"api/admin/tasks/{task.TaskID}");
                    if (response.IsSuccessStatusCode)
                    {
                        Tasks.Remove(task);
                        FilteredTasks.Remove(task);
                        await Application.Current.MainPage.DisplayAlert("Success", "Task deleted successfully", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Failed to delete task: {ex.Message}", "OK");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class AdminLoginResponse
    {
        public string Token { get; set; } = "";
        public string Role { get; set; } = "";
    }

    public class AdminUserDto
    {
        public int UserID { get; set; }
        public string F_Name { get; set; } = "";
        public string L_Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Bio { get; set; } = "";
        public DateTime Created_Date { get; set; }
        public DateTime? Updated_Date { get; set; }
        public bool IsActive { get; set; }
        public int TaskCount { get; set; }
        
        public string FullName => $"{F_Name} {L_Name}";
    }

    public class AdminTaskDto
    {
        public int TaskID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime? Due_Date { get; set; }
        public TaskStatus Task_Status { get; set; }
        public TaskPriority Priority_Level { get; set; }
        public DateTime Created_Date { get; set; }
        public DateTime? Updated_Date { get; set; }
        public bool IsShared { get; set; }
        
        public Color PriorityColor => Priority_Level switch
        {
            TaskPriority.High => Colors.Red,
            TaskPriority.Medium => Colors.Orange,
            TaskPriority.Low => Colors.Green,
            _ => Colors.Gray
        };
        
        public Color StatusColor => Task_Status switch
        {
            TaskStatus.Completed => Colors.Green,
            TaskStatus.InProgress => Colors.Blue,
            TaskStatus.Pending => Colors.Orange,
            TaskStatus.Cancelled => Colors.Red,
            _ => Colors.Gray
        };
    }

    public enum TaskStatus
    {
        Pending,
        InProgress,
        Completed,
        Cancelled
    }

    public enum TaskPriority
    {
        Low = 1,
        Medium = 2,
        High = 3
    }
}