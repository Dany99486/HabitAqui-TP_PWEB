using System.ComponentModel.DataAnnotations;

namespace Ficha1_P1_V1.Models
{
	public class Habitacao
	{
		public int Id { get; set; }
		public String Localizacao { get; set; }
		public String Tipo { get; set; }
		public DateTime DataInicio { get; set; }
		public DateTime DataFim { get; set; }
		public int PeriodoMinimo { get; set; }
		public decimal Preco { get; set; }
		public String Locador { get; set; }
		[Range(1, 5, ErrorMessage = "Minimo:1,Max:5")]
		public int LocadorAvaliacao { get; set; }
	}
}
