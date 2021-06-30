using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MojeFunkcjeUniwersalneNameSpace
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SYSTEMTIME
    {
        public short wYear;
        public short wMonth;
        public short wDayOfWeek;
        public short wDay;
        public short wHour;
        public short wMinute;
        public short wSecond;
        public short wMilliseconds;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public static implicit operator System.Drawing.Point(POINT p)
        {
            return new System.Drawing.Point(p.X, p.Y);
        }

        public static implicit operator POINT(System.Drawing.Point p)
        {
            return new POINT(p.X, p.Y);
        }
    }

    public enum MonitorOptions : uint
    {
        MONITOR_DEFAULTTONULL = 0x00000000,
        MONITOR_DEFAULTTOPRIMARY = 0x00000001,
        MONITOR_DEFAULTTONEAREST = 0x00000002
    }

    public static class SafeNativeMethods
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible")]
        [DllImport("user32.dll")]
        public static extern bool LockWindowUpdate(IntPtr hWndLock);

        public delegate bool Win32Callback(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.Dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr parentHandle, Win32Callback callback, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hwndChild, IntPtr hwndNewParent);

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("User32.dll")]
        public static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// Biblioteka systemowa ustawiająca ustawienie pozycji okna.
        /// </summary>
        /// <param name="hWnd">Uchwyt do okna, które należy przemieścić.</param>
        /// <param name="hWndInsertAfter">?</param>
        /// <param name="X">Punkt X rozpoczęcia osadzania okna (lewa krawędź okna).</param>
        /// <param name="Y">Punkt Y rozpoczęcia osadzania okna (górna krawędź okna).</param>
        /// <param name="cx">Szerokość okna.</param>
        /// <param name="cy">Wysokość okna.</param>
        /// <param name="uFlags">Flaga.</param>
        /// <returns>Czy udało się wykonać operację.</returns>
        /// <remarks>
        /// Należy brać pod uwagę, że pozycja okna ustawia się w odniesieniu do rodzica, do którego uchwyt został podany przy
        /// osadzaniu okna. Zatem jeśli okno jest wyświetlane wewnątrz jakiejś kontrolki to punkty X i Y odnoszą się do tej kontrolki
        /// (nie zaś do całego ekranu komputera).
        /// </remarks>
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X,
           int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public static readonly IntPtr InvalidHandleValue = IntPtr.Zero;
        public const int SW_MAXIMIZE = 3;

        /// <summary>
        /// Metoda służąca do zmiany czasu
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetSystemTime(ref SYSTEMTIME st);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();



        /// <summary>
        /// https://www.pinvoke.net/default.aspx/user32/MonitorFromPoint.html
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="dwFlags"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

        /// <summary>
        /// Wyślij wiadomość do okna
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="wMsg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        /// <summary>
        /// WM_SETREDRAW
        /// </summary>
        public const int WM_SETREDRAW = 11;

    }
}
