using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ficha1_P1_V1.Models
{
	public class Arrendamento
	{
		public int Id { get; set; }

		[Display(Name = "Data de Início", Prompt = "Introduza a Data de Início da Aula!")]
		public DateTime? DataInicio { get; set; }
		[Display(Name = "Data de Fim", Prompt = "Introduza a Data de Fim da Aula!")]
		[Required(ErrorMessage = "Indique a data de Início!")]
		public DateTime? DataFim { get; set; }
		[Display(Name = "Periodo Mínimo", Prompt = "Indique o Periodo Mínimo!")]
		public int PeriodoMinimo { get; set; }
		[Display(Name = "Periodo Máximo", Prompt = "Indique o Periodo Máximo!")]
		public int PeriodoMaximo { get; set; }
		[Display(Name = "Preço", Prompt = "Indique o Preço!")]
		public decimal Preco { get; set; }
		public int habitacaoId { get; set; }

		[Display(Name = "Habitacao", Prompt = "Indique o Tipo de Aula!")]
		public Habitacao habitacao { get; set; }

		[Display(Name = "Locador ID")]
		public string locadorId { get; set; }

		[Display(Name = "Locador")]
		public ApplicationUser locador { get; set; }

		[Display(Name = "Avaliação", Prompt = "Indique a Avaliação!")]
		public List<Avaliacao>? Avaliacao { get; set; }

		[Display(Name = "Aceite")] //Aceitar/rejeitar arrendamento
		public bool Aceite { get; set; }

		[Display(Name = "Entregue a cliente")] //Estado da habitacao
		public string? EstadoEntregue { get; set; }

        [Display(Name = "Recebido do Cliente")] //Estado da habitacao quando recebida
        public string? EstadoRecebido { get; set; }

        [Display(Name = "Equipamentos opcionais")]
		public string? EquipamentosOpcionais { get; set; }

		[Display(Name = "Danos na habitação")]
		public string? DanosHabitacao { get; set; }

		[Display(Name = "Observações")]
		public string? Observacoes { get; set; }

		//Recebido do cliente, grava o estado depois de o cliente entregar a habitacao
        [Display(Name = "Equipamentos opcionais")]
        public string? EquipamentosOpcionaisC { get; set; }

        [Display(Name = "Danos na habitação")]
        public string? DanosHabitacaoC { get; set; }

        [Display(Name = "Observações")]
        public string? ObservacoesC { get; set; }
    }
}
