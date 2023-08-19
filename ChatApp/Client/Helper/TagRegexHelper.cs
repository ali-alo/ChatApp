using ChatApp.Server.Models;
using System.Text.RegularExpressions;

namespace ChatApp.Client.Helper
{
    public static class TagRegexHelper
    {
        private static string _tagRegex = @"#[\w\d]+";

        public static List<Tag> GetTagsFromString(string input)
        {
            var matches = GetRegexMatches(input);
            List<Tag> tags = matches.Cast<Match>()
                .Select(m => new Tag { Name = m.Value.Trim().ToLower() })
                .ToList();
            return tags;
        }

        public static MatchCollection GetRegexMatches(string input)
        {
            Regex hashtagRegex = new Regex(_tagRegex);
            return hashtagRegex.Matches(input);
        }

        public static bool ValidateRegexMatch(string tagName) => Regex.IsMatch(tagName, _tagRegex);
    }
}
