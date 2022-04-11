using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
///using DocList  = System.Collections.Generic.List<AE.Visualization.TextEditorGdiFrame.TextDocument>;

namespace AE.Visualization
{
	public partial class TextEditorGdiFrame : Frame
	{
		///public DocList      Documents;
		///public TextDocument CurrentDocument {get{return this.Documents[this.CurrentDocumentIndex];}}
		private int         CurrentDocumentIndex = 0;///////
		public Size         BufferSize;

		public Image SampleImage = Bitmap.FromFile(@"D:\Documents and Settings\EN\Мои документы\Мои рисунки\315px-International_Morse_Code_svg.png");
	
		///public TextEditorGdiFrame()
		//{
		//    this.Documents = new List<TextDocument>();
		//    {
		//        var _Doc1 = new TextDocument(this);
		//        {
		//            _Doc1.ReadString(System.IO.File.ReadAllText(@"L:\Development\Sienna\Software\AE.Studio\bin\Debug\0.src", Encoding.Default));
		//        }
		//        this.Documents.Add(_Doc1);
		//    }
		//}
		///public void ScrollBy(int iColDelta, int iRowDelta)
		//{
		//    var _CurrOffset = this.CurrentDocument.Scroll.Offset;

		//    if(iRowDelta != 0 && this.CurrentDocument.Rows.Count > this.BufferSize.Height)
		//    {
		//        var _TgtOffsY     = _CurrOffset.Y + iRowDelta;
		//        var _LimOffsY     = MathEx.Clamp(_TgtOffsY, 0, this.CurrentDocument.Rows.Count - this.BufferSize.Height);
		//        var _LimRowDelta  = iRowDelta - (_TgtOffsY - _LimOffsY);

		//        this.CurrentDocument.Scroll.Offset.Y = _LimOffsY;
		//        this.RotateBuffer(_LimRowDelta);

		//        int _FrBufRowI, _ToBufRowI, _FrDocRowI;
		//        {
		//            if(iRowDelta > 0)
		//            {
		//                _FrBufRowI = this.BufferSize.Height - _LimRowDelta;
		//                _ToBufRowI = this.BufferSize.Height;

		//                _FrDocRowI = _CurrOffset.Y + this.BufferSize.Height;
		//            }
		//            else
		//            {
		//                _FrBufRowI = 0;
		//                _ToBufRowI = -_LimRowDelta;

		//                _FrDocRowI = _CurrOffset.Y + _LimRowDelta;
		//            }
		//        }
				
		//        for(int cBufRi = _FrBufRowI, cDocRi = _FrDocRowI; cBufRi < _ToBufRowI; cBufRi++, cDocRi++)
		//        {
		//            var cCells = this.CurrentDocument.Rows[cDocRi].Chars;
		//                cCells = cCells.GetRange(0, Math.Min(cCells.Count, this.BufferSize.Width - this.CurrentDocument.LineNumberOffset) );

		//            this.UpdateRow(cCells, cDocRi + 1, cBufRi);
		//        }
		//    }
		//    else if(iColDelta != 0)
		//    {
			
		//    }
		//    //else throw new WTFE();

		//    this.Canvas.Invalidate();
		//}
		///public void ScrollToCursor()
		//{
		//    var _VScrollDelta =
		//    (
		//        Math.Min(this.CurrentDocument.Cursor.Position.Y - this.CurrentDocument.Scroll.Offset.Y, 0)
		//        + 
		//        Math.Max(this.CurrentDocument.Cursor.Position.Y - (this.CurrentDocument.Scroll.Offset.Y + this.BufferSize.Height - 1), 0)
		//    );

		//    if(_VScrollDelta != 0) this.ScrollBy(0, _VScrollDelta);
		//}

		public override void DrawForeground(GraphicsContext iGrx)
		{
			///base.DrawForeground(iGrx);

			///Routines.Drawing.DrawText(this, iGrx);

			//for(var cY = 0; cY < 500; cY += 10)
			//{
			//    for(var cX = 0; cX < 500; cX += 10)
			//    {
			//        var cSrcRect = new Rectangle(150,150,10,10);
			//        var cDstRect = new Rectangle(cX,cY, 10,10);

			//        ///iGrx.Device.DrawImageUnscaled(this.SampleImage, cDstRect, cSrcRect, GraphicsUnit.Pixel);
			//        iGrx.Device.DrawImageUnscaled(this.SampleImage, cDstRect);
			//    }
			//}

			

			////iGrx.Device.CopyFromScreen
			// iGrx.Device.FillEllipse(Brushes.DarkBlue, new
			//    Rectangle(10,10,60,60));
			//iGrx.Device.FillRectangle(Brushes.Khaki, new
			//    Rectangle(20,30,60,10));
			//iGrx.Device.CopyFromScreen(new Point(0, 0), new Point(0, 0), 
			//    new Size(1024, 768));

		}
	}
}
