using Ficha1_P1_V1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ficha1_P1_V1.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<Habitacao> Habitacao { get; set; }
        public DbSet<Arrendamento> Arrendamento { get; set; }
    }
}