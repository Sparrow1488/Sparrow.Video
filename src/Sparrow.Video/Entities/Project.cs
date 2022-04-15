using Sparrow.Video.Enums;

namespace Sparrow.Video.Entities
{
    public class Project
    {
        public static bool IsAvailable { get; private set; }
        public static string Name { get; private set; }

        public static string CreateProject()
        {
            string name = $"project_{Guid.NewGuid()}";
            Name = name;
            Paths.CreateProject(name);
            IsAvailable = true;
            return name;
        }

        public static void UseExistsProject(string projectName)
        {
            Name = projectName;
            Paths.CreateProject(projectName);
            IsAvailable = true;
        }
    }
}
