using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace RemovePoETitleBorders
{
    class Program
    {
        //Finds a window by class name
        [DllImport("USER32.DLL")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        //Sets a window to be a child window of another window
        [DllImport("USER32.DLL")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        //Sets window attributes
        [DllImport("USER32.DLL")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        //Gets window attributes
        [DllImport("USER32.DLL")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        
        [DllImport ("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);
        
        //assorted constants needed
        public static int GWL_STYLE = -16;
        public static int WS_CHILD = 0x40000000; //child window
        public static int WS_BORDER = 0x00800000; //window with border
        public static int WS_DLGFRAME = 0x00400000; //window with double border but no title
        public static int WS_CAPTION = WS_BORDER | WS_DLGFRAME; //window with a title bar

        static void Main(string[] args)
        {
            Thread.Sleep(2000);
            Process[] Procs = Process.GetProcesses();
            foreach (Process proc in Procs)
            {
                if (proc.ProcessName.StartsWith("PathOfExile"))
                {
                    IntPtr pFoundWindow = proc.MainWindowHandle;
                    int style = GetWindowLong(pFoundWindow, GWL_STYLE);
                    SetWindowLong(pFoundWindow, GWL_STYLE, (style & ~WS_CAPTION));
                    Thread.Sleep(500);
                    var simulator = new InputSimulator();
                    ActivateWindow(pFoundWindow);
                    simulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.LWIN, VirtualKeyCode.UP);
                    Thread.Sleep(250);
                    ActivateWindow(pFoundWindow);
                    simulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.LWIN, VirtualKeyCode.DOWN);
                    Thread.Sleep(250);
                    ActivateWindow(pFoundWindow);
                    simulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.LWIN, VirtualKeyCode.UP);
                    Thread.Sleep(250);
                }
            }
        }

        private static void ActivateWindow(IntPtr handle)
        {
            while (GetForegroundWindow() != handle)
            {
                Thread.Sleep(250);
                SetForegroundWindow(handle);
            }
        }
    }
}