import os

# Define los controladores con nombre y modelo asociado (solo nombre para este script)
controllers = [
    "UserController",
    "UserGroupController",
    "GastoTipoController",
    "FondoMonetarioController",
    "PresupuestoMovimientoController",
    "GastoRegistroController",
    "GastoDetalleController",
    "DepositoController",
    "MovimientosController",
    "ReporteController"
]

# Carpeta destino donde se crean los archivos
output_folder = "GeneratedControllers"
os.makedirs(output_folder, exist_ok=True)

# Plantilla básica para cada controlador
controller_template = """
using Microsoft.AspNetCore.Mvc;
using pdt.Models;
using pdt.Data;

namespace pdt.Controllers
{{
    [ApiController]
    [Route("api/[controller]")]
    public class {controller_name} : ControllerBase
    {{
        private readonly AppDbContext _context;

        public {controller_name}(AppDbContext context)
        {{
            _context = context;
        }}

        // GET: api/{controller_name}
        [HttpGet]
        public IActionResult GetAll()
        {{
            // TODO: Implementar lógica para obtener todos los registros
            return Ok();
        }}

        // GET: api/{controller_name}/5
        [HttpGet("{{id}}")]
        public IActionResult GetById(int id)
        {{
            // TODO: Implementar lógica para obtener un registro por id
            return Ok();
        }}

        // POST: api/{controller_name}
        [HttpPost]
        public IActionResult Create([FromBody] object model)
        {{
            // TODO: Implementar lógica para crear un registro
            return CreatedAtAction(nameof(GetById), new {{ id = 0 }}, model);
        }}

        // PUT: api/{controller_name}/5
        [HttpPut("{{id}}")]
        public IActionResult Update(int id, [FromBody] object model)
        {{
            // TODO: Implementar lógica para actualizar un registro
            return NoContent();
        }}

        // DELETE: api/{controller_name}/5
        [HttpDelete("{{id}}")]
        public IActionResult Delete(int id)
        {{
            // TODO: Implementar lógica para eliminar un registro
            return NoContent();
        }}
    }}
}}
"""

for ctrl in controllers:
    file_path = os.path.join(output_folder, f"{ctrl}.cs")
    with open(file_path, "w", encoding="utf-8") as f:
        f.write(controller_template.format(controller_name=ctrl))

print(f"Se generaron {len(controllers)} controladores en la carpeta '{output_folder}'.")
