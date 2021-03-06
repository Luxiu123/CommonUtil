using CommonUITools.View;
using ModernWpf.Controls;
using NLog;
using Ookii.Dialogs.Wpf;
using System;
using System.Windows;
using System.Windows.Input;

namespace CommonUtil.View;

public sealed partial class DownloadInfoDialog : BaseDialog {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty URLProperty = DependencyProperty.Register("URL", typeof(string), typeof(DownloadInfoDialog), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty SaveDirProperty = DependencyProperty.Register("SaveDir", typeof(string), typeof(DownloadInfoDialog), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty ErrorMessageProperty = DependencyProperty.Register("ErrorMessage", typeof(string), typeof(DownloadInfoDialog), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 下载链接
    /// </summary>
    public string URL {
        get { return (string)GetValue(URLProperty); }
        set { SetValue(URLProperty, value); }
    }
    /// <summary>
    /// 保存目录，默认当前用户桌面
    /// </summary>
    public string SaveDir {
        get { return (string)GetValue(SaveDirProperty); }
        set { SetValue(SaveDirProperty, value); }
    }
    /// <summary>
    /// 错误信息提示
    /// </summary>
    public string ErrorMessage {
        get { return (string)GetValue(ErrorMessageProperty); }
        set { SetValue(ErrorMessageProperty, value); }
    }

    public DownloadInfoDialog() {
        InitializeComponent();
        SaveDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    }

    /// <summary>
    /// 选择保存目录
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ChooseSaveDirMouseUpHandler(object sender, MouseButtonEventArgs e) {
        e.Handled = true;
        var dialog = new VistaFolderBrowserDialog {
            Description = "选择保存文件夹",
            UseDescriptionForTitle = true
        };
        if (dialog.ShowDialog(Application.Current.MainWindow) == true) {
            SaveDir = dialog.SelectedPath;
        }
    }

    /// <summary>
    /// 关闭前 handler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void DialogClosingHandler(ContentDialog sender, ContentDialogClosingEventArgs args) {
        if (args.Result != ContentDialogResult.Primary) {
            return;
        }
        // 防止未及时更新
        URL = URLTextBox.Text;
        // 合法性判断
        if (string.IsNullOrEmpty(SaveDir)) {
            ErrorMessage = "保存文件夹不能为空";
            args.Cancel = true;
        } else if (string.IsNullOrEmpty(URL)) {
            ErrorMessage = "下载链接不能为空";
            args.Cancel = true;
        }
        // 清空错误消息
        if (!args.Cancel) {
            ErrorMessage = "";
        }
    }
}
