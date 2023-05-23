using WebAPIAutores.Entidades;

namespace WebAPIAutores.DTOs
{
    public class ActualizarLlaveDTO
    {
        public int LlaveId { get; set; }
        public bool Activa { get; set; }
        public bool ActualizarLlave { get; set; }
    }
}
