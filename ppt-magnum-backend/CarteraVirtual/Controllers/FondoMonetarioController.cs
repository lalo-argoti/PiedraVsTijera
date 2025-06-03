using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pdt.Data;
using pdt.Models;
using System.Security.Claims;
using System;


namespace pdt.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FondoMonetarioController : ControllerBase
    {
        //var random = new Random();
        //var codigoGenerado = DateTime.Now.ToString("ddMMyyHHmmssfff") + random.Next(100, 1000).ToString();

        private readonly AppDbContext _context;
        private readonly Random _random = new Random();
        public FondoMonetarioController(AppDbContext context)
        {
            _context = context;
        }

        //private string GenerarCodigo()
        //{
        //    return DateTime.Now.ToString("ddMMyyHHmmssfff") + _random.Next(100, 1000).ToString();
        //}

        private int GetUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            int userId = GetUserId();

            var fondos = _context.FondosMonetarios
                .Where(f => f.Propietario == userId)
                .Select(f => new FondoMonetarioDto
                {
                    Id = f.Id,
                    Nombre = f.Nombre,
                    Tipo = f.Tipo,
                    CapitalCOP = f.CapitalCOP,
                    CapitalUSD = f.CapitalUSD
                })
                .ToList();

            return Ok(fondos);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            int userId = GetUserId();

            var fondo = _context.FondosMonetarios
                .Where(f => f.Id == id && f.Propietario == userId)
                .Select(f => new FondoMonetarioDto
                {
                    Id = f.Id,
                    Nombre = f.Nombre,
                    Tipo = f.Tipo,
                    CapitalCOP = f.CapitalCOP,
                    CapitalUSD = f.CapitalUSD
                })
                .FirstOrDefault();

            if (fondo == null)
                return NotFound();

            return Ok(fondo);
        }

        [HttpPost]
        public IActionResult Create([FromBody] FondoMonetarioDto model)
        {
            int userId = GetUserId();

            
            var random = new Random();
            var codigoGenerado = DateTime.Now.ToString("ddMMyyHHmmssfff") + random.Next(100, 999).ToString();

            var entity = new FondoMonetario
            {  
                Id = codigoGenerado,    
                
                Nombre = model.Nombre,
                Tipo = model.Tipo,
                CapitalCOP = model.CapitalCOP,
                CapitalUSD = model.CapitalUSD,
                Propietario = userId
            };

            _context.FondosMonetarios.Add(entity);
            _context.SaveChanges();

            var dto = new FondoMonetarioDto
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Tipo = entity.Tipo,
                CapitalCOP = entity.CapitalCOP,
                CapitalUSD = entity.CapitalUSD
            };

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, dto);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] FondoMonetarioDto model)
        {
            int userId = GetUserId();

            var existente = _context.FondosMonetarios
                .FirstOrDefault(f => f.Id == id && f.Propietario == userId);

            if (existente == null)
                return NotFound();

            existente.Nombre = model.Nombre;
            existente.Tipo = model.Tipo;
            existente.CapitalCOP = model.CapitalCOP;
            existente.CapitalUSD = model.CapitalUSD;

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            int userId = GetUserId();

            var fondo = _context.FondosMonetarios
                .FirstOrDefault(f => f.Id == id && f.Propietario == userId);

            if (fondo == null)
                return NotFound();

            _context.FondosMonetarios.Remove(fondo);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
