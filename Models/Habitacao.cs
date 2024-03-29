﻿using System.ComponentModel.DataAnnotations;

namespace Ficha1_P1_V1.Models
{
    public enum TipoHabitacao
    {
        Casa,
        Apartamento,
        Condominio,
        Quinta,
        Residencia
        // Adicione outros tipos conforme necessário
    }
    public class Habitacao
	{
		public int Id { get; set; }
		public string Localizacao { get; set; }
		public TipoHabitacao Tipo { get; set; }
		[Display(Name = "Categoria(Quartos)", Prompt = "Indique a categoria(T1,T2,T3,...)!")]
		public int? CategoriaId { get; set; }
		public Categoria Categoria { get; set; }
		public string Descricao { get; set; }
	}
}
