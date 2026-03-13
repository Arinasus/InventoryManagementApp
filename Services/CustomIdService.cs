using InventoryManagementApp.Model;
using InventoryManagementApp.Models;
using System.Globalization;

namespace InventoryManagementApp.Services
{
    public class CustomIdService
    {
        public bool Validate(string customId, Inventory inventory)
        {
            var parts = inventory.CustomIdParts
                .OrderBy(p => p.Order)
                .ToList();

            string s = customId;

            foreach (var part in parts)
            {
                switch (part.Type)
                {
                    case CustomIdPartType.FixedText:
                        if (!MatchFixedText(ref s, part.Value))
                            return false;
                        break;

                    case CustomIdPartType.Random20Bit:
                        if (!MatchDigits(ref s, 6))
                            return false;
                        break;

                    case CustomIdPartType.Random32Bit:
                        if (!MatchDigits(ref s, 10))
                            return false;
                        break;

                    case CustomIdPartType.Random6Digits:
                        if (!MatchDigits(ref s, 6))
                            return false;
                        break;

                    case CustomIdPartType.Random9Digits:
                        if (!MatchDigits(ref s, 9))
                            return false;
                        break;

                    case CustomIdPartType.Guid:
                        if (!MatchGuid(ref s))
                            return false;
                        break;

                    case CustomIdPartType.DateTime:
                        if (!MatchDateTime(ref s))
                            return false;
                        break;

                    case CustomIdPartType.Sequence:
                        if (!MatchSequence(ref s))
                            return false;
                        break;

                    default:
                        return false;
                }
            }

            return s.Length == 0;
        }

        private bool MatchFixedText(ref string s, string expected)
        {
            if (string.IsNullOrEmpty(expected))
                return false;

            if (!s.StartsWith(expected))
                return false;

            s = s.Substring(expected.Length);
            return true;
        }

        private bool MatchDigits(ref string s, int length)
        {
            if (s.Length < length)
                return false;

            var segment = s.Substring(0, length);

            if (!segment.All(char.IsDigit))
                return false;

            s = s.Substring(length);
            return true;
        }

        private bool MatchGuid(ref string s)
        {
            if (s.Length < 36)
                return false;

            var segment = s.Substring(0, 36);

            if (!Guid.TryParse(segment, out _))
                return false;

            s = s.Substring(36);
            return true;
        }

        private bool MatchDateTime(ref string s)
        {
            if (s.Length < 14)
                return false;

            var segment = s.Substring(0, 14);

            if (!DateTime.TryParseExact(segment, "yyyyMMddHHmmss", null,
                DateTimeStyles.None, out _))
                return false;

            s = s.Substring(14);
            return true;
        }

        private bool MatchSequence(ref string s)
        {
            int len = 0;

            while (len < s.Length && char.IsDigit(s[len]))
                len++;

            if (len == 0)
                return false;

            s = s.Substring(len);
            return true;
        }
    }
}
