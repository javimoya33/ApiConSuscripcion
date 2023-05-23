namespace WebAPIAutores.DTOs
{
    public class LimitarPeticionesConfiguracion
    {
        public int PeticionesPorDiaConLlaveGratuita { get; set; }
        public string[] ListaBlancaRutas { get; set; }
    }
}
