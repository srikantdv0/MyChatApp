using System;
namespace ChatShared.Models
{
    public class User
    {
        public string UserName { get; set; } = String.Empty;
        public string ConnectionId { get; set; } = String.Empty;
        public Dictionary<string, List<string>> messages { get; set; } = new Dictionary<string, List<string>>();
    }
}

