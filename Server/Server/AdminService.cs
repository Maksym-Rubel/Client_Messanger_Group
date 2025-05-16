public class AdminService
{
    private HashSet<string> approvedUsers = new();
    private Dictionary<string, DateTime> bannedUntil = new();

    public void Approve(string username) => approvedUsers.Add(username);

    public void Ban(string username, TimeSpan duration)
    {
        bannedUntil[username] = DateTime.Now.Add(duration);
    }

    public void Unban(string username) => bannedUntil.Remove(username);

    public bool IsApproved(string username)
    {
        if (bannedUntil.TryGetValue(username, out var until))
        {
            if (DateTime.Now < until)
                return false;
            else
                bannedUntil.Remove(username);
        }
        return approvedUsers.Contains(username);
    }

    public void Remove(string username)
    {
        approvedUsers.Remove(username);
        bannedUntil.Remove(username);
    }
}
