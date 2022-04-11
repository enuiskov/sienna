	using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

//namespace AE
//{
static class Imports
{
	public enum ObjectType : uint
	{
		OBJ_PEN = 1,
		OBJ_BRUSH = 2,
		OBJ_DC = 3,
		OBJ_METADC = 4,
		OBJ_PAL = 5,
		OBJ_FONT = 6,
		OBJ_BITMAP = 7,
		OBJ_REGION = 8,
		OBJ_METAFILE = 9,
		OBJ_MEMDC = 10,
		OBJ_EXTPEN = 11,
		OBJ_ENHMETADC = 12,
		OBJ_ENHMETAFILE = 13
	}

	public enum TernaryRasterOperations : uint
	{
		SRCCOPY = 0x00CC0020,
		SRCPAINT = 0x00EE0086,
		SRCAND = 0x008800C6,
		SRCINVERT = 0x00660046,
		SRCERASE = 0x00440328,
		NOTSRCCOPY = 0x00330008,
		NOTSRCERASE = 0x001100A6,
		MERGECOPY = 0x00C000CA,
		MERGEPAINT = 0x00BB0226,
		PATCOPY = 0x00F00021,
		PATPAINT = 0x00FB0A09,
		PATINVERT = 0x005A0049,
		DSTINVERT = 0x00550009,
		BLACKNESS = 0x00000042,
		WHITENESS = 0x00FF0062,
		CAPTUREBLT = 0x40000000
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Rect
	{
		public int Left, Top, Right, Bottom;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct BITMAPFILEHEADER
	{
		public ushort bfType;
		public uint bfSize;
		public ushort bfReserved1;
		public ushort bfReserved2;
		public uint bfOffBits;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct BITMAPINFOHEADER
	{
		public uint biSize;
		public int biWidth;
		public int biHeight;
		public ushort biPlanes;
		public ushort biBitCount;
		public uint biCompression;
		public uint biSizeImage;
		public int biXPelsPerMeter;
		public int biYPelsPerMeter;
		public uint biClrUsed;
		public uint biClrImportant;

		public void Init()
		{
			biSize = (uint)Marshal.SizeOf(this);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct BITMAPINFO
	{
		public uint biSize;
		public int biWidth;
		public int biHeight;
		public ushort biPlanes;
		public ushort biBitCount;
		public uint biCompression;
		public uint biSizeImage;
		public int biXPelsPerMeter;
		public int biYPelsPerMeter;
		public uint biClrUsed;
		public uint biClrImportant;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public uint[] cols;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct BITMAP
	{
		public int bmType;
		public int bmWidth;
		public int bmHeight;
		public int bmWidthBytes;
		public ushort bmPlanes;
		public ushort bmBitsPixel;
		public IntPtr bmBits;
	};

	[DllImport("user32.dll", SetLastError = true)]
	public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

	[DllImport("user32.dll")]
	public static extern IntPtr GetDC(IntPtr WindowHandle);

	[DllImport("user32.dll")]
	public static extern void ReleaseDC(IntPtr WindowHandle, IntPtr DC);

	[DllImport("user32.dll")]
	public static extern IntPtr GetWindowRect(IntPtr WindowHandle, ref Rect rect);

	[DllImport("user32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool PrintWindow(IntPtr hwnd, IntPtr DC, uint nFlags);

	[DllImport("user32.dll")]
	public static extern int SetForegroundWindow(IntPtr hWnd);

	[DllImport("user32.dll")]
	public static extern IntPtr ShowWindow(IntPtr hWnd, int nCmdShow);

	[DllImport("user32.dll")]
	public static extern int GetWindowRgn(IntPtr hWnd, IntPtr hRgn);

	[DllImport("gdi32.dll")]
	public static extern IntPtr GetCurrentObject(IntPtr DC, ObjectType uObjectType);

	[DllImport("gdi32.dll")]
	public static extern int GetObject(IntPtr hObject, int nCount, ref BITMAP lpObject);

	[DllImport("gdi32.dll", SetLastError = true)]
	public static extern IntPtr CreateCompatibleDC(IntPtr DC);

	[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
	public static extern bool DeleteDC(IntPtr DC);

	[DllImport("gdi32.dll")]
	public static extern IntPtr CreateCompatibleBitmap(IntPtr DC, int nWidth, int nHeight);

	[DllImport("gdi32.dll", ExactSpelling = true, PreserveSig = true, SetLastError = true)]
	public static extern IntPtr SelectObject(IntPtr DC, IntPtr hgdiobj);

	[DllImport("gdi32.dll")]
	public static extern bool DeleteObject(IntPtr hObject);

	[DllImport("gdi32.dll")]
	public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

	[DllImport("gdi32.dll")]
	static extern bool FillRgn(IntPtr DC, IntPtr hrgn, IntPtr hbr);

	[DllImport("gdi32.dll")]
	public static extern IntPtr CreateSolidBrush(uint crColor);

	[DllImport("gdi32.dll")]
	public static extern int GetDIBits(IntPtr DC, IntPtr hbmp, uint uStartScan, uint cScanLines, [Out] byte[] lpvBits, ref BITMAPINFO lpbmi, uint uUsage);

	[DllImport("gdi32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BitBlt(IntPtr DC, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr SrcDC, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);
}

	//using System;
	//using System.Collections.Generic;
	//using System.Drawing;
	//using System.Drawing.Imaging;
	//using System.IO;
	//using System.Linq;
	//using System.Runtime.InteropServices;
	//using System.Text;
	//using System.Windows.Forms;

	//namespace Imaging
	//{
class PBitmap
{
	private Bitmap Img = null;
	private Imports.BITMAPINFO Info;
	private Imports.BITMAPFILEHEADER bFileHeader;

	public PBitmap(IntPtr Window, int X, int Y, uint BitsPerPixel = 24)
	{
		IntPtr DC = Imports.GetDC(Window);
		Imports.BITMAP Bmp = new Imports.BITMAP();
		IntPtr hBmp = Imports.GetCurrentObject(DC, Imports.ObjectType.OBJ_BITMAP);

		if (Imports.GetObject(hBmp, Marshal.SizeOf(Bmp), ref Bmp) == 0)
			throw new Exception("ERROR_INVALID_WINDOW_HANDLE.");

		Imports.Rect BMBox = new Imports.Rect();
		Imports.GetWindowRect(Window, ref BMBox);   //GetClientRect
		int width = BMBox.Right - BMBox.Left;
		int height = BMBox.Bottom - BMBox.Top;

		IntPtr MemDC = Imports.GetDC(IntPtr.Zero);
		IntPtr SDC = Imports.CreateCompatibleDC(MemDC);
		IntPtr hSBmp = Imports.CreateCompatibleBitmap(MemDC, width, height);
		Imports.DeleteObject(Imports.SelectObject(SDC, hSBmp));

		Imports.BitBlt(SDC, 0, 0, width, height, DC, X, Y, Imports.TernaryRasterOperations.SRCCOPY);
		long size = ((width * BitsPerPixel + 31) / 32) * 4 * height;

		Info = new Imports.BITMAPINFO();
		bFileHeader = new Imports.BITMAPFILEHEADER();

		Info.biSize = (uint)Marshal.SizeOf(Info);
		Info.biWidth = width;
		Info.biHeight = height;
		Info.biPlanes = 1;
		Info.biBitCount = (ushort)BitsPerPixel;
		Info.biCompression = 0;
		Info.biSizeImage = (uint)size;
		bFileHeader.bfType = 0x4D42;
		bFileHeader.bfOffBits = (uint)(Marshal.SizeOf(bFileHeader) + Marshal.SizeOf(Info));
		bFileHeader.bfSize = (uint)(bFileHeader.bfOffBits + size);

		byte[] ImageData = new byte[size];
		Imports.GetDIBits(SDC, hSBmp, 0, (uint)height, ImageData, ref Info, 0);

		var bw = new BinaryWriter(File.Open("C:/Users/*******/Desktop/Foo.bmp", FileMode.OpenOrCreate));

		bw.Write((short)0x4D42);
		bw.Write((uint)(bFileHeader.bfOffBits + size));
		bw.Write((uint)0);
		bw.Write((uint)54);
		bw.Write((uint)Info.biSize);
		bw.Write(width);
		bw.Write(height);
		bw.Write((short)Info.biPlanes);
		bw.Write(BitsPerPixel);
		bw.Write((uint)0);
		bw.Write((uint)size);
		bw.Write((uint)0);
		bw.Write((uint)0);
		bw.Write((uint)0);
		bw.Write((uint)0);
		bw.Write(ImageData);

		bw.Flush();
		bw.Close();
		bw.Dispose();

		Imports.DeleteDC(SDC);
		Imports.DeleteObject(hSBmp);
		Imports.ReleaseDC(IntPtr.Zero, MemDC);
		Imports.ReleaseDC(Window, DC);
	}
}
public class Samples
{
	static void SampleDrawAnywhereOnScreen()
	{
	  //// Draw a 100 x 100 pixel area with black
	  //  HDC hdc = GetDC(NULL);
	  //  for (int x = 0; x < 100; x++)
	  //      for (int y = 0; y < 100; y++)
	  //          SetPixel(hdc, x+100, y+100, RGB(0, 0, 0));

	  //  ReleaseDC(NULL, hdc);

	}
}
//}

//}

