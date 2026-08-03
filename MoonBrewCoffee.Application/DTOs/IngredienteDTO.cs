using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoonBrewCoffee.Application.DTOs
{
    public class IngredienteDTO
    {
        public int IdIngrediente { get; set; }

        public string Nombre { get; set; } = "";

        public string UnidadMedida { get; set; } = "";

        public bool Activo { get; set; }
    }
}