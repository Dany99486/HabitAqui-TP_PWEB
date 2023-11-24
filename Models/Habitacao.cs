using System.ComponentModel.DataAnnotations;

namespace Ficha1_P1_V1.Models
{
	public class Habitacao
	{
		public int Id { get; set; }
		public string Localizacao { get; set; }
		public string Tipo { get; set; }
		public string Descricao { get; set; }
	}
}
