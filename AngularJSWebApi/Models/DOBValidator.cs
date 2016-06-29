using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AngularJSWebApi.Models
{
    public class DOBValidator:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var date1 = DateTime.Now;
            var date2 = Convert.ToDateTime(value);
            int yy = date1.Year - date2.Year;
            int mm = date1.Month - date2.Month;
            int dd = date1.Day - date2.Day;
            if(yy>=18 && yy<=100)
            {

                return ValidationResult.Success;       
                 
            }
            else
            {
                return new ValidationResult("*Age Should be in between 18-100");
            }
        }
    }
}