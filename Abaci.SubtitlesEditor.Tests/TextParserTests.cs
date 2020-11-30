using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Abaci.SubtitlesEditor.Tests
{
    [TestClass]
    [DeploymentItem("Abaci.SubtitlesEditor.Tests.TestRegex.srt")]
    public class TextParserTests
    {
        private readonly TextParser parser = new TextParser();
        public TestContext TestContext { get;set;}
        [TestMethod]
        public void TestFullParseFromFile()
        {
            int expected_count = 9;
            string text = File.ReadAllText("Abaci.SubtitlesEditor.Tests.TestRegex.srt");
            SubtitleEntryCollection list = parser.Parse(text);
            foreach(SubtitleEntry entry in list)
            {
                Console.WriteLine($"{entry.ToString()}\n");
            }
            Assert.AreEqual(expected_count, list.Count, "Subtitle entry count mismatch");
            return;
        }
        [TestMethod]
        public void TestParseLongTimeFormatWithPeriod()
        {
            TimeSpan start_time = TimeSpan.FromSeconds(1.5);
            TimeSpan end_time = TimeSpan.FromSeconds(5.5);
            string text = $"TestParseMillisecondsEntry\n{start_time:G} --> {end_time:G}\nsubtitle content";
            this.TestContext.WriteLine($"Parsing subtitle entry from text:\n'{text}'");
            SubtitleEntryCollection subtitles = this.parser.Parse(text);
            Assert.AreEqual(1, subtitles.Count, "Subtitle count mismatch");
            SubtitleEntry subtitle = subtitles[0];
            Assert.AreEqual(start_time, subtitle.StartTime, "Start time mismatch");
            Assert.AreEqual(end_time, subtitle.EndTime, "End time mismatch");
        }
        [TestMethod]
        public void TestParseShortTimeFormatWithPeriod()
        {
            TimeSpan start_time = TimeSpan.FromSeconds(1.5);
            TimeSpan end_time = TimeSpan.FromSeconds(5.5);
            string text = $"TestParseMillisecondsEntry\n{start_time:g} --> {end_time:g}\nsubtitle content";
            this.TestContext.WriteLine($"Parsing subtitle entry from text:\n'{text}'");
            SubtitleEntryCollection subtitles = this.parser.Parse(text);
            Assert.AreEqual(1, subtitles.Count, "Subtitle count mismatch");
            SubtitleEntry subtitle = subtitles[0];
            Assert.AreEqual(start_time, subtitle.StartTime, "Start time mismatch");
            Assert.AreEqual(end_time, subtitle.EndTime, "End time mismatch");
        }
        [TestMethod]
        public void TestConstantShortTimeFormatWithPeriod()
        {
            TimeSpan start_time = TimeSpan.FromSeconds(1.5);
            TimeSpan end_time = TimeSpan.FromSeconds(5.5);
            string text = $"TestParseMillisecondsEntry\n{start_time:c} --> {end_time:c}\nsubtitle content";
            this.TestContext.WriteLine($"Parsing subtitle entry from text:\n'{text}'");
            SubtitleEntryCollection subtitles = this.parser.Parse(text);
            Assert.AreEqual(1, subtitles.Count, "Subtitle count mismatch");
            SubtitleEntry subtitle = subtitles[0];
            Assert.AreEqual(start_time, subtitle.StartTime, "Start time mismatch");
            Assert.AreEqual(end_time, subtitle.EndTime, "End time mismatch");
        }
    }
}
