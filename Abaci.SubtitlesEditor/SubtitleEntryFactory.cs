using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Abaci.SubtitlesEditor
{
    public class SubtitleEntryFactory
    {
        private readonly TextParser parser = new TextParser();
        public SubtitleEntryCollection CreateFromText(string text)
        {
            SubtitleEntryCollection subtitles = new SubtitleEntryCollection();
            if(!string.IsNullOrWhiteSpace(text))
                subtitles = this.parser.Parse(text);
            return subtitles;
        }
        public SubtitleEntryCollection CreateFromFile(string path)
        {
            string text = File.ReadAllText(path);
            return this.CreateFromText(text);
        }
        public void Save(SubtitleEntryCollection subtitles, string path)
        {
            File.WriteAllText(path, subtitles.ToString());
        }
    }
}
