using Microsoft.AspNetCore.Identity;

namespace The_CoAuthors.Models
{
    public class UserInventory
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public List<Item> Items { get; set; }
    }
}
