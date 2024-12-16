using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServiceProvider.Models;
using System.Reflection.Metadata;

namespace ServiceProvider.Database
{
    public class AppDbContext : DbContext
    {
		public DbSet<IUser> Users { get; set; }
        public DbSet<Service> Services { get; set; }
		public DbSet<UserService> UserServices { get; set; }
		public DbSet<ServiceRequest> ServiceRequests { get; set; }
		public DbSet<BannedUser> BannedUsers { get; set; }

		public AppDbContext(DbContextOptions options) : base(options)
        {
        }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
		}
	}
}
