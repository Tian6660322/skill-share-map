using SkillShareMap.Models;

namespace SkillShareMap.Services;

public class UserState
{
    public User? CurrentUser { get; private set; }

    public bool IsAuthenticated => CurrentUser != null;

    public event Action? OnChange;

    public void SetUser(User? user)
    {
        CurrentUser = user;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
