using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json;
using The_CoAuthors.Models;
using static The_CoAuthors.Models.DataTransfers;

namespace The_CoAuthors.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIPromptStringBuilder : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AIPromptStringBuilder(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<string> NPCByRoom = new List<string>()
        {
            "You are Lucy. You have been taking care of your best friend's grandma ever since her " +
            "granddaughter disappeared. You arrived in the morning and started making breakfast. " +
            "It wasn't unusual for the grandma to not come down right away, so you kept busy and " +
            "waited until she came down. It was 11 a.m. and the grandma still didn’t come downstairs, " +
            "so you went up to investigate. When you arrived in her room, you found her body. " +
            "The life in her looked like it had been sucked out, her eyes hollow, skin ice cold. " +
            "You called the cops immediately, and that is how our player (the detective) has been " +
            "called in. This is a murder mystery, so we need to figure out the murderer. Don’t make " +
            "it obvious, but Lucy is the murderer — make her seem completely shaken and innocent.",

            "NPC2",

            "NPC3"
        };

        public List<string> RoomNames = new List<string>()
        {
            "living room. Some of the clues located around include a book (on a coffee table), " +
            "a pair of glasses (under an end table beside the couch), a necklace (on the ground, " +
            "in plain sight), and a key (on the top of a bookshelf). You can gently lead the user " +
            "toward picking up these clues along the way. Sometimes, you should talk about yourself, " +
            "the room, or anything else that fits in the storyline, such as details about the grandma, " +
            "who you are to the grandma, etc. Not every response should be a clue!",

            "Stairway",

            "Summoning Circle"
        };

        public List<List<string>> ItemsInRooms = new List<List<string>>()
        {
            new List<string> { "key", "glasses", "necklace", "book" },

            new List<string> { },

            new List<string> { }
        };

        // AI
        public async Task<string> OllamaPrompt(string prompt)
        {
            using var client = new HttpClient();

            var content = new StringContent(
                JsonSerializer.Serialize(new Dictionary<string, object>
                {
                    { "model", "mistral"},
                    { "prompt", prompt},
                    { "stream", false }
                }),
                System.Text.Encoding.UTF8,
                "application/json"
                );
            var response = await client.PostAsync("http://localhost:11434/api/generate", content);
            return await response.Content.ReadAsStringAsync();
        }

        [HttpGet("firstNpcPrompt")]
        public async Task<IActionResult> FirstNpcPrompt(int roomNumber)
        {
            string prompt = $"You are acting as a character in a game. In the game, the user is looking " +
                $"around a room for clickable objects that act as clues. Here is your character description: \r\n \r\n" +

                $"{NPCByRoom[roomNumber - 1]} \r\n \r\n" +

                $"The room you are currently located in is the {RoomNames[roomNumber - 1]} \r\n \r\n" +

                $"Begin playing the character immediately. I will prompt you each time I need a response from " +
                $"you with this prompt: " +
                $"\"Say something. The clues the user has not yet found are:\"" +
                $" with a list of the clues. You should first begin by introducing yourself to the detective." +
                $" This should last multiple responses and be reasonably detailed.\r\n \r\n" +

                $"You can never respond with more than 300 characters.\r\n \r\n" +

                $"Begin.";

            var OllamaReply = await OllamaPrompt(prompt);
            return Ok(OllamaReply);
        }

        [HttpGet("generalNpcPrompt")]
        public async Task<IActionResult> GeneralNpcPrompt(int roomNumber)
        {

            List<string> remainingItems = ItemsInRooms[roomNumber - 1];

            // Duplicate of the GetUserInventory method inside PlayerInventoryController
            var currentUserId = _userManager.GetUserId(User);

                var items = _context.UserInventories
                    .Where(u => u.UserId == currentUserId)
                    .SelectMany(u => u.Items)
                    .Select(i => i.Name)
                    .ToList();

            foreach (var item in items)
            {
                // If item is not in userInventory, add to remainingItems
                if (ItemsInRooms[roomNumber - 1].Contains(item))
                {
                    remainingItems.Remove(item);
                }
            }

            string cluesList = string.Join(", ", remainingItems);

            // Needs logic to weed out the clues in a specific room.
            string prompt = $"Say something. Remember, not every response should be a clue. The " +
                $"clues the user has not yet found are: " +
                $"{cluesList}";
            var OllamaReply = await OllamaPrompt(prompt);

            return Ok(OllamaReply);
        }
    }
}
