using System.ComponentModel.DataAnnotations;

namespace Ficha1_P1_V1.Models
{
    public class Aluno
    {
        public int Id { get; set; }

        [Display(Name = "Nome", Prompt = "Introduza o nome do aluno!")]
        public string Name { get; set; }
        public string Mail { get; set; }
    }
}
