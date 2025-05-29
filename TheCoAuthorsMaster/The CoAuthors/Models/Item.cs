using Microsoft.EntityFrameworkCore;

namespace The_CoAuthors.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
