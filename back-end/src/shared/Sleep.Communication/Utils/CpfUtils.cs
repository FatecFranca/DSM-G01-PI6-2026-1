using System.Text.RegularExpressions;

namespace Sleep.Communication.Utils
{
    public static class CpfUtils
    {
        public static bool IsValid(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            string formatedCpf = Regex.Replace(cpf, @"\D", string.Empty);

            if (formatedCpf.Length != 11)
                return false;

            if (HasAllSameDigits(formatedCpf))
                return false;

            return ValidateCheckDigits(formatedCpf);
        }

        public static string Format(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return string.Empty;

            string cleaned = Regex.Replace(cpf, @"\D", string.Empty);

            if (cleaned.Length != 11)
                return cpf;

            return $"{cleaned.Substring(0, 3)}.{cleaned.Substring(3, 3)}.{cleaned.Substring(6, 3)}-{cleaned.Substring(9, 2)}";
        }

        public static string RemoveFormat(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return string.Empty;

            return Regex.Replace(cpf, @"\D", string.Empty);
        }

        private static bool HasAllSameDigits(string cpf)
        {
            for (int i = 1; i < cpf.Length; i++)
            {
                if (cpf[i] != cpf[0])
                    return false;
            }
            return true;
        }

        private static bool ValidateCheckDigits(string cpf)
        {
            int sum = 0;
            for (int i = 0; i < 9; i++)
                sum += int.Parse(cpf[i].ToString()) * (10 - i);

            int remainder = sum % 11;
            int digit1 = remainder < 2 ? 0 : 11 - remainder;

            sum = 0;
            for (int i = 0; i < 10; i++)
                sum += int.Parse(cpf[i].ToString()) * (11 - i);

            remainder = sum % 11;
            int digit2 = remainder < 2 ? 0 : 11 - remainder;

            return cpf[9] == digit1.ToString()[0] &&
                   cpf[10] == digit2.ToString()[0];
        }
    }
}
