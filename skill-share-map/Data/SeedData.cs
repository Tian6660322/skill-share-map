using SkillShareMap.Models;

namespace SkillShareMap.Data;

public static class SeedData
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Check if database already has data
        if (context.Users.Any())
            return;

        // Create Student Users
        var students = new List<User>
        {
            new User
            {
                Username = "alice_student",
                Email = "alice@university.edu",
                PasswordHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("password123")),
                Role = UserRole.Student,
                FirstName = "Alice",
                LastName = "Johnson",
                SchoolName = "University of Technology Sydney",
                Bio = "Computer Science student, love coding!",
                IsVerified = true,
                HomeBaseLatitude = -33.8836,
                HomeBaseLongitude = 151.2006,
                HomeBaseAddress = "15 Broadway, Ultimo NSW 2007",
                ReputationLevel = 4.5
            },
            new User
            {
                Username = "bob_student",
                Email = "bob@university.edu",
                PasswordHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("password123")),
                Role = UserRole.Student,
                FirstName = "Bob",
                LastName = "Smith",
                SchoolName = "University of Sydney",
                Bio = "Design enthusiast and photographer",
                IsVerified = true,
                HomeBaseLatitude = -33.8879,
                HomeBaseLongitude = 151.1876,
                HomeBaseAddress = "Camperdown NSW 2006",
                ReputationLevel = 4.0
            },
            new User
            {
                Username = "charlie_student",
                Email = "charlie@university.edu",
                PasswordHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("password123")),
                Role = UserRole.Student,
                FirstName = "Charlie",
                LastName = "Brown",
                SchoolName = "UNSW Sydney",
                Bio = "Engineering student, happy to help with math and tech",
                IsVerified = true,
                HomeBaseLatitude = -33.9173,
                HomeBaseLongitude = 151.2313,
                HomeBaseAddress = "Kensington NSW 2052",
                ReputationLevel = 5.0
            }
        };

        // Create Company/School Users
        var companies = new List<User>
        {
            new User
            {
                Username = "techcorp",
                Email = "hr@techcorp.com",
                PasswordHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("password123")),
                Role = UserRole.Company,
                CompanyName = "TechCorp Australia",
                WebsiteUrl = "https://techcorp.com.au",
                CompanyDescription = "Leading technology company in Sydney",
                HomeBaseLatitude = -33.8650,
                HomeBaseLongitude = 151.2094,
                HomeBaseAddress = "200 George St, Sydney NSW 2000"
            },
            new User
            {
                Username = "design_studio",
                Email = "contact@designstudio.com",
                PasswordHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("password123")),
                Role = UserRole.Company,
                CompanyName = "Creative Design Studio",
                WebsiteUrl = "https://designstudio.com",
                CompanyDescription = "Award-winning design agency",
                HomeBaseLatitude = -33.8697,
                HomeBaseLongitude = 151.2079,
                HomeBaseAddress = "Surry Hills NSW 2010"
            }
        };

        context.Users.AddRange(students);
        context.Users.AddRange(companies);
        context.SaveChanges();

        // Create Wallets for all users
        foreach (var user in students.Concat(companies))
        {
            context.Wallets.Add(new Wallet
            {
                UserId = user.Id,
                Balance = 1000
            });
        }
        context.SaveChanges();

        // Create Tasks
        var tasks = new List<SkillTask>
        {
            new SkillTask
            {
                Title = "Help with Python Assignment",
                Description = "Need help debugging a Python script for data analysis project",
                Category = TaskCategory.TechHelp,
                Status = SkillTaskStatus.Open,
                Budget = 50,
                Deadline = DateTime.UtcNow.AddDays(7),
                CreatorId = students[0].Id,
                Latitude = -33.8836,
                Longitude = 151.2006,
                LocationAddress = "UTS Building 11"
            },
            new SkillTask
            {
                Title = "Photography for Event",
                Description = "Need a photographer for a student club event this weekend",
                Category = TaskCategory.PhotoVideo,
                Status = SkillTaskStatus.Open,
                Budget = 150,
                Deadline = DateTime.UtcNow.AddDays(3),
                CreatorId = students[1].Id,
                Latitude = -33.8879,
                Longitude = 151.1876,
                LocationAddress = "University of Sydney, Quadrangle"
            },
            new SkillTask
            {
                Title = "Logo Design for Startup",
                Description = "Looking for a creative designer to create a modern logo",
                Category = TaskCategory.CreativeDesign,
                Status = SkillTaskStatus.Open,
                Budget = 200,
                Deadline = DateTime.UtcNow.AddDays(14),
                CreatorId = students[2].Id,
                Latitude = -33.9173,
                Longitude = 151.2313,
                LocationAddress = "UNSW Campus"
            },
            new SkillTask
            {
                Title = "Essay Proofreading",
                Description = "Need someone to proofread my 3000-word essay",
                Category = TaskCategory.WritingEditing,
                Status = SkillTaskStatus.Open,
                Budget = 80,
                Deadline = DateTime.UtcNow.AddDays(5),
                CreatorId = students[0].Id,
                Latitude = -33.8836,
                Longitude = 151.2006,
                LocationAddress = "UTS Library"
            },
            new SkillTask
            {
                Title = "Spanish Language Tutor",
                Description = "Looking for a Spanish tutor for conversational practice",
                Category = TaskCategory.LanguageHelp,
                Status = SkillTaskStatus.Open,
                Budget = 40,
                Deadline = DateTime.UtcNow.AddDays(30),
                CreatorId = students[1].Id,
                Latitude = -33.8879,
                Longitude = 151.1876,
                LocationAddress = "Sydney CBD"
            },
            new SkillTask
            {
                Title = "Math Exam Preparation",
                Description = "Need help preparing for calculus final exam",
                Category = TaskCategory.StudyHelp,
                Status = SkillTaskStatus.Assigned,
                Budget = 100,
                Deadline = DateTime.UtcNow.AddDays(10),
                CreatorId = students[0].Id,
                AssignedToId = students[2].Id,
                Latitude = -33.8836,
                Longitude = 151.2006,
                LocationAddress = "UTS Building 2",
                IsDepositPaid = true,
                DepositAmount = 10
            }
        };

        context.SkillTasks.AddRange(tasks);
        context.SaveChanges();

        // Create Jobs
        var jobs = new List<Job>
        {
            new Job
            {
                Title = "Software Engineering Intern",
                Responsibilities = "Develop web applications using React and Node.js. Work with senior developers on real projects.",
                Qualifications = "Currently studying Computer Science or related field. Familiar with JavaScript and web development.",
                EmploymentType = EmploymentType.Internship,
                PostedById = companies[0].Id,
                Latitude = -33.8650,
                Longitude = 151.2094,
                LocationAddress = "200 George St, Sydney NSW 2000",
                IsOpen = true
            },
            new Job
            {
                Title = "Graphic Design Intern",
                Responsibilities = "Create visual content for social media and marketing campaigns. Assist senior designers with projects.",
                Qualifications = "Portfolio showcasing design skills. Proficient in Adobe Creative Suite.",
                EmploymentType = EmploymentType.PartTime,
                PostedById = companies[1].Id,
                Latitude = -33.8697,
                Longitude = 151.2079,
                LocationAddress = "Surry Hills NSW 2010",
                IsOpen = true
            },
            new Job
            {
                Title = "UI/UX Designer",
                Responsibilities = "Design user interfaces for mobile and web applications. Conduct user research and testing.",
                Qualifications = "2+ years experience in UI/UX design. Strong portfolio required.",
                EmploymentType = EmploymentType.FullTime,
                PostedById = companies[0].Id,
                Latitude = -33.8650,
                Longitude = 151.2094,
                LocationAddress = "200 George St, Sydney NSW 2000",
                IsOpen = true
            }
        };

        context.Jobs.AddRange(jobs);
        context.SaveChanges();

        // Create Courses
        var courses = new List<Course>
        {
            new Course
            {
                Title = "Introduction to Web Development",
                Description = "Learn HTML, CSS, and JavaScript basics",
                Category = TaskCategory.TechHelp,
                Type = CourseType.OnlineCourse,
                Duration = "6 weeks",
                Difficulty = DifficultyLevel.Beginner,
                InstructorName = "Dr. Sarah Tech",
                ImageUrl = "/images/courses/web-dev.jpg",
                ExternalUrl = "https://example.com/web-dev"
            },
            new Course
            {
                Title = "Photography Masterclass",
                Description = "Master digital photography and photo editing",
                Category = TaskCategory.PhotoVideo,
                Type = CourseType.Workshop,
                Duration = "3 days",
                Difficulty = DifficultyLevel.Intermediate,
                InstructorName = "John Photographer",
                ImageUrl = "/images/courses/photography.jpg"
            },
            new Course
            {
                Title = "Creative Writing Workshop",
                Description = "Improve your writing skills and storytelling",
                Category = TaskCategory.WritingEditing,
                Type = CourseType.Workshop,
                Duration = "4 weeks",
                Difficulty = DifficultyLevel.Beginner,
                InstructorName = "Emma Writer",
                ImageUrl = "/images/courses/writing.jpg"
            },
            new Course
            {
                Title = "Graphic Design Fundamentals",
                Description = "Learn design principles and Adobe Creative Suite",
                Category = TaskCategory.CreativeDesign,
                Type = CourseType.OnlineCourse,
                Duration = "8 weeks",
                Difficulty = DifficultyLevel.Beginner,
                InstructorName = "Mark Designer",
                ImageUrl = "/images/courses/graphic-design.jpg"
            },
            new Course
            {
                Title = "Spanish for Beginners",
                Description = "Start your Spanish learning journey",
                Category = TaskCategory.LanguageHelp,
                Type = CourseType.OnlineCourse,
                Duration = "10 weeks",
                Difficulty = DifficultyLevel.Beginner,
                InstructorName = "Maria Garcia",
                ImageUrl = "/images/courses/spanish.jpg"
            },
            new Course
            {
                Title = "Advanced Mathematics Techniques",
                Description = "Master calculus and linear algebra",
                Category = TaskCategory.StudyHelp,
                Type = CourseType.WebSeminar,
                Duration = "5 weeks",
                Difficulty = DifficultyLevel.Advanced,
                InstructorName = "Prof. David Math",
                ImageUrl = "/images/courses/math.jpg"
            }
        };

        context.Courses.AddRange(courses);
        context.SaveChanges();

        // Create some skill progress for Charlie (the expert helper)
        var charlieProgress = new List<UserSkillProgress>
        {
            new UserSkillProgress
            {
                UserId = students[2].Id,
                Category = TaskCategory.StudyHelp,
                TotalXp = 850,
                CurrentTier = BadgeTier.Expert
            },
            new UserSkillProgress
            {
                UserId = students[2].Id,
                Category = TaskCategory.TechHelp,
                TotalXp = 450,
                CurrentTier = BadgeTier.Advanced
            }
        };

        context.UserSkillProgress.AddRange(charlieProgress);
        context.SaveChanges();

        // Create badges for Charlie
        var charlieBadges = new List<UserBadge>
        {
            new UserBadge
            {
                UserId = students[2].Id,
                Category = TaskCategory.StudyHelp,
                Tier = BadgeTier.Expert
            },
            new UserBadge
            {
                UserId = students[2].Id,
                Category = TaskCategory.TechHelp,
                Tier = BadgeTier.Advanced
            }
        };

        context.UserBadges.AddRange(charlieBadges);
        context.SaveChanges();

        // Create some ratings
        var ratings = new List<Rating>
        {
            new Rating
            {
                FromUserId = students[0].Id,
                ToUserId = students[2].Id,
                TaskId = tasks[5].Id,
                Category = TaskCategory.StudyHelp,
                Stars = 5,
                Comment = "Charlie is an amazing tutor! Very patient and knowledgeable.",
                XpAwarded = 12
            },
            new Rating
            {
                FromUserId = students[1].Id,
                ToUserId = students[2].Id,
                Stars = 4,
                Comment = "Great help with my coding assignment. Would recommend!",
                XpAwarded = 10
            }
        };

        context.Ratings.AddRange(ratings);
        context.SaveChanges();
    }
}
