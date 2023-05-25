using AutoMapper.Features;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entidades;

namespace WebAPIAutores.Middlewares
{
    public static class LimitarPeticionesMiddlewareExtensions
    {
        public static IApplicationBuilder UseLimitarPeticiones(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LimitarPeticionesMiddleware>();
        }
    }

    public class LimitarPeticionesMiddleware
    {
        private readonly RequestDelegate siguiente;
        private readonly IConfiguration configuration;

        public LimitarPeticionesMiddleware(RequestDelegate siguiente, IConfiguration configuration)
        {
            this.siguiente = siguiente;
            this.configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext context)
        {
            var limitarPeticionesConfiguracion = new LimitarPeticionesConfiguracion();
            configuration.GetRequiredSection("LimitarPeticiones").Bind(limitarPeticionesConfiguracion);

            var ruta = httpContext.Request.Path.ToString();
            var estaLaRutaEnListaBlanca = limitarPeticionesConfiguracion.ListaBlancaRutas.Any(x => ruta.Contains(x));

            if (estaLaRutaEnListaBlanca)
            {
                await siguiente(httpContext);
                return;
            }

            var llaveStringValues = httpContext.Request.Headers["X-Api-Key"];

            if (llaveStringValues.Count == 0)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Debe proveer la llave en la cabecera X-Api-Key");

                return;
            }

            if (llaveStringValues.Count > 1)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Solo una llave debe estar presente");

                return;
            }

            var llave = llaveStringValues[0];

            var llaveDB = await context.LlavesApi
                .Include(x => x.RestriccionesDominio)
                .Include(x => x.RestriccionesIP)
                .Include(x => x.Usuario)
                .FirstOrDefaultAsync(x => x.Llave == llave);

            if (llaveDB == null)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("La llave no existe");

                return;
            }

            if (!llaveDB.Activa)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("La llave se encuentra inactiva");

                return;
            }

            if (llaveDB.TipoLlave == Entidades.TipoLlave.Gratuita)
            {
                var hoy = DateTime.Today;
                var mañana = hoy.AddDays(1);
                var cantidadPeticionesRealizadasHoy =
                    await context.Peticiones.CountAsync(x => x.LlaveId == llaveDB.Id
                    && x.FechaPeticion >= hoy && x.FechaPeticion < mañana);

                if (cantidadPeticionesRealizadasHoy >= limitarPeticionesConfiguracion.PeticionesPorDiaConLlaveGratuita)
                {
                    httpContext.Response.StatusCode = 429; // Demasiadas peticiones
                    await httpContext.Response.WriteAsync("Ha excedido el límite de peticiones por día. Si desea realizar más " +
                        "peticiones actualice su suscripción a una cuenta profesional");

                    return;
                }
            }
            else if (llaveDB.Usuario.NoPaga)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Debes pagar tu suscripción mensual para continuar.");
                return;
            }

            /*var superaRestricciones = PeticionSuperaAlgunaDeLasRestricciones(llaveDB, httpContext);

            if (!superaRestricciones)
            {
                httpContext.Response.StatusCode = 403;
                return;
            }*/

            var peticion = new Peticion
            {
                LlaveId = llaveDB.Id,
                FechaPeticion = DateTime.UtcNow
            };
            context.Add(peticion);
            await context.SaveChangesAsync();

            await siguiente(httpContext);
        }

        private bool PeticionSuperaAlgunaDeLasRestricciones(LlaveAPI llaveAPI, HttpContext context)
        {
            var hayRestricciones = llaveAPI.RestriccionesDominio.Any() || llaveAPI.RestriccionesIP.Any();

            if (!hayRestricciones)
            {
                return true;
            }

            var peticionSuperaLasRestriccionesDeDominio =
                PeticionSuperaLasRestriccionesDeDominio(llaveAPI.RestriccionesDominio, context);

            var peticionSuperaLasRestriccionesDeIP =
                PeticionSuperaLasRestriccionesDeIP(llaveAPI.RestriccionesIP, context);

            return peticionSuperaLasRestriccionesDeDominio || peticionSuperaLasRestriccionesDeIP;
        }

        private bool PeticionSuperaLasRestriccionesDeDominio(List<RestriccionDominio> restricciones, HttpContext context)
        {
            if (restricciones == null || restricciones.Count == 0)
            {
                return false;
            }

            // De qué dominio viene la petición
            var referer = context.Request.Headers["Referer"].ToString();

            if (referer == string.Empty)
            {
                return false;
            }

            Uri myUri = new Uri(referer);  
            string host = myUri.Host;

            var superaRestriccion = restricciones.Any(x => x.Dominio == host);

            return superaRestriccion;
        }

        private bool PeticionSuperaLasRestriccionesDeIP(List<RestriccionIP> restricciones, HttpContext context)
        {
            if (restricciones == null || restricciones.Count == 0)
            {
                return false;
            }

            var IP = context.Connection.RemoteIpAddress.ToString();

            if (IP == string.Empty)
            {
                return false;
            }

            var superaRestriccion = restricciones.Any(x => x.IP == IP);

            return superaRestriccion;
        }
    }
}
