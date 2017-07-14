using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Matbot.Commands
{
    class ScreenSaverCommand : Structure.Command
    {
        int WM_SYSCOMMAND = 0x112;
        int SC_MONITORPOWER = 0xF170;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(int hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern void mouse_event(Int32 dwFlags, Int32 dx, Int32 dy, Int32 dwData, UIntPtr dwExtraInfo);

        private const int MOUSEEVENTF_MOVE = 0x0001;

        private void Wake()
        {
            mouse_event(MOUSEEVENTF_MOVE, 0, 1, 0, UIntPtr.Zero);
            mouse_event(MOUSEEVENTF_MOVE, 0, -1, 0, UIntPtr.Zero);
        }

        public enum MonitorState
        {
            on = -1,
            off = 2,
            standby = 1
        }




        public ScreenSaverCommand() : base("screensaver")
        {
            RequiredRank = UserRank.Admin;
        }

        public override void Execute(Message message)
        {
            SendMessage(0xFFFF, WM_SYSCOMMAND, (IntPtr)SC_MONITORPOWER, (IntPtr)(MonitorState.off));
        }

        public void Execute(Message message, MonitorState state)
        {
            if(state != MonitorState.on) SendMessage(0xFFFF, WM_SYSCOMMAND, (IntPtr)SC_MONITORPOWER, (IntPtr)(MonitorState.off));
            else
            {
                Wake();
            }
        }
    }
}
