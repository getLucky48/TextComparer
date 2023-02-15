using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TextComparer
{

    class Program
    {

        static void Print(string text, bool select = false)
        {

            if (select)
                Console.ForegroundColor = ConsoleColor.Green;

            Console.Write(text);

            Console.ResetColor();

        }

        static void Main(string[] args)
        {

            string srcText = Data.srcText;
            string cmpText = Data.cmpText;

            Dictionary<int, string> srcParts = TextComparer.GetParts(srcText);
            Dictionary<int, string> cmpParts = TextComparer.GetParts(cmpText, srcParts.Count);

            List<TextComparer.Shingle> srcShingles = TextComparer.Shingle.GetList(srcParts, true);
            List<TextComparer.Shingle> cmpShingles = TextComparer.Shingle.GetList(cmpParts, true);

            HashSet<int> srcMatchedKeys = TextComparer.GetMatched(srcShingles, cmpShingles);
            HashSet<int> cmpMatchedKeys = TextComparer.GetMatched(cmpShingles, srcShingles);

            foreach(KeyValuePair<int, string> pair in srcParts)
            {

                Print(pair.Value, srcMatchedKeys.Contains(pair.Key));

            }

            Console.WriteLine();

            foreach (KeyValuePair<int, string> pair in cmpParts)
            {

                Print(pair.Value, cmpMatchedKeys.Contains(pair.Key));

            }

        }

    }

}
