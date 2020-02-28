using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;


//Animation Frame File Format(.BIN)
//---------------------------------

//[1]
//Format Header
//BYTE	-> 'A' Animal
//BYTE	-> 'M' Monster
//BYTE	-> 'O' Other
//BYTE	-> 'U' Underwater
//DWORD	-> Version
//DWORD	-> Length
//DWORD	-> Animation ID
//WORD	-> start coord X, relative to center of a tile
//WORD	-> start coord Y, relative to center of a tile
//WORD	-> end coord X, relative to center of a tile
//WORD	-> end coord Y, relative to center of a tile
//DWORD	-> Color count
//DWORD	-> Color table address
//DWORD	-> Frame count
//DWORD	-> Frame table address

//	[2] Color table
//	BYTE	-> R
//	BYTE	-> G
//	BYTE	-> B
//	BYTE	-> Alpha

//	[3] Frame table
//	WORD	-> Move ID
//	WORD	-> Frame number
//	WORD	-> start coord X, relative to center of a tile
//	WORD	-> start coord Y, relative to center of a tile
//	WORD	-> end coord X, relative to center of a tile
//	WORD	-> end coord Y, relative to center of a tile
//	DWORD	-> Frame pixel data offset, relative to start of the frame table

//	[4]  Pixel data
//	Unknown


namespace KRFrameViewer
{
	public class KRFrameViewer : Form
	{
		private IContainer components;

		private ToolStripButton btn_openToolStrip;

		private ToolStripButton btn_SaveToolstrip;

		private ToolStripSeparator toolStripSeparator;

		private ToolStripButton btn_about;

		private BackgroundWorker worker;

		private FolderBrowserDialog folderBrowserDialog1;

		private byte[] m_Head;

		private uint m_Version;

		private uint m_Length;

		private uint m_ID;

		private short m_InitCoordsX;

		private short m_InitCoordsY;

		private short m_EndCoordsX;

		private short m_EndCoordsY;

		private uint m_ColorCount;

		private uint mColorAddress;

		private uint m_FrameCount;

		private uint m_FrameAddress;

		private List<ColorEntry> m_Colours;

		private List<FrameEntry> m_Frames;

		private byte[] _ImageData;

