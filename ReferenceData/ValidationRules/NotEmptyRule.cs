using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ReferenceData
{
    /// <summary>
    /// ValidationRule to check if input value is not empty
    /// </summary>
    public class NotEmptyRule : ValidationRule
    {
        /// <summary>
        /// Name of field to check
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if value is valid, otherwise False</returns>
        protected virtual bool IsValidValue(object value)
        {
            var isValid = value != null && !string.IsNullOrWhiteSpace(value.ToString());
            return isValid;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!IsValidValue(value))
            {
                return new ValidationResult(false, FieldName + " cannot be empty");
            }

            return new ValidationResult(true, null);
        }
    }

    /// <summary>
    /// ValidationRule to check if specified item has valid Id
    /// </summary>
    public class ValidIdRule : NotEmptyRule
    {
        protected override bool IsValidValue(object value)
        {
            var isValid = value is int && ((int) value) != -1;
            return isValid;
        }
    }
}
