using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WebAPIAutores.DTOs;

namespace WebAPIAutores.Controllers
{
    [ApiController]
    [Route("api/facturas")]
    public class FacturasController: ControllerBase
    {
        private readonly ApplicationDbContext context;

        public FacturasController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Pagar(PagarFacturaDTO pagarFacturaDTO)
        {
            var facturaDB = await context.Facturas
                .Include(x => x.Usuario)
                .FirstOrDefaultAsync(x => x.Id == pagarFacturaDTO.FacturaId);

            if (facturaDB == null)
            {
                return NotFound();
            }

            if (facturaDB.Pagada)
            {
                return BadRequest("La factura ya fue saldada");
            }

            // Lógica para pagar la factura
            // Suponiendo que el pago ha pasado por una pasarela de pago y ha sido exitoso

            facturaDB.Pagada = true;
            await context.SaveChangesAsync();

            var tieneFacturasPendientesDePago =
                await context.Facturas
                    .AnyAsync(x => x.UsuarioId == facturaDB.UsuarioId && !x.Pagada && x.FechaLimiteDePago < DateTime.Today);

            if (!tieneFacturasPendientesDePago)
            {
                facturaDB.Usuario.NoPaga = false;
                await context.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}
