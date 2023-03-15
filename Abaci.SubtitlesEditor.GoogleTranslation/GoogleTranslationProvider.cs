using System;
using System.Threading.Tasks;

namespace Abaci.SubtitlesEditor.GoogleTranslation
{
    public class GoogleTranslationProvider : ITranslationProvider
    {
        public Task<string> TranslateAsync(string text, string target_language)
        {
            throw new NotImplementedException();
        }
    }
}
