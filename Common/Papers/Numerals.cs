using System.Text;
using Scholar.Enums;

namespace Scholar.Common.Papers
{
    /// <summary>Formats question/section numbers per the institute's numeral style.</summary>
    public static class Numerals
    {
        public static string Format(int number, NumeralStyle style) =>
            style == NumeralStyle.Roman ? ToRoman(number) : number.ToString();

        private static string ToRoman(int number)
        {
            if (number <= 0)
            {
                return number.ToString();
            }

            (int Value, string Symbol)[] map =
            {
                (1000, "M"), (900, "CM"), (500, "D"), (400, "CD"),
                (100, "C"), (90, "XC"), (50, "L"), (40, "XL"),
                (10, "X"), (9, "IX"), (5, "V"), (4, "IV"), (1, "I")
            };

            StringBuilder sb = new();
            foreach ((int value, string symbol) in map)
            {
                while (number >= value)
                {
                    sb.Append(symbol);
                    number -= value;
                }
            }
            return sb.ToString();
        }
    }
}
