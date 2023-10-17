using System.ComponentModel.DataAnnotations;

namespace Ficha1_P1_V1.Models
{
    public class Curso
    {
        public int Id { get; set; }

        [Display(Name = "Designação", Prompt = "Introduza a designação do curso!")]
        public string Designacao { get; set; }
        public string Grau { get; set; }
    }

}
