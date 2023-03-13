using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Models
{
    public class CuentaCreacionViewModel: Cuenta
    {
        public IEnumerable<SelectListItem> TiposCuentas { get; set; }
    }
}

// SelectListItem:
// util para combobox, herramienta interna de .net