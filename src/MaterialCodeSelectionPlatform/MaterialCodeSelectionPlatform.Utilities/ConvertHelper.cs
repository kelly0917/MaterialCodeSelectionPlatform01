using System;

namespace MaterialCodeSelectionPlatform.Utilities
{
    public class ConvertHelper
    {
        public static float? ConvertFloatPointData(double designQty, double? allowance, int? roundUpDigit)
        {
            if (roundUpDigit.HasValue && allowance.HasValue)
            {
                float d = (float)(1.0 / Math.Pow(10, roundUpDigit.Value));
                float oldValue = (float)(designQty * allowance.Value);
                var result  = (float)Math.Round(oldValue, roundUpDigit.Value);

                if (result < oldValue)
                {
                    result += d;
                }

                return result;
            }

            return null;
        }
    }
}