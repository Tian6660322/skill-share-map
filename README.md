### Skill Share Map

Skill Share Map connects students, companies, and schools. It shows short-term tasks and jobs on a map. The app is built with Blazor Server, EF Core (SQLite), and MudBlazor.

### Features

* Browse the map to find tasks in blue and jobs in orange. You can filter them by category, status, and distance.
* Users can post a task. A post includes a title, description, category, budget, deadline, and location. Location can be found with geolocation or an address.
* A helper accepts a task. The poster then pays a 10% deposit. The deposit is credited to the helper right away.
* Completing a task has two steps. The helper marks the task as done. The poster confirms it is complete. Then the system sends the remaining payment.
* Users can chat and send price offers. An accepted offer updates the task's price.
* Users get ratings, XP, badges, and reputation after a task. Reviews appear on user profiles.
* The wallet page shows a user's balance. It also shows a full transaction history. The history includes deposits, payments, and refunds.
* The classroom page has learning resources. It includes a placeholder for a future AI coach.

### Prerequisites

* .NET SDK 9.0
* A modern browser like Chrome, Edge, Firefox, or Safari
* Internet access for map tiles and geocoding
* The project uses a SQLite database at `Data/app.db`. The database comes with sample data. Migrations run at startup. Delete the `app.db` file to reset the data.

### How To Run

1.  Open a terminal in the project folder.
2.  Run `dotnet restore`.
3.  Then run `dotnet run`.
4.  The application runs at `https://localhost:7289/login`. The console may show a different URL based on your `launchSettings.json` file.
5.  In Visual Studio or VS Code, press F5 to run.

### Demo Accounts

* **Student:** `alice_student` / `password123`
* **Company:** `techcorp` / `password123`
* Find more test accounts like `bob_student` in `Data/SeedData.cs`.

### Usage

* **Login/Registration:** Go to `/login` to sign in or register. You can register as a Student, Company, or School.
* **Map:** On the map, you can switch between Tasks and Jobs. Set your filters and click 'Apply Filters'. Click a map marker to see a summary card. Then click to view full details.
* **Post Task:** Go to `/post-task` to create a new task. Fill in the form. You can use your current location or search for an address. Then submit the form.
* **Accept + Deposit:** On a task's detail page, click 'Accept'. The poster pays a 10% deposit. The task status changes to 'Assigned'.
* **Complete + Payout:** The helper marks the task as done. The poster confirms completion. The system then credits the final payment.
* **Chat & Offers:** Users can send price offers in the chat. An accepted offer becomes the new task price.
* **Wallet:** The `/wallet` page shows your balance and transaction history. History items are marked as `TaskDeposit`, `TaskPayment`, or `TaskRefund`.
* **Profile:** Your profile at `/profile` shows your badges and skills. It also lists your tasks and recent reviews.

### Important Behaviors

* The deposit is not held in escrow. The system credits the helper when they accept the task.
* The system refunds the deposit to the poster if a task is cancelled.
* The 'Add Funds' and 'Withdraw' buttons are placeholders. The app does not connect to a real payment provider.
* Geocoding uses the Nominatim service. This sometimes causes small delays. The service has a usage limit of one request per second.

### Rubric Mapping

* **Code Quality:** The code uses clear names. Comments explain services and pages.
* **Interfaces:** Find service interfaces in `Services/Interfaces/*.cs`. Examples include `IAuthService` and `ITaskService`.
* **Polymorphism:** The project uses polymorphism in component lifecycle overrides. See `Shared/MainLayout.razor` for an example.
* **NUnit Test:** The test at `SkillShareMap.Test/AuthServiceTest.cs` covers the registration process.
* **LINQ and Lambdas:** The code uses LINQ and lambda expressions for filtering and ordering tasks. See `Pages/Index.razor` and `Services/TaskService.cs`.
* **Generics:** The project uses generic `List<T>` collections. Find them in models like `Models/user.cs` and in the database context.
* **Interface Design:** The application has more than four screens. It uses over six different UI elements. Screens include the map, wallet, and profile pages. Elements include buttons, sliders, and tables.
* **Functionality:** The main user flows are working. Forms include validation and show error messages.
* **Bonus Features:** The project uses the Blazor and MudBlazor UI frameworks. It uses EF Core with SQLite and LINQ. It connects to the external Nominatim API.

### Project Structure

* `Pages/Index.razor`: Shows the map, filters, and marker cards.
* `Pages/PostTask.razor`: Contains the form for posting a task.
* `Pages/TaskDetail.razor`: Handles task acceptance, completion, and ratings.
* `Pages/Wallet.razor`: Displays the user's wallet balance and transactions.
* `Pages/Chat.razor`: Manages chat messages and price offers.
* `Pages/Profile.razor`: Shows user profiles, badges, and reviews.
* `Services/TaskService.cs`: Manages task creation, reading, updating, and deletion.
* `Services/WalletService.cs`: Processes deposits, payments, and refunds.
* `Services/ChatService.cs`: Handles chat messages and price offers.
* `Services/*Service.cs`: These services manage reviews, XP, reputation, and badges.
* `Services/GeoService.cs`: Handles geocoding and distance calculations.
* `Data/ApplicationDbContext.cs`: Defines the database entities and their relationships.
* `Models/*.cs`: Contains the data models like `User`, `SkillTask`, and `Wallet`.
* `wwwroot/js/*.js`: JavaScript files for the Leaflet map and geolocation.
* `SkillShareMap.Test/*`: The NUnit project for all unit tests.

### Testing

1.  Go to the solution's root directory.
2.  Run `dotnet test`.
3.  The test project is `SkillShareMap.Test`. It uses an in-memory database for test isolation.

### Limitations & Future Work

* Future work includes adding a real payment system. This requires transaction locking.
* The application can be improved with stronger password hashing. It also needs email verification and more server-side checks.
* A future version will have real-time chat using SignalR. It will also include notifications.
* The geocoding can be improved with a cache and better rate limiting. The database needs spatial queries for performance.
* Admin tools are needed for reports and content moderation.
* The project needs more unit tests. Tests should cover wallet calculations, geo-filters, and the full task process.