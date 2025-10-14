using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using SkillShareMap.Data;
using SkillShareMap.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add MudBlazor for UI components
builder.Services.AddMudServices();

// Add our database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register all application services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IXpService, XpService>();
builder.Services.AddScoped<IReputationService, ReputationService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<UserState>();

// Add HTTP client for external APIs
builder.Services.AddHttpClient();

var app = builder.Build();

// Initialize database with seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Ensure database is created
    context.Database.EnsureCreated();

    // Seed data
    SeedData.Initialize(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
