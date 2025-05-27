


namespace Server_Nika
{
    public class UserManager
    {
        private List<User> users = new();

        public void AddUser(User user)
        {
            if (!users.Any(u => u.Username == user.Username))
                users.Add(user);
        }

        public void RemoveUser(string username)
        {
            users.RemoveAll(u => u.Username == username);
        }

        public List<User> GetAllUsers() => users;
    }
}