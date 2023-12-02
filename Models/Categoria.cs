using System.ComponentModel.DataAnnotations;

namespace Ficha1_P1_V1.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        [Display(Name = "Categoria", Prompt = "Introduza a Categoria!")]
        [Required(ErrorMessage = "Indique o nome da Categoria!")]
        public string Nome { get; set; }
        [Display(Name = "Disponível?")]
        public bool Disponivel { get; set; }
        public ICollection<Habitacao> Habitacao { get; set; }
    }
}
