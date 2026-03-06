using RecruitmentPlatformAPI.Models.Reference;

namespace RecruitmentPlatformAPI.Data.Seed
{
    /// <summary>
    /// Seed data for Skills reference table (50 common tech and soft skills)
    /// </summary>
    public static class SkillSeed
    {
        private static readonly DateTime SeedCreatedAt = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static List<Skill> GetSkills()
        {
            return new List<Skill>
            {
                // Programming Languages
                new() { Id = 1, Name = "C#", CreatedAt = SeedCreatedAt },
                new() { Id = 2, Name = "JavaScript", CreatedAt = SeedCreatedAt },
                new() { Id = 3, Name = "TypeScript", CreatedAt = SeedCreatedAt },
                new() { Id = 4, Name = "Python", CreatedAt = SeedCreatedAt },
                new() { Id = 5, Name = "Java", CreatedAt = SeedCreatedAt },
                new() { Id = 6, Name = "C++", CreatedAt = SeedCreatedAt },
                new() { Id = 7, Name = "PHP", CreatedAt = SeedCreatedAt },
                new() { Id = 8, Name = "Ruby", CreatedAt = SeedCreatedAt },
                new() { Id = 9, Name = "Go", CreatedAt = SeedCreatedAt },
                new() { Id = 10, Name = "Swift", CreatedAt = SeedCreatedAt },

                // Frontend Frameworks
                new() { Id = 11, Name = "React", CreatedAt = SeedCreatedAt },
                new() { Id = 12, Name = "Angular", CreatedAt = SeedCreatedAt },
                new() { Id = 13, Name = "Vue.js", CreatedAt = SeedCreatedAt },
                new() { Id = 14, Name = "Next.js", CreatedAt = SeedCreatedAt },
                new() { Id = 15, Name = "HTML/CSS", CreatedAt = SeedCreatedAt },
                new() { Id = 16, Name = "Tailwind CSS", CreatedAt = SeedCreatedAt },

                // Backend & Frameworks
                new() { Id = 17, Name = "ASP.NET Core", CreatedAt = SeedCreatedAt },
                new() { Id = 18, Name = "Node.js", CreatedAt = SeedCreatedAt },
                new() { Id = 19, Name = "Django", CreatedAt = SeedCreatedAt },
                new() { Id = 20, Name = "Spring Boot", CreatedAt = SeedCreatedAt },
                new() { Id = 21, Name = "Express.js", CreatedAt = SeedCreatedAt },
                new() { Id = 22, Name = "Flask", CreatedAt = SeedCreatedAt },

                // Databases
                new() { Id = 23, Name = "SQL Server", CreatedAt = SeedCreatedAt },
                new() { Id = 24, Name = "PostgreSQL", CreatedAt = SeedCreatedAt },
                new() { Id = 25, Name = "MySQL", CreatedAt = SeedCreatedAt },
                new() { Id = 26, Name = "MongoDB", CreatedAt = SeedCreatedAt },
                new() { Id = 27, Name = "Redis", CreatedAt = SeedCreatedAt },
                new() { Id = 28, Name = "Entity Framework", CreatedAt = SeedCreatedAt },

                // Cloud & DevOps
                new() { Id = 29, Name = "AWS", CreatedAt = SeedCreatedAt },
                new() { Id = 30, Name = "Azure", CreatedAt = SeedCreatedAt },
                new() { Id = 31, Name = "Docker", CreatedAt = SeedCreatedAt },
                new() { Id = 32, Name = "Kubernetes", CreatedAt = SeedCreatedAt },
                new() { Id = 33, Name = "CI/CD", CreatedAt = SeedCreatedAt },
                new() { Id = 34, Name = "Git", CreatedAt = SeedCreatedAt },
                new() { Id = 35, Name = "Linux", CreatedAt = SeedCreatedAt },

                // Mobile
                new() { Id = 36, Name = "React Native", CreatedAt = SeedCreatedAt },
                new() { Id = 37, Name = "Flutter", CreatedAt = SeedCreatedAt },
                new() { Id = 38, Name = "Android", CreatedAt = SeedCreatedAt },
                new() { Id = 39, Name = "iOS", CreatedAt = SeedCreatedAt },

                // Data & AI
                new() { Id = 40, Name = "Machine Learning", CreatedAt = SeedCreatedAt },
                new() { Id = 41, Name = "Data Analysis", CreatedAt = SeedCreatedAt },
                new() { Id = 42, Name = "Power BI", CreatedAt = SeedCreatedAt },

                // General / Soft Skills
                new() { Id = 43, Name = "REST APIs", CreatedAt = SeedCreatedAt },
                new() { Id = 44, Name = "GraphQL", CreatedAt = SeedCreatedAt },
                new() { Id = 45, Name = "Agile/Scrum", CreatedAt = SeedCreatedAt },
                new() { Id = 46, Name = "Unit Testing", CreatedAt = SeedCreatedAt },
                new() { Id = 47, Name = "Problem Solving", CreatedAt = SeedCreatedAt },
                new() { Id = 48, Name = "Communication", CreatedAt = SeedCreatedAt },
                new() { Id = 49, Name = "Project Management", CreatedAt = SeedCreatedAt },
                new() { Id = 50, Name = "UI/UX Design", CreatedAt = SeedCreatedAt }
            };
        }
    }
}
