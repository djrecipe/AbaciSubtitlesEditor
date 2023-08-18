using Abaci.SubtitlesEditor;

namespace Abaci.Subtitles.Tests
{
    public class CollectionTests
    {
        [Fact]
        public void TestMaxTime()
        {
            int expected_max_seconds = 315;
            SubtitleEntryCollection collection = new SubtitleEntryCollection();
            collection.Add(new SubtitleEntry { EndTime = TimeSpan.FromSeconds(15) });
            collection.Add(new SubtitleEntry { EndTime = TimeSpan.FromSeconds(2) });
            collection.Add(new SubtitleEntry { EndTime = TimeSpan.FromSeconds(expected_max_seconds) });
            collection.Add(new SubtitleEntry { EndTime = TimeSpan.FromSeconds(0) });
            collection.Add(new SubtitleEntry { EndTime = TimeSpan.FromSeconds(17) });
            int result_max_seconds = (int)collection.EndTime.TotalSeconds;
            Assert.Equal(expected_max_seconds, result_max_seconds);
        }
    }
}