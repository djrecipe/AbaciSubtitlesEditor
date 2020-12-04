using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abaci.SubtitlesEditor
{
    public abstract class TranslationProviderTools
    {
        public static string GetFlattenedContent(SubtitleEntryCollection subtitles)
        {
            List<string> content = subtitles.Select(s => s.Content).ToList();
            string text = string.Join("\n", content);
            return text;
        }
        public static void SetFlattenedContent(ref SubtitleEntryCollection subtitles, string text)
        {
            List<string> list = text.Split('\n').ToList();
            for(int i=0; i<subtitles.Count; i++)
            {
                subtitles[i].Content = list[i];
            }
            return;
        }
        public async static Task<SubtitleEntryCollection> ReplaceContent(SubtitleEntryCollection subtitles, ITranslationProvider translator, string language)
        {
            string text = TranslationProviderTools.GetFlattenedContent(subtitles);
            Task<string> task = translator.TranslateAsync(text, language);
            string result = await task;
            TranslationProviderTools.SetFlattenedContent(ref subtitles, result);
            return subtitles;
        }
    }
}
