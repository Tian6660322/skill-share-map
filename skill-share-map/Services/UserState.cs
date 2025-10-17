using Blazored.LocalStorage;
using Microsoft.JSInterop;
using SkillShareMap.Models;
using SkillShareMap.Models.DTOs;
using System.Diagnostics;
using System.Text.Json;

namespace SkillShareMap.Services;

public class UserState
{
    private const string UserSessionKey = "userSession";
    private readonly IServiceProvider _serviceProvider;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _hasRendered;

    public User? CurrentUser { get; private set; }
    private bool _isInitialized;

    public bool IsAuthenticated => CurrentUser != null;
    public bool IsInitialized => _isInitialized;

    public event Action? OnChange;

    public UserState(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void MarkAsRendered()
    {
        _hasRendered = true;
    }

    public async Task InitializeAsync()
    {
        await _initLock.WaitAsync();
        try
        {
            // Prevent double initialization
            if (_isInitialized)
                return;

            Debug.WriteLine("[UserState] Starting initialization...");

            // Get scoped services
            using var scope = _serviceProvider.CreateScope();
            var localStorage = scope.ServiceProvider.GetRequiredService<ILocalStorageService>();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

            var session = await localStorage.GetItemAsync<UserSession>(UserSessionKey);
            Debug.WriteLine($"[UserState] Session from storage: {(session != null ? $"UserId={session.Id}" : "null")}");

            if (session != null)
            {
                // Restore full user data from database using the session info
                CurrentUser = await authService.GetUserByIdAsync(session.Id);
                Debug.WriteLine($"[UserState] User loaded: {(CurrentUser != null ? CurrentUser.Username : "null")}");
            }

            _isInitialized = true;
            Debug.WriteLine($"[UserState] Initialization complete. IsAuthenticated: {IsAuthenticated}");
            NotifyStateChanged();
        }
        catch (Exception ex)
        {
            // If there's an error reading from storage, just continue with null user
            Debug.WriteLine($"[UserState] Error during initialization: {ex.Message}");
            CurrentUser = null;
            _isInitialized = true;
            NotifyStateChanged();
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task SetUserAsync(User? user)
    {
        CurrentUser = user;

        // Only try to save to localStorage if the component has been rendered
        if (_hasRendered)
        {
            try
            {
                // Get scoped services
                using var scope = _serviceProvider.CreateScope();
                var localStorage = scope.ServiceProvider.GetRequiredService<ILocalStorageService>();

                if (user != null)
                {
                    var session = UserSession.FromUser(user);
                    await localStorage.SetItemAsync(UserSessionKey, session);
                    Debug.WriteLine($"[UserState] User session saved: UserId={session.Id}, Username={session.Username}");
                }
                else
                {
                    await localStorage.RemoveItemAsync(UserSessionKey);
                    Debug.WriteLine("[UserState] User session cleared");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UserState] Error saving to localStorage: {ex.Message}");
                // Continue anyway - state is still set in memory
            }
        }
        else
        {
            Debug.WriteLine($"[UserState] Component not rendered yet, skipping localStorage save. User: {(user != null ? user.Username : "null")}");
        }

        NotifyStateChanged();
    }

    public async Task LogoutAsync()
    {
        await SetUserAsync(null);
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
