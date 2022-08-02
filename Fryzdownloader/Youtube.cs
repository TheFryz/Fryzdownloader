using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using YoutubeExplode;
using YoutubeExplode.Converter;

namespace Fryzdownloader
{
    class Youtube
    {
        public string Link { get; }
        public string Format { get; }
        public bool Playlist { get; }

        public Youtube(string link, string format, bool playlist)
        {
            Link = link;
            Format = format;
            Playlist = playlist;
            go();
        }

        async void go()
        {
            if (string.IsNullOrEmpty(Link)) { MessageBox.Show("Youtube Link is Blank"); return; }
            var youtube = new YoutubeClient();

            if (Playlist == true)
            {
                var playlist = await youtube.Playlists.GetAsync(Link);
                var playlisttitle = Regex.Replace(playlist.Title, @"[^a-zA-Z0-9[\]._-]", " ");
                Form1._Form1.logsBox.Items.Add("Getting Songs: " + playlist.Title);
                Directory.CreateDirectory(Properties.Settings.Default.output + playlisttitle);
                await foreach (var video in youtube.Playlists.GetVideosAsync(Link))
                {
                    var title = Regex.Replace(video.Title, @"[^a-zA-Z0-9[\]._-]", " ");
                    Form1._Form1.logsBox.Items.Add("Searching: " + video.Title);
                    var author = video.Author;
                    var filename = author + " - " + title + Format;
                    var filepath = Properties.Settings.Default.output + playlisttitle + @"\";
                    Form1._Form1.logsBox.Items.Add("Downloading: " + filename);
                    await youtube.Videos.DownloadAsync(video.Id, filepath + filename);
                    Form1._Form1.logsBox.Items.Add("Downloaded: " + filename);
                }
            }
            else
            {
                var video = await youtube.Videos.GetAsync(Link);
                Form1._Form1.logsBox.Items.Add("Searching: " + video.Title);
                var title = Regex.Replace(video.Title, @"[^a-zA-Z0-9[\]._-]", " ");
                var author = video.Author;
                var filename = author + " - " + title + Format;
                var filepath = Properties.Settings.Default.output;
                Form1._Form1.logsBox.Items.Add("Downloading: " + filename);
                await youtube.Videos.DownloadAsync(video.Id, filepath + filename);
                Form1._Form1.logsBox.Items.Add("Downloaded: " + filename);

            }
            if (Properties.Settings.Default.notify == true) { System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.noti); player.Play(); }
        }
    }
}
