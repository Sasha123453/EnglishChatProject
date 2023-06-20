
namespace TestProject.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Difficulty { get; set; }
        public string Solution { get; set; }
        public string NormalizedSolution { get; set; }
        public string Type { get; set; }
        public int Section { get; set; }
        public int Number { get; set; }
        public string NoteForBot { get; set; }
        public Exercise(string name, string description, int difficulty, string solution, string type, int section, int number, string noteForBot)
        {
            Name = name;
            Description = description;
            Difficulty = difficulty;
            Solution = solution;
            Type = type;
            Section = section;
            Number = number;
            NoteForBot = noteForBot;
        }
        public Exercise() { }
    }
}
