using System.Windows;

namespace Fryzdownloader
{
    class Soundcloud
    {
        public string Link { get; }
        public string Format { get; }
        public bool Playlist { get; }
        public Soundcloud(string link, string format, bool playlist)
        {
            Link = link;
            Format = format;
            Playlist = playlist;
            MessageBox.Show("Soundcloud is Disabled Currently");
            if (Properties.Settings.Default.notify == true) { System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.noti); player.Play(); }
        }


    }
}