		private long _ImageDataOffset;
		private OpenFileDialog openFileDialog1;
		private TreeView tree_frames;
		private PictureBox colorTableFrame;
		private Label label1;
		private PictureBox mainImageFrame;
		private Label label2;
		private TextBox statusBar;
		private RichTextBox txt_info;
		private ProgressBar pBar;
		private ToolStripContainer toolStripContainer1;
		private ToolStrip toolStrip1;
		private ToolStripButton openButton;
		private ToolStripPanel BottomToolStripPanel;
		private ToolStripPanel TopToolStripPanel;
		private ToolStripPanel RightToolStripPanel;
		private ToolStripPanel LeftToolStripPanel;
		private ToolStripContentPanel ContentPanel;
		private ToolStripButton exportButton;
		private string m_ExtractionFolder;
		
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		public void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KRFrameViewer));
            this.btn_openToolStrip = new System.Windows.Forms.ToolStripButton();
            this.btn_SaveToolstrip = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.btn_about = new System.Windows.Forms.ToolStripButton();
            this.worker = new System.ComponentModel.BackgroundWorker();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.tree_frames = new System.Windows.Forms.TreeView();
            this.colorTableFrame = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.mainImageFrame = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.statusBar = new System.Windows.Forms.TextBox();
            this.txt_info = new System.Windows.Forms.RichTextBox();
            this.pBar = new System.Windows.Forms.ProgressBar();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.openButton = new System.Windows.Forms.ToolStripButton();
            this.exportButton = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.colorTableFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainImageFrame)).BeginInit();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_openToolStrip
            // 
            this.btn_openToolStrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_openToolStrip.Name = "btn_openToolStrip";
            this.btn_openToolStrip.Size = new System.Drawing.Size(56, 22);
            this.btn_openToolStrip.Text = "&Open";
            this.btn_openToolStrip.Click += new System.EventHandler(this.OpenButtonClick);
            // 
            // btn_SaveToolstrip
            // 
            this.btn_SaveToolstrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_SaveToolstrip.Name = "btn_SaveToolstrip";
            this.btn_SaveToolstrip.Size = new System.Drawing.Size(79, 22);
            this.btn_SaveToolstrip.Text = "&Extract All";
            this.btn_SaveToolstrip.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // btn_about
            // 
            this.btn_about.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btn_about.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_about.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_about.Name = "btn_about";
            this.btn_about.Size = new System.Drawing.Size(23, 22);
            this.btn_about.Text = "He&lp";
            this.btn_about.Click += new System.EventHandler(this.AboutButtonClick);
            // 
            // worker
            // 
            this.worker.WorkerReportsProgress = true;
            this.worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.worker_DoWork);
            this.worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.worker_ProgressChanged);
            this.worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.worker_RunWorkerCompleted);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Multiselect = true;
            // 
            // BottomToolStripPanel
            // 
            this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // TopToolStripPanel
            // 
            this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // RightToolStripPanel
            // 
            this.RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // LeftToolStripPanel
            // 
            this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // ContentPanel
            // 
            this.ContentPanel.Size = new System.Drawing.Size(150, 3);
            // 
            // tree_frames
            // 
            this.tree_frames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tree_frames.Location = new System.Drawing.Point(12, 42);
            this.tree_frames.Name = "tree_frames";
            this.tree_frames.Size = new System.Drawing.Size(96, 477);
            this.tree_frames.TabIndex = 1;
            this.tree_frames.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.tree_frames_BeforeSelect);
            this.tree_frames.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tree_frames_AfterSelect);
            this.tree_frames.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tree_frames_NodeMouseClick);
            // 
            // colorTableFrame
            // 
            this.colorTableFrame.BackColor = System.Drawing.SystemColors.Info;
            this.colorTableFrame.Location = new System.Drawing.Point(114, 42);
            this.colorTableFrame.Name = "colorTableFrame";
            this.colorTableFrame.Size = new System.Drawing.Size(480, 101);
            this.colorTableFrame.TabIndex = 2;
            this.colorTableFrame.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(114, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Colors Table:";
            // 
            // mainImageFrame
            // 
            this.mainImageFrame.BackColor = System.Drawing.Color.AliceBlue;
            this.mainImageFrame.Location = new System.Drawing.Point(114, 169);
            this.mainImageFrame.MinimumSize = new System.Drawing.Size(270, 270);
            this.mainImageFrame.Name = "mainImageFrame";
            this.mainImageFrame.Size = new System.Drawing.Size(270, 270);
            this.mainImageFrame.TabIndex = 4;
            this.mainImageFrame.TabStop = false;
            this.mainImageFrame.WaitOnLoad = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(114, 152);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Frame:";
            // 
            // statusBar
            // 
            this.statusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusBar.Location = new System.Drawing.Point(0, 489);
            this.statusBar.Name = "statusBar";
            this.statusBar.ReadOnly = true;
            this.statusBar.Size = new System.Drawing.Size(641, 20);
            this.statusBar.TabIndex = 6;
            // 
            // txt_info
            // 
            this.txt_info.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_info.Location = new System.Drawing.Point(390, 169);
            this.txt_info.Name = "txt_info";
            this.txt_info.ReadOnly = true;
            this.txt_info.Size = new System.Drawing.Size(185, 278);
            this.txt_info.TabIndex = 7;
            this.txt_info.Text = "";
            // 
            // pBar
            // 
            this.pBar.Location = new System.Drawing.Point(291, 12);
            this.pBar.Name = "pBar";
            this.pBar.Size = new System.Drawing.Size(336, 10);
            this.pBar.TabIndex = 9;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.AutoScroll = true;
            this.toolStripContainer1.ContentPanel.Controls.Add(this.toolStrip1);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.pBar);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.txt_info);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.statusBar);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.label2);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.mainImageFrame);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.label1);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.colorTableFrame);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.tree_frames);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(641, 509);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(641, 534);
            this.toolStripContainer1.TabIndex = 10;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStrip1
            // 
            this.toolStrip1.AllowMerge = false;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openButton,
            this.exportButton});
            this.toolStrip1.Location = new System.Drawing.Point(9, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(87, 25);
            this.toolStrip1.TabIndex = 8;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // openButton
            // 
            this.openButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.openButton.Image = ((System.Drawing.Image)(resources.GetObject("openButton.Image")));
            this.openButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(40, 22);
            this.openButton.Text = "Open";
            this.openButton.Click += new System.EventHandler(this.OpenButtonClick);
            // 
            // exportButton
            // 
            this.exportButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.exportButton.Image = ((System.Drawing.Image)(resources.GetObject("exportButton.Image")));
            this.exportButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(44, 22);
            this.exportButton.Text = "Export";
            this.exportButton.ToolTipText = "Export";
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // KRFrameViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(641, 534);
            this.Controls.Add(this.toolStripContainer1);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(549, 503);
            this.Name = "KRFrameViewer";
            this.ShowIcon = false;
            this.Text = "KRFrameViewer 0.6.1";
            ((System.ComponentModel.ISupportInitialize)(this.colorTableFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainImageFrame)).EndInit();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

		}
		public KRFrameViewer()
		{
			this.InitializeComponent();
			this.m_Colours = new List<ColorEntry>();
			this.m_Frames = new List<FrameEntry>();
		}

		private void AboutButtonClick(object sender, EventArgs e)
		{
			(new About()).ShowDialog();
		}

		private void OpenButtonClick(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.Clear();

			    using (var fileStream = new FileStream(this.openFileDialog1.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			    {
				    using (BinaryReader binaryReader = new BinaryReader(fileStream))
				    {
					    if (!ReadHeader(binaryReader))
					    {
						    statusBar.Text = "Invalid file.";
					    }
					    else
					    {
						    ReadColors(binaryReader);
						    ReadFrames(binaryReader);
						    ReadPixels(binaryReader);
                        }
				    }
			    }

			    Bitmap colorImgBmp = new Bitmap((int)(this.m_ColorCount + 100), 101);
			    int num = 0;
			    for (int i = 0; i < this.m_Colours.Count; i++)
			    {
				    Color pixel = this.m_Colours[i].Pixel;
				    if (i % 32 == 0)
				    {
					    num += 10;
				    }
				    for (int j = 0; j < 100; j++)
				    {
					    colorImgBmp.SetPixel(i + num, j, pixel);
				    }
				    colorImgBmp.SetPixel(i + num, 100, Color.Black);
			    }

				PictureBox size = this.colorTableFrame;
				Size size1 = this.colorTableFrame.Size;

				size.Size = new System.Drawing.Size((int)(this.m_ColorCount + 100), size1.Height);
				this.colorTableFrame.Image = colorImgBmp;

				for (var f = 0; f < this.m_FrameCount; f++)
                {
                    TreeNode childNode = new TreeNode()
                    {
                        Tag = this.m_Frames[f]
                    };
                    ushort frame = this.m_Frames[f].Frame;
                    childNode.Text = frame.ToString();
                    tree_frames.Nodes.Add(childNode);
                }

			}
		}

        private void exportButton_Click(object sender, EventArgs e)
		{
			if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
			{
				this.m_ExtractionFolder = this.folderBrowserDialog1.SelectedPath;
                //MessageBox.Show("Selected folder is:" + m_ExtractionFolder);
				this.worker.RunWorkerAsync();
			}
			this.pBar.Maximum = this.tree_frames.Nodes.Count;
			this.pBar.Minimum = 0;
			this.pBar.Value = 0;
		}

        private void tree_frames_AfterSelect(object sender, TreeViewEventArgs e)
        {
			e.Node.BackColor = Color.LightGray;
			this.ChangeFrame(e.Node);

		}

        private void tree_frames_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (this.tree_frames.SelectedNode != null)
            {
                this.tree_frames.SelectedNode.BackColor = Color.Transparent;
            }
        }

        private void tree_frames_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
			this.ChangeFrame(e.Node);
		}


		//TODO: Save the image together with the background for the Gump
        //TODO: Make it so it's possible to open more than one bin file at a time.
		//TODO: Make it so it's possible to open a entire folder full of subfolders inside
		//TODO: Export to .vd directly

		private void ChangeFrame(TreeNode node)
		{

			this.statusBar.Text = string.Concat("Node Selected: ", node.Text);
			this.tree_frames.SelectedNode = node;
			
			FrameEntry currentFrame = (FrameEntry)node.Tag;
			Bitmap frameImage = this.LoadFrameImage(currentFrame);

			//TODO:Make this a unique method that takes the node as a param and returns a bitmap
			//The total size of the frame image
			int backgroundImgWidth = Math.Abs(this.m_EndCoordsX - this.m_InitCoordsX);
			int backgroundImgHeight = Math.Abs(this.m_EndCoordsY - this.m_InitCoordsY);
			Bitmap backgroundImage = new Bitmap(backgroundImgWidth, backgroundImgHeight);

			//Draws the background based on the offset provided by the data
            for (int x = 0; x < backgroundImgWidth; x++)
			{
				for (int y = 0; y < backgroundImgHeight; y++)
				{
					backgroundImage.SetPixel(x, y, Color.Pink);
				}
			}

			//Expands the frame if the image can't fit
			if (this.mainImageFrame.Size.Width < backgroundImgWidth || this.mainImageFrame.Size.Height < backgroundImgHeight)
			{
				Size size = base.Size;
				int width = size.Width + (backgroundImgWidth - this.mainImageFrame.Width);
				Size size1 = base.Size;
				base.Size = new Size(width, size1.Height + (backgroundImgHeight - this.mainImageFrame.Height));
				RichTextBox txtInfo = this.txt_info;
				Point location = this.txt_info.Location;
				int x = location.X + (backgroundImgWidth - this.mainImageFrame.Width);
				Point point = this.txt_info.Location;
				txtInfo.Location = new Point(x, point.Y);
				this.mainImageFrame.Size = new System.Drawing.Size(backgroundImgWidth, backgroundImgHeight);
			}


			//TODO:Make this a unique method that takes a background image bmp and returns a new bitmap with both combined
            //Draws the frame on top of the background image
			for (var x = 0; x < frameImage.Width; x++)
			{
				for (var y = 0; y < frameImage.Height; y++)
                {
                    var i = Math.Abs(this.m_InitCoordsX - currentFrame.InitCoordsX);
                    var j = Math.Abs(this.m_InitCoordsY - currentFrame.InitCoordsY);
                    var pixel = frameImage.GetPixel(x, y);

					backgroundImage.SetPixel(i + x, j + y, pixel);
                }
			}

			this.mainImageFrame.BackgroundImageLayout = ImageLayout.Center;
			this.mainImageFrame.BackgroundImage = backgroundImage;

			//Show the header info on the right panel
			ShowFrameInfo(currentFrame);
        }

        private void ShowFrameInfo(FrameEntry tag)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(string.Concat("Animation ID: ", this.m_ID, "\n\n"));
			stringBuilder.Append(string.Concat("Length: ", this.m_Length, "\n\n"));
			stringBuilder.Append(string.Concat("Version: ", this.m_Version, "\n"));
            stringBuilder.Append(string.Concat("ColourCount: ", this.m_ColorCount, "\n"));
            stringBuilder.Append(string.Concat("ColourOffset: ", this.mColorAddress, "\n"));
            stringBuilder.Append(string.Concat("FramesCount: ", this.m_FrameCount, "\n"));
            stringBuilder.Append(string.Concat("FramesOffset: ", this.m_FrameAddress, "\n\n"));
            object[] mInitCoordsX =
            {
                "MainInitX: ", this.m_InitCoordsX, "\tMainInitY: ", this.m_InitCoordsY, "\nMainEndX: ", this.m_EndCoordsX,
                "\tMainEndY: ", this.m_EndCoordsY, "\n\n"
            };
            stringBuilder.Append(string.Concat(mInitCoordsX));
            object[] initCoordsX = {"Initial X:\t", tag.InitCoordsX, " End X:\t", tag.EndCoordsX, "\n"};
            stringBuilder.Append(string.Concat(initCoordsX));
            object[] initCoordsY = {"Initial Y:\t", tag.InitCoordsY, " End Y:\t", tag.EndCoordsY, "\n"};
            stringBuilder.Append(string.Concat(initCoordsY));
            object[] objArray = {"\nWidth: ", tag.Width, " Height: ", tag.Height};
            stringBuilder.Append(string.Concat(objArray));
            stringBuilder.Append(string.Concat("\n\nSize: ", tag.Width * tag.Height));
            this.txt_info.Text = string.Concat(stringBuilder.ToString(), "\nOffset: ", tag.DataOffset);

		}

        private void Clear()
		{
			base.Size = this.MinimumSize;
			this.mainImageFrame.Size = this.mainImageFrame.MinimumSize;
			this.txt_info.Location = new Point(388, 168);
			this.mColorAddress = 0;
			this.m_ColorCount = 0;
			this.m_Colours.Clear();
			this.m_EndCoordsX = 0;
			this.m_EndCoordsY = 0;
			this.m_FrameAddress = 0;
			this.m_FrameCount = 0;
			this.m_Frames.Clear();
			this.m_ID = 0;
			this.m_InitCoordsX = 0;
			this.m_InitCoordsY = 0;
			this.m_Length = 0;
			this.m_Version = 0;
			this.tree_frames.Nodes.Clear();
			this.colorTableFrame.Image = this.colorTableFrame.InitialImage;
			this.mainImageFrame.BackgroundImage = null;
			this.txt_info.Clear();
		}

		private Color CombineColors(Color sourceColor, Color targetColor, int factor)
		{
			long argb = (long)sourceColor.ToArgb();
			long num = (long)targetColor.ToArgb();
			long num1 = (argb & (long)16711935) * (long)factor;
			long num2 = (num & (long)16711935) * (long)(16 - factor);
			long num3 = num1 + num2;
			long num4 = (Int64)((argb >> 4 & (Int64)(-1044496) * (long)factor));
			long num5 = (Int64)((num >> 4 & (Int64)(-1044496) * (long)(16 - factor)));
			long num6 = num4 + num5;
			long num7 = (argb >> 4 & (long)267390960) * (long)factor;
			long num8 = (num >> 4 & (long)267390960) * (long)(16 - factor);
			long num9 = num7 + num8;
			long num10 = (num3 >> 4 ^ num6) & (long)16711935 ^ num9;
			return Color.FromArgb((int)num10);
		}




		//Inner image frame
		public Bitmap LoadFrameImage(FrameEntry currentFrameEntry)
		{
			int x;
			int y;
			var frameWidth = Math.Abs(currentFrameEntry.EndCoordsX - currentFrameEntry.InitCoordsX);
			var frameHeight = Math.Abs(currentFrameEntry.EndCoordsY - currentFrameEntry.InitCoordsY);
			byte[] numArray = this._ImageData;
			var frameImage = new Bitmap(frameWidth, frameHeight);

			//Changes the alpha of the image and the background pixels to a color that will be combined later
			for (x = 0; x < frameWidth; x++)
			{
				for (y = 0; y < frameHeight; y++)
                {
                    var color = Color.PaleGreen;
					frameImage.SetPixel(x, y, color);
                }
			}
			
			//Zero the counters
            x = 0;
			y = 0;

			//Draw and combine the background color with the image pixels, using the previous defined color
            var dataOffset = (int)(currentFrameEntry.DataOffset - this._ImageDataOffset);

			while (y < frameHeight)
			{
     
				var num3 = dataOffset;
				dataOffset = num3 + 1;
				byte k = numArray[num3];
				if (k >= 128)
				{
                    Color pixel;
                    Color color;
                    byte num;
					var num4 = dataOffset;
					dataOffset = num4 + 1;
					var num5 = numArray[num4];
					var num6 = num5 / 16;
					var num7 = num5 % 16;
					if (num6 > 0)
					{
						var num8 = dataOffset;
						dataOffset = num8 + 1;
						num = numArray[num8];
						pixel = this.m_Colours[num].Pixel;
						color = frameImage.GetPixel(x, y);
						pixel = this.CombineColors(pixel, color, num6);
						frameImage.SetPixel(x, y, pixel);
						this.NextCoordinate(ref x, ref y, frameWidth, frameHeight);
					}
					for (k = (byte)(k - 128); k > 0; k = (byte)(k - 1))
					{
						var num9 = dataOffset;
						dataOffset = num9 + 1;
						num = numArray[num9];
						pixel = this.m_Colours[num].Pixel;
						frameImage.SetPixel(x, y, pixel);
						this.NextCoordinate(ref x, ref y, frameWidth, frameHeight);
					}
					if (num7 <= 0)
					{
						continue;
					}
					var num10 = dataOffset;
					dataOffset = num10 + 1;
					num = numArray[num10];
					pixel = this.m_Colours[num].Pixel;
					color = frameImage.GetPixel(x, y);
					pixel = this.CombineColors(pixel, color, num7);
					frameImage.SetPixel(x, y, pixel);
					this.NextCoordinate(ref x, ref y, frameWidth, frameHeight);
				}
				else
				{
					while (k > 0)
					{
						this.NextCoordinate(ref x, ref y, frameWidth, frameHeight);
						k = (byte)(k - 1);
					}
				}
			}
			return frameImage;
		}

		private bool NextCoordinate(ref int curX, ref int curY, int width, int height)
		{
			curX++;
			if (curX >= width)
			{
				curX = 0;
				curY++;
			}
			if (curY < height && curX < width)
			{
				return true;
			}
			return false;
		}

		private bool ReadColors(BinaryReader reader)
		{
			reader.BaseStream.Seek(this.mColorAddress, SeekOrigin.Begin);
			this.m_Colours.Clear();
			for (int i = 0; i < this.m_ColorCount; i++)
			{
				ColorEntry colorEntry = new ColorEntry(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
				this.m_Colours.Add(colorEntry);
			}
			return true;
		}

		private bool ReadFrames(BinaryReader reader)
		{
			reader.BaseStream.Seek(this.m_FrameAddress, SeekOrigin.Begin);
			this.m_Frames.Clear();
			for (var i = 0; i < this.m_FrameCount; i++)
			{
				FrameEntry frameEntry = new FrameEntry(reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16(), (uint)(this.m_FrameAddress + (i * 16) + reader.ReadUInt32()), this.m_ColorCount);
				this.m_Frames.Add(frameEntry);
			}
			return true;
		}

		private bool ReadHeader(BinaryReader reader)
		{
			this.m_Head = reader.ReadBytes(4);
			if (this.m_Head[0] != 65 || this.m_Head[1] != 77 || this.m_Head[2] != 79)
			{
				return false;
			}
			this.m_Version = reader.ReadUInt32();
			this.m_Length = reader.ReadUInt32();
			this.m_ID = reader.ReadUInt32();
			this.m_InitCoordsX = reader.ReadInt16();
			this.m_InitCoordsY = reader.ReadInt16();
			this.m_EndCoordsX = reader.ReadInt16();
			this.m_EndCoordsY = reader.ReadInt16();
			this.m_ColorCount = reader.ReadUInt32();
			this.mColorAddress = reader.ReadUInt32();
			this.m_FrameCount = reader.ReadUInt32();
			this.m_FrameAddress = reader.ReadUInt32();
			return true;
		}

		private bool ReadPixels(BinaryReader reader)
		{
			this._ImageDataOffset = this.m_FrameAddress + this.m_FrameCount * 16;
			this._ImageData = new byte[(int)(this.m_Length - this._ImageDataOffset)];
			reader.BaseStream.Seek(this._ImageDataOffset, SeekOrigin.Begin);
			this._ImageData = reader.ReadBytes((int)(this.m_Length - this._ImageDataOffset));
			return true;
		}



		//TODO:Make it export either the current img with a liste.txt or a combined bmp file
		//Export the image as a bmp
		private void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			for (int i = 0; i < this.tree_frames.Nodes.Count; i++)
			{

				this.worker.ReportProgress(i);
				Bitmap bitmap = this.LoadFrameImage((FrameEntry)this.tree_frames.Nodes[i].Tag);
				bitmap.Save(string.Format("{0}\\{1}.bmp", this.m_ExtractionFolder, i.ToString().PadLeft(2, '0')[0] + "_" + i.ToString().PadLeft(2, '0')[1]), ImageFormat.Bmp);
				var centerX = Math.Abs(bitmap.Width/2);//Width/2 + 1 if odd
				var centerY = Math.Abs(bitmap.Height/4) * -1;//Height/4 + 1 if odd
                var listeFilePath = Path.Combine(m_ExtractionFolder,"liste.txt");


				if (File.Exists(listeFilePath))
                {
                    using (StreamWriter sw = File.AppendText(listeFilePath))
					{
						WriteFrameInfo(i, centerX, centerY, sw);
					}
				}
                else
				{ 
					using (StreamWriter sw = File.CreateText(listeFilePath))

					{
						WriteFrameInfo(i, centerX, centerY, sw);
					}
				}
            }
		}

		private static void WriteFrameInfo(int i, int centerX, int centerY, StreamWriter sw)
		{
			sw.WriteLine("{0}", i.ToString().PadLeft(2, '0')[0] + ":" + i.ToString().PadLeft(2, '0')[1]);
			sw.WriteLine("CenterX: {0}", centerX.ToString());
			sw.WriteLine("CenterY: {0}", centerY.ToString());
		}

		private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			this.pBar.Value = e.ProgressPercentage;
		}

		private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			this.pBar.Value = 0;
			MessageBox.Show("Extraction completed.");
		}




    }

}