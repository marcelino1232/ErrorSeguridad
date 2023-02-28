﻿using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.GlobalValidation
{
    public class PrimeraLetraMayusculaAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }      
            
            var PrimeraLetra = value.ToString()[0].ToString();

            if(PrimeraLetra != PrimeraLetra.ToUpper())
            {
                return new ValidationResult("La primera letra debe ser mayuscula");
            }

            return ValidationResult.Success;

        }
    }
}
