namespace SignalRChatServer.Abstracts
{   
    public enum UserStatus {Off, On, Ready, Play}
    public class ChatUser
    {
        public string Name { get; set; }
        public string ConnectionId { get; set; }
        public string? GroupName { get; set; } = null;
        public UserStatus Status { get; set; } = UserStatus.Off;
    }
}
