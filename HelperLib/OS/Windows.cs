using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows;

namespace OS
{
  public class Windows
  {
    #region Platform x86 or x64
    public enum Platform
    {
      X86,
      X64,
      Unknown
    }
    public const ushort PROCESSOR_ARCHITECTURE_INTEL = 0;
    public const ushort PROCESSOR_ARCHITECTURE_IA64 = 6;
    public const ushort PROCESSOR_ARCHITECTURE_AMD64 = 9;
    public const ushort PROCESSOR_ARCHITECTURE_UNKNOWN = 0xFFFF;

    [StructLayout(LayoutKind.Sequential)]
    public struct SYSTEM_INFO
    {
      public ushort wProcessorArchitecture;
      public ushort wReserved;
      public uint dwPageSize;
      public IntPtr lpMinimumApplicationAddress;
      public IntPtr lpMaximumApplicationAddress;
      public UIntPtr dwActiveProcessorMask;
      public uint dwNumberOfProcessors;
      public uint dwProcessorType;
      public uint dwAllocationGranularity;
      public ushort wProcessorLevel;
      public ushort wProcessorRevision;
    };

    [DllImport("kernel32.dll")]
    public static extern void GetNativeSystemInfo(ref SYSTEM_INFO lpSystemInfo);

    public static Platform GetPlatform()
    {
      SYSTEM_INFO sysInfo = new SYSTEM_INFO();
      GetNativeSystemInfo(ref sysInfo);

      switch (sysInfo.wProcessorArchitecture)
      {
        case PROCESSOR_ARCHITECTURE_AMD64:
          return Platform.X64;
        case PROCESSOR_ARCHITECTURE_INTEL:
          return Platform.X86;
        default:
          return Platform.Unknown;
      }
    }
    #endregion
  }
  public class MouseUtilities
  {
    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Point
    {
      public Int32 X;
      public Int32 Y;
    };

    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(ref Win32Point pt);

    [DllImport("user32.dll")]
    public static extern bool ScreenToClient(IntPtr hwnd, ref Win32Point pt);

    /// <summary>
    /// Returns the mouse cursor location.  This method is necessary during 
    /// a drag-drop operation because the WPF mechanisms for retrieving the
    /// cursor coordinates are unreliable.
    /// </summary>
    /// <param name="relativeTo">The Visual to which the mouse coordinates will be relative.</param>
    public static Point GetMousePosition(Visual relativeTo)
    {
      Win32Point mouse = new Win32Point();
      GetCursorPos(ref mouse);

      // Using PointFromScreen instead of Dan Crevier's code (commented out below)
      // is a bug fix created by William J. Roberts.  Read his comments about the fix
      // here: http://www.codeproject.com/useritems/ListViewDragDropManager.asp?msg=1911611#xx1911611xx
      return relativeTo.PointFromScreen(new Point((double)mouse.X, (double)mouse.Y));

      #region Commented Out
      //System.Windows.Interop.HwndSource presentationSource =
      //    (System.Windows.Interop.HwndSource)PresentationSource.FromVisual( relativeTo );
      //ScreenToClient( presentationSource.Handle, ref mouse );
      //GeneralTransform transform = relativeTo.TransformToAncestor( presentationSource.RootVisual );
      //Point offset = transform.Transform( new Point( 0, 0 ) );
      //return new Point( mouse.X - offset.X, mouse.Y - offset.Y );
      #endregion // Commented Out
    }
  }
}
