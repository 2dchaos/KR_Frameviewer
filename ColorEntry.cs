using System.Drawing;

namespace KRFrameViewer
{
	public class ColorEntry
	{
		private byte m_R;

		private byte m_G;

		private byte m_B;

		private byte m_Alpha;

		private Color m_Color;

		public byte Alpha
		{
			get
			{
				return this.m_Alpha;
			}
		}

		public byte B
		{
			get
			{
				return this.m_B;
			}
		}

		public byte G
		{
			get
			{
				return this.m_G;
			}
		}

		public Color Pixel
		{
			get
			{
				return this.m_Color;
			}
		}

		public byte R
		{
			get
			{
				return this.m_R;
			}
		}

		public ColorEntry(byte R, byte G, byte B, byte Alpha)
		{
			this.m_R = R;
			this.m_B = B;
			this.m_G = G;
			this.m_Alpha = Alpha;
			this.m_Color = Color.FromArgb((int)R, (int)G, (int)B);
		}
	}
}