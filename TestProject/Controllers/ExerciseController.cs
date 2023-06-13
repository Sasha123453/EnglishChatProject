
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

    public ExerciseController(ApplicationContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    [Authorize]
    public async Task<IActionResult> UserExercise(int page = 1)
    {
        int pageSize = 10;
        string userId = _userManager.GetUserId(User);
        List<ExerciseWithSolutionStatus> exercises = await GetExercisesFromDataSource(userId);

        IPagedList<ExerciseWithSolutionStatus> pagedExercises = exercises.ToPagedList(page, pageSize);

        var viewModel = new ExercisesViewModel<ExerciseWithSolutionStatus>
        {
            ExercisesWithSolutionStatus = pagedExercises,
            CurrentPage = pagedExercises.PageNumber,
            TotalPages = pagedExercises.PageCount
        };

        return View(viewModel);
    }

    public async Task<List<ExerciseWithSolutionStatus>> GetExercisesFromDataSource(string userId)
    {
        var result = await (from exercise in _context.Exercises
                            join solution in _context.ExerciseSolutions
                            on new { ExerciseId = exercise.Id, UserId = userId }
                            equals new { ExerciseId = solution.ExerciseId, UserId = solution.UserId }
                            into solutionGroup
                            from solutionOrNull in solutionGroup.DefaultIfEmpty()
                            select new ExerciseWithSolutionStatus
                            {
                                Exercise = exercise,
                                HasSolution = solutionOrNull != null
                            }).ToListAsync();

        return result;
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
                return Json(new { success = true, message = "Верный ответ"});
            }
            else if (checkIfSolved)
            {
                return StatusCode(409, "Эта задача уже решена");
            }
            else
            {
                return StatusCode(400, "Неверный ответ");
            }
        }
        catch (Exception ex)
        {
            string errorMessage = "An error occurred while processing the chat request. Details: " + ex.Message;
            return Json(new { success = false, error = errorMessage });
        }
    }

}