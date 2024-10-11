using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for MediaControls.xaml
    /// </summary>
    public partial class MediaControls : UserControl
    {
        public event EventHandler? PlayClicked;
        public event EventHandler? PauseClicked;
        public event EventHandler? StopClicked;

        public MediaControls()
        {
            InitializeComponent();

            
            PlayButton.Click += (s, e) => PlayClicked?.Invoke(this, EventArgs.Empty);
            PauseButton.Click += (s, e) => PauseClicked?.Invoke(this, EventArgs.Empty);
            StopButton.Click += (s, e) => StopClicked?.Invoke(this, EventArgs.Empty);
        }

   
        public double SliderValue
        {
            get => SeekSlider.Value;
            set => SeekSlider.Value = value;
        }

        public double SliderMaximum
        {
            get => SeekSlider.Maximum;
            set => SeekSlider.Maximum = value;
        }

        public string TimeText
        {
            get => TimeDisplay.Text;
            set => TimeDisplay.Text = value;
        }
    }
}