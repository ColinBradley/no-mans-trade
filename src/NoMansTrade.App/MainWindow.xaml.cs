using System.Windows;

namespace NoMansTrade.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ViewModels.Application mApplication;

        public MainWindow()
        {
            mApplication = new ViewModels.Application();
            
            this.InitializeComponent();

            mApplication.Initialize();

            this.DataContext = mApplication;
        }
    }
}
