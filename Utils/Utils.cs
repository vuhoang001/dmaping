using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace InvoiceHub.Utils;

public class Utils
{
    private class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }

    public static string SerializeToXml<T>(T data)
    {
        var       serializer = new XmlSerializer(typeof(T));
        using var sw         = new Utf8StringWriter();
        serializer.Serialize(sw, data);
        return sw.ToString();
    }

    public static long GenerateCode(int id)
    {
        var date   = DateTime.Now;
        var result = int.Parse($"{date.Day}{date.Month}{date.Year} + {id}");
        return result;
    }


    public static bool IsPhoneNumber(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        phoneNumber = phoneNumber.Trim();

        // Chuẩn VN:
        // 0xxxxxxxxx (10 số)
        // +84xxxxxxxxx
        var regex = new Regex(@"^(0\d{9}|\+84\d{9})$");

        return regex.IsMatch(phoneNumber);
    }
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
    }

    public static string NumberToText(decimal inputNumber, bool suffix = true)
    {
        if (inputNumber == 0) return "Không đồng";
        var unitNumbers = new string[]
            { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
        string[] placeValues = new string[] { "", "nghìn", "triệu", "tỷ" };
        bool     isNegative  = false;

        // -12345678.3445435 => "-12345678"
        string sNumber = inputNumber.ToString("#");
        double number  = Convert.ToDouble(sNumber);
        if (number < 0)
        {
            number     = -number;
            sNumber    = number.ToString();
            isNegative = true;
        }


        int ones, tens, hundreds;

        int positionDigit = sNumber.Length; // last -> first

        string result = " ";


        if (positionDigit == 0)
            result = unitNumbers[0] + result;
        else
        {
            int placeValue = 0;

            while (positionDigit > 0)
            {
                // Check last 3 digits remain ### (hundreds tens ones)
                tens = hundreds = -1;
                ones = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                positionDigit--;
                if (positionDigit > 0)
                {
                    tens = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                    positionDigit--;
                    if (positionDigit > 0)
                    {
                        hundreds = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                        positionDigit--;
                    }
                }

                if ((ones > 0) || (tens > 0) || (hundreds > 0) || (placeValue == 3))
                    result = placeValues[placeValue] + result;

                placeValue++;
                if (placeValue > 3) placeValue = 1;

                if ((ones == 1) && (tens > 1))
                    result = "một " + result;
                else
                {
                    if ((ones == 5) && (tens > 0))
                        result = "lăm " + result;
                    else if (ones > 0)
                        result = unitNumbers[ones] + " " + result;
                }

                if (tens < 0)
                    break;
                else
                {
                    if ((tens == 0) && (ones > 0)) result = "lẻ "             + result;
                    if (tens == 1) result                 = "mười "           + result;
                    if (tens > 1) result                  = unitNumbers[tens] + " mươi " + result;
                }

                if (hundreds < 0) break;
                else
                {
                    if ((hundreds > 0) || (tens > 0) || (ones > 0))
                        result = unitNumbers[hundreds] + " trăm " + result;
                }

                result = " " + result;
            }
        }

        result = result.Trim();
        if (isNegative) result = "Âm " + result;
        return char.ToUpper(result[0]) + result.Substring(1) + (suffix ? " đồng chẵn" : "");
    }
}