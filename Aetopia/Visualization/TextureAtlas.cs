using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace AE.Visualization
{
	/*
	================================================================================
		class ExampleFrame
		{
			public void AllocateRegions()
			{
				var _AllFrames = new List<Frame>();
				{
					///_AllFrames.Add(this.Frame);
					_AllFrames.AddRange(this.Frame.GetAllChildFrames());
				}
				
				var _Sizes   = new Size [_AllFrames.Count];
				var _Indices = new Int32[_Sizes.Length];
				{
					for(var cFi = 0; cFi < _AllFrames.Count; cFi++)
					{
						if(_AllFrames[cFi].Image == null) continue;
						_Sizes[cFi] = _AllFrames[cFi].Image.Size;
					}
				}
				this.FramesAtlas.AllocateRegions(_Sizes, this.FramesAtlas.TestSize);
			}
		}
	================================================================================
	*/

	public class RegionInfo
	{
		public Rectangle Bounds;
		public int       Index;
		public bool      IsAllocated;
	}
	public class TextureAtlas
	{
		public int          Size;
		public RegionInfo[] Regions;
		public Bitmap       Texture;
		public int          TestSize = 16;//256;//2048;//4196;//1024;//512;//2048;//512;//1024;//2048;//512;
		
		public TextureAtlas()
		{
			this.Size    = 0;
			this.Regions = null;
			this.Texture = null;
		}

		
		public void AllocateRegions(Size[] iSizes, int iAtlasSize)
		{
			if(iSizes.Length == 0)
			{
				this.Regions = null;
				this.Size    = -1;
				this.Texture = null;

				return;
			}

			var _Regions = new RegionInfo[iSizes.Length];
			{
				for(var cSi = 0; cSi < iSizes.Length; cSi++)
				{
					_Regions[cSi] = new RegionInfo{Bounds = new Rectangle(Point.Empty, iSizes[cSi]), Index = cSi, IsAllocated = false};
				}
			}
			Array.Sort<RegionInfo>(_Regions, new Comparison<RegionInfo>(this.CompareAreas));


			var _AtlasRect  = new Rectangle(0,0,iAtlasSize,iAtlasSize);
			var _MaxSize   = 0;

			//var _Rects = new List<Rectangle>(_Regions.Length);
			//{
			var _FirstRegion = _Regions[0];
			{
				if(_AtlasRect.Contains(_FirstRegion.Bounds))
				{
					_MaxSize = Math.Max(_FirstRegion.Bounds.Width,_FirstRegion.Bounds.Height);

					_FirstRegion.IsAllocated = true;
					//_Rects.Add(_FirstRect);
					//cSizeIsAccepted = true;
				}
			}
			if(_Regions.Length > 1)
			{
				for(var cRi = 1; cRi < _Regions.Length; cRi++)
				{
					Point _LeastPoint = new Point(Int32.MaxValue, Int32.MaxValue);
					//var cRegionIsAccepted = false;

					var cxRegion = _Regions[cRi];
					
					foreach(var cyRegion in _Regions)
					{
						if(!cyRegion.IsAllocated) continue;

						for(var cPass = 0; cPass <= 1; cPass++)
						{
							/*
								1 - RT, 2 - LB, 3,4 - RT,LB (90 deg)
							*/
							//???
							var cPoint = cPass == 0 ? new Point(cyRegion.Bounds.Right,cyRegion.Bounds.Top) : new Point(cyRegion.Bounds.Left,cyRegion.Bounds.Bottom);
							var cRect = new Rectangle(cPoint, cxRegion.Bounds.Size);

							if(!_AtlasRect.Contains(cRect)) continue;

							var cIsIntersects = false;
							{
								//foreach(var cyRect in oRects)
								foreach(var czRegion in _Regions)
								{
									//if(!cyRegion.IsAllocated) continue;
									if(!czRegion.IsAllocated) continue;


									if(cRect.IntersectsWith(czRegion.Bounds))
									{
										cIsIntersects = true;
										break;
									}
								}
							}

							if(!cIsIntersects)
							{
								if(_LeastPoint.X == Int32.MaxValue) _LeastPoint = cPoint;
								else
								{
									var _LeastRB = Point.Add(_LeastPoint, cxRegion.Bounds.Size);
									var cRectRB  = Point.Add(cPoint,      cxRegion.Bounds.Size);

									if(Math.Max(cRectRB.X, cRectRB.Y) < Math.Max(_LeastRB.X, _LeastRB.Y))
									{
										_LeastPoint = cPoint;
									}
								}

								cxRegion.IsAllocated = true;
								//cRegionIsAccepted = true;
							}
						}
					}
					//if(cRegionIsAccepted)
					if(cxRegion.IsAllocated)
					{
						cxRegion.Bounds.Location = _LeastPoint;
						_MaxSize = Math.Max(_MaxSize, Math.Max(_LeastPoint.X, _LeastPoint.Y));
						//oRects.Add(new Rectangle(_LeastPoint, cxRegion.Bounds.Size));
						
					}
				}
			}
			Array.Sort<RegionInfo>(_Regions, new Comparison<RegionInfo>(this.CompareIndices));

			this.Regions = _Regions;


			

				
				//foreach(var cRegI in _RegII)
				//{
					
					

				//    if(!cSizeIsAccepted)
				//    {
				//        throw new Exception();
				//    }
				//}
			//}
			//Console.WriteLine("MaxSize: " + _MaxSize + ", TxLim: " + Math.Pow(2, Math.Ceiling(Math.Log(_MaxSize,2))));

			this.Size    = (int)Math.Pow(2, Math.Ceiling(Math.Log(_MaxSize,2)));
			//this.Regions = 

			//return oRects.ToArray();
		}
		private int CompareAreas(RegionInfo ixInfo, RegionInfo iyInfo)
		{
			var _xArea = ixInfo.Bounds.Width * ixInfo.Bounds.Height;
			var _yArea = iyInfo.Bounds.Width * iyInfo.Bounds.Height;

			return -_xArea.CompareTo(_yArea);
		}
		private int CompareIndices(RegionInfo ixInfo, RegionInfo iyInfo)
		{
			return ixInfo.Index.CompareTo(iyInfo.Index);
		}
	}
}
