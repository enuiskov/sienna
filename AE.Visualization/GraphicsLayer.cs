using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace AE.Visualization
{
	public class GraphicsLayer
	{
		public Frame           OwnerFrame;
		public Bitmap          Image;
		public GraphicsContext Context;
		public Phase           CurrentPhase;


		public GraphicsLayer(Frame iOwnerFrame)
		{
			this.OwnerFrame = iOwnerFrame;
			
			this.Init();
		}
		
		protected virtual void Init()
		{
			if(this.Image != null) throw new Exception("WTFE");
			
			this.SetProcessingPhase(Phase.Initial);
		}
		
		protected virtual void Reset()
		{
			if(this.Image != null)
			{
				this.DeleteImage();
				//this.CreateImage();
			}
			this.Init();
		}
		

		public virtual void UpdateSize(int iWidth, int iHeight)
		{
			//Console.WriteLine("UpdSize");

			//if(this.Image == null)                                         throw new WTFE();
			if(iWidth == 0 || iHeight == 0)                                return;
			if(this.Image == null || (iWidth != this.Image.Width || iHeight != this.Image.Height))
			{
				this.DeleteImage();
				this.CreateImage(iWidth / this.OwnerFrame.Zoom, iHeight / this.OwnerFrame.Zoom);
			}
			this.Invalidate();
		}
		public void Invalidate()
		{
			this.SetProcessingPhase(Phase.Drawing);
		}
		public void SetProcessingPhase(Phase iPhase)
		{
			this.CurrentPhase = iPhase;
		}
		

		protected virtual void CreateImage(int iWidth, int iHeight)
		{
			if(this.Image != null) throw new Exception("WTFE");

			this.Image   = new Bitmap(iWidth, iHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
			this.Context = new GraphicsContext(this.Image, GdiColorPalette.Default);
		}
		protected virtual void DeleteImage()
		{
			if(this.Image == null) return;

			this.Context.Dispose(); this.Context = null;
			this.Image  .Dispose(); this.Image   = null;
		}

		public enum Phase
		{
			Initial,
			Drawing,
			Synchronization,
			Ready,
		}
	}
}
