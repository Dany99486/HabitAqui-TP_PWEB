using Ficha1_P1_V1.Models;

namespace Ficha1_P1_V1.ViewModels
{
    public class GestorInfoViewModel
    {
        //Da empresa
        public Empresa? Empresa { get; set; }
        //Gestão de funcionários
        public List<ApplicationUser>? Funcionarios { get; set; }

        //Gestão de arrendamentos
        public List<Arrendamento>? Arrendamentos { get; set; }

        //Gestão do parque habitacional
        public List<Habitacao>? Habitacoes { get; set; }
    }
}
