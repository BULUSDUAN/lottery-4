using System;
namespace Colin.Lottery.Models
{
    public class UserData
    {
        public UserData(string connectionId, string userName) : this(connectionId, userName, true, DateTime.Now)
        {
        }

        public UserData(string connectionId, string userName, bool isActive, DateTime connecteAt)
        {
            ConnectionId = connectionId;
            UserName = userName;
            IsActive = isActive;
            ConnecteAt = connecteAt;
        }

        public string ConnectionId { get; set; }
        public string UserName { get; set; }
        public bool IsActive { get; set; }
        public DateTime ConnecteAt { get; set; }

        public void GetOffLine(string connectionId)
        {
            this.IsActive = false;
        }
    }
}
