namespace SignalRChat.Models
{
    public static class RandomBoolGenerator
    {
        public static bool Generate()
        {
            if (new Random().Next(0, 2) == 0) return false;
            else return true;
        }
    }
}
