namespace Ficha1_P1_V1.Models
{
    public class Empresa
    {
	    public int EmpresaId { get; set; }
        public string Nome { get; set; }
        public string Localidade { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        
        //public static int TrabalhadorID { get; set; } = 1;
        //public void AddTrabalhador() { TrabalhadorID++; }
    }
}
