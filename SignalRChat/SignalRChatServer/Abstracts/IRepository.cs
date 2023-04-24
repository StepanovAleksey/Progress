namespace SignalRChatServer.Abstracts
{
    public interface IRepository
    {
       public void AddUser(ChatUser user);
       public void DeleteUser(string connectionId);
       public ChatUser? GetUser(string connectionId);
       public IEnumerable<ChatUser>? GetAllUsers();
       public ChatUser? GetUserWithStatus(UserStatus status);
       public ChatUser? GetUserWithName(string name);
       public int GetUserWithGroup(string groupName);
       public void ClearRepo();
        
    }
}