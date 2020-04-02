using System;
using System.Text;

namespace Methods
{
    /// <summary>
    /// Metoder för att randoma fram namn, ssn och symptomlevel.
    /// </summary>
    public class Randomers
    {
        public static string GenerateName()
        {
            Random r = new Random();
            int len = r.Next(5, 10);
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "t", "v", "w", "x" };
            string[] vowels = { "a", "e", "i", "o", "u", "y" };
            string Name = "";
            Name += consonants[r.Next(consonants.Length)].ToUpper();
            Name += vowels[r.Next(vowels.Length)];
            int b = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
            while (b < len)
            {
                Name += consonants[r.Next(consonants.Length)];
                b++;
                Name += vowels[r.Next(vowels.Length)];
                b++;
            }

            return Name;
        }
        public static string GenerateSSN()
        {
            StringBuilder sr = new StringBuilder();
            Random r = new Random();
            int year = r.Next(1940, 2020);
            sr.Append(year);
            int month = r.Next(1, 12);
            if (month < 10)
            {
                sr.Append("0");
            }
            sr.Append(month);
            int day = r.Next(1, 28);
            if (day < 10)
            {
                sr.Append("0");
            }
            sr.Append(day);
            sr.Append("-");
            int lastFour = r.Next(1000, 9999);
            sr.Append(lastFour);

            return sr.ToString();
        }
        public static int GenerateSymptomLevel()
        {
            Random r = new Random();
            int level = r.Next(1, 9);
            return level;
        }
    }
}
