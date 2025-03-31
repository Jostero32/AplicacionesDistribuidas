using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi_Persona.Context;
using WebApi_Persona.Models;

namespace WebApi_Persona.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly ILogger<PedidosController> logger;

        public PedidosController(AppDBContext context, ILogger<PedidosController> logger)
        {
            _context = context;
            this.logger = logger;
        }

        // GET: api/Pedidos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
        {
            logger.LogInformation("Obteniendo todos los pedidos");
            return await _context.Pedidos.ToListAsync();
        }

        // GET: api/Pedidos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> GetPedido(int id)
        {
            logger.LogInformation("Buscando pedido con ID: {Id}", id);
            var pedido = await _context.Pedidos.FindAsync(id);

            if (pedido == null)
            {
               logger.LogWarning("Pedido con ID {Id} no encontrado", id);
                return NotFound();
            }

            return pedido;
        }

        // PUT: api/Pedidos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedido(int id, Pedido pedido)
        {
            if (id != pedido.Id)
            {
                logger.LogWarning("ID mismatch: URL ID {Id} vs Body ID {PedidoId}", id, pedido.Id);
                return BadRequest();
            }

            _context.Entry(pedido).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                logger.LogInformation("Pedido con ID {Id} actualizado correctamente", id);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!PedidoExists(id))
                {
                    logger.LogWarning(ex, "Error de concurrencia: Pedido con ID {Id} no existe", id);
                    return NotFound();
                }
                else
                {
                    logger.LogError(ex, "Error de concurrencia al actualizar pedido con ID {Id}", id);
                    throw;
                }
            }

            return NoContent();
        }


        // POST: api/Pedidos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Pedido>> PostPedido(Pedido pedido)
        {
            logger.LogInformation("Creando nuevo pedido con ID: {Id}", pedido.Id);

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();
            logger.LogInformation("Pedido creado exitosamente con ID: {Id}", pedido.Id);

            return CreatedAtAction("GetPedido", new { id = pedido.Id }, pedido);
        }

        // DELETE: api/Pedidos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedido(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                logger.LogWarning("No se encontró el pedido con ID {Id} para eliminar", id);

                return NotFound();
            }

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();
            logger.LogInformation("Pedido con ID {Id} eliminado correctamente", id);

            return NoContent();
        }

        private bool PedidoExists(int id)
        {
            return _context.Pedidos.Any(e => e.Id == id);
        }
    }
}
