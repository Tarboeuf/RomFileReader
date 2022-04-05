using RomFileReader.Libraries;

namespace RomFileReader.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Settings settings = new Settings();
            view.ViewModel = new MainViewModel(new FileManager(settings), new RomDataExtractor(), settings);
        }

    }
}
