using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Security.Cryptography;
using System.Diagnostics.CodeAnalysis;

namespace TextComparer
{

    class TextComparer
    {

        public class TextFieldComparer : IEqualityComparer<Shingle>
        {

            public bool Equals(Shingle x, Shingle y)
            {

                return x.text == y.text;
            
            }

            public int GetHashCode([DisallowNull] Shingle obj)
            {

                return obj.text.GetHashCode();
            
            }

        }

        public class Shingle
        {
            
            private static string[] marks = new string[] {
                ",", ".", "!", "-", "–", "_", "/", "\"", "'", "=", "+", ")",
                "(", "*", "&", "?", "^", ":", ";", "%", "$", "#", "№", "@",
                "`", "~", "<", ">", "}", "{", "|", "«", "»", "\n", "\r", "…",
                "no", "$", "#", "№"
            };

            private static int shingleLen = 3;

            public HashSet<int> ids { get; set; }

            public string text { get; set; }

            public Shingle(HashSet<int> ids, string str)
            {

                this.ids = ids;
                this.text = str;

            }

            public Shingle(int id, string str)
            {

                if (this.ids == null)
                    this.ids = new HashSet<int>();

                this.ids.Add(id);
                this.text = str;

            }

            public static void Canonization(ref string text)
            {

                text = text.ToLower();

                foreach (string m in marks)
                    text = text.Replace(m, " ");

            }

            public static List<Shingle> GetList(Dictionary<int, string> parts, bool hash = false)
            {

                MD5 md5Hash = MD5.Create();

                List<Shingle> shingles = new List<Shingle>();

                for (int i = 0; i < parts.Count; i++)
                {

                    HashSet<int> keys = new HashSet<int>();

                    string shingle = string.Empty;

                    for (int j = 0; j < shingleLen; j++)
                    {

                        if (i + j < parts.Count)
                        {

                            KeyValuePair<int, string> p = parts.ElementAt(i + j);

                            string word = p.Value;

                            Canonization(ref word);

                            shingle += word.Trim() + " ";
                                                        
                            keys.Add(p.Key);

                        }

                    }

                    shingle = shingle.Trim();

                    if (hash)
                    {

                        byte[] bytes = Encoding.UTF8.GetBytes(shingle);
                        byte[] hashBytes = md5Hash.ComputeHash(bytes);

                        shingle = BitConverter.ToString(hashBytes).Replace("-", "");

                    }

                    shingles.Add(new Shingle(keys, shingle));

                }

                return shingles;

            }

        }

        public static Dictionary<int, string> GetParts(string text, int beginkey = 0)
        {

            Dictionary<int, string> words = new Dictionary<int, string>();

            Regex regex = new Regex(@"\w*\w\W{0,}", RegexOptions.CultureInvariant | RegexOptions.Multiline);

            MatchCollection matches = regex.Matches(text);

            foreach(Match m in matches)
            {

                words.Add(beginkey, m.Value);
                
                beginkey++;
            
            }

            return words;

        }

        public static HashSet<int> GetMatched(List<Shingle> src, List<Shingle> cmp)
        {

            HashSet<int> keys = src.Intersect(cmp, new TextFieldComparer())
                .SelectMany(t => t.ids)
                .ToHashSet();

            return keys;

        }

    }

}
