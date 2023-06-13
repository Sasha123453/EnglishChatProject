using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAI_API;
using System.Collections.Concurrent;
using TestProject.Areas.Identity.Data;
using TestProject.Models;

public class ChatController : Controller
{
    private readonly ApplicationContext _context;
    static OpenAIAPI api = new OpenAIAPI(new APIAuthentication("Your_key"));
    static ConcurrentDictionary<string, OpenAI_API.Chat.Conversation> userConversations = new ConcurrentDictionary<string, OpenAI_API.Chat.Conversation>();
    public ChatController(ApplicationContext context)
    {
        _context = context;
    }
    [HttpGet]
    [Authorize]
    public ActionResult Index()
    {
        string systemMessage = $"Ты - чатбот на сайте для изучения английского языка. Пользователь с ником ${User.Identity.Name} обратился к тебе за помощью, помоги ему";
        if (!userConversations.TryGetValue(User.Identity.Name, out _)) { SetSystemMessage(systemMessage); }
        return View();
    }
    [HttpGet]
    [ActionName("IndexById")]
    [Authorize]
    public async Task<IActionResult> Index(int id)
    {
        Exercise exercise = await _context.Exercises.FirstAsync(x => x.Id == id);
        string systemMessage = $"Ты - чатбот на сайте для изучения английского языка. Пользователь с ником ${User.Identity.Name} обратился к тебе за помощью, " +
            $"он не может выполнить задание со следующими данными: название=${exercise.Name}, описание={exercise.Description}, решение=${exercise.Solution}, примечание=${exercise.NoteForBot}";
        userConversations.TryRemove(User.Identity.Name, out _);
        SetSystemMessage(systemMessage);
        return View("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Chat(string userInput)
    {
        try
        {
            string response = await GenerateChatResponse(userInput);

            if (!string.IsNullOrEmpty(response))
            {
                return Json(new { success = true, message = response });
            }
            else
            {
                return StatusCode(500, "Failed to generate chat response.");
            }
        }
        catch (Exception ex)
        {
            string errorMessage = "An error occurred while processing the chat request. Details: " + ex.Message;
            return Json(new { success = false, error = errorMessage });
        }
    }
    private async Task<string> GenerateChatResponse(string userInput)
    {
        userConversations.TryGetValue(User.Identity.Name, out var chat);
        chat.AppendUserInput(userInput);
        string response = await chat.GetResponseFromChatbotAsync();
        return response;
    }
    private void SetSystemMessage(string message)
    {
        var chat = api.Chat.CreateConversation();
        chat.AppendSystemMessage(message);
        userConversations.TryAdd(User.Identity.Name, chat);
    }
    [HttpPost]
    public IActionResult GetChatMessages()
    {
        try
        {
            if (userConversations.TryGetValue(User.Identity.Name, out var existingConversation))
            {
                var messagesList = existingConversation.Messages;
                return Json(new { success = true, messages = messagesList });
            }
            else
            {
                return StatusCode(500, "");
            }
        }
        catch (Exception ex)
        {
            string errorMessage = "An error occurred while processing the chat request. Details: " + ex.Message;
            return Json(new { success = false, error = errorMessage });
        }
    }
    [HttpPost]
    public IActionResult ChatReset()
    {
        try
        {
            if (userConversations.TryRemove(User.Identity.Name, out _))
            {
                string systemMessage = $"Ты - чатбот на сайте для изучения английского языка. Пользователь с ником ${User.Identity.Name} обратился к тебе за помощью, помоги ему";
                SetSystemMessage(systemMessage);
                return Json(new { success = true});
            }
            else
            {
                return StatusCode(400, "");
            }
        }
        catch (Exception ex)
        {
            string errorMessage = "An error occurred while processing the chat request. Details: " + ex.Message;
            return Json(new { success = false, error = errorMessage });
        }
    }

}
