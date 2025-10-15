using Blazored.LocalStorage;
using SkillShareMap.Models;
using SkillShareMap.Models.DTOs;

namespace SkillShareMap.Services;

public class UserState
{
    private const string UserSessionKey = "userSession";
    private readonly ILocalStorageService _localStorage;
    private readonly IAuthService _authService;

    public User? CurrentUser { get; private set; }

    public bool IsAuthenticated => CurrentUser != null;

    public event Action? OnChange;

    public UserState(ILocalStorageService localStorage, IAuthService authService)
    {
        _localStorage = localStorage;
        _authService = authService;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var session = await _localStorage.GetItemAsync<UserSession>(UserSessionKey);
            if (session != null)
            {
                // Restore full user data from database using the session info
                CurrentUser = await _authService.GetUserByIdAsync(session.Id);
            }
            NotifyStateChanged();
        }
        catch
        {
            // If there's an error reading from storage, just continue with null user
            CurrentUser = null;
        }
    }

    public async Task SetUserAsync(User? user)
    {
        CurrentUser = user;

        if (user != null)
        {
            var session = UserSession.FromUser(user);
            await _localStorage.SetItemAsync(UserSessionKey, session);
        }
        else
        {
            await _localStorage.RemoveItemAsync(UserSessionKey);
        }

        NotifyStateChanged();
    }

    public async Task LogoutAsync()
    {
        await SetUserAsync(null);
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
