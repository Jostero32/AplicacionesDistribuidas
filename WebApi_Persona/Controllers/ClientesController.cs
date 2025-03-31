using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi_Persona.Context;
using WebApi_Persona.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace WebApi_Persona.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(AppDBContext context, ILogger<ClientesController> logger)
        {
            _context = context;
            _logger = logger;
            _logger.LogInformation("ClientesController inicializado");
        }

        // GET: api/Clientes
        [HttpGet]
        
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            _logger.LogInformation("Iniciando solicitud GET para todos los clientes");
            try
            {
                var clientes = await _context.Clientes.ToListAsync();
                _logger.LogInformation("Retornando {Count} clientes", clientes.Count);
                return clientes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener clientes");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // GET: api/Clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            _logger.LogDebug("Buscando cliente con ID: {Id}", id);

            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                _logger.LogWarning("Cliente con ID {Id} no encontrado", id);
                return NotFound();
            }

            _logger.LogInformation("Cliente encontrado: {@Cliente}", cliente);
            return cliente;
        }

        // PUT: api/Clientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, Cliente cliente)
        {
            _logger.LogInformation("Iniciando actualización para cliente ID: {Id}", id);

            if (id != cliente.Id)
            {
                _logger.LogWarning("Discrepancia de ID: URL {UrlId} vs Body {BodyId}", id, cliente.Id);
                return BadRequest();
            }

            _context.Entry(cliente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Cliente ID {Id} actualizado exitosamente", id);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ClienteExists(id))
                {
                    _logger.LogWarning(ex, "Error de concurrencia: Cliente ID {Id} no existe", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Error de concurrencia al actualizar cliente ID {Id}", id);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar cliente ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // POST: api/Clientes
        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
        {
            _logger.LogInformation("Creando nuevo cliente: {@Cliente}", cliente);

            try
            {
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Cliente creado exitosamente con ID: {Id}", cliente.Id);
                return CreatedAtAction("GetCliente", new { id = cliente.Id }, cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear cliente");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // DELETE: api/Clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            _logger.LogInformation("Eliminando cliente ID: {Id}", id);

            try
            {
                var cliente = await _context.Clientes.FindAsync(id);
                if (cliente == null)
                {
                    _logger.LogWarning("Cliente ID {Id} no encontrado para eliminar", id);
                    return NotFound();
                }

                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Cliente ID {Id} eliminado exitosamente", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar cliente ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }
    }
}