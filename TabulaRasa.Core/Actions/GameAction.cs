namespace TabulaRasa.Core.Actions
{
     public class GameAction
    {
        public string[] Args { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; }
        public int OtherEntity1 { get; }
        public int OtherEntity2 { get; }
        public string Type { get; }
        public string Value { get; set; }

        public GameAction() { }
        public GameAction(string type, int senderId, params string[] args) : this(type, senderId, 0, 0, 0, args) { }
        public GameAction(string type, int senderId, int receiverId, params string[] args) : this(type, senderId, receiverId, 0, 0, args) { }
        public GameAction(string type, int senderId, int receiverId, int other1, params string[] args) : this(type, senderId, receiverId, other1, 0, args) { }
        public GameAction(string type, int senderId, int receiverId, int other1, int other2, params string[] args)
        {
            Args = args;
            SenderId = senderId;
            ReceiverId = receiverId;
            Type = type.ToLower();
            OtherEntity1 = other1;
            OtherEntity2 = other2;
        }
    }
}