using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace KRFrameViewer
{
    public class About : Form
    {
        private IContainer components;

        private Button button1;

        private Label label1;

        private TextBox textBox1;

        private LinkLabel linkLabel1;

        private LinkLabel linkLabel2;

        private Label label2;

        private TextBox textBox2;

        private Label label3;

        private TextBox textBox3;

        private PictureBox pictureBox1;

        public About()
        {
            this.InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.button1 = new Button();
            this.label1 = new Label();
            this.textBox1 = new TextBox();
            this.linkLabel1 = new LinkLabel();
            this.linkLabel2 = new LinkLabel();
            this.label2 = new Label();
            this.textBox2 = new TextBox();
            this.label3 = new Label();
            this.textBox3 = new TextBox();
            this.pictureBox1 = new PictureBox();
            ((ISupportInitialize)this.pictureBox1).BeginInit();
            base.SuspendLayout();
            this.button1.Enabled = false;
            this.button1.Location = new Point(12, 229);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(108, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Check for Updates";
            this.button1.UseVisualStyleBackColor = true;
            this.label1.AutoSize = true;
            this.label1.Location = new Point(21, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Authors:";
            this.textBox1.Location = new Point(77, 6);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(195, 20);
            this.textBox1.TabIndex = 2;
            this.textBox1.Text = "Kons - Wim";
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new Point(140, 234);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(41, 13);
            this.linkLabel1.TabIndex = 3;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "UoDev";
            this.linkLabel1.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new Point(202, 234);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(65, 13);
            this.linkLabel2.TabIndex = 4;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "Kons' Forum";
            this.linkLabel2.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            this.label2.AutoSize = true;
            this.label2.Location = new Point(25, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Thanks:";
            this.textBox2.Location = new Point(77, 32);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(195, 58);
            this.textBox2.TabIndex = 6;
            this.textBox2.Text = "- Wim, for helping me a lot with ASM\r\n- Malganis\r\n- Smjert";
            this.label3.AutoSize = true;
            this.label3.Location = new Point(8, 101);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Description:";
            this.textBox3.Location = new Point(77, 98);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(195, 125);
            this.textBox3.TabIndex = 8;
            this.textBox3.Text = "Format supported:  .bin Animation files from Ultima Online : Kingdom Reborn Client.\r\n\r\nTo get files you need to uncompress \r\n-AnimationFrameX.uop\r\n-Paperdoll.uop\r\n with Malganis Unpacking Editor.";
            this.pictureBox1.BackColor = SystemColors.ControlDarkDark;
            this.pictureBox1.Location = new Point(12, 128);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(55, 95);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.ClientSize = new System.Drawing.Size(284, 264);
            base.Controls.Add(this.pictureBox1);
            base.Controls.Add(this.textBox3);
            base.Controls.Add(this.label3);
            base.Controls.Add(this.textBox2);
            base.Controls.Add(this.label2);
            base.Controls.Add(this.linkLabel2);
            base.Controls.Add(this.linkLabel1);
            base.Controls.Add(this.textBox1);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.button1);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "About";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            this.Text = "KR FrameReader";
            base.TopMost = true;
            ((ISupportInitialize)this.pictureBox1).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start("http://uodev.de/viewforum.php?f=66");
            }
            catch
            {
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start("http://nextgamers.it/forum/viewforum.php?f=7");
            }
            catch
            {
            }
        }
    }
}