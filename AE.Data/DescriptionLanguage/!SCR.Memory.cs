using System;
using System.Collections.Generic;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Reflection;
//using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace AE.Data.DescriptionLanguage
{
	public class MemoryItemInfo
	{
		public string Name;
		public Type   Type;
		///private object Value;
		public string FriendlyValue;
		public int    BeginOffset;
		public int    EndOffset;
		public int    Length;

		//public string GetValueString()
		//{
		//    throw new NotImplementedException();
		//}
	}
	public class UnmanagedMemory : IDisposable
	{
		public IntPtr UnmanagedPointer;
		public int    Size;

		public        UnmanagedMemory(int iSize)
		{
			this.UnmanagedPointer = Marshal.AllocHGlobal(iSize);
			this.Size = iSize;

			this.Reset(true);
		}
		public void   Reset(bool iDoZeroContent)
		{
			if(iDoZeroContent)
			{
				for(var cO = 0; cO < this.Size; cO ++)
				{
					this.WriteByte(cO,0);
				}
			}
		}

		
		//public byte   GetByte()
		//{
		//    return this.GetByte(this.Pointer);
		//}
		public byte   ReadByte(int iOffset)
		{
			return Marshal.ReadByte(this.UnmanagedPointer, iOffset);
		}
		
		public byte   ReadBytes(int iOffset, int iCount)
		{
			throw new NotImplementedException();
			//return Marshal.Copy(this.UnmanagedBasePointer, iOffset);
		}
		//public int    GetInt32()
		//{
		//    return this.GetInt32(this.Pointer);
		//}
		public int    ReadInt32(int iOffset)
		{
			return Marshal.ReadInt32(this.UnmanagedPointer, iOffset);
		}


		public ushort ReadUInt16(int iOffset)
		{
			throw new NotImplementedException();
		}
		public uint   ReadUInt32(int iOffset)
		{
			throw new NotImplementedException();
		}
		

		public void   WriteBytes  (int iOffset, byte[] iValues)
		{
			//Marshal.SizeOf(iValues[0]);
			
			Marshal.Copy(iValues, 0, new IntPtr(this.UnmanagedPointer.ToInt32() + iOffset), iValues.Length);
			///throw new NotImplementedException();
		}
		public void   WriteByte   (int iOffset, byte iValue)
		{
			Marshal.WriteByte(this.UnmanagedPointer, iOffset, iValue);
			///throw new NotImplementedException();
		}
		public void   WriteSByte  (int iOffset, sbyte iValue)
		{
			throw new NotImplementedException();
		}
		public void   WriteInt16  (int iOffset, Int16 iValue)
		{
			throw new NotImplementedException();
		}
		public void   WriteUInt16 (int iOffset, UInt16 iValue)
		{
			throw new NotImplementedException();
		}
		public void   WriteInt32  (int iOffset, Int32 iValue)
		{
			Marshal.WriteInt32(this.UnmanagedPointer, iOffset, iValue);
			//throw new NotImplementedException();
		}
		public void   WriteUInt32 (int iOffset, UInt32 iValue)
		{
			throw new NotImplementedException();
		}
		public void   WriteInt64  (int iOffset, Int64 iValue)
		{
			throw new NotImplementedException();
		}
		public void   WriteUInt64 (int iOffset, UInt64 iValue)
		{
			throw new NotImplementedException();
		}
		
		#region Члены IDisposable

		public void Dispose()
		{
			Marshal.FreeHGlobal(this.UnmanagedPointer);
			Console.WriteLine("DISPOSED");
		}

		#endregion
	}
	public class DescribedMemory : UnmanagedMemory
	{
		public Dictionary<int,MemoryItemInfo> DescriptionMap;
 
		public DescribedMemory(int iSize) : base(iSize)
		{
			this.DescriptionMap = new Dictionary<int,MemoryItemInfo>();
		}
		public void AddDescription(object iDescription, int iOffset, int iLength)
		{
			throw new NotImplementedException();
		}
		public void RemoveDescription(int iOffset)
		{
			throw new NotImplementedException();
		}
		public void RemoveDescription(int iOffset, int iLength, bool iDoCheck)
		{
			throw new NotImplementedException();
		}
		public void RemoveDescriptions(int iOffset, int iLength)
		{
			///~~ raise an exception if specified range doesn't exactly match whole chunks;

			throw new NotImplementedException();
		}
	}
	public class ByteStack
	{
		public UnmanagedMemory Memory;
		public int             BaseOffset;
		public int             Length;
		public int             Pointer;
		public int             MinAddress {get{return this.BaseOffset - this.Length;}}
		//public int             MemoryPointer {get{return this.BaseOffset - this.Pointer;}set{this.Pointer = this.BaseOffset - value;}}
		public byte[]          AsUInt8
		{
			get
			{
				var oValues = new byte[Math.Min(this.Length, Int32.MaxValue)];
				{
					for(var cVi = 0; cVi < oValues.Length; cVi++)
					{
						oValues[cVi] = this.GetByte(cVi * 1);
					}
				}
				return oValues;
			}
		}
		public int[]           AsInt32
		{
			get
			{
				var oValues = new Int32[Math.Min(this.Length / 4, Int32.MaxValue)];
				{
					for(var cVi = 0; cVi < oValues.Length; cVi++)
					{
						oValues[cVi] = this.GetInt32(cVi * 4);
					}
				}
				return oValues;
			}
		}

		///[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		//public IntPtr UnmanagedBasePointer;
		//public IntPtr UnmanagedPointer;

		
		
		//public UnmanagedMemoryStream Data;
		//public int Pointer {get{return this.Data.Position;}set{this.Data.Position = value;}}
		
		public ByteStack(UnmanagedMemory iMemory, int iLength)
		{
			this.Memory = iMemory;
			this.BaseOffset = this.Memory.Size; ///~~;
			this.Length = iLength;

			this.Reset(true);
		}
		
		public void Reset(bool iDoZeroContent)
		{
			if(iDoZeroContent)
			{
				for(var cO = this.MinAddress; cO < this.BaseOffset; cO ++)
				{
					this.Memory.WriteByte(cO, 0);
				}
			}
			this.Pointer = this.BaseOffset;
		}
		


		public void PushBytes(byte[] iValues)
		{

			this.Pointer -= iValues.Length * sizeof(byte);
			this.Memory.WriteBytes(this.Pointer, iValues);
		}
		public void PushByte(byte iValue)
		{
			this.Pointer -= sizeof(byte);
			this.Memory.WriteByte(this.Pointer, iValue);
		}
		public void PushSByte(sbyte iValue)
		{
			throw new NotImplementedException();
		}
		
		public void PushInt16(Int16 iValue)
		{
			this.Pointer -= sizeof(Int16);
			this.Memory.WriteInt16(this.Pointer, iValue);
		}
		public void PushUInt16(UInt16 iValue)
		{
			throw new NotImplementedException();
		}
		public void PushInt32(Int32 iValue)
		{
			this.Pointer -= sizeof(Int32);
			this.Memory.WriteInt32(this.Pointer, iValue);
			//throw new NotImplementedException();
		}
		public void PushUInt32(uint iValue)
		{
			throw new NotImplementedException();
		}
		public void PushFloat16()
		{
			throw new NotImplementedException();
		}
		public void PushFloat32(float iValue)
		{
			throw new NotImplementedException();
		}
		public void PushFloat64(float iValue)
		{
			throw new NotImplementedException();
		}

		public byte PopByte()
		{
			var oRes = this.Memory.ReadByte(this.Pointer);
			this.Pointer += sizeof(byte);
			return oRes;
		}
		public int PopInt32()
		{
			var oRes = this.Memory.ReadInt32(this.Pointer);
			this.Pointer += sizeof(Int32);
			return oRes;
		}

		public int PeekInt32()
		{
			return this.Memory.ReadInt32(this.Pointer);
		}


		public void Drop()
		{
			this.PopInt32();
		}

		//public void SetByte(int iOffset, int iValue)
		//{
		//    this.Memory.WriteInt32(this.Pointer + iOffset, iValue);
		//}

		public byte GetByte(int iOffset)
		{
			return this.Memory.ReadByte(this.Pointer + iOffset);
		}
		public int GetInt32(int iOffset)
		{
			return this.Memory.ReadInt32(this.Pointer + iOffset);
		}
		public void SetInt32(int iOffset, int iValue)
		{
			this.Memory.WriteInt32(this.Pointer + iOffset, iValue);
		}
		//public void Push(object iValue)
		//{
		//    this.Push(null, iValue);
		//}
		//public void Push(string iName, object iValue)
		//{
		//    ///if(this.Position >= this.Items.Length) throw new BadCodeException("Stack overflow");
		//    this.Position --;
		//    if(this.Position < 0) throw new BadCodeException("Stack overflow");

		//    this.Items[this.Position] = new NamedStackItem{Name = iName, Value = iValue};
		//}
		//public void DropByte()
		//{
		//    this.Pointer += 1;
		//    if(this.Position > this.Items.Length) throw new BadCodeException("Stack already is empty");
		//}
		//public NamedStackItem Pop()
		//{
		//    var oItem = this.Peek();
		//    this.Drop();

		//    return oItem;
		//}
		//public NamedStackItem Peek()
		//{
		//    return this.Items[this.Position];
		//}

		
	}
	public class X_NamedStack
	{
		///[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public ByteStack        Data;
		public MemoryItemInfo[] InfoMap;
		//public NamedStackItem[] Names;
		///public int              Pointer {get{return this.Data.Pointer;}set{this.Data.Pointer = value;}}

		//public 
		public int Pointer {get{return this.Data.Pointer;} set{if(value < 0 || value > this.Data.Length) throw new ArgumentException("Invalid pointer index specified"); this.Data.Pointer = value;}}
		public MemoryItemInfo this[int iOffset]
		{
			get{if(iOffset % 4 != 0) throw new Exception(); return this.InfoMap[this.Pointer + iOffset];}
			set{if(iOffset % 4 != 0) throw new Exception(); this.InfoMap[this.Pointer + iOffset] = value;}
		}

		public X_NamedStack(UnmanagedMemory iMemory, int iSize)
		{
			this.Data    = new ByteStack(iMemory, iSize);
			this.InfoMap = new MemoryItemInfo[iSize];

			this.Reset(true);
		}
		public void Reset(bool iDoZeroContent)
		{
			this.Data.Reset(iDoZeroContent);
			
			//this.Position = this.Items.Length;

			if(iDoZeroContent) for(var cIi = 0; cIi < this.Data.Length; cIi ++)
			{
				this.InfoMap[cIi] = new MemoryItemInfo{};
			}

			//if(iDoZeroContent) for(var cIi = 0; cIi < this.Items.Length; cIi ++)
			//{
			//    this.Items[cIi] = null;
			//}
		}
		public string GetValueString(int iOffset)
		{
			//var _Info     = this.InfoMap[iOffset];
			//var _BinValue = this.Data.[iOffset];

			throw new NotImplementedException();
		}

		public void Push(object iValue)
		{
			this.Push(null, iValue);
		}
		public void Push(string iName, object iValue)
		{
			switch(iValue.GetType().Name)
			{
				case "BinaryValue" :
				{
					this.Data.PushBytes((iValue as BinaryValue).BinaryData);
					break;
				}
				case "Byte" :
				{
					throw new NotImplementedException();
					break;
				}
				case "Int32" :
				{
					this.Data.PushInt32((Int32)iValue);
					///this.InfoMap[this.Pointer] = new MemoryItemInfo{Name = iName, Type = iValue.GetType(), FriendlyValue = iValue.ToString()};

					break;
				}
				case "String" :
				{
					this.Data.PushInt32(0xfedcba);
					///this.InfoMap[this.Pointer] = new MemoryItemInfo{Name = iName, Type = iValue.GetType(), FriendlyValue = iValue.ToString()};

					break;
				}
				case "CallInfo" :
				{
					var _CallInfo = iValue as CallInfo;
					this.Data.PushInt32(_CallInfo.SrcAddress);
					///this.InfoMap[this.Pointer] = new MemoryItemInfo{Name = iName, Type = iValue.GetType(), FriendlyValue = _CallInfo.ToString()};

					break;
				}
				//case "Byte[]" :
				//{
				//    this.Data.PushBytes(iValue as Byte[]);

				//    this.Info[this.Pointer] = new StackItemInfo{Name = iName, Type = iValue.GetType(), Value = iValue.ToString()};

				//    break;
				//}

				default : throw new NotImplementedException();
			}


			//throw new NotImplementedException();
			/////if(this.Position >= this.Items.Length) throw new BadCodeException("Stack overflow");
			//this.Pointer --;
			//if(this.Position < 0) throw new BadCodeException("Stack overflow");

			//this.Items[this.Position] = new NamedStackItem{Name = iName, Type = iValue.GetType().Name, Value = iValue};
		}
		public void Drop()
		{
			this.Drop(sizeof(Int32));
		}
		public void Drop(int iWordSize)
		{
			this.Pointer += iWordSize;
			//throw new NotImplementedException();
			//this.Position ++;
			if(this.Pointer > this.Data.Length) throw new BadCodeException("Stack already is empty");
		}
		
		public MemoryItemInfo Pop()
		{
			throw new Exception();

			//var oItem = this.Peek();
			//this.Drop();

			//return oItem;
		}
		public MemoryItemInfo Peek()
		{
			return this.InfoMap[this.Pointer];
		}
		//public NamedStackItem Peek()
		//{
		//    return this.Peek(sizeof(Int32));
		//}
		//public NamedStackItem PeekByte(int iWordSize)
		//{
		//    return this.Names[this.Pointer + iWordSize];
		//}
	}
	
	//public class _NamedStack 
	//{
	//    public Stack<string> Names;
	//    public ByteStack     Data;

	//    public void PushItem()
	//    {
			
	//    }
	//}
	
	//public class ByteStack
	//{
	//    ///[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	//    public ByteStream Stream;

	//    public byte[] Data;
	//    public int    Pointer;

	//    //public NamedStackItem this[int iOffset]
	//    //{
	//    //    get{return this.Items[this.Position + iOffset];}
	//    //    set{this.Items[this.Position + iOffset] = value;}
	//    //}

	//    public ByteStack(int iSize)
	//    {
	//        this.Data = new byte[iSize];

	//        this.Reset(true);
	//    }
	//    public void Reset(bool iDoZeroContent)
	//    {
	//        this.Pointer = this.Data.Length;

	//        if(iDoZeroContent) for(var cBi = 0; cBi < this.Data.Length; cBi ++)
	//        {
	//            this.Data[cBi] = 0;
	//        }
	//    }
	//    public void PushByte(byte iValue)
	//    {
	//        this.Data[this.Pointer -= 1] = iValue;

	//        //this.Stream.
	//    }
	//    public void PushSByte(sbyte iValue)
	//    {
	//        this.PushByte((byte)iValue);
	//    }
	//    public void PushInt16(int iValue)
	//    {
	//        throw new NotImplementedException();
	//    }
	//    public void PushUInt16(uint iValue)
	//    {
	//        throw new NotImplementedException();
	//    }
	//    public void PushInt32(int iValue)
	//    {
	//        throw new NotImplementedException();
	//    }
	//    public void PushUInt32(uint iValue)
	//    {
	//        throw new NotImplementedException();
	//    }
	//    public void PushFloat16()
	//    {
	//        throw new NotImplementedException();
	//    }
	//    public void PushFloat32(float iValue)
	//    {
	//        throw new NotImplementedException();
	//    }
	//    public void PushFloat64(float iValue)
	//    {
	//        throw new NotImplementedException();
	//    }

	//    public void Push(object iValue)
	//    {
	//        this.Push(null, iValue);
	//    }
	//    public void Push(string iName, object iValue)
	//    {
	//        ///if(this.Position >= this.Items.Length) throw new BadCodeException("Stack overflow");
	//        this.Position --;
	//        if(this.Position < 0) throw new BadCodeException("Stack overflow");

	//        this.Items[this.Position] = new NamedStackItem{Name = iName, Value = iValue};
	//    }
	//    public void DropByte()
	//    {
	//        this.Pointer += 1;
	//        if(this.Position > this.Items.Length) throw new BadCodeException("Stack already is empty");
	//    }
	//    public NamedStackItem Pop()
	//    {
	//        var oItem = this.Peek();
	//        this.Drop();

	//        return oItem;
	//    }
	//    public NamedStackItem Peek()
	//    {
	//        return this.Items[this.Position];
	//    }
	//}
}
