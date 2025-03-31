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
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly ILogger<UsuariosController> logger;
        private readonly IConfiguration configuration;

        public UsuariosController(AppDBContext context,ILogger<UsuariosController> logger,IConfiguration configuration)
        {
            _context = context;
            this.logger = logger;
            this.configuration = configuration;
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                logger.LogError("No se encontro el usuario");
                return NotFound();
            }

            return usuario;
        }

        // POST: api/Usuarios
        [Route("register")]
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            logger.LogInformation("Usuario con email "+usuario.Email+" creado");
            return CreatedAtAction("GetUsuario", new { id = usuario.Id }, usuario);
        }

        [Route("login")]
        [HttpPost]
        public async Task<ActionResult<string>> PostLogin(Usuario usuario)
        {
            logger.LogInformation("Iniciando sesion");
            var usuarioBuscar = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == usuario.Email);
            if (usuarioBuscar==null)
            {
                logger.LogError("Usuario no Existe");
                return NotFound();
            }

            if (usuarioBuscar.Contraseña==usuario.Contraseña)
            {
                logger.LogInformation("Usuario Correcto Generando token");
                var tokenProvider = new TokenProvider(configuration);
                return tokenProvider.Create(usuario);
            }
            logger.LogError("Contraseña Incorrecta");
            return BadRequest();
            
        }

    }
}
