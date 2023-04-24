using Microsoft.AspNetCore.SignalR;
using SignalRChatServer.Abstracts;

namespace SignalRChat.Hubs
{
    // [Authorize]
    public class ChatHub : Hub
    {
        private IRepository _repo;
        
        public ChatHub(IRepository repo):base()
        {
            _repo = repo;
        }
  
        /// <summary>
        /// Генерируем имя для нового пользователя, добавляем в словарь подключениё и отправляем вызвавшему подключение.
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            var userName = UserNameGenerate();
            _repo.AddUser(new ChatUser() { Name = userName, ConnectionId = Context.ConnectionId });
            await Clients.Caller.SendAsync("Notify", userName);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await this.RemoveFromOldGroup(Context.ConnectionId);
            _repo.DeleteUser(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Генерируем уникально имя пользователя
        /// </summary>
        /// <returns></returns>
        private string UserNameGenerate()
        {
            var userName = "User_" + (new Random().Next(99999).ToString());
            if (_repo.GetAllUsers()?.FirstOrDefault(p=>p.Name == userName) != null)
            {
               return UserNameGenerate();
            }
            else
                return userName;
        }

        /// <summary>
        /// Генерируем уникально имя группы
        /// </summary>
        /// <returns></returns>
        private string GroupNameGenerate()
        {
            var groupName = "Group_" + (new Random().Next(99999).ToString());
            if (_repo.GetUserWithGroup(groupName) != 0)
            {
                return GroupNameGenerate();
            }
            else
                return groupName;
        }
        public async Task SendMessage(string user, string message)
        {
            var groupName = _repo.GetUser(Context.ConnectionId)?.GroupName;
            await Clients.Group(groupName).SendAsync("ReceiveMessage", user, message);
        }

        /// <summary>
        /// Добавление пользователя в групповой чат
        /// </summary>
        public async Task AddToGroup()
        {
            var firstUser = _repo.GetUserWithStatus(UserStatus.Ready);
            var user = _repo.GetUser(Context.ConnectionId);
            if (firstUser == null)
            {
                var groupName = GroupNameGenerate();
                user.Status = UserStatus.Ready;
                user.GroupName = groupName;
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                await Clients.Caller.SendAsync("Waiting",$"Ожидаем первого готового игрока!");
            }
            else 
            {
                var groupName = firstUser.GroupName;
                user.Status = UserStatus.Play;
                firstUser.Status = UserStatus.Play;
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                user.GroupName = groupName;
                await Clients.Group(groupName).SendAsync("SendToGroup", $"{user.Name}", "true", $" присоединился к группе {groupName}.");
            }         
        }

        public async Task RemoveFromOldGroup(string connectionId)
        {
            var user = _repo.GetUser(connectionId);
            if (user?.GroupName!=null)
            {
                user.Status = UserStatus.On;
                await Clients.Group(user.GroupName).SendAsync("SendToGroup", $"{user.Name}", "false", $" покинул группу {user?.GroupName}.");
                await Groups.RemoveFromGroupAsync(user.ConnectionId, user.GroupName);
                Task.WaitAll();             
                user.GroupName = null;
            }
            
        }
        /// <summary>
        /// Перегруженная функция для js-клиента
        /// </summary>
        /// <returns></returns>
        public async Task RemoveFromGroup() => await RemoveFromOldGroup(Context.ConnectionId);




    }
}
