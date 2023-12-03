using Microsoft.CodeAnalysis.Options;
using System.ComponentModel.DataAnnotations;

namespace Ficha1_P1_V1.Models
{
    public enum TipoClassificacao
    {
		MuitoMau,
		Mau,
		Razoavel,
		Bom,
		MuitoBom
	}
    public class Avaliacao
    {
        public int Id { get; set; }
        [Display(Name = "Classificação")]
        public TipoClassificacao Classificacao { get; set; }
        [Display(Name = "Comentário")]
        public string Comentario { get; set; }
        [Display(Name = "ID do Arrendamento")]
        public int ArrendamentoId { get; set; }
        public Arrendamento Arrendamento { get; set; }
    }
}