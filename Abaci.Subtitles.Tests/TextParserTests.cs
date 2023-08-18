using Abaci.SubtitlesEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abaci.Subtitles.Tests
{
    public class TextParserTests
    {
        private readonly TextParser parser = new TextParser();
        [Fact]
        public void TestFullParseFromFile()
        {
            int expected_count = 9;
            string text = File.ReadAllText("Abaci.SubtitlesEditor.Tests.TestRegex.srt");
            SubtitleEntryCollection list = parser.Parse(text);
            foreach (SubtitleEntry entry in list)
            {
                Console.WriteLine($"{entry.ToString()}\n");
            }
            Assert.Equal(expected_count, list.Count);
            return;
        }
        [Fact]
        public void TestParseLongTimeFormatWithPeriod()
        {
            TimeSpan start_time = TimeSpan.FromSeconds(1.5);
            TimeSpan end_time = TimeSpan.FromSeconds(5.5);
            string text = $"TestParseMillisecondsEntry\n{start_time:G} --> {end_time:G}\nsubtitle content";
            SubtitleEntryCollection subtitles = this.parser.Parse(text);
            Assert.Equal(1, subtitles.Count);
            SubtitleEntry subtitle = subtitles[0];
            Assert.Equal(start_time, subtitle.StartTime);
            Assert.Equal(end_time, subtitle.EndTime);
        }
        [Fact]
        public void TestParseShortTimeFormatWithPeriod()
        {
            TimeSpan start_time = TimeSpan.FromSeconds(1.5);
            TimeSpan end_time = TimeSpan.FromSeconds(5.5);
            string text = $"TestParseMillisecondsEntry\n{start_time:g} --> {end_time:g}\nsubtitle content";
            SubtitleEntryCollection subtitles = this.parser.Parse(text);
            Assert.Equal(1, subtitles.Count);
            SubtitleEntry subtitle = subtitles[0];
            Assert.Equal(start_time, subtitle.StartTime);
            Assert.Equal(end_time, subtitle.EndTime);
        }
        [Fact]
        public void TestConstantShortTimeFormatWithPeriod()
        {
            TimeSpan start_time = TimeSpan.FromSeconds(1.5);
            TimeSpan end_time = TimeSpan.FromSeconds(5.5);
            string text = $"TestParseMillisecondsEntry\n{start_time:c} --> {end_time:c}\nsubtitle content";
            SubtitleEntryCollection subtitles = this.parser.Parse(text);
            Assert.Equal(1, subtitles.Count);
            SubtitleEntry subtitle = subtitles[0];
            Assert.Equal(start_time, subtitle.StartTime);
            Assert.Equal(end_time, subtitle.EndTime);
        }
    }
}
