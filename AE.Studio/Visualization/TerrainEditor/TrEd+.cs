using System;
using System.Collections.Generic;
using System.ComponentModel;
//using System.Drawing;
//using System.Data;
////using System.Text;
//using System.Windows.Forms;
//using System.IO;
using OpenTK;

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

//using System.Drawing;
//using OpenTK.Input;


namespace AE.Visualization
{
	public partial class TerrainEditorFrame : ModelViewer
	{
		public int[] DisplayLists;
		//public bool  IsLightingEnabled = true;
		//public bool  IsTexturingEnabled = true;
		//public bool  IsFogEnabled = false;
		public int[] TextIds;/// = new int[]{};

		public int DetailLevel = 15;
		public float TexScale = 100f;
		public int Seed = 14;
		public Texture2D Grass;
		//public Texture2D Grass;
		//public Texture2D Grass;

		public TerrainEditorFrame()
		{
			///(G.Application as Studio).AnyZoomableFrame = this;
			///this.Viewpoint.Update(new Vector3d(5,-5,5),0);

			this.Grass = Texture2D.FromFile("Textures/Snow.jpg");

			var _View = this.Views.Perspective;
			
			_View.Target = new Vector3d(20,-20,0);
			///_View.Target = new Vector3d(50,-50,0);
			///_View.Eye    = new Vector3d(100,-50,50);

			//_View.EyeAzimuth = 180 * MathEx.DTR;
			//_View.EyeZenith = -30 * MathEx.DTR;
			_View.EyeDistance = 60;


			_View.Update();

		}
		protected override void OnKeyDown(KeyEventArgs iEvent)
		{
			base.OnKeyDown(iEvent);

			switch(iEvent.KeyCode)
			{
				case Keys.Space:
				case Keys.F5: 
				{
					Routines.Generation.Terrain(this, true);

					break;
				}
				//case Keys.F6:
				//{
				//    this.IsLightingEnabled = !this.IsLightingEnabled;
				//    //if(GL.GetInteger(GetPName.Lighting) == 1) GL.Disable(EnableCap.Lighting);
				//    //else                                      GL.Enable (EnableCap.Lighting);

				//    break;
				//}
				//case Keys.F7:
				//{
				//    this.IsTexturingEnabled = !this.IsTexturingEnabled;
				//    //if(GL.GetInteger(GetPName.Lighting) == 1) GL.Disable(EnableCap.Lighting);
				//    //else                                      GL.Enable (EnableCap.Lighting);

				//    break;
				//}
				case Keys.PageUp:   this.Seed--; Routines.Generation.Terrain(this, false); break;
				case Keys.PageDown: this.Seed++; Routines.Generation.Terrain(this, false); break;

				case Keys.Oemplus:   if(iEvent.Control) this.TexScale *= 2; else this.DetailLevel++; Routines.Generation.Terrain(this, false); break;
				case Keys.OemMinus:  if(iEvent.Control) this.TexScale /= 2; else this.DetailLevel--; Routines.Generation.Terrain(this, false); break;
			}
		}
		public override void CustomRender()
		{
			//base.Render();
			if(this.TextIds == null)
			{
				//Text
				//this.TextIds = new int[3]{GL.Ge.GetTex.GenLists(1),-1,-1};
			}

			if(this.DisplayLists == null)
			{
				Routines.Generation.Terrain(this, false);
			}
			Routines.Rendering.Draw(this);
		}
	}
}