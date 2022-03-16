using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using TodoApplication.Commands;
using TodoApplication.Model;

namespace TodoApplication.ViewModels
{
    public class TodoItemModel : ViewModelBase
    {
        ObservableCollection<TodoItem> _todoItemList = new();
        TodoItem _newTodoItem = new();
        string _errorMsg = string.Empty;

        public IDelegateCommand CreateTodoItemCommand { protected get; set; }
        public IDelegateCommand UpdateTodoItemCommand { protected get; set; }
        public IDelegateCommand DeleteTodoItemCommand { protected get; set; }

        public TodoItemModel()
        {
            CreateTodoItemCommand = new DelegateCommand(ExecuteCreateTodoItem);
            UpdateTodoItemCommand = new DelegateCommand(ExecuteUpdateTodoItem);
            DeleteTodoItemCommand = new DelegateCommand(ExecuteDeleteTodoItem);

            GetTodoItems();
        }

        public ObservableCollection<TodoItem> TodoItemList
        {
            get { return _todoItemList; }
            set { SetProperty(ref _todoItemList, value); }
        }

        public TodoItem NewTodoItem
        {
            get { return _newTodoItem; }
            set { SetProperty(ref _newTodoItem, value); }
        }

        public string ErrorMsg
        {
            get { return _errorMsg; }
            set { SetProperty(ref _errorMsg, value); }
        }

        private async void GetTodoItems()
        {
            var _connectionUrl = "https://localhost:7085/api/todoitems";
            HttpClient _client = new();
            try
            {
                var _response = await _client.GetFromJsonAsync<ObservableCollection<TodoItem>>(_connectionUrl);

                if (_response != null)
                {
                    TodoItemList.Clear();
                    TodoItemList = _response;
                }
            } catch (HttpRequestException)
            {
                ErrorMsg = "[DB Error]: Could not obtain To Do items.";
            }
        }

        async void ExecuteCreateTodoItem(object param)
        {
            if(string.IsNullOrEmpty(NewTodoItem.Title) || string.IsNullOrEmpty(NewTodoItem.Description)){
                ErrorMsg = "[Form Error]: All fields must be filled in.";
            } else
            {
                ErrorMsg = "";
                var _connectionUrl = "https://localhost:7085/api/todoitems";
                HttpClient _client = new();

                try
                {
                    var _response = await _client.PostAsJsonAsync(_connectionUrl, _newTodoItem);

                    if (_response.IsSuccessStatusCode)
                    {
                        NewTodoItem.Title = "";
                        NewTodoItem.Description = "";
                        OnPropertyChanged(NewTodoItem.Title);
                        OnPropertyChanged(NewTodoItem.Description);

                        GetTodoItems();
                    }
                }
                catch (HttpRequestException)
                {
                    ErrorMsg = "[DB Error]: Could not save new To Do item.";
                }
            }
        }

        async void ExecuteUpdateTodoItem(object param)
        {
            var _connectionUrl = "https://localhost:7085/api/todoitems/" + (int) param;
            TodoItem _todoItem = TodoItemList.First((item) => item.Id == (int) param);
            _todoItem.IsComplete = true;
            HttpClient _client = new();

            try
            {
                var _response = await _client.PutAsJsonAsync(_connectionUrl, _todoItem);

                if (_response.IsSuccessStatusCode)
                {
                    GetTodoItems();
                }
            }
            catch (HttpRequestException)
            {
                ErrorMsg = "[DB Error]: Could not update To Do item.";
            }
        }

        async void ExecuteDeleteTodoItem(object param)
        {
            var _connectionUrl = "https://localhost:7085/api/todoitems/" + (int) param;
            HttpClient _client = new();

            try
            {
                var _response = await _client.DeleteAsync(_connectionUrl);

                if (_response.IsSuccessStatusCode)
                {
                    GetTodoItems();
                }
            } catch (HttpRequestException)
            {
                ErrorMsg = "[DB Error]: Could not delete To Do item.";
            }
        }
    }
}
