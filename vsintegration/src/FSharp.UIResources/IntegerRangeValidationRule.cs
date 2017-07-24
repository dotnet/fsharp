
using System.Globalization;
using System.Windows.Controls;

namespace Microsoft.VisualStudio.FSharp.UIResources
{

    public class IntegerRangeValidationRule : ValidationRule
    {
        public IntegerRangeValidationRule()
            : base(ValidationStep.RawProposedValue, true)
        {
        }

        public int Min { get; set; }

        public int Max { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string)
            {
                var text = (string)value;
                int i = 0;
                if (int.TryParse(text, out i) &&
                    i >= Min && i <= Max)
                {
                    return ValidationResult.ValidResult;
                }
            }

            return new ValidationResult(false, $"Expected a number between {Min} and {Max}");
        }
    }
}
