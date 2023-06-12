using Microsoft.Identity.Client;
using TestProject.Areas.Identity.Data;

namespace TestProject.Models
{
    public class ExerciseSolution
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public virtual Exercise Excercise { get; set; }
        public int ExerciseId { get; set; }
        public DateTime SubmissionDate { get; set; }
        public ExerciseSolution(string userId, int exerciseId)
        {
            SubmissionDate = DateTime.Now;
            UserId = userId;
            ExerciseId = exerciseId;
        }
    }
}
