using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManejoPresupuesto.Servicios
{

    public interface IServicioUsuarios {
        public int ObtenerUsuarioId();
    }

    public class ServicioUsuarios: IServicioUsuarios
    {
        public int ObtenerUsuarioId(){
            return 1;
        }
    }
}