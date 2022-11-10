using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TSPWPF.ViewModel;

namespace TSPWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const uint MF_BYCOMMAND = 0x00000000;
        const uint MF_GRAYED = 0x00000001;
        const uint MF_ENABLED = 0x00000000;

        const uint SC_CLOSE = 0xF060;

        public MainWindow()
        {
            InitializeComponent();
            Grid.DataContext = new MainViewModel(this);
        }

        public void SwitchCloseButton(bool enabled)
        {
            // Disable close button
            if (!enabled)
            {
                IntPtr hwnd = new WindowInteropHelper(this).Handle;
                IntPtr hMenu = GetSystemMenu(hwnd, false);
                EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
            }
            else
            {
                IntPtr hwnd = new WindowInteropHelper(this).Handle;
                IntPtr hMenu = GetSystemMenu(hwnd, false);
                EnableMenuItem(hMenu, SC_CLOSE, MF_ENABLED);
            }
        }
    }
}