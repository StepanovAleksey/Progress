namespace SignalRChatServer.Abstracts
{
    public class Repository : IRepository
    {
        private List<ChatUser> _userCollection = new List<ChatUser>();
       
        public void AddUser(ChatUser user)
        {
            _userCollection.Add(user);
        }
        public ChatUser? GetUser(string connectionId)
        {
            return _userCollection.FirstOrDefault(p => p.ConnectionId == connectionId);
        }

        public IEnumerable<ChatUser>? GetAllUsers()
        {
            return _userCollection;
        }

        public void ClearRepo()
        {
            _userCollection.Clear();
        }
      
        public void DeleteUser(string connectionId)
        {
            var temp = _userCollection.FirstOrDefault(p => p.ConnectionId == connectionId);
            if (temp != null) _userCollection.Remove(temp);
        }

        public ChatUser? GetUserWithStatus(UserStatus status)
        {
            return _userCollection.FirstOrDefault(p => p.Status == status);
        }
        public int GetUserWithGroup(string groupName)
        {
            return _userCollection.Select(p=>p).Where(p => p.GroupName == groupName).Count();
        }
        public ChatUser? GetUserWithName(string name)
        {
            return _userCollection.FirstOrDefault(p => p.Name == name);
        }
      
    }
}
