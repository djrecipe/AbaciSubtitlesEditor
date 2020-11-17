using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Abaci.SubtitlesEditor
{
    internal class TextParser
    {
        private static string REGEX_MAIN = @"(?<Label>[^\r?\n]+\r?\n)* *((?<StartTime>\d{2}:\d{2}:\d{2}(,\d{3})*) *--> *(?<EndTime>\d{2}:\d{2}:\d{2}(,\d{3})*)) *(?<Content>(\r?\n[^\r?\n]+)*)";
        public List<SubtitleEntry> Parse(string text)
        {
            Regex regex = new Regex(TextParser.REGEX_MAIN);
            MatchCollection matches = regex.Matches(text);
            return this.ProcessMatches(matches);
        }
        private List<SubtitleEntry> ProcessMatches(MatchCollection matches)
        {
            List<SubtitleEntry> subtitles = new List<SubtitleEntry>();
            foreach(Match match in matches)
            {
                subtitles.Add(this.ProcessMatch(match));
            }
            return subtitles;
        }
        private SubtitleEntry ProcessMatch(Match match)
        {
            Group group_label = match.Groups["Label"];
            Group group_start_time = match.Groups["StartTime"];
            Group group_end_time = match.Groups["EndTime"];
            Group group_content = match.Groups["Content"];
            SubtitleEntry new_entry = new SubtitleEntry
            {
                Content = group_content.Value.Trim(new char[]{'\n','\r',' '}),
                Label = group_label.Success ? group_label.Value.Trim(new char[] { '\n', '\r', ' ' }) : null,
                StartTime = this.ProcessTimeString(group_start_time.Value),
                EndTime = this.ProcessTimeString(group_end_time.Value)
            };
            return new_entry;
        }
        private TimeSpan ProcessTimeString(string text)
        {
            string clean_text = text.Replace(',', '.');
            return TimeSpan.Parse(clean_text);
        }
    }
}
