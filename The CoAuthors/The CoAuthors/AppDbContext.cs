using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using The_CoAuthors.Models;
using Microsoft.AspNetCore.Identity;

namespace The_CoAuthors
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Item> Items { get; set; }
        public DbSet<UserInventory> UserInventories { get; set; }
    }
}
