using Microsoft.Win32;
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
            }
        }
        //Open Tags
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Examples of reading tag data from currently selected file.
            var year = currentFile.Tag.Year;
            var title = currentFile.Tag.Title;
            var album = currentFile.Tag.Album;

            tagNameBox.Text = title + " : " + year + album.ToString();
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
