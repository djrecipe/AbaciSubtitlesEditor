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
        [TestMethod]
        public void TestRegex()
        {
            int expected_count = 9;
            string text = File.ReadAllText("Abaci.SubtitlesEditor.Tests.TestRegex.srt");
            TextParser parser = new TextParser();
            List<SubtitleEntry> list = parser.Parse(text);
            foreach(SubtitleEntry entry in list)
            {
                Console.WriteLine($"{entry.ToString()}\n");
            }
            Assert.AreEqual(expected_count, list.Count, "Subtitle entry count mismatch");
            return;
        }
    }
}
