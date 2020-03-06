namespace KRFrameViewer
{
	public class PixelEntry
	{
		private ushort m_RowHeader;

		private ushort m_RowOffset;

		private byte[] m_Data;

		public ushort RowHeader
		{
			get
			{
				return this.m_RowHeader;
			}
		}

		public ushort RowOffset
		{
			get
			{
				return this.m_RowOffset;
			}
		}

		public PixelEntry(ushort RowHead, ushort RowOff)
		{
			this.m_RowHeader = RowHead;
			this.m_RowOffset = RowOff;
		}
	}
}