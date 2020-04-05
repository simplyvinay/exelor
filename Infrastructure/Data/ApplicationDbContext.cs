using ApiStarter.Domain.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ApiStarter.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<
                                                        User, 
                                                        Role, 
                                                        int, 
                                                        UserClaim, 
                                                        UserRole, 
                                                        UserLogin, 
                                                        RoleClaim, 
                                                        UserToken>
    {
        private readonly IConfiguration _configuration;
        public ApplicationDbContext(
            DbContextOptions options,
            IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
            //optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(
            ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("User");
            builder.Entity<Role>().ToTable("Role");
            builder.Entity<UserClaim>().ToTable("UserClaim");
            builder.Entity<UserLogin>().ToTable("UserLogin");
            builder.Entity<UserRole>().ToTable("UserRole");
            builder.Entity<UserToken>().ToTable("UserToken");
            builder.Entity<RoleClaim>().ToTable("RoleClaim");
        }
    }
}