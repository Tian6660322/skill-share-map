using SkillShareMap.Models;

namespace SkillShareMap.Data;

public static class SeedData
{
    public static void Initialize(ApplicationDbContext context)
    {
        var userSeeds = new List<User>
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
            },
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

        foreach (var userSeed in userSeeds)
        {
            if (!context.Users.Any(u => u.Username == userSeed.Username))
            {
                context.Users.Add(userSeed);
            }
        }

        context.SaveChanges();

        var trackedUsernames = userSeeds.Select(u => u.Username).ToList();
        var userLookup = context.Users
            .Where(u => trackedUsernames.Contains(u.Username))
            .ToDictionary(u => u.Username, u => u);

        if (!userLookup.TryGetValue("alice_student", out var alice) ||
            !userLookup.TryGetValue("bob_student", out var bob) ||
            !userLookup.TryGetValue("charlie_student", out var charlie) ||
            !userLookup.TryGetValue("techcorp", out var techcorp) ||
            !userLookup.TryGetValue("design_studio", out var designStudio))
        {
            return;
        }

        foreach (var user in userLookup.Values)
        {
            if (!context.Wallets.Any(w => w.UserId == user.Id))
            {
                context.Wallets.Add(new Wallet
                {
                    UserId = user.Id,
                    Balance = 1000
                });
            }
        }

        context.SaveChanges();

        var tasksToSeed = new List<SkillTask>
        {
            new SkillTask
            {
                Title = "Help with Python Assignment",
                Description = "Need help debugging a Python script for data analysis project",
                Category = TaskCategory.TechHelp,
                Status = SkillTaskStatus.Open,
                Budget = 50,
                Deadline = DateTime.UtcNow.AddDays(7),
                CreatorId = alice.Id,
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
                CreatorId = bob.Id,
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
                CreatorId = charlie.Id,
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
                CreatorId = alice.Id,
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
                CreatorId = bob.Id,
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
                CreatorId = alice.Id,
                AssignedToId = charlie.Id,
                Latitude = -33.8836,
                Longitude = 151.2006,
                LocationAddress = "UTS Building 2",
                IsDepositPaid = true,
                DepositAmount = 10
            }
        };

        foreach (var taskSeed in tasksToSeed)
        {
            if (!context.SkillTasks.Any(t => t.Title == taskSeed.Title))
            {
                context.SkillTasks.Add(taskSeed);
            }
        }

        context.SaveChanges();

        var taskTitles = tasksToSeed.Select(t => t.Title).ToList();
        var taskLookup = context.SkillTasks
            .Where(t => taskTitles.Contains(t.Title))
            .ToDictionary(t => t.Title, t => t);

        var jobsToSeed = new List<Job>
        {
            new Job
            {
                Title = "Software Engineering Intern",
                Responsibilities = "Develop web applications using React and Node.js. Work with senior developers on real projects.",
                Qualifications = "Currently studying Computer Science or related field. Familiar with JavaScript and web development.",
                EmploymentType = EmploymentType.Internship,
                PostedById = techcorp.Id,
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
                PostedById = designStudio.Id,
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
                PostedById = techcorp.Id,
                Latitude = -33.8650,
                Longitude = 151.2094,
                LocationAddress = "200 George St, Sydney NSW 2000",
                IsOpen = true
            }
        };

        foreach (var jobSeed in jobsToSeed)
        {
            if (!context.Jobs.Any(j => j.Title == jobSeed.Title))
            {
                context.Jobs.Add(jobSeed);
            }
        }

        context.SaveChanges();

        var coursesToSeed = new List<Course>
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
                ImageUrl = "/images/courses/web-dev.svg",
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
                ImageUrl = "/images/courses/photography.svg"
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
                ImageUrl = "/images/courses/writing.svg"
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
                ImageUrl = "/images/courses/graphic-design.svg"
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
                ImageUrl = "/images/courses/spanish.svg"
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
                ImageUrl = "/images/courses/math.svg"
            }
        };

        foreach (var courseSeed in coursesToSeed)
        {
            if (!context.Courses.Any(c => c.Title == courseSeed.Title))
            {
                context.Courses.Add(courseSeed);
            }
        }

        context.SaveChanges();

        var progressSeeds = new List<UserSkillProgress>
        {
            new UserSkillProgress
            {
                UserId = charlie.Id,
                Category = TaskCategory.StudyHelp,
                TotalXp = 850,
                CurrentTier = BadgeTier.Expert
            },
            new UserSkillProgress
            {
                UserId = charlie.Id,
                Category = TaskCategory.TechHelp,
                TotalXp = 450,
                CurrentTier = BadgeTier.Advanced
            }
        };

        foreach (var progressSeed in progressSeeds)
        {
            if (!context.UserSkillProgress.Any(p =>
                    p.UserId == progressSeed.UserId &&
                    p.Category == progressSeed.Category))
            {
                context.UserSkillProgress.Add(progressSeed);
            }
        }

        context.SaveChanges();

        var badgeSeeds = new List<UserBadge>
        {
            new UserBadge
            {
                UserId = charlie.Id,
                Category = TaskCategory.StudyHelp,
                Tier = BadgeTier.Expert
            },
            new UserBadge
            {
                UserId = charlie.Id,
                Category = TaskCategory.TechHelp,
                Tier = BadgeTier.Advanced
            }
        };

        foreach (var badgeSeed in badgeSeeds)
        {
            if (!context.UserBadges.Any(b =>
                    b.UserId == badgeSeed.UserId &&
                    b.Category == badgeSeed.Category &&
                    b.Tier == badgeSeed.Tier))
            {
                context.UserBadges.Add(badgeSeed);
            }
        }

        context.SaveChanges();

        if (!taskLookup.TryGetValue("Math Exam Preparation", out var mathPrepTask))
        {
            return;
        }

        var ratingSeeds = new List<Rating>
        {
            new Rating
            {
                FromUserId = alice.Id,
                ToUserId = charlie.Id,
                TaskId = mathPrepTask.Id,
                Category = TaskCategory.StudyHelp,
                Stars = 5,
                Comment = "Charlie is an amazing tutor! Very patient and knowledgeable.",
                XpAwarded = 12
            },
            new Rating
            {
                FromUserId = bob.Id,
                ToUserId = charlie.Id,
                Category = TaskCategory.TechHelp,
                Stars = 4,
                Comment = "Great help with my coding assignment. Would recommend!",
                XpAwarded = 10
            }
        };

        foreach (var ratingSeed in ratingSeeds)
        {
            if (!context.Ratings.Any(r => r.Comment == ratingSeed.Comment))
            {
                context.Ratings.Add(ratingSeed);
            }
        }

        context.SaveChanges();
    }
}
