using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ManejoPresupuesto.validaciones;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Models
{
    public class TipoCuenta //:IValidatableObject
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [StringLength(maximumLength:50, MinimumLength=3, ErrorMessage ="La longitud del campo {0} debe ser entre {2}-{1} caracteres")]
        [PrimeraLetraMayuscula] // custom validator
        [Remote(action:"VerificarExisteTipoCuenta", controller:"TiposCuentas")]
        public string Nombre { get; set; }
        
        public int UsuarioId { get; set; }
        
        public int Orden { get; set; }

        //* validacion a nivel de modelo
        // public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        // {
        //     if(Nombre!= null && Nombre.Length > 0){
        //         var primeraLetra = Nombre[0].ToString();

        //         if (primeraLetra != primeraLetra.ToUpper())
        //         {
        //             yield return new ValidationResult(
        //                 "La primera letra debe ser mayúscula",
        //                 new[]{nameof(Nombre)}
        //             );
        //         }
        //     }
            
        // }

        // * atributos de ejemplo para validaciones
        // [Required(ErrorMessage ="El campo {0} es requerido")]
        // [EmailAddress(ErrorMessage = "el campo debe tener formato email valido")]
        // public string Email { get; set; }

        // [Range(minimum:18, maximum:130,ErrorMessage ="El valor debe estar entre {1} y {2}")]
        // public string Edad { get; set; }

        // [Url(ErrorMessage ="el campo debe ser una url valida")]
        // public string URL { get; set; }

        // [CreditCard(ErrorMessage ="la tarjeta de credito no es valida :(")]
        // [Display(Name ="Tarjeta de Credito")]
        // public string TarjetaCredito { get; set; }
    }
}