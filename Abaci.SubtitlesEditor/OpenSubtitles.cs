using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace Abaci.SubtitlesEditor
{
    public class OpenSubtitles
    {
        private class LoginResult
        {
            public class UserResult
            {
                public double Allowed_downloads { get; set; }
                public double Allowed_translations { get; set; }
                public string Level { get; set; }
                public double User_id { get; set; }
                public bool Ext_installed { get; set; }
                public bool Vip { get; set; }
            }
            public UserResult User { get; set; }
            public string Base_url { get; set; }
            public string Token { get; set; }
            public double Status { get; set; }
        }
        private class DownloadResult
        {
            public string Link { get; set; }
            public string File_name { get; set; }
            public double Requests { get; set; }
            public double Remaining { get; set; }
            public string Message { get; set; }
            public string Reset_time { get; set; }
            public string Reset_time_utc { get; set; }
        }
        private class LoginRequest
        {
            public string username { get; set; }
            public string password { get; set; }
        }
        private class DownloadRequest
        {
            public int file_id { get; set; }
        }
        private readonly HttpClient client = new HttpClient();
        public bool LoggedIn { get; set; } = false;
        public OpenSubtitles(string api_key)
        {
            client.BaseAddress = new Uri("https://stoplight.io/mocks/opensubtitles/opensubtitles-api/2781383/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Api-Key", api_key);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<string> Download(int id)
        {
            var req = new DownloadRequest() { file_id = id };
            var response = await client.PostAsJsonAsync("download", req);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<DownloadResult>();
            return result.Link;
        }

        public async Task Login(string username, string password)
        {
            var req = new LoginRequest() { username = username, password = password };
            var response = await client.PostAsJsonAsync("login", req);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<LoginResult>();
            LoggedIn = !string.IsNullOrWhiteSpace(result?.Token);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {result.Token}");
            return;
        }
    }
}
