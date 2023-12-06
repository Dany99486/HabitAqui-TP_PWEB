using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Ficha1_P1_V1.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Primeiro Nome")]
        public string PrimeiroNome { get; set; }
        [Display(Name = "Último Nome")]
        public string UltimoNome { get; set; }
        [Display(Name = "Data de Nascimento")]
        public DateTime DataNascimento { get; set; }
        [Display(Name = "NIF")]
        public int NIF { get; set; }

        [Range(1, 5, ErrorMessage = "Minimo:1,Max:5")]
        public int? LocadorAvaliacao { get; set; }
		public ICollection<Arrendamento> Arrendamentos { get; set; }

        [Display(Name = "Empresa ID")]
        public int empresaId { get; set; }
        //public int? trabalhadorNumero { get; set; }
    }
}
