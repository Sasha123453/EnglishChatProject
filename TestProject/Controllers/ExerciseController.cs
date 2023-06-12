
using X.PagedList;
using Microsoft.AspNetCore.Mvc;
using TestProject.Areas.Identity.Data;
using TestProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

public class ExerciseController : Controller
{
    private readonly ApplicationContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ExerciseController(ApplicationContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    [Authorize]
    public async Task<IActionResult> UserExercise(int page = 1)
    {
        int pageSize = 10;
        List<Exercise> exercises = await GetExercisesFromDataSource();

        IPagedList<Exercise> pagedExercises = exercises.ToPagedList(page, pageSize);

        var viewModel = new ExercisesViewModel<Exercise>
        {
            Exercises = pagedExercises,
            CurrentPage = pagedExercises.PageNumber,
            TotalPages = pagedExercises.PageCount
        };

        return View(viewModel);
    }

    private async Task<List<Exercise>> GetExercisesFromDataSource()
    {
        return await _context.Exercises.ToListAsync();
    }
    [HttpPost]
    public async Task<IActionResult> CheckAnswer(string userInput, string exerciseModelId)
    {
        try
        {
            Exercise checkExercise = await _context.Exercises.FirstAsync(x => x.Id.ToString() == exerciseModelId);
            string userId =  _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            bool checkIfSolved = await _context.ExerciseSolutions.AnyAsync(x => x.ExerciseId == checkExercise.Id && x.UserId == userId);
            if (checkExercise.Solution == userInput && !checkIfSolved)

            {
                ExerciseSolution userSolution = new ExerciseSolution(userId, checkExercise.Id);
                await _context.ExerciseSolutions.AddAsync(userSolution);
                user.Score += checkExercise.Difficulty;
                user.LatestExerciseId = checkExercise.Id;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Верный ответ", checkIfSolved });
            }
            else if (checkIfSolved)
            {
                return StatusCode(500, "Эта задача уже решена");
            }
            else
            {
                return StatusCode(500, "Неверный ответ");
            }
        }
        catch (Exception ex)
        {
            string errorMessage = "An error occurred while processing the chat request. Details: " + ex.Message;
            return Json(new { success = false, error = errorMessage });
        }
    }
    [HttpGet]
    public async Task<IActionResult> IsExerciseSolved(int exerciseId)
    {
        try
        {
            string userId = _userManager.GetUserId(User);
            bool isSolved = await _context.ExerciseSolutions.AnyAsync(solution => solution.UserId == userId && solution.ExerciseId == exerciseId);
            return Json(new { isSolved });
        }
        catch (Exception ex)
        {
            return Json(new { error = ex.Message });
        }
    }

}