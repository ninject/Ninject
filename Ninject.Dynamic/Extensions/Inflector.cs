// Taken from Andrew Peters
// More info: http://andrewpeters.net/inflectornet/

#region Usings

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

#endregion

namespace Ninject.Dynamic.Extensions
{
    internal static class Inflector
    {
        #region Default Rules

        static Inflector()
        {
            AddPlural("$", "s");
            AddPlural("s$", "s");
            AddPlural("(ax|test)is$", "$1es");
            AddPlural("(octop|vir)us$", "$1i");
            AddPlural("(alias|status)$", "$1es");
            AddPlural("(bu)s$", "$1ses");
            AddPlural("(buffal|tomat)o$", "$1oes");
            AddPlural("([ti])um$", "$1a");
            AddPlural("sis$", "ses");
            AddPlural("(?:([^f])fe|([lr])f)$", "$1$2ves");
            AddPlural("(hive)$", "$1s");
            AddPlural("([^aeiouy]|qu)y$", "$1ies");
            AddPlural("(x|ch|ss|sh)$", "$1es");
            AddPlural("(matr|vert|ind)ix|ex$", "$1ices");
            AddPlural("([m|l])ouse$", "$1ice");
            AddPlural("^(ox)$", "$1en");
            AddPlural("(quiz)$", "$1zes");

            AddSingular("s$", "");
            AddSingular("(n)ews$", "$1ews");
            AddSingular("([ti])a$", "$1um");
            AddSingular("((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$", "$1$2sis");
            AddSingular("(^analy)ses$", "$1sis");
            AddSingular("([^f])ves$", "$1fe");
            AddSingular("(hive)s$", "$1");
            AddSingular("(tive)s$", "$1");
            AddSingular("([lr])ves$", "$1f");
            AddSingular("([^aeiouy]|qu)ies$", "$1y");
            AddSingular("(s)eries$", "$1eries");
            AddSingular("(m)ovies$", "$1ovie");
            AddSingular("(x|ch|ss|sh)es$", "$1");
            AddSingular("([m|l])ice$", "$1ouse");
            AddSingular("(bus)es$", "$1");
            AddSingular("(o)es$", "$1");
            AddSingular("(shoe)s$", "$1");
            AddSingular("(cris|ax|test)es$", "$1is");
            AddSingular("(octop|vir)i$", "$1us");
            AddSingular("(alias|status)es$", "$1");
            AddSingular("^(ox)en", "$1");
            AddSingular("(vert|ind)ices$", "$1ex");
            AddSingular("(matr)ices$", "$1ix");
            AddSingular("(quiz)zes$", "$1");

            AddIrregular("person", "people");
            AddIrregular("man", "men");
            AddIrregular("child", "children");
            AddIrregular("sex", "sexes");
            AddIrregular("move", "moves");

            AddUncountable("equipment");
            AddUncountable("information");
            AddUncountable("rice");
            AddUncountable("money");
            AddUncountable("species");
            AddUncountable("series");
            AddUncountable("fish");
            AddUncountable("sheep");
        }

        #endregion

        private static readonly List<Rule> _plurals = new List<Rule>
                                                          {
                                                          };

        private static readonly List<Rule> _singulars = new List<Rule>();
        private static readonly List<string> _uncountables = new List<string>();

        private static void AddIrregular(string singular, string plural)
        {
            AddPlural("(" + singular[0] + ")" + singular.Substring(1) + "$", "$1" + plural.Substring(1));
            AddSingular("(" + plural[0] + ")" + plural.Substring(1) + "$", "$1" + singular.Substring(1));
        }

        private static void AddUncountable(string word)
        {
            _uncountables.Add(word.ToUpperInvariant());
        }

        private static void AddPlural(string rule, string replacement)
        {
            _plurals.Add(new Rule(rule, replacement));
        }

        private static void AddSingular(string rule, string replacement)
        {
            _singulars.Add(new Rule(rule, replacement));
        }

        public static string Pluralize(this string word)
        {
            return ApplyRules(_plurals, word);
        }

        public static string Singularize(this string word)
        {
            return ApplyRules(_singulars, word);
        }

        private static string ApplyRules(IList<Rule> rules, string word)
        {
            var result = word;

            if (!_uncountables.Contains(word.ToUpperInvariant()))
            {
                for (var i = rules.Count - 1; i >= 0; i--)
                {
                    if ((result = rules[i].Apply(word)) != null)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        public static string Titleize(this string word)
        {
            return Regex.Replace(Humanize(Underscore(word)), @"\b([a-z])",
                                 delegate(Match match) { return match.Captures[0].Value.ToUpper(CultureInfo.InvariantCulture); });
        }

        public static string Humanize(this string lowercaseAndUnderscoredWord)
        {
            return Capitalize(Regex.Replace(lowercaseAndUnderscoredWord, @"_", " "));
        }

        public static string Pascalize(this string lowercaseAndUnderscoredWord)
        {
            return Regex.Replace(lowercaseAndUnderscoredWord, "(?:^|_)(.)", match => match.Groups[1].Value.ToUpper(CultureInfo.InvariantCulture));
        }

        public static string Camelize(this string lowercaseAndUnderscoredWord)
        {
            return Uncapitalize(Pascalize(lowercaseAndUnderscoredWord));
        }

        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase",
            Justification = "Lower case is required")]
        public static string Underscore(this string pascalCasedWord)
        {
            return Regex.Replace(
                Regex.Replace(
                    Regex.Replace(pascalCasedWord, @"([A-Z]+)([A-Z][a-z])", "$1_$2"), @"([a-z\d])([A-Z])",
                    "$1_$2"), @"[-\s]", "_").ToLowerInvariant();
        }

        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase",
            Justification = "Lower case is required.")]
        public static string Capitalize(this string word)
        {
            return word.Substring(0, 1).ToUpperInvariant() + word.Substring(1).ToLowerInvariant();
        }

        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase",
            Justification = "Lower case is required.")]
        public static string Uncapitalize(this string word)
        {
            return word.Substring(0, 1).ToLowerInvariant() + word.Substring(1);
        }

        public static string Ordinalize(this string number)
        {
            var n = int.Parse(number, CultureInfo.InvariantCulture);
            var nMod100 = n%100;

            if (nMod100 >= 11 && nMod100 <= 13)
            {
                return number + "th";
            }

            switch (n%10)
            {
                case 1:
                    return number + "st";
                case 2:
                    return number + "nd";
                case 3:
                    return number + "rd";
                default:
                    return number + "th";
            }
        }

        public static string Dasherize(this string underscoredWord)
        {
            return underscoredWord.Replace('_', '-');
        }

        #region Nested type: Rule

        private class Rule
        {
            private readonly Regex _regex;
            private readonly string _replacement;

            public Rule(string pattern, string replacement)
            {
                _regex = new Regex(pattern, RegexOptions.IgnoreCase);
                _replacement = replacement;
            }

            public string Apply(string word)
            {
                if (!_regex.IsMatch(word))
                {
                    return null;
                }

                return _regex.Replace(word, _replacement);
            }
        }

        #endregion
    }
}