using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace TimeTracking
{
    public class NewDateTimePicker : DateTimePicker
    {
        [DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        static extern Int32 SetWindowTheme(IntPtr hWnd, String textSubAppName, String textSubIdList);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);


        protected override void OnDropDown(EventArgs eventargs)
        {
            if (Application.RenderWithVisualStyles)
            {
                const int DTM_GETMONTHCAL = 0x1008;

                //Get handle of calendar control - disable theming
                IntPtr hCalendar = SendMessage(this.Handle, DTM_GETMONTHCAL, IntPtr.Zero, IntPtr.Zero);
                if (hCalendar != IntPtr.Zero)
                {
                    SetWindowTheme(hCalendar, "", "");

                    //Get handle of parent popup - resize appropriately
                    IntPtr hParent = GetParent(hCalendar);
                    if (hParent != IntPtr.Zero)
                    {
                        //The size you specify here will depend on the CalendarFont size chosen
                        MoveWindow(hParent, 0, 0, 250, 250, true);
                    }
                }
            }

            base.OnDropDown(eventargs);
        }
    }
}
