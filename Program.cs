using System;
using System.Windows.Forms;

namespace KRFrameViewer
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new KRFrameViewer());
        }
    }
}