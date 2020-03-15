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

        private BackgroundWorker worker;

        private FolderBrowserDialog exportFileDialog;

        private byte[] m_Head;

        private List<uint> m_Version;

        private List<uint> m_Length;

        private List<uint> m_ID;

        private List<short> m_InitCoordsX;

        private List<short> m_InitCoordsY;

        private List<short> m_EndCoordsX;

        private List<short> m_EndCoordsY;

        private List<uint> m_ColorCount;

        private List<uint> m_ColorAddress;

        private List<uint> m_FrameCount;

        private List<uint> m_FrameAddress;

        //private List<List<ColorEntry>> m_Colors;

        private Dictionary<int, List<ColorEntry>> m_Colors = new Dictionary<int, List<ColorEntry>>();


        private Dictionary<int, List<FrameEntry>> m_Frames = new Dictionary<int, List<FrameEntry>>();

        //private Dictionary<int, List<uint>> m_colorList = 

        private Dictionary<int, byte[]> _ImageData = new Dictionary<int, byte[]>();

        private List<long> _ImageDataOffset;

        private string m_ExtractionFolder;

        private OpenFileDialog openFileDialog;
        private TreeView treeFramesBox;
        private PictureBox colorTableBox;
        private PictureBox mainImageBox;
        private TextBox statusBar;
        private RichTextBox infoBox;
        private ProgressBar progressBar;
        private ToolStripContainer mainContainer;
        private ToolStrip toolStrip;
        private ToolStripPanel TopToolStripPanel;
        private ToolStripContentPanel ContentPanel;
        private ToolStripDropDownButton fileDropdown;
        private ToolStripMenuItem openFileMenuItem;
        private ToolStripMenuItem openFolderFileMenuItem;
        private ToolStripMenuItem exportMenuItem;
        private ToolStripMenuItem exitFileMenuItem;
        private FolderBrowserDialog openFolderDialog;
        private int animId = 0;
        private int animFrame = 0;

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
            this.m_Version = new List<uint>();
            this.m_Length = new List<uint>();
            this.m_ID = new List<uint>();
            this.m_InitCoordsX = new List<short>();
            this.m_InitCoordsY = new List<short>();
            this.m_EndCoordsX = new List<short>();
            this.m_EndCoordsY = new List<short>();
            this.m_ColorCount = new List<uint>();
            this.m_ColorAddress = new List<uint>();
            this.m_FrameCount = new List<uint>();
            this.m_FrameAddress = new List<uint>();
            this._ImageDataOffset = new List<long>();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(KRFrameViewer));
            this.worker = new BackgroundWorker();
            this.exportFileDialog = new FolderBrowserDialog();
            this.openFileDialog = new OpenFileDialog();
            this.TopToolStripPanel = new ToolStripPanel();
            this.ContentPanel = new ToolStripContentPanel();
            this.treeFramesBox = new TreeView();
            this.colorTableBox = new PictureBox();
            this.mainImageBox = new PictureBox();
            this.statusBar = new TextBox();
            this.infoBox = new RichTextBox();
            this.progressBar = new ProgressBar();
            this.mainContainer = new ToolStripContainer();
            this.toolStrip = new ToolStrip();
            this.fileDropdown = new ToolStripDropDownButton();
            this.openFileMenuItem = new ToolStripMenuItem();
            this.openFolderFileMenuItem = new ToolStripMenuItem();
            this.exportMenuItem = new ToolStripMenuItem();
            this.exitFileMenuItem = new ToolStripMenuItem();
            this.openFolderDialog = new FolderBrowserDialog();
            ((ISupportInitialize)(this.colorTableBox)).BeginInit();
            ((ISupportInitialize)(this.mainImageBox)).BeginInit();
            this.mainContainer.ContentPanel.SuspendLayout();
            this.mainContainer.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // worker
            // 
            this.worker.WorkerReportsProgress = true;
            this.worker.DoWork += new DoWorkEventHandler(this.WorkerExportFile);
            this.worker.ProgressChanged += new ProgressChangedEventHandler(this.worker_ProgressChanged);
            this.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.worker_RunWorkerCompleted);
            // 
            // exportFileDialog
            // 
            this.exportFileDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            this.openFileDialog.Multiselect = true;
            // 
            // TopToolStripPanel
            // 
            this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // ContentPanel
            // 
            this.ContentPanel.Size = new System.Drawing.Size(150, 3);
            // 
            // treeFramesBox
            // 
            this.treeFramesBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeFramesBox.Location = new System.Drawing.Point(12, 42);
            this.treeFramesBox.Name = "treeFramesBox";
            this.treeFramesBox.Size = new System.Drawing.Size(96, 410);
            this.treeFramesBox.TabIndex = 1;
            this.treeFramesBox.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.tree_frames_BeforeSelect);
            this.treeFramesBox.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tree_frames_AfterSelect);
            this.treeFramesBox.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tree_frames_NodeMouseClick);
            // 
            // colorTableBox
            // 
            this.colorTableBox.BackColor = System.Drawing.SystemColors.Info;
            this.colorTableBox.Location = new System.Drawing.Point(114, 42);
            this.colorTableBox.Name = "colorTableBox";
            this.colorTableBox.Size = new System.Drawing.Size(480, 101);
            this.colorTableBox.TabIndex = 2;
            this.colorTableBox.TabStop = false;
            // 
            // mainImageBox
            // 
            this.mainImageBox.BackColor = System.Drawing.Color.DimGray;
            this.mainImageBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mainImageBox.InitialImage = null;
            this.mainImageBox.Location = new System.Drawing.Point(114, 169);
            this.mainImageBox.MinimumSize = new System.Drawing.Size(300, 300);
            this.mainImageBox.Name = "mainImageBox";
            this.mainImageBox.Size = new System.Drawing.Size(300, 300);
            this.mainImageBox.TabIndex = 4;
            this.mainImageBox.TabStop = false;
            this.mainImageBox.WaitOnLoad = true;
            // 
            // statusBar
            // 
            this.statusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusBar.Location = new System.Drawing.Point(0, 493);
            this.statusBar.Name = "statusBar";
            this.statusBar.ReadOnly = true;
            this.statusBar.Size = new System.Drawing.Size(624, 20);
            this.statusBar.TabIndex = 6;
            // 
            // infoBox
            // 
            this.infoBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.infoBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.infoBox.Location = new System.Drawing.Point(420, 169);
            this.infoBox.Name = "infoBox";
            this.infoBox.ReadOnly = true;
            this.infoBox.Size = new System.Drawing.Size(165, 283);
            this.infoBox.TabIndex = 7;
            this.infoBox.Text = "";
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar.Location = new System.Drawing.Point(0, 469);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(624, 24);
            this.progressBar.TabIndex = 9;
            // 
            // mainContainer
            // 
            // mainContainer.ContentPanel
            // 
            this.mainContainer.ContentPanel.AutoScroll = true;
            this.mainContainer.ContentPanel.Controls.Add(this.toolStrip);
            this.mainContainer.ContentPanel.Controls.Add(this.progressBar);
            this.mainContainer.ContentPanel.Controls.Add(this.infoBox);
            this.mainContainer.ContentPanel.Controls.Add(this.statusBar);
            this.mainContainer.ContentPanel.Controls.Add(this.mainImageBox);
            this.mainContainer.ContentPanel.Controls.Add(this.colorTableBox);
            this.mainContainer.ContentPanel.Controls.Add(this.treeFramesBox);
            this.mainContainer.ContentPanel.Size = new System.Drawing.Size(641, 509);
            this.mainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainContainer.Location = new System.Drawing.Point(0, 0);
            this.mainContainer.Name = "mainContainer";
            this.mainContainer.Size = new System.Drawing.Size(641, 534);
            this.mainContainer.TabIndex = 0;
            // 
            // toolStrip
            // 
            this.toolStrip.AllowMerge = false;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileDropdown});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(624, 25);
            this.toolStrip.Stretch = true;
            this.toolStrip.TabIndex = 8;
            this.toolStrip.Text = "toolStrip1";
            // 
            // fileDropdown
            // 
            this.fileDropdown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.fileDropdown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileMenuItem,
            this.openFolderFileMenuItem,
            this.exportMenuItem,
            this.exitFileMenuItem});
            this.fileDropdown.Image = ((System.Drawing.Image)(resources.GetObject("fileDropdown.Image")));
            this.fileDropdown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fileDropdown.Name = "fileDropdown";
            this.fileDropdown.Size = new System.Drawing.Size(38, 22);
            this.fileDropdown.Text = "FIle";
            // 
            // openFileMenuItem
            // 
            this.openFileMenuItem.Name = "openFileMenuItem";
            this.openFileMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openFileMenuItem.Text = "Open";
            this.openFileMenuItem.Click += new System.EventHandler(this.OpenButtonClick);
            // 
            // openFolderFileMenuItem
            // 
            this.openFolderFileMenuItem.Name = "openFolderFileMenuItem";
            this.openFolderFileMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openFolderFileMenuItem.Text = "Open Folder";
            this.openFolderFileMenuItem.Click += new System.EventHandler(this.OpenFolderClick);
            // 
            // exportMenuItem
            // 
            this.exportMenuItem.Name = "exportMenuItem";
            this.exportMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exportMenuItem.Text = "Export";
            this.exportMenuItem.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // exitFileMenuItem
            // 
            this.exitFileMenuItem.Name = "exitFileMenuItem";
            this.exitFileMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitFileMenuItem.Text = "Exit";
            // 
            // openFolderDialog
            // 
            this.openFolderDialog.ShowNewFolderButton = false;
            // 
            // KRFrameViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(641, 534);
            this.Controls.Add(this.mainContainer);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(549, 503);
            this.Name = "KRFrameViewer";
            this.ShowIcon = false;
            this.Text = "KRFrameViewer 0.6.1";
            ((System.ComponentModel.ISupportInitialize)(this.colorTableBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainImageBox)).EndInit();
            this.mainContainer.ContentPanel.ResumeLayout(false);
            this.mainContainer.ContentPanel.PerformLayout();
            this.mainContainer.ResumeLayout(false);
            this.mainContainer.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);

        }
        public KRFrameViewer()
        {
            this.InitializeComponent();
        }
        private void OpenFolderClick(object sender, EventArgs e)
        {
            if(openFolderDialog.ShowDialog() == DialogResult.OK)
            {
                var selectedPath = openFolderDialog.SelectedPath;
                var c = 0;
                foreach(string file in Directory.EnumerateFiles(selectedPath,"*.bin"))
                {
                    var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite); ;
                    BinaryReader binaryReader = new BinaryReader(fileStream);
                    ReadHeader(binaryReader);
                    ReadColors(binaryReader, c);
                    ReadFrames(binaryReader, c);
                    ReadPixels(binaryReader, c);
                    CreateParentNode(c);
                    c++;
                }
                //MessageBox.Show("Ok!");
            }
        }

        private void CreateParentNode(int c)
        {
            TreeNode parentNode = new TreeNode()
            {
                Tag = c
            };
            treeFramesBox.Nodes.Add(parentNode);

            for (var f = 0; f < this.m_FrameCount[c]; f++)
            {
                TreeNode childNode = new TreeNode()
                {
                    Tag = this.m_Frames[c][f]
                };
                ushort frame = this.m_Frames[c][f].Frame;
                childNode.Text = frame.ToString();
                parentNode.Nodes.Add(childNode);
            }
            parentNode.Text = this.m_Frames[c][0].ID.ToString();
        }

        private void OpenButtonClick(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var c = 0;
                this.Clear(c);
                using (var fileStream = new FileStream(this.openFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (BinaryReader binaryReader = new BinaryReader(fileStream))
                    {
                        if (!ReadHeader(binaryReader))
                        {
                            statusBar.Text = "Invalid file.";
                        }
                        else
                        {
                            ReadColors(binaryReader, c);
                            ReadFrames(binaryReader, c);
                            ReadPixels(binaryReader, c);
                            //FillColorList(binaryReader);
                        }
                    }
                }
                //this.colorTableBox.Image = CreateColorBarBitmap();
                CreateTreeNode(c);
            }
        }
        private void CreateTreeNode(int c)
        {
            for (var f = 0; f < this.m_FrameCount[c]; f++)
            {
                TreeNode childNode = new TreeNode()
                {
                    Tag = this.m_Frames[c][f]
                };
                ushort frame = this.m_Frames[c][f].Frame;
                childNode.Text = frame.ToString();
                treeFramesBox.Nodes.Add(childNode);
            }
        }
        private void tree_frames_AfterSelect(object sender, TreeViewEventArgs e)
        {
            e.Node.BackColor = Color.LightGray;
            if (e.Node.Parent != null)
            { 
                this.ChangeFrame(e.Node, e.Node.Parent);
            }
        }
        private void tree_frames_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (this.treeFramesBox.SelectedNode != null)
            {
                this.treeFramesBox.SelectedNode.BackColor = Color.Transparent;
            }
        }
        private void tree_frames_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if(e.Node.Parent != null)
            {
                this.ChangeFrame(e.Node, e.Node.Parent);
            }
        }
        private Bitmap CreateColorBarBitmap(int c)
        {
            Bitmap colorImgBmp = new Bitmap((int)(this.m_ColorCount[c] + 100), 101);

            int num = 0;
            for (int i = 0; i < this.m_Colors[c].Count; i++)
            {
                Color pixel = this.m_Colors[c][i].Pixel;
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

            PictureBox size = this.colorTableBox;
            Size size1 = this.colorTableBox.Size;

            size.Size = new Size((int)(this.m_ColorCount[c] + 100), size1.Height);

            return colorImgBmp;
        }
        private void exportButton_Click(object sender, EventArgs e)
        {
            if (this.exportFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.m_ExtractionFolder = this.exportFileDialog.SelectedPath;
                this.worker.RunWorkerAsync();
            }
            this.progressBar.Maximum = this.treeFramesBox.Nodes.Count;
            this.progressBar.Minimum = 0;
            this.progressBar.Value = 0;
        }
        //TODO: Save the image together with the background for the Gump
        //TODO: Make it so it's possible to open a entire folder full of subfolders inside
        //TODO: Export to .vd directly
        private void ChangeFrame(TreeNode childNode, TreeNode parentNode)
        {
            
            var c = int.Parse(parentNode.Tag.ToString());
            this.colorTableBox.Image = CreateColorBarBitmap(c);
            var backgroundImgWidth = Math.Abs(this.m_EndCoordsX[c] - this.m_InitCoordsX[c]);
            var backgroundImgHeight = Math.Abs(this.m_EndCoordsY[c] - this.m_InitCoordsY[c]);
            this.treeFramesBox.SelectedNode = childNode;
            FrameEntry currentFrame = (FrameEntry)childNode.Tag;
            Bitmap frameImage = this.LoadFrameImage(currentFrame, c);
            Bitmap backgroundImage = new Bitmap(backgroundImgWidth, backgroundImgHeight);
            //Draws the background based on the offset provided by the data
            for (int x = 0; x < backgroundImgWidth; x++)
            {
                for (int y = 0; y < backgroundImgHeight; y++)
                {
                    backgroundImage.SetPixel(x, y, Color.Black);
                }
            }
            //Expands the frame if the image can't fit
            if (this.mainImageBox.Size.Width < backgroundImgWidth || this.mainImageBox.Size.Height < backgroundImgHeight)
            {
                Size size = base.Size;
                int width = size.Width + (backgroundImgWidth - this.mainImageBox.Width);
                Size size1 = base.Size;
                base.Size = new Size(width, size1.Height + (backgroundImgHeight - this.mainImageBox.Height));
                RichTextBox txtInfo = this.infoBox;
                Point location = this.infoBox.Location;
                int x = location.X + (backgroundImgWidth - this.mainImageBox.Width);
                Point point = this.infoBox.Location;
                txtInfo.Location = new Point(x, point.Y);
                this.mainImageBox.Size = new Size(backgroundImgWidth, backgroundImgHeight);
            }
            //TODO:Make this a unique method that takes a background image bmp and returns a new bitmap with both combined
            //Draws the frame on top of the background image
            for (var x = 0; x < frameImage.Width; x++)
            {
                for (var y = 0; y < frameImage.Height; y++)
                {
                    var i = Math.Abs(this.m_InitCoordsX[c] - currentFrame.InitCoordsX);
                    var j = Math.Abs(this.m_InitCoordsY[c] - currentFrame.InitCoordsY);
                    var pixel = frameImage.GetPixel(x, y);

                    backgroundImage.SetPixel(i + x, j + y, pixel);
                }
            }
            this.mainImageBox.BackgroundImageLayout = ImageLayout.Center;
            this.mainImageBox.BackgroundImage = backgroundImage;
            ShowFrameInfo(currentFrame);
        }
        private void ShowFrameInfo(FrameEntry tag)
        {
            StringBuilder stringBuilder = new StringBuilder();
            //stringBuilder.Append(string.Concat("Frames per Angle:", this.m_FrameCount[] / 5, "\n\n"));
            stringBuilder.Append(string.Concat("Length: ", this.m_Length, "\n\n"));
            stringBuilder.Append(string.Concat("Version: ", this.m_Version, "\n"));
            stringBuilder.Append(string.Concat("ColourCount: ", this.m_ColorCount, "\n"));
            stringBuilder.Append(string.Concat("ColourOffset: ", this.m_ColorAddress, "\n"));
            stringBuilder.Append(string.Concat("FramesCount: ", this.m_FrameCount, "\n"));
            stringBuilder.Append(string.Concat("FramesOffset: ", this.m_FrameAddress, "\n\n"));
            object[] mInitCoordsX =
            {
                "MainInitX: ", this.m_InitCoordsX, "\tMainInitY: ", this.m_InitCoordsY, "\nMainEndX: ", this.m_EndCoordsX,
                "\tMainEndY: ", this.m_EndCoordsY, "\n\n"
            };
            stringBuilder.Append(string.Concat(mInitCoordsX));

            object[] initCoordsX = { "Initial X:\t", tag.InitCoordsX, " End X:\t", tag.EndCoordsX, "\n" };
            stringBuilder.Append(string.Concat(initCoordsX));
            object[] initCoordsY = { "Initial Y:", tag.InitCoordsY, " End Y:\t", tag.EndCoordsY, "\n" };
            stringBuilder.Append(string.Concat(initCoordsY));
            object[] frameAndID = { "Frame:", tag.Frame, " ID:\t", tag.ID, "\n" };
            stringBuilder.Append(string.Concat(frameAndID));
            object[] objArray = { "\nWidth: ", tag.Width, " Height: ", tag.Height };
            stringBuilder.Append(string.Concat(objArray));
            stringBuilder.Append(string.Concat("\n\nSize: ", tag.Width * tag.Height));
            this.infoBox.Text = string.Concat(stringBuilder.ToString(), "\nOffset: ", tag.DataOffset);
        }

        private void Clear(int c)
        {
            this.m_ColorAddress[c] = 0;
            this.m_ColorCount[c] = 0;
            this.m_Colors[c].Clear();
            this.m_EndCoordsX[c] = 0;
            this.m_EndCoordsY[c] = 0;
            this.m_FrameAddress[c] = 0;
            this.m_FrameCount[c] = 0;
            this.m_Frames[c].Clear();
            this.m_ID[c] = 0;
            this.m_InitCoordsX[c] = 0;
            this.m_InitCoordsY[c] = 0;
            this.m_Length[c] = 0;
            this.m_Version[c] = 0;
            this.treeFramesBox.Nodes.Clear();
            this.colorTableBox.Image = this.colorTableBox.InitialImage;
            this.mainImageBox.BackgroundImage = null;
            this.infoBox.Clear();
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
        public Bitmap LoadFrameImage(FrameEntry currentFrameEntry, int c)
        {
            int x;
            int y;
            byte[] numArray = this._ImageData[c];
            var frameWidth = Math.Abs(currentFrameEntry.EndCoordsX - currentFrameEntry.InitCoordsX);
            var frameHeight = Math.Abs(currentFrameEntry.EndCoordsY - currentFrameEntry.InitCoordsY);
            var frameImage = new Bitmap(frameWidth, frameHeight);
            //Changes the alpha of the image and the background pixels to a color that will be combined later
            for (x = 0; x < frameWidth; x++)
            {
                for (y = 0; y < frameHeight; y++)
                {
                    var color = Color.White;
                    frameImage.SetPixel(x, y, color);
                }
            }

            //Zero the counters
            x = 0;
            y = 0;

            //The address where the data with the pixels starts and ends
            var dataOffset = (int)(currentFrameEntry.DataOffset - this._ImageDataOffset[c]);

            //Draws the pixels based on the stored data
            while (y < frameHeight)
            {
                var num3 = dataOffset;
                dataOffset = num3 + 1;//Start of the drawing, offset + 1
                byte k = numArray[num3];
                if (k >= 128)
                {
                    Color pixel;
                    Color color;
                    byte byte_;
                    var num4 = dataOffset;
                    dataOffset = num4 + 1;
                    var num5 = numArray[num4];
                    var num6 = num5 / 16;
                    var num7 = num5 % 16;
                    if (num6 > 0)
                    {
                        var num8 = dataOffset;
                        dataOffset = num8 + 1;
                        byte_ = numArray[num8];
                        pixel = this.m_Colors[c][byte_].Pixel;
                        color = frameImage.GetPixel(x, y);
                        pixel = this.CombineColors(pixel, color, num6);
                        frameImage.SetPixel(x, y, pixel);
                        this.NextCoordinate(ref x, ref y, frameWidth, frameHeight);
                    }
                    for (k = (byte)(k - 128); k > 0; k = (byte)(k - 1))
                    {
                        var num9 = dataOffset;
                        dataOffset = num9 + 1;
                        byte_ = numArray[num9];
                        pixel = this.m_Colors[c][byte_].Pixel;
                        frameImage.SetPixel(x, y, pixel);
                        this.NextCoordinate(ref x, ref y, frameWidth, frameHeight);
                    }
                    if (num7 <= 0)
                    {
                        continue;
                    }
                    var num10 = dataOffset;
                    dataOffset = num10 + 1;
                    byte_ = numArray[num10];
                    pixel = this.m_Colors[c][byte_].Pixel;
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
        private bool ReadHeader(BinaryReader reader)
        {
            this.m_Head = reader.ReadBytes(4);
            if (this.m_Head[0] != 65 || this.m_Head[1] != 77 || this.m_Head[2] != 79)
            {
                return false;
            }
            this.m_Version.Add(reader.ReadUInt32());
            this.m_Length.Add(reader.ReadUInt32());
            this.m_ID.Add(reader.ReadUInt32());
            this.m_InitCoordsX.Add(reader.ReadInt16());
            this.m_InitCoordsY.Add(reader.ReadInt16());
            this.m_EndCoordsX.Add(reader.ReadInt16());
            this.m_EndCoordsY.Add(reader.ReadInt16());
            this.m_ColorCount.Add(reader.ReadUInt32());
            this.m_ColorAddress.Add(reader.ReadUInt32());
            this.m_FrameCount.Add(reader.ReadUInt32());
            this.m_FrameAddress.Add(reader.ReadUInt32());
            return true;
        }
        private bool ReadColors(BinaryReader reader, int c)
        {
           reader.BaseStream.Seek(this.m_ColorAddress[c], SeekOrigin.Begin);
           for (int i = 0; i < (int)m_ColorCount[c]; i++)
           {
                ColorEntry colorEntry = new ColorEntry(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                //this.m_Colors[c].Add(colorEntry);

                if(m_Colors.ContainsKey(c))
                {
                    m_Colors[c].Add(colorEntry);
                }else
                {
                    m_Colors[c] = new List<ColorEntry> { colorEntry };
                }
     
            }
            return true;
        }
        private bool ReadFrames(BinaryReader reader, int c)
        {
            reader.BaseStream.Seek(this.m_FrameAddress[c], SeekOrigin.Begin);
            //this.m_Frames[c].Clear();
            for (var i = 0; i < this.m_FrameCount[c]; i++)
            {
                FrameEntry frameEntry = new FrameEntry(reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16(), (uint)(this.m_FrameAddress[c] + (i * 16) + reader.ReadUInt32()), this.m_ColorCount[c]);
                if (m_Frames.ContainsKey(c))
                {
                    m_Frames[c].Add(frameEntry);
                }
                else
                {
                    m_Frames[c] = new List<FrameEntry> { frameEntry };
                }
            }
            return true;
        }
        private bool ReadPixels(BinaryReader reader, int c)
        {
            this._ImageDataOffset.Add(this.m_FrameAddress[c] + this.m_FrameCount[c] * 16);
            this._ImageData[c] = new byte[(int)(this.m_Length[c] - this._ImageDataOffset[c])];
            reader.BaseStream.Seek(this._ImageDataOffset[c], SeekOrigin.Begin);
            this._ImageData[c] = reader.ReadBytes((int)(this.m_Length[c] - this._ImageDataOffset[c]));
            return true;
        }

        //TODO:Make it export either the current img with a liste.txt or a combined bmp file
        //Export the image as a bmp
        private void WorkerExportFile(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < this.treeFramesBox.Nodes.Count; i++)
            {
                int HALFTILE = 22;
                var currentFrame = (FrameEntry)this.treeFramesBox.Nodes[i].Tag;

                var animFramesPerAnimation = m_FrameCount[animId] / 5; //TODO: Change this animId var
                this.worker.ReportProgress(i);
                Bitmap bitmap = this.LoadFrameImage((FrameEntry)this.treeFramesBox.Nodes[i].Tag, animId);

                var centerX = currentFrame.InitCoordsX * -1;
                var centerY = Math.Abs(currentFrame.EndCoordsY) - HALFTILE;
                var listeFilePath = Path.Combine(m_ExtractionFolder, "liste.txt");

                bitmap.Save(string.Format("{0}\\{1}.bmp", this.m_ExtractionFolder, animId + "_" + animFrame, ImageFormat.Bmp));

                animFrame++;
                if (animFrame >= animFramesPerAnimation)
                {
                    //CreatePaletteFile(animId); TODO:Make function wait for the file to be written
                    animId++;
                    animFrame = 0;
                }


                if (File.Exists(listeFilePath))
                {
                    using (StreamWriter sw = File.AppendText(listeFilePath))
                    {
                        WriteListe(sw, animId, animFrame, centerX, centerY);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.CreateText(listeFilePath))
                    {
                        WriteListe(sw, animId, animFrame, centerX, centerY);
                    }
                }
            }
        }

        //Ultima is completely 16 bit based--meaning that each pixel is UWORD value that can be broken down as follows:
        //        Where U = Unused, R = Red, G = Green and B = Blue

        //So, to convert to 32 bit:

        //Color32 = ((((Color16 >> 10) & 0x1F) * 0xFF / 0x1F) |
        //((((Color16 >> 5) & 0x1F) * 0xFF / 0x1F) << 8) |
        //(((Color16 & 0x1F) * 0xFF / 0x1F) << 16));
        //private void CreatePaletteFile(int i)
        //{

        //    var paletteFilePath = Path.Combine(m_ExtractionFolder, "palette_" + i + ".txt");

        //    for (var c = 0; c <= m_ColorCount; c++)
        //    {
        //        if (File.Exists(paletteFilePath))
        //        {
        //            using (StreamWriter sw = File.AppendText(paletteFilePath))
        //            {
        //                var pixel = m_Colors[c].Pixel;
        //                var color = pixel.R * 256 + pixel.G * 256 + pixel.B * 256;
        //                sw.WriteLine(color);
        //            }
        //        }
        //        else
        //        {
        //            using (StreamWriter sw = File.CreateText(paletteFilePath))

        //            {
        //                var pixel = m_Colors[c].Pixel;
        //                var color = pixel.R * 256 + pixel.G * 256 + pixel.B * 256;
        //                sw.WriteLine(color);
        //            }
        //        }
        //    }
        //}

        private static void WriteListe(StreamWriter sw, int animId, int animFrame, int centerX, int centerY)
        {
            sw.WriteLine("{0}", animId + ":" + animFrame);
            sw.WriteLine("CenterX: {0}", centerX.ToString());
            sw.WriteLine("CenterY: {0}", centerY.ToString());
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar.Value = e.ProgressPercentage;
        }
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            animFrame = 0;
            animId = 0;
            this.progressBar.Value = 0;
            MessageBox.Show("Extraction completed.");
        }
    }

}