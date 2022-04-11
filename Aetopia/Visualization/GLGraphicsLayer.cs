using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
//using System.Drawing.Imaging;

namespace AE.Visualization
{
	public class GLGraphicsLayer : GraphicsLayer
	{
		///public Bitmap Image;
		public int    TexID;
		public int    TexSize;
		public int    AtlasRegion;
		///public Phase  CurrentPhase;

		public GLGraphicsLayer(Frame iOwnerFrame) : base(iOwnerFrame)
		{
			///this.Init();
		}
		
		protected override void Init()
		{
			base.Init();
			//if(this.Image != null) throw new WTFE();

			//this.SetProcessingPhase(Phase.Initial);
			this.UpdateTextureProperties(-1,-1,-1);
		}
		
		protected override void Reset()
		{
			if(this.Image != null) this.DeleteImage();
			if(this.TexID != -1)   this.DeleteTexture();

			this.Init();
		}
		

		public override void UpdateSize(int iWidth, int iHeight)
		{
			//Console.WriteLine("UpdSize");

			//if(this.Image == null)                                         throw new WTFE();
			if(iWidth == 0 || iHeight == 0)                                return;
			if(this.Image == null || (iWidth != this.Image.Width || iHeight != this.Image.Height))
			{
				var _Width = iWidth / this.OwnerFrame.Zoom;
				var _Height = iHeight / this.OwnerFrame.Zoom;

				this.DeleteImage();
				this.CreateImage(_Width, _Height);


				var _NewTexSize = GLGraphicsLayer.GetTextureSize(_Width, _Height);
				{
					if(_NewTexSize != this.TexSize)
					{
						this.DeleteTexture();
						this.CreateTexture();
					}
				}
			}
			this.Invalidate();
		}
		//public void RequestRedraw()
		//{
		//    this.CurrentState = State.Redraw;
		//}
		//public void RequestSync()
		//{
		//    this.CurrentState = State.Sync;
		//}
		//public void Ready()
		//{
		//    this.CurrentState = State.Render;
		//}
		//public void CancelUpdate()
		//{
		//    this.NeedsUpdate = false;
		//}
		//public void SyncTexture
		//public void UpdateTexture()
		//{
		//    Console.WriteLine("UpdTex");

		//    //this.DeleteTexture();
		//    //this.CreateTexture(iWidth, iHeight);

		//    if(this.TexSize == GraphicsLayer.GetTextureSize(iWidth, iHeight))
		//    {
		//        //Console.WriteLine("- x x x x x - " + iWidth + ":" + iHeight);
		//        //return;
		//    }

		//    if(this.TexID != -1)            this.DeleteTexture();
		//    if(iWidth != 0 && iHeight != 0) this.CreateTexture(iWidth, iHeight);
		//}

		
		private void CreateTexture()
		{
			var _TexID   = GL.GenTexture();
			var _TexSize = GLGraphicsLayer.GetTextureSize(this.Image.Width, this.Image.Height);
			{
				GL.BindTexture  (TextureTarget.Texture2D, _TexID);
				GL.TexImage2D   (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _TexSize, _TexSize, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
				//GL.TexImage2D   (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _TexSize, _TexSize, 0, PixelFormat.Bgra, PixelType.Bitmap, IntPtr.Zero);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
			}

			//Console.WriteLine("-CreTex : " + _TexSize);

			this.UpdateTextureProperties(_TexID, _TexSize, -1);
		}
		private void DeleteTexture()
		{
			//Console.WriteLine("-DelTex");
			
			GL.DeleteTexture(this.TexID);

			this.UpdateTextureProperties(-1,-1,-1);
		}
		
		private void UpdateTextureProperties(int iTexID, int iTexSize, int iAtlasRegion)
		{
			this.TexID       = iTexID;
			this.TexSize     = iTexSize;
			this.AtlasRegion = iAtlasRegion;
		}
		internal void UpdateTextureData()
		{
		    var _Data = this.Image.LockBits
		    (
				//new Rectangle(0, 0, this.Image.Width, 3),
				new Rectangle(0, 0, this.Image.Width, this.Image.Height),
		        System.Drawing.Imaging.ImageLockMode.ReadOnly,
				System.Drawing.Imaging.PixelFormat.Format32bppArgb
				///System.Drawing.Imaging.PixelFormat.Format32bppPArgb
		    );
		    GL.BindTexture(TextureTarget.Texture2D, this.TexID);
			
		    GL.TexSubImage2D
		    (
		        TextureTarget.Texture2D, 0,
				//0,(int)((float)DateTime.Now.Millisecond*0.3), this.Image.Width, 3,
				0,0,this.Image.Width, this.Image.Height,
		        PixelFormat.Bgra,
				PixelType.UnsignedByte,
		        _Data.Scan0
		    );
		    this.Image.UnlockBits(_Data);

		    this.SetProcessingPhase(Phase.Ready);
		}
		///internal void UpdateTextureData_FULL()
		//{
		//    var _Data = this.Image.LockBits
		//    (
		//        new Rectangle(0, 0, this.Image.Width, this.Image.Height),
		//        System.Drawing.Imaging.ImageLockMode.ReadOnly,
		//        System.Drawing.Imaging.PixelFormat.Format32bppArgb
		//        //System.Drawing.Imaging.PixelFormat.Format32bppPArgb
		//    );
		//    GL.BindTexture(TextureTarget.Texture2D, this.TexID);
			
		//    GL.TexSubImage2D
		//    (
		//        TextureTarget.Texture2D, 0,
		//        0,0, this.Image.Width, this.Image.Height,
		//        PixelFormat.Bgra,
		//        PixelType.UnsignedByte,
		//        _Data.Scan0
		//    );
		//    this.Image.UnlockBits(_Data);

		//    this.SetProcessingPhase(Phase.Ready);
		//}


		//internal void UpdateTextureData()
		//{
		//    var _Data = this.Image.LockBits(new Rectangle(0, 0, this.Image.Width, this.Image.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
		//    {
		//        GL.BindTexture(TextureTarget.Texture2D, this.TexID);
		//        GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0,0, this.Image.Width, this.Image.Height, PixelFormat.Bgra, PixelType.UnsignedByte, _Data.Scan0);

		//        this.Image.UnlockBits(_Data);
		//    }
		//    this.SetProcessingPhase(Phase.Ready);
		//}

		public static int GetTextureSize(int iWidth, int iHeight)
		{
			return (int)Math.Pow(2, Math.Ceiling(Math.Log(Math.Max(iWidth, iHeight),2)));
		}
		///public enum Phase
		//{
		//    Initial,
		//    Drawing,
		//    Synchronization,
		//    Ready,
		//}
	}
}
