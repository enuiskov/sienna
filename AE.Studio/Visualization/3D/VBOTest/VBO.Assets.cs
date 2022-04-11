using System;
using System.Collections.Generic;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace AE.Visualization
{
	public struct XVertex
	{
		public float X;
		public float Y;
		public float Z;
		//public float W;

		public XVertex(float iX, float iY, float iZ)
		{
			this.X = iX;
			this.Y = iY;
			this.Z = iZ;
			//this.W = 0;
		}
	}
	public struct XTriangle
	{
		public XVertex V1;
		public XVertex V2;
		public XVertex V3;

		public XTriangle(XVertex iV1, XVertex iV2, XVertex iV3)
		{
			this.V1 = iV1;
			this.V2 = iV2;
			this.V3 = iV3;
		}
	}
	public struct XQuad
	{
		public XTriangle T1;
		public XTriangle T2;
		
		public XQuad(XTriangle iT1, XTriangle iT2)
		{
			this.T1 = iT1;
			this.T2 = iT2;
		}
		public static int SizeOf = System.Runtime.InteropServices.Marshal.SizeOf(new XQuad());
	}
	public partial class VBOTest : ModelViewer
	{
		public new struct Routines
		{
			public static Bitmap TexBmp;
			public static int    TexID = -1;
			public static bool   IsTexSmoothEnabled = true;
			public static bool   IsMipmapEnabled = false;

			public static void   LoadTexture(string iFileName)
			{
				TexBmp = (Bitmap)Bitmap.FromFile(iFileName);
				
				if(TexBmp.Width != TexBmp.Height) throw new Exception("WTFE");


				var _TexSize = TexBmp.Width; if((_TexSize & (_TexSize - 1)) != 0) throw new Exception("WTFE");
				


				if(TexID != -1) GL.DeleteTexture(TexID);

				TexID = GL.GenTexture();
				{
					GL.BindTexture  (TextureTarget.Texture2D, TexID);
					GL.TexImage2D   (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _TexSize, _TexSize, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);

					//GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
					//GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,  (int)All.Linear);
					GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)(IsTexSmoothEnabled ? All.Linear : All.Nearest));
					GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)(IsTexSmoothEnabled ? All.Linear : All.Nearest));
					

					if(IsMipmapEnabled)
					{
						GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, (int)All.True);
					}
					//else
					//{
						

					//    if(this.IsTexSmoothEnabled)
					//    {
							
					//    }
					//    else
					//    {
							
					//    }
					//}
					  //glTexParameteri(GL_TEXTURE_2D, GL_GENERATE_MIPMAP, GL_TRUE );






					var _Data = TexBmp.LockBits
					(
						new Rectangle(0, 0, _TexSize,_TexSize),
						System.Drawing.Imaging.ImageLockMode.ReadOnly,
						System.Drawing.Imaging.PixelFormat.Format32bppArgb
					);
					GL.TexSubImage2D
					(
						TextureTarget.Texture2D, 0,
						0,0,_TexSize, _TexSize,
						PixelFormat.Bgra,
						PixelType.UnsignedByte,
						_Data.Scan0
					);
					TexBmp.UnlockBits(_Data);
				}
			}
		}
	}
	

	public partial class VBOTest : ModelViewer
	{

	}
}