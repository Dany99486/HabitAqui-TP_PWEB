using System.ComponentModel.DataAnnotations;

namespace Ficha1_P1_V1.Models
{
    public enum TipoHabitacao
    {
        Casa,
        Apartamento,
        Condominio,
        Quinta,
        Residencia
    }
    public class Habitacao
	{
		public int Id { get; set; }
		public string Localizacao { get; set; }
		public TipoHabitacao Tipo { get; set; }
		[Display(Name = "Categoria(Quartos)", Prompt = "Indique a categoria(T1,T2,T3,...)!")]
		public int? CategoriaId { get; set; }
		public Categoria Categoria { get; set; }
		public string Descricao { get; set; }

        [Display(Name = "Empresa ID")]
        public int empresaId { get; set; }

        [Display(Name = "Empresa")]
        public Empresa Empresa { get; set; }

        [Display(Name = "Imagem")]
        public string Imagem { get; set; }

        [Display(Name = "Gerido Por")]
        public ApplicationUser Funcionario { get; set; }

        [Display(Name = "Gerido Por")]
        public ApplicationUser Gestor { get; set; }

        [Display(Name = "Estado")]
        public bool estado { get; set; }

        [Display(Name = "Reservado")]
        public bool reservado { get; set; }
    }
}
