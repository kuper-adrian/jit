using System.Text;

namespace jira_issue_time_tracker.Extensions
{
    internal static class StringExtensions
    {
        public static string ToBase64EncodedString(this string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
