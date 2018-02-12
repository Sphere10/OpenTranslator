using System;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenTranslator.Utils
{
    public static class StringExtension
    {

        /// <summary>
        /// Delete file storaged, if exists
        /// </summary>
        /// <param name="filePath"></param>
        public static void DeleteFile(this String filePath)
        {
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
        }

        /// <summary>
        /// Replace the string that is related to Scape Codes
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
		public static string Unescape(this String str)
        {
            str = Regex.Replace(str, @"(^|[^\\])\\r", "$1\r"); //carriage return 
            str = Regex.Replace(str, @"(^|[^\\])\\n", "$1\n"); //new line
            str = Regex.Replace(str, @"(^|[^\\])\\t", "$1\t"); //tab

            str = str.Replace("\\\"", "\"");

            return str;
        }

        /// <summary>
        /// This evaluates text param and remove non alphanumerics characters from it
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveNonAlphanumerics(this String text)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            text = rgx.Replace(text, "");

            return text;
        }

        /// <summary>
        ///  Converts the phrase to specified convention.
        /// </summary>
        /// <param name="phrase"></param>
        /// <param name="cases"></param>
        /// <returns></returns>
        public static string ConvertCaseString(this String phrase, Case cases)
        {
            string[] splittedPhrase = phrase.Split(' ', '-', '.');
            var sb = new StringBuilder();

            if (cases == Case.CamelCase)
            {
                sb.Append(splittedPhrase[0].ToLower());
                splittedPhrase[0] = string.Empty;
            }
            else if (cases == Case.PascalCase)
                sb = new StringBuilder();

            foreach (String s in splittedPhrase)
            {
                char[] splittedPhraseChars = s.ToCharArray();
                if (splittedPhraseChars.Length > 0)
                {
                    splittedPhraseChars[0] = ((new String(splittedPhraseChars[0], 1)).ToUpper().ToCharArray())[0];
                }
                sb.Append(new String(splittedPhraseChars));
            }
            return sb.ToString();
        }

        public enum Case
        {
            PascalCase,
            CamelCase
        }
    }
}