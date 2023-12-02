using Microsoft.CodeAnalysis.Options;

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
        public TipoClassificacao Classificacao { get; set; }
        public string Comentario { get; set; }
        public int ArrendamentoId { get; set; }
        public Arrendamento Arrendamento { get; set; }
    }
}