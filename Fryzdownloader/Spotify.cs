using SpotifyAPI.Web;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Converter;
using ATL;

namespace Fryzdownloader
{
    class Spotify
    {
        //Very slow and idk how to fix
        //Very slow and idk how to fix
        //Very slow and idk how to fix
        //Very slow and idk how to fix
        //I think its the searching part
        public string Link { get; set;  }
        public string Format { get; }
        public bool Playlist { get; set; }
        public bool Metadata { get; }
        public Spotify(string link, string format, bool playlist, bool metadata)
        {
            Link = link; Format = format; Playlist = playlist; Metadata = metadata;
            Go();
        }
        async void Go()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.spotifyid)) { MessageBox.Show("Spotify Client ID is empty"); return; }
            if (string.IsNullOrEmpty(Properties.Settings.Default.spotifysecret)) { MessageBox.Show("Spotify Client Secret ID is empty"); return; }
            if (string.IsNullOrEmpty(Link)) { MessageBox.Show("Spotify Link is Blank"); return; }
            Directory.CreateDirectory(Properties.Settings.Default.output);

            var config = SpotifyClientConfig.CreateDefault();
            var request = new ClientCredentialsRequest(Properties.Settings.Default.spotifyid, Properties.Settings.Default.spotifysecret);
            var response = await new OAuthClient(config).RequestToken(request);

            var spotify = new SpotifyClient(config.WithToken(response.AccessToken));
            var youtube = new YoutubeClient();

            if (Form1._Form1.spotifyAutoDetermine.Checked == true)
            {
                if (Link.StartsWith("https://open.spotify.com/track/")) { Link = (Link.Substring(31, 22)); Playlist = false; Form1._Form1.logsBox.Items.Add("Auto Link to: Track");  }
                if (Link.StartsWith("https://open.spotify.com/playlist/")) { Link = (Link.Substring(34, 22)); Playlist = true; Form1._Form1.logsBox.Items.Add("Auto Link to: Playlist");  }
                
                Form1._Form1.spotifyLink.Text = Link;
            }

            if (Playlist == true)
            {
                if (Link.StartsWith("https://open.spotify.com/track/")) { MessageBox.Show("Pasted Track Link w/ checked Playlist"); return; }
                if (Link.StartsWith("https://open.spotify.com/playlist/")) { Link = (Link.Substring(34, 22)); Form1._Form1.spotifyLink.Text = Link; }

                var list = await spotify.Playlists.Get(Link);
                Form1._Form1.logsBox.Items.Add("Getting Songs:" + list.Name);
                var playlisttitle = Regex.Replace(list.Name, @"[^a-zA-Z0-9[\]._-]", " ");
                Directory.CreateDirectory(Properties.Settings.Default.output + playlisttitle + @"\");
                foreach (PlaylistTrack<IPlayableItem> item in list.Tracks.Items)
                {
                    if (item.Track is FullTrack track)
                    {
                        var convert = await youtube.Search.GetResultsAsync(track.Artists[0].Name + " " + track.Name + " Lyrics").CollectAsync(1);
                        Form1._Form1.logsBox.Items.Add("Searching:" + track.Artists[0].Name + " "+  track.Name + " Lyrics");
                        var video = await youtube.Videos.GetAsync(convert.FirstOrDefault().Url);
                        var title = Regex.Replace(video.Title, @"[^a-zA-Z0-9[\]._-]", " ");
                        var author = Regex.Replace(video.Author.ChannelTitle, @"[^a-zA-Z0-9[\]._-]", " ");
                        var filename = author + " - " + title + Format;
                        var filepath = Properties.Settings.Default.output + playlisttitle + @"\";
                        Form1._Form1.logsBox.Items.Add("Downloading: " + filename);
                        Track theTrack = new(filepath + filename);
                        await youtube.Videos.DownloadAsync(convert.FirstOrDefault().Url, filepath + filename);
                        if (Metadata == true)
                        {
                            theTrack.Artist = track.Artists[0].Name;
                            theTrack.Title = track.Name;
                            theTrack.Album = track.Album.Name;
                            theTrack.DiscNumber = track.DiscNumber;
                            theTrack.Popularity = track.Popularity;
                            theTrack.TrackNumber = track.TrackNumber;
                            theTrack.Comment = "Downloaded with FryzDownloader";
                            theTrack.Save();
                            Form1._Form1.logsBox.Items.Add("Added Metadata Too: " + filename);
                        }
                        Form1._Form1.logsBox.Items.Add("Downloaded: " + filename);
                    }
                }
            }
            else
            {
                if (Link.StartsWith("https://open.spotify.com/track/")) { Link = (Link.Substring(31, 22)); Form1._Form1.spotifyLink.Text = Link; }
                if (Link.StartsWith("https://open.spotify.com/playlist/")) { MessageBox.Show("Pasted Playlist Link w/out checking Playlist"); return; }

                var song = await spotify.Tracks.Get(Link);
                Form1._Form1.logsBox.Items.Add("Searching: " + song.Artists[0].Name + " " + song.Name + " Lyrics");
                var convert = await youtube.Search.GetResultsAsync(song.Artists[0].Name + " " + song.Name + " Lyrics").CollectAsync(1);
                var video = await youtube.Videos.GetAsync(convert.FirstOrDefault().Url);
                var filename = Regex.Replace(video.Author.ChannelTitle, @"[^a-zA-Z0-9[\&!()]._-]", " ") + " - " + Regex.Replace(video.Title, @"[^a-zA-Z0-9[\&!()]._-]", " ") + Format;
                var filepath = Properties.Settings.Default.output;
                Form1._Form1.logsBox.Items.Add("Downloading: " + filename);
                Track theTrack = new(filepath + filename);
                await youtube.Videos.DownloadAsync(convert.FirstOrDefault().Url, filepath + filename);
                if (Metadata == true)
                {
                    theTrack.Artist = song.Artists[0].Name;

                    theTrack.Title = song.Name;
                    theTrack.Album = song.Album.Name;
                    theTrack.DiscNumber = song.DiscNumber;
                    theTrack.Popularity = song.Popularity;
                    theTrack.TrackNumber = song.TrackNumber;
                    theTrack.Comment = "Downloaded with FryzDownloader";
                    theTrack.Save();
                    Form1._Form1.logsBox.Items.Add("Added Metadata Too: " + filename);
                }
                Form1._Form1.logsBox.Items.Add("Downloaded: " + filename);
            }
            if (Properties.Settings.Default.notify == true) { System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.noti);  player.Play(); }
        }
    }
}
