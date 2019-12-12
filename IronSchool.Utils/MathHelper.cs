using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronSchool.Utils
{
    public class MathHelper
    {
        public static Decimal GetDecimalFromNullable(Decimal? value)
        {
            return Convert.ToDecimal(value == null ? 0 : value);
        }
        public static Decimal GetDecimalFromNullable(Double? value)
        {
            return Convert.ToDecimal(value == null ? 0 : value);
        }

        public static Decimal Round(Decimal? value, int positions)
        {
            if (value == null)
                return 0;
            else
                return System.Math.Round(Convert.ToDecimal(value), positions, MidpointRounding.AwayFromZero);
        }

        public static decimal ParseNumberFromString(string text)
        {
            decimal aux = 0;
            Decimal.TryParse(text, out aux);
            return aux;
        }
    }
}
