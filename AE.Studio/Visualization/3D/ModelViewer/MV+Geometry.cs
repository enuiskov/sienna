using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace AE.Visualization
{
	public class AAA
	{
		//Vbo LoadVBO<TVertex>(TVertex[] vertices, short[] elements) where TVertex : struct
		//  {
		//      Vbo handle = new Vbo();
		//      int size;

		//      // To create a VBO:
		//      // 1) Generate the buffer handles for the vertex and element buffers.
		//      // 2) Bind the vertex buffer handle and upload your vertex data. Check that the buffer was uploaded correctly.
		//      // 3) Bind the element buffer handle and upload your element data. Check that the buffer was uploaded correctly.

		//      GL.GenBuffers(1, out handle.VboID);
		//      GL.BindBuffer(BufferTarget.ArrayBuffer, handle.VboID);
		//      GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * BlittableValueType.StrideOf(vertices)), vertices,
		//                    BufferUsageHint.StaticDraw);
		//      GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
		//      if (vertices.Length * BlittableValueType.StrideOf(vertices) != size)
		//          throw new ApplicationException("Vertex data not uploaded correctly");

		//      GL.GenBuffers(1, out handle.EboID);
		//      GL.BindBuffer(BufferTarget.ElementArrayBuffer, handle.EboID);
		//      GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(elements.Length * sizeof(short)), elements,
		//                    BufferUsageHint.StaticDraw);
		//      GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
		//      if (elements.Length * sizeof(short) != size)
		//          throw new ApplicationException("Element data not uploaded correctly");

		//      handle.NumElements = elements.Length;
		//      return handle;
		//  }

		//  void Draw(Vbo handle)
		//  {
		//      // To draw a VBO:
		//      // 1) Ensure that the VertexArray client state is enabled.
		//      // 2) Bind the vertex and element buffer handles.
		//      // 3) Set up the data pointers (vertex, normal, color) according to your vertex format.
		//      // 4) Call DrawElements. (Note: the last parameter is an offset into the element buffer
		//      //    and will usually be IntPtr.Zero).

		//      GL.EnableClientState(ArrayCap.ColorArray);
		//      GL.EnableClientState(ArrayCap.VertexArray);

		//      GL.BindBuffer(BufferTarget.ArrayBuffer, handle.VboID);
		//      GL.BindBuffer(BufferTarget.ElementArrayBuffer, handle.EboID);

		//      GL.VertexPointer(3, VertexPointerType.Float, BlittableValueType.StrideOf(CubeVertices), new IntPtr(0));
		//      GL.ColorPointer(4, ColorPointerType.UnsignedByte, BlittableValueType.StrideOf(CubeVertices), new IntPtr(12));

		//      GL.DrawElements(PrimitiveType.Triangles, handle.NumElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
		//  }

	}
}