using Ficha1_P1_V1.Models;

namespace Ficha1_P1_V1.ViewModels
{
	public class FuncionarioViewModel
	{
		//Contem arrendamento que contem a respetiva habitação e o respetivo cliente
		public ApplicationUser Funcionario { get; set; }

		public List<Habitacao> Habitacao { get; set; }
	}
}
