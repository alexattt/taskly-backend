using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TasklyAPI.Models;

namespace TasklyAPI.DbContext
{
	public class TaskDbContext : IdentityDbContext<User>
	{
		public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
		{
		}

		public DbSet<TaskItem> Tasks { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<TaskItem>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Name).IsRequired().HasMaxLength(500);
				entity.Property(e => e.Description).HasMaxLength(1500);
				entity.Property(e => e.Priority).IsRequired();
				entity.Property(e => e.UserId).IsRequired();

				entity.HasOne(e => e.User)
					  .WithMany(u => u.Tasks)
					  .HasForeignKey(e => e.UserId)
					  .OnDelete(DeleteBehavior.Cascade);
			});
		}
	}
}
