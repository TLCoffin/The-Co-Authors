using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using The_CoAuthors.Models;
using static The_CoAuthors.Models.DataTransfers;

namespace The_CoAuthors.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIPromtStringBuilder : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public List<string> NPCByRoom = new List<string>()
        {
            "Lucy",
            "NPC2",
            "NPC3"
        };

        public List<string> RoomNames = new List<string>()
        {
            "Living Room",
            "Stairway",
            "Summoning Circle"
        };

        public List<string> ItemsInRooms = new List<string>()
        {
            "Glasses Necklace Book",
            "item1 item2 item3",
            "item1 item2 item3"
        };

        [HttpGet("talkToNPC")]
        public async Task<IActionResult> TalkToNPC(int roomNumber)
        {
            // User userId to fetch the user's list
            //var userId = _userManager.GetUserId(User);

            // In future, use items that they have already picked up and take them off the list
            //var userInventory = _context.UserInventories.Where(u => u.UserId == userId);

            string prompt = $"You are {NPCByRoom[roomNumber - 1]}, you are in the {RoomNames[roomNumber - 1]}, give a clue to a detective about one of the three items they have missed; {ItemsInRooms[roomNumber - 1]}";

            return Ok(prompt);
        }
    }
}
