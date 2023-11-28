using System.ComponentModel.DataAnnotations;
using Ficha1_P1_V1.Models;

namespace Ficha1_P1_V1.ViewModels
{
	public class PesquisaViewModel
	{
		public List<Arrendamento>? ListaDeArrendamentos { get; set; }
		public int NumResultados { get; set; }
		[Display(Name = "PESQUISA DE CURSOS ...", Prompt = "introduza o texto a pesquisar")]
		public string? TextoAPesquisar { get; set; }
		public TipoHabitacao? Tipo { get; set; }
		public DateTime? dataInicio { get; set; }
		public DateTime? dataFim { get; set; }
		public int? periodoMinimo { get; set; }
	}
}
