using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Auth.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        [HttpGet("agenda-por-tipo")]
        public IActionResult GetAgendaByType([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            // Simulação de dados de compromissos por tipo
            var data = new List<object>
            {
                new { Tipo = "Família", Quantidade = 5 },
                new { Tipo = "Lazer", Quantidade = 8 },
                new { Tipo = "Trabalho", Quantidade = 3 },
                new { Tipo = "Estudo", Quantidade = 4 }
            };

            return Ok(new
            {
                startDate,
                endDate,
                data
            });
        }

        [HttpGet("agenda-por-prioridade")]
        public IActionResult GetAgendaByPriority([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            // Simulação de dados de compromissos por prioridade
            var data = new List<object>
            {
                new { Prioridade = "Alta", Quantidade = 6 },
                new { Prioridade = "Média", Quantidade = 7 },
                new { Prioridade = "Baixa", Quantidade = 4 }
            };

            return Ok(new
            {
                startDate,
                endDate,
                data
            });
        }
    }
}
