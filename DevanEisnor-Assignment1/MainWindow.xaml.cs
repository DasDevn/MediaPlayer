using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DevanEisnor_Assignment1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //CITE https://wpf-tutorial.com/misc/dispatchertimer/
        private DispatcherTimer timer;
        TagLib.File? currentFile;

        public MainWindow()
        {
            InitializeComponent();

            // Set up the DispatcherTimer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            //events for media
            MediaControls.PlayClicked += Play_Executed;
            MediaControls.PauseClicked += Pause_Executed;
            MediaControls.StopClicked += Stop_Executed;
        }

        //Opens File
        //Added try catch
        //Code from Assignment 1 helper file
        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Filter = "MP3 files(*.mp3)|*.mp3|All files(*.*)|*.*";

            if (fileDlg.ShowDialog() == true)
            {
                try
                {
                    currentFile = TagLib.File.Create(fileDlg.FileName);
                    myMediaPlayer.Source = new Uri(fileDlg.FileName);
                    myMediaPlayer.Play();
                    LoadAlbumArt();

                    myMediaPlayer.MediaOpened += (s, ev) =>
                    {
                        if (myMediaPlayer.NaturalDuration.HasTimeSpan)
                        {
                            MediaControls.SliderMaximum = myMediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                        }
                        timer.Start();
                    };
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to open the file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        //CITE https://wpf-tutorial.com/misc/dispatchertimer/
        //Get song length, and update slider
        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (myMediaPlayer.NaturalDuration.HasTimeSpan)
            {
                // Get current playback position and total duration
                var currentPosition = myMediaPlayer.Position;
                var totalDuration = myMediaPlayer.NaturalDuration.TimeSpan;

                // Update MediaControls' slider and time display
                MediaControls.SliderValue = currentPosition.TotalSeconds;
                MediaControls.SliderMaximum = totalDuration.TotalSeconds;
                MediaControls.TimeText = $"{currentPosition.ToString(@"m\:ss")} / {totalDuration.ToString(@"m\:ss")}";
            }
        }

        //https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/how-to-use-a-bitmapimage?view=netframeworkdesktop-4.8
        private void LoadAlbumArt()
        {
            try
            {
                if (currentFile?.Tag.Pictures.Length > 0)
                {
                    
                    var bin = currentFile.Tag.Pictures[0].Data.Data;

                    using (var stream = new MemoryStream(bin))
                    {
                        //Create class, get image, converts, and is for display 
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.StreamSource = stream;
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.EndInit();

                        // Set source to the Image to the album art
                        Mp3Image.Source = image;
                    }
                }

            }
            catch (Exception ex)
            {
                // Log the error or show a message if necessary
                MessageBox.Show($"Failed to load album art: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
               
            }
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
                tagNameBox.Inlines.Add(new Run(title) { FontSize = 16, FontWeight = FontWeights.Bold });
                tagNameBox.Inlines.Add(new LineBreak());
                tagNameBox.Inlines.Add(new Run(artist) { FontSize = 12, FontStyle = FontStyles.Italic });
                tagNameBox.Inlines.Add(new LineBreak());

                // Add album and year with smaller font
                tagNameBox.Inlines.Add(new Run($"{album} ({year})") { FontSize = 10 });

                // Make the TextBlock visible
                tagNameBox.Visibility = Visibility.Visible;
            }
        }

        //Shows/Hides Edit tags when button is pressed populates with current data
        private void EditTags_Click(object sender, RoutedEventArgs e)
        {
            if (currentFile == null)
            {
                MessageBox.Show("Please open an MP3 file first.", "No File Loaded", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Toggle visibility: if currently visible, collapse it
            if (EditTagPanel.Visibility == Visibility.Visible)
            {
                EditTagPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                TitleTextBox.Text = currentFile.Tag.Title ?? string.Empty;
                ArtistTextBox.Text = currentFile.Tag.FirstPerformer ?? string.Empty;
                AlbumTextBox.Text = currentFile.Tag.Album ?? string.Empty;
                YearTextBox.Text = currentFile.Tag.Year > 0 ? currentFile.Tag.Year.ToString() : string.Empty;

                EditTagPanel.Visibility = Visibility.Visible;
            }
        }

        //https://stackoverflow.com/questions/56919566/how-to-save-data-from-wpf-application-to-file
        //Help from ChatGBT to modify the code so that it would save - still not saving perfectly
        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (currentFile == null)
            {
                MessageBox.Show("Please open an MP3 file first.", "No File Loaded", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Stop, close, release file file
            myMediaPlayer.Stop();
            myMediaPlayer.Close();
            myMediaPlayer.Source = null;

            // Update the tag information
            currentFile.Tag.Title = TitleTextBox.Text;
            currentFile.Tag.Performers = new[] { ArtistTextBox.Text };
            currentFile.Tag.Album = AlbumTextBox.Text;
            currentFile.Tag.Year = uint.TryParse(YearTextBox.Text, out uint year) ? year : 0;

            //added try catch
            try
            {
                currentFile.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save tags: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            EditTagPanel.Visibility = Visibility.Collapsed;

            //add try catch
            try
            {
                currentFile = TagLib.File.Create(currentFile.Name); 
                myMediaPlayer.Source = new Uri(currentFile.Name); 
                myMediaPlayer.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to reopen the file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // Play Command Execution
        private void Play_Executed(object? sender, EventArgs e)
        {
            if (myMediaPlayer != null)
            {
                myMediaPlayer.Play();
                timer.Start();
            }
        }

        // Pause Command Execution
        private void Pause_Executed(object? sender, EventArgs e)
        {
            if (myMediaPlayer != null)
            {
                myMediaPlayer.Pause();
                timer.Stop();
            }
        }

        // Stop Command Execution
        private void Stop_Executed(object? sender, EventArgs e)
        {
            if (myMediaPlayer != null)
            {
                myMediaPlayer.Stop();
                timer.Stop();
                MediaControls.SliderValue = 0;
            }
        }

        // Enable command if media player is not null and a file is loaded
        private void Media_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = myMediaPlayer != null && myMediaPlayer.Source != null;
        }
        private void CanExecute_Always(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

    }
}
