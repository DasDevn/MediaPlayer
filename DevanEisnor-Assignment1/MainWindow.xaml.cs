using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DevanEisnor_Assignment1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TagLib.File currentFile;

        public MainWindow()
        {
            InitializeComponent();
        }

        //Opens File
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            //Example of instantiating an OpenFileDialog
            OpenFileDialog fileDlg = new OpenFileDialog();

            //Create a file filter
            fileDlg.Filter = "MP3 files(*.mp3)|*.mp3|All files(*.*)|*.*";

            //ShowDialog shows onscreen for the user
            //By default it return true if the user selects a file and hits "Open"
            if (fileDlg.ShowDialog() == true)
            {
                //The filename property stores the full path that was selected
                fileNameBox.Text = fileDlg.FileName;

                //Example of creating a TagLib file object, for accessing mp3 metadata
                currentFile = TagLib.File.Create(fileDlg.FileName);

                //Set the source of the media player element.
                myMediaPlayer.Source = new Uri(fileDlg.FileName);
                myMediaPlayer.Play();

                LoadAlbumArt();
            }
        }

        //https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/how-to-use-a-bitmapimage?view=netframeworkdesktop-4.8
        //With some help from chatGBT
        private void LoadAlbumArt()
        {
            if (currentFile.Tag.Pictures.Length > 0)
            {
                // Get the album art as a byte array
                var bin = currentFile.Tag.Pictures[0].Data.Data;

                // Convert the byte array to a BitmapImage
                using (var stream = new MemoryStream(bin))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = stream;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();

                    // Set the source of the Image control to the album art
                    Mp3Image.Source = image;
                }
            }
            // If no album art is found, do nothing to keep the default image
        }
        //Open Tags
        private void Tag_Click(object sender, RoutedEventArgs e)
        {
            // Check if an MP3 file is loaded
            if (currentFile == null)
            {
                MessageBox.Show("Please open an MP3 file first.", "No File Loaded", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Toggle visibility: if currently visible, collapse it
            if (tagNameBox.Visibility == Visibility.Visible)
            {
                tagNameBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                // If currently collapsed, show the tag information
                var title = currentFile.Tag.Title;
                var artist = currentFile.Tag.FirstPerformer;
                var album = currentFile.Tag.Album;
                var year = currentFile.Tag.Year;

                //clear so it doesnt keep writing
                tagNameBox.Inlines.Clear();

                //https://stackoverflow.com/questions/24130980/how-can-i-insert-a-newline-into-a-textblock-without-xaml
                tagNameBox.Inlines.Add(new Run(title) { FontSize = 24, FontWeight = FontWeights.Bold });
                tagNameBox.Inlines.Add(new LineBreak());
                tagNameBox.Inlines.Add(new Run(artist) { FontSize = 20, FontStyle = FontStyles.Italic });
                tagNameBox.Inlines.Add(new LineBreak());

                // Add album and year with smaller font
                tagNameBox.Inlines.Add(new Run($"{album} ({year})") { FontSize = 16 });

                // Make the TextBlock visible
                tagNameBox.Visibility = Visibility.Visible;
            }
        }

        //Play MP3
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            myMediaPlayer.Play();
        }

        //Pause MP3
        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            myMediaPlayer.Pause();
        }

        //Stop MP3
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            myMediaPlayer.Stop();
        }

        //quit application
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
