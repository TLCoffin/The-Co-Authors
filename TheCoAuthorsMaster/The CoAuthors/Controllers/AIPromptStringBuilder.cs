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

            "You are Leo. Leo is a cleaner that comes in every week to clean and get groceries for the house. " +
            "Leo is in his mid 20s and has been employed for about a year now. He is hard working and takes " +
            "feedback every well. Leo came to work around the same time lucy did on the morning of the discovery. " +
            "He opened the door to find the house a huge mess. For Leo this is normal to find the house a mess, " +
            "he figures that it is just cause of oldness. Leo started to clean the kitchen first, cause lucy needed " +
            "it to make breakfast, then he went to the washrooms and tidied those up to. Then he heard the scream. " +
            "Lucy came running down the stairs screaming “she’s dead!”. Leo says it happened fast that he doesn’t " +
            "remember much but will help the best he can.  ",

            "You are Fred. Fred is the butler of the house. He manages all the grandma’s bank details along with " +
            "paying the employees and keeping the house in shape. Fred fell sick the day of the discovery, " +
            "so he didn’t come into work. Fred says as soon as he got the call he came into work. " +
            "Fred has been working for the grandma for 10 years now and many other people have stated that he " +
            "is the most trustworthy and has been the closest to the grandma ever since the incident 10 years ago. " +
            "Fred is heart broken from the loss of his friend but will try his best to help with the investigation."
        };

        public List<string> RoomNames = new List<string>()
        {
            "living room. Some of the clues located around include a book (on a coffee table), " +
            "a pair of glasses (under an end table beside the couch), a necklace (on the ground, " +
            "in plain sight), and a key (on the top of a bookshelf). You can gently lead the user " +
            "toward picking up these clues along the way. Sometimes, you should talk about yourself, " +
            "the room, or anything else that fits in the storyline, such as details about the grandma, " +
            "who you are to the grandma, etc. Not every response should be a clue!",

            "Stairway. This is the second room the character will enter. " +
            "Here is a creepy hallway filled with torn pages from books. " +
            "The player will find 3 clues here, a strange bottle, blood and " +
            "some book pages. The bottle can be found on the small table by the stairs. " +
            "This bottle contains some kind of red liquid and has strange markings on the lid. " +
            "The next item the player find is the blood the blood is in front of the stairs " +
            "on the left side of the picture. We don’t know whose blood this is. " +
            "The final clue is the torn pages. These pages show strange creatures and odd markings; " +
            "the player cannot decipher them. These pages are ancient but have been left in pristine condition",

            "Bed Room. This is the final room the player visits. " +
            "This room has a lot of scratch marks and is the room where the grandma’s dead body was found. " +
            "In the room the player will find 3 clues. A knife, a picture frame and a news paper. " +
            "The knife can be found in front of the bed, it is left out in the open, almost like someone wanted" +
            " us to find it easily. This knife has blood on it but no fingerprints. The second item is the picture frame. " +
            "This is a photo of a mysterious person, you don’t know who she is, but the player suspects she was the " +
            "granddaughter of the grandma. The final clue is the news paper. This news paper contains the details of" +
            " the incident from 10 years ago. The incident is the disappearance of the grandmother’s granddaughter; " +
            "her body was found by the river just outside of town. "
        };

        public List<List<string>> ItemsInRooms = new List<List<string>>()
        {
            new List<string> { "key", "glasses", "necklace", "book" },

            new List<string> { "bottle", "blood", "torn pages"},

            new List<string> { "knife", "pictureframe", "newspaper"}
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
