using System.ComponentModel;
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

            mApplication.Load();

            this.DataContext = mApplication;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            mApplication.Save().Wait();
        }
    }
}
