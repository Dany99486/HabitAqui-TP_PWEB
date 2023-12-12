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
		[Display(Name = "Categoria(Quartos)", Prompt = "Indique a categoria!")]
		public int? CategoriaId { get; set; }
		public Categoria Categoria { get; set; }
		public string? Descricao { get; set; }

        [Display(Name = "Empresa ID")]
        public int EmpresaId { get; set; }

        [Display(Name = "Gerido pelo Funcionário")]
        public string? FuncionarioDaHabitacaoId { get; set; }

        [Display(Name = "Gerido pelo Gestor")]
        public string? GestorDaHabitacaoId { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; } //Nao é o estado da habitacao

        [Display(Name = "Estado da habitação")]
        public string? EstadoHabitacao { get; set; }

        [Display(Name = "Reservado")]
        public bool Reservado { get; set; }
    }
}
