using LiteLoaderFixerR.Utils;
using PeNet;
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

        private void BrowseQQ_Click(object sender, RoutedEventArgs e)
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

        private void FixQQ_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ElectronPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            var qq_exe_path = System.IO.Path.Combine(QQPath.Text, "QQ.exe");
            var code = new FunctionEngine(new PeFile(ElectronPath.Text));
            var qq = new FunctionEngine(new PeFile(qq_exe_path));

            var signatures = code.GetFunctionSignature("CompileFunctionInternal", 115);

            if (signatures == null)
            {
                Status.Content = "未就绪：无法找到关键函数";
                FixQQ.IsEnabled = false;
                return;
            }

            foreach (var signature in signatures)
            {
                var rvas = qq.GetFunctionRva(signature.Signature);

                if (rvas == null)
                {
                    continue;
                }

                if (rvas.Count == 1)
                {
                    Status.Content = "就绪";
                    FixQQ.IsEnabled = true;
                    return;
                }
            }

            Status.Content = "未就绪：无法匹配关键函数";
            FixQQ.IsEnabled = false;
        }

        private void BrowseElectron_Click(object sender, RoutedEventArgs e)
        {
            var select_electron_dialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "Electron 程序|*.exe"
            };

            var result = select_electron_dialog.ShowDialog();
            if (result == true)
            {
                ElectronPath.Text = select_electron_dialog.FileName;
            }
        }
    }
}