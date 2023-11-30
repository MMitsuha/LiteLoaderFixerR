using System.Diagnostics;
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

namespace LiteLoaderFixerR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            QQPath.Text = Utils.QQ.GetQQLocation();
        }

        private void QQPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            var qq_exe_path = System.IO.Path.Combine(QQPath.Text, "QQ.exe");
            var version = FileVersionInfo.GetVersionInfo(qq_exe_path);

            QQVersion.Text = version.FileVersion;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var select_qq_dialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "QQ 主程序|QQ.exe"
            };

            var result = select_qq_dialog.ShowDialog();
            if (result == true)
            {
                QQPath.Text = System.IO.Path.GetDirectoryName(select_qq_dialog.FileName);
            }
        }
    }
}