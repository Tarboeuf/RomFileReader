using ReactiveUI;
using System.Reactive.Disposables;

namespace RomFileReader.UI
{
    /// <summary>
    /// Logique d'interaction pour MainView.xaml
    /// </summary>
    public partial class MainView
    {
        public MainView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                ViewModel?.Activator.Activate().DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Games, v => v.dataGrid.ItemsSource)
                    .DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Country, v => v.tbCountry.Text)
                    .DisposeWith(d);

                this.Bind(ViewModel, vm => vm.GameTitle, v => v.tbTitle.Text)
                    .DisposeWith(d);

                this.Bind(ViewModel, vm => vm.AreSettingsFine, v => v.expander.IsExpanded, b => !b, b => !b)
                    .DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Settings.LocalGameFilePath, v => v.tbLocalPath.Text)
                    .DisposeWith(d);

                this.BindValidationToHelperText(v => v.ViewModel!.Settings, ViewModel!.Settings, vm => vm.LocalGameFilePath, v => v.tbLocalPath)
                    .DisposeWith(d);

                this.BindValidationToHelperText(v => v.ViewModel!.Settings, ViewModel!.Settings, vm => vm.SdCardGameFilePath, v => v.tbSdCardPath)
                    .DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Settings.SdCardGameFilePath, v => v.tbSdCardPath.Text)
                    .DisposeWith(d);

                this.Bind(ViewModel,
                    vm => vm.IsLoading,
                    v => v.loadBorder.Visibility,
                    vm => vm ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed,
                    v => v == System.Windows.Visibility.Visible)
                        .DisposeWith(d);
            });
        }
    }
}
