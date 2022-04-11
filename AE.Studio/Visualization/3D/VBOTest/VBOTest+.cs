using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace AE.Visualization
{
	public partial class VBOTest : ModelViewer
	{
		protected override void OnLoad(GenericEventArgs iEvent)
		{
			base.OnLoad(iEvent);

			
		}
		public override void CustomRender()
		{
			///base.Render();
			if(Routines.TexID == -1)
			{
				Routines.LoadTexture(@"L:\Development\!3DSMAX\Maps\celeb_0011.bmp");
			}


			//this.Views.CurrentOrthographic.Matrix
			ModelViewer.Routines.Rendering.SetProjectionMatrix(this);
			//ModelViewer.Routines.Rendering.Draw(this);

			
			
			GL.BindTexture(TextureTarget.Texture2D, Routines.TexID);
			GL.Color4(1.0f,1.0f,1.0f,1.0f);
			ModelViewer.Routines.Rendering.EnableEnvironment(false,false,false);
			//this.EnvOn();
			//this.DrawCube_Immed();

			
			this.DrawCube_VBO();
			
			ModelViewer.Routines.Rendering.DisableEnvironment();
			
			
			//GL.Disable(EnableCap.Lighting);
			//GL.BufferData
			///Test();
		}

		int[] VBOHandles = null;
		int[] DataLengths = null;

		public void InitVBO()
		{
			
		}

		int VerticesBeginAt  = -1;
		int VerticesLength   = -1;

		int NormalsBeginAt   = -1;
		int NormalsLength    = -1;

		int TexCoordsBeginAt = -1;
		int TexCoordsLength  = -1;

		public void DrawCube_VBO()
		{
			/**
				
				glGenBuffersARB(1, &vertex_buffer);
				glBindBufferARB(GL_ARRAY_BUFFER_ARB, vertex_buffer);
				glBufferDataARB(GL_ARRAY_BUFFER_ARB, size, vertices, GL_STATIC_DRAW_ARB);

				glGenBuffersARB(1, &texcoord_buffer);
				glBindBufferARB(GL_ARRAY_BUFFER_ARB, texcoord_buffer);
				glBufferDataARB(GL_ARRAY_BUFFER_ARB, size, texcoords, GL_STATIC_DRAW_ARB);

				then this when you want to use the buffers:

				glBindBufferARB(GL_ARRAY_BUFFER_ARB, vertex_buffer);
				glVertexPointer(3, GL_FLOAT, 0, 0);
				glEnableClientState(GL_VERTEX_ARRAY);

				glBindBufferARB(GL_ARRAY_BUFFER_ARB, texcoord_buffer);
				glTexCoordPointer(3, GL_FLOAT, 0, 0);
				glEnableClientState(GL_TEXTURE_COORD_ARRAY);
			
			*/



			/**
				BETTER	
				------------------------------------


				Report postPosted September 1, 2003
				It is not necessary to create that many VBOs. Remember that there is overhead associated
				with each VBO. Just bind the buffer and call BufferData on the vertex data then just call
				it again with the texcoord data:



				glGenBuffersARB(1, &only_need_one_buffer);
				glBindBufferARB(GL_ARRAY_BUFFER_ARB, only_need_one_buffer);
				glBufferDataARB(GL_ARRAY_BUFFER_ARB, size, vertices, GL_STATIC_DRAW_ARB);
				glBufferDataARB(GL_ARRAY_BUFFER_ARB, size, texcoords, GL_STATIC_DRAW_ARB);

				then this when you want to use the buffers:

				glBindBufferARB(GL_ARRAY_BUFFER_ARB, only_need_one_buffer);
				glVertexPointer(3, GL_FLOAT, 0, 0);
				glEnableClientState(GL_VERTEX_ARRAY);
				glTexCoordPointer(3, GL_FLOAT, 0, 0);
				glEnableClientState(GL_TEXTURE_COORD_ARRAY); 
			
			
			*/


			if(VBOHandles == null)
			{
				//Инициализация VBO - делается единожды, при старте программы
				//Создание переменной для хранения идентификатора VBO
				 //triangleVBO;

				//Вершины треугольника (в обходе против часовой стрелки)
				//Vertex[] _Data = new Vertex[]
				//{
				//   new Vertex( 1.0f, 0.0f,  1.0f),
				//   new Vertex( 0.0f, 0.0f, -1.0f),
				//   new Vertex(-1.0f, 0.0f,  1.0f)
				//};
				

				var _Size = 100;
				var _U    = 0.1f; ///~~ some offset unit;
				var _Us   = _U * 0.0f;
				var _Rng  = new Random(0);

				var _Vertices  = new XQuad[_Size * _Size];
				var _TexCoords = new float[_Size * _Size * 2 * 3 * 2];
				var _Normals   = new float[_Vertices.Length * 2 * 3 * 3];
				{
					for(var cVi = 0; cVi < _TexCoords.Length; cVi++)
					{
						_TexCoords[cVi] = (float)_Rng.NextDouble();
					}
					for(var cVi = 0; cVi < _Normals.Length; cVi += 3)
					{
						_Normals[cVi    ] = ((float)_Rng.NextDouble() / 2 - 0.25f) * 1f;
						_Normals[cVi + 1] = ((float)_Rng.NextDouble() / 2 - 0.25f) * 1f;
						_Normals[cVi + 2] = 1f;

						//_Normals[cVi    ] = 1f;
						//_Normals[cVi + 1] = 0;
						//_Normals[cVi + 2] = 1f;
					}
					for(var cRi = 0; cRi < _Size; cRi ++)
					{
						var cRowOffs = cRi * _Size;
						var cX = cRi * _U;

						for(var cCi = 0; cCi < _Size; cCi ++)
						{
							//var _ArrayIndex = cRowOffs + cCi;
							var cY = cCi * _U;

							var _ArrayOffset = cRowOffs + cCi;

							_Vertices[_ArrayOffset] = new XQuad
							(
								new XTriangle
								(
									new XVertex(cX + _Us,       cY + _Us,      0f),
									new XVertex(cX + _U - _Us,  cY + _Us,      0f),
									new XVertex(cX + _Us,       cY + _U - _Us, 0f)
								),
								new XTriangle
								(
									new XVertex(cX + _U - _Us,  cY + _Us ,      0f),
									new XVertex(cX + _U - _Us,  cY + _U - _Us, 0f),
									new XVertex(cX + _Us,       cY + _U - _Us, 0f)
								)
							);
						}
					}
				}


				//Создание нового VBO и сохранение идентификатора VBO
				this.VBOHandles = new int[1];
				///this.DataLengths = new int[]{_Vertices.Length, _Normals.Length,  _TexCoords.Length}; ///~~ triangle count needed for drawarrays;

				this.VerticesBeginAt = 0;
				this.VerticesLength = _Vertices.Length * XQuad.SizeOf;

				this.NormalsBeginAt = this.VerticesBeginAt + this.VerticesLength;
				this.NormalsLength  = _Normals.Length * 4;

				this.TexCoordsBeginAt = this.NormalsBeginAt + this.NormalsLength;
				this.TexCoordsLength  = _TexCoords.Length * 4;

				GL.GenBuffers(1, this.VBOHandles);

				//Установка активности VBO
				GL.BindBuffer(BufferTarget.ArrayBuffer, VBOHandles[0]);

				//Выгрузка данных вершин в видеоустройство

				
				
				GL.BufferData<XQuad>(BufferTarget.ArrayBuffer,    (IntPtr)(this.VerticesLength + this.NormalsLength + this.TexCoordsLength), _Vertices, BufferUsageHint.StaticDraw);
				///GL.BufferData(BufferTarget.ArrayBuffer, IntPtr.Zero, IntPtr.Zero, BufferUsageHint.StaticDraw);

				GL.BufferSubData<XQuad>(BufferTarget.ArrayBuffer, (IntPtr)this.VerticesBeginAt,  (IntPtr)this.VerticesLength,  _Vertices);
				GL.BufferSubData(BufferTarget.ArrayBuffer,       (IntPtr)this.NormalsBeginAt,   (IntPtr)this.NormalsLength,   _Normals);
				GL.BufferSubData(BufferTarget.ArrayBuffer,       (IntPtr)this.TexCoordsBeginAt, (IntPtr)this.TexCoordsLength, _TexCoords);
			}

			GL.BindBuffer(BufferTarget.ArrayBuffer, this.VBOHandles[0]);

			GL.VertexPointer(3, VertexPointerType.Float, 0, this.VerticesBeginAt);   
			GL.EnableClientState(ArrayCap.VertexArray);

			GL.NormalPointer(NormalPointerType.Float, 0, this.NormalsBeginAt);
			GL.EnableClientState(ArrayCap.NormalArray);

			GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, this.TexCoordsBeginAt);
			GL.EnableClientState(ArrayCap.TextureCoordArray);

			//Рисование треугольника, указывая количества вершин
			///GL.DrawArrays(PrimitiveType.Triangles, 0, this.DataLengths[0] / sizeof(float) / 3);

			///GL.Color4(this.Palette.Adapt(new CHSAColor(0.7f,6)));
			///GL.DrawArrays(PrimitiveType.Lines, 0, this.DataLengths[0] * 6 / 1);///this.DataLengths[0]);

			GL.Enable(EnableCap.Texture2D);
			GL.Enable(EnableCap.CullFace);
			///GL.Normal3(0,0,0.1f);
			GL.DrawArrays(PrimitiveType.Triangles, 0, this.VerticesLength / XQuad.SizeOf * 6);///this.DataLengths[0]);
			GL.Disable(EnableCap.CullFace);
			GL.Disable(EnableCap.Texture2D);

			//Указание отобразить нарисованное немедленно
			///GL.Flush();


		}
		public void DrawCube_Immed()
		{
			GL.Enable(EnableCap.Texture2D);
			//GL.Color4(this.Palette.Adapt(CHSAColor.Glare));
			GL.Color4(1.0f,1.0f,1.0f,1.0f);

			GL.Begin(PrimitiveType.Quads);
			{
				//GL.TexCoord2(0f,0f); GL.Vertex3(+1,+1,0);
				//GL.TexCoord2(0f,1f); GL.Vertex3(+1,-1,0);
				//GL.TexCoord2(1f,1f); GL.Vertex3(-1,-1,0);
				//GL.TexCoord2(1f,0f); GL.Vertex3(-1,+1,0);

				GL.Normal3(0,0,0.1); GL.TexCoord2(1f,0f); GL.Vertex3(+1,+1,0);
				GL.Normal3(0,0,0.1); GL.TexCoord2(1f,1f); GL.Vertex3(+1,-1,0);
				GL.Normal3(0,0,0.1); GL.TexCoord2(0f,1f); GL.Vertex3(-1,-1,0);
				GL.Normal3(0,0,0.1); GL.TexCoord2(0f,0f); GL.Vertex3(-1,+1,0);
			}
			GL.End();
			GL.Disable(EnableCap.Texture2D);

		}
	}
}