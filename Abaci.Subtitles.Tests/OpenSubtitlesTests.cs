using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Abaci.SubtitlesEditor;

namespace Abaci.Subtitles.Tests
{
    public class OpenSubtitlesTests
    {
        private readonly SecretAppsettingReader reader = new SecretAppsettingReader();
        [Fact]
        public async void Login()
        {
            var section = reader.ReadSection("OpenSubtitles");
            var key = section["ApiKey"];
            var user = section["user"];
            var password = section["password"];
            OpenSubtitles subtitles = new OpenSubtitles(key);
            await subtitles.Login(user, password);
            Assert.True(subtitles.LoggedIn);
            return;
        }
        [Fact]
        public async void Download()
        {
            var section = reader.ReadSection("OpenSubtitles");
            var key = section["ApiKey"];
            var user = section["user"];
            var password = section["password"];
            OpenSubtitles subtitles = new OpenSubtitles(key);
            await subtitles.Login(user, password);
            var result = await subtitles.Download(1);
            Assert.NotNull(result);
            return;
        }
    }
}
