namespace TestProject.Models
{
    public class ExercisesViewModel<T>
    {
        public IEnumerable<T> Exercises { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
    }
}
