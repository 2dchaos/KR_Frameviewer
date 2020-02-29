using System;

namespace KRFrameViewer
{
	public class FrameEntry
	{
		private ushort _mid;

		private ushort m_Frame;

		private short m_InitCoordsX;

		private short m_InitCoordsY;

		private short m_EndCoordsX;

		private short m_EndCoordsY;

		private uint m_DataOffset;

		private int m_width;

		private int m_height;

		public uint DataOffset
		{
			get
			{
				return this.m_DataOffset;
			}
		}

		public short EndCoordsX
		{
			get
			{
				return this.m_EndCoordsX;
			}
		}

		public short EndCoordsY
		{
			get
			{
				return this.m_EndCoordsY;
			}
		}

		public ushort Frame
		{
			get
			{
				return this.m_Frame;
			}
		}

		public int Height
		{
			get
			{
				return this.m_height;
			}
		}

		public ushort ID
		{
			get
			{
				return this._mid;
			}
		}

		public short InitCoordsX
		{
			get
			{
				return this.m_InitCoordsX;
			}
		}

		public short InitCoordsY
		{
			get
			{
				return this.m_InitCoordsY;
			}
		}

		public int Width
		{
			get
			{
				return this.m_width;
			}
		}

		public FrameEntry(ushort ID, ushort Frame, short initcoordsX, short InitCoordsY, short EndCoordsX, short EndcoordsY, uint DataOffset, uint Colournumber)
		{
			this._mid = ID;
			this.m_Frame = Frame;
			this.m_InitCoordsX = initcoordsX;
			this.m_InitCoordsY = InitCoordsY;
			this.m_EndCoordsX = EndCoordsX;
			this.m_EndCoordsY = EndcoordsY;
			this.m_DataOffset = DataOffset;
			this.m_height = this.m_EndCoordsY - this.m_InitCoordsY;
			this.m_width = this.m_EndCoordsX - this.m_InitCoordsX;
		}
	}
}