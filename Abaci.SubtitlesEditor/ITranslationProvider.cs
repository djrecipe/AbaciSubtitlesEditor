using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abaci.SubtitlesEditor
{
    public interface ITranslationProvider
    {
        Task<string> TranslateAsync(string text, string target_language);
    }
}
