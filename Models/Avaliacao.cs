namespace Ficha1_P1_V1.Models
{
    public class Avaliacao
    {
        public int Id { get; set; }
        public int Classificacao { get; set; }
        public string Comentario { get; set; }
        public int ArrendamentoId { get; set; }
        public Arrendamento Arrendamento { get; set; }
    }
}