using Microsoft.AspNetCore.Identity;

namespace WebAPIAutores.Entidades
{
    public class Usuario: IdentityUser
    {
        public bool NoPaga { get; set; }
    }
}
