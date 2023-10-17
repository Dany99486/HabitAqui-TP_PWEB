using Ficha1_P1_V1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ficha1_P1_V1.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Aluno> Alunos { get; set; }

        public DbSet<Curso> Cursos { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}