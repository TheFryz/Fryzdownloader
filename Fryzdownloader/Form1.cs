using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Fryzdownloader
{
    public partial class Form1 : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        //public ArrayList songlog = new ArrayList();

        public static string Link { get; set; }
        public static string Format { get; set; }
        public static bool playlist { get; set; }

        public static bool Metadata { get; set; }

        public static Form1 _Form1;


        public Form1()
        {
            InitializeComponent();
            GetSettings();

            Opacity = 0;
            timer.Interval = 10;
            timer.Tick += new EventHandler(fadeIn);
            timer.Start();


            _Form1 = this;

            page.SelectedTab = homePage;
            home.BackColor = Color.FromArgb(52, 52, 52);

            quality.SelectedIndex = 0;

            home.MouseDown += new MouseEventHandler(TabControler);
            spotify.MouseClick += new MouseEventHandler(TabControler);
            youtube.MouseClick += new MouseEventHandler(TabControler);
            soundcloud.MouseClick += new MouseEventHandler(TabControler);
            settings.MouseClick += new MouseEventHandler(TabControler);
            downloads.MouseClick += new MouseEventHandler(TabControler);

            page.Selecting += new TabControlCancelEventHandler(PageControler);

            spotifyButton.MouseClick += new MouseEventHandler(MediaControler);
            youtubeButton.MouseClick += new MouseEventHandler(MediaControler);
            soundcloudButton.MouseClick += new MouseEventHandler(MediaControler);

            navPanel.MouseMove += new MouseEventHandler(WindowMove);

            if (spotifyAutoMeta.Checked == true) { Metadata = true; spotifyMetadata.Checked = true; } else { Metadata = false; spotifyMetadata.Checked = false; }

        }
        private void TabControler(object Sender, MouseEventArgs e)
        {
            ResetNavbar();
            page.SelectTab(((Control)Sender).Name + "Page");
            ((Control)Sender).BackColor = Color.FromArgb(52, 52, 52);
        }
        void fadeIn(object sender, EventArgs e)
        {
            if (Opacity >= 0.95)
                timer.Stop();
            else
                Opacity += 0.05;
        }
        private void PageControler(object sender, TabControlCancelEventArgs e)
        {
            TabPage Current = (sender as TabControl).SelectedTab;
            SaveSettings();
        }
        private void MediaControler(Object Sender, MouseEventArgs e)
        {
            var Media = ((Control)Sender).Name.Substring(0, ((Control)Sender).Name.Length - 6);

            if (Media == "spotify")
            {
                if (spotifyplaylist.Checked == true) { playlist = true; }
                if (spotifyMp3.Checked == true) { Format = ".mp3"; } else { Format = ".mp4"; }
                if (spotifyMetadata.Checked == true) { Metadata = true; } else { Metadata = false; }
                
                Link = spotifyLink.Text;
                Spotify Exec = new Spotify(Link, Format, playlist, Metadata);
            }
            if (Media == "youtube")
            {
                if (youtubeplaylist.Checked == true) { playlist = true; }
                if (youtubeMp3.Checked == true) { Format = ".mp3"; } else { Format = ".mp4"; }
                Link = youtubeLink.Text;
                Youtube exec = new Youtube(Link, Format, playlist);
            }
            if (Media == "soundcloud")
            {
                if (soundcloudplaylist.Checked == true) { playlist = true; }
                if (soundcloudMp3.Checked == true) { Format = ".mp3"; } else { Format = ".mp4"; }
                Link = soundcloudLink.Text;
                Soundcloud exec = new Soundcloud(Link, Format, playlist);
            }
        }
        private void WindowMove(object Sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void linkLabel1_Click(object sender, EventArgs e) { Process.Start(new ProcessStartInfo { FileName = "https://google.com", UseShellExecute = true }); }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e) { timer.Interval = 10; timer.Start(); timer.Tick += new EventHandler(AppClose); }
        private void exitTab_Click(object sender, EventArgs e) { timer.Interval = 10; timer.Start(); timer.Tick += new EventHandler(AppClose); }
        private void ResetNavbar()
        {
            home.BackColor = Color.FromArgb(60, 60, 60);
            spotify.BackColor = Color.FromArgb(60, 60, 60);
            youtube.BackColor = Color.FromArgb(60, 60, 60);
            soundcloud.BackColor = Color.FromArgb(60, 60, 60);
            settings.BackColor = Color.FromArgb(60, 60, 60);
            downloads.BackColor = Color.FromArgb(60, 60, 60);
        }
        void GetSettings()
        {
            spotifyid.Text = Properties.Settings.Default.spotifyid;
            spotifysecret.Text = Properties.Settings.Default.spotifysecret;
            output.Text = Properties.Settings.Default.output;
            if (Properties.Settings.Default.autometadata == true) { spotifyAutoMeta.Checked = true;  } else { spotifyAutoMeta.Checked = false; }
            if (Properties.Settings.Default.autodetermine == true) { spotifyAutoDetermine.Checked = true; } else { spotifyAutoDetermine.Checked = false; }
            if (Properties.Settings.Default.notify == true) { notifyOn.Checked = true; notifyOff.Checked = false; } else { notifyOn.Checked = false; notifyOff.Checked = true; }
            if (Properties.Settings.Default.discordstatus == true) { statusOn.Checked = true; statusOff.Checked = false; } else { statusOn.Checked = false; statusOff.Checked = true; }
        }
        void SaveSettings()
        {
            Properties.Settings.Default.spotifyid = spotifyid.Text;
            Properties.Settings.Default.spotifysecret = spotifysecret.Text;
            Properties.Settings.Default.output = output.Text;
            Properties.Settings.Default.Save();
            if (notifyOn.Checked == true) { Properties.Settings.Default.notify = true; } else { Properties.Settings.Default.notify = false; }
            if (statusOn.Checked == true) { Properties.Settings.Default.discordstatus = true; } else { Properties.Settings.Default.discordstatus = false; }
            if (spotifyAutoMeta.Checked == true) { Properties.Settings.Default.notify = true; } else { Properties.Settings.Default.autometadata = false; }
            if (spotifyAutoDetermine.Checked == true) { Properties.Settings.Default.autodetermine = true; } else { Properties.Settings.Default.autodetermine = false; } 
        }
        void Finished()
        {
            //songlog.Add(logtest.Text);
            //logsBox.Items.Add(logtest.Text);
            //var json = (JsonConvert.SerializeObject(songlog));
            //MessageBox.Show(json);
            page.SelectedTab = homePage;
            home.BackColor = Color.FromArgb(52, 52, 52);
        }
        
        void AppClose(object sender, EventArgs e)
        {
            if (Opacity <= 0)     //check if opacity is 0
            {
                timer.Stop();
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
                Application.Exit();
            }
            else
            {
                Opacity -= 0.05;
            }
        }
    }
}
