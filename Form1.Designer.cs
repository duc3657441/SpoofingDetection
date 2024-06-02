namespace SpoofingDetectionWinformApp
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBox1 = new PictureBox();
            btnExit = new Button();
            outputRichTextBox = new RichTextBox();
            btnCloseCamera = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(800, 600);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // btnExit
            // 
            btnExit.Location = new Point(806, 477);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(215, 52);
            btnExit.TabIndex = 1;
            btnExit.Text = "Exit";
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += button1_Click;
            // 
            // outputRichTextBox
            // 
            outputRichTextBox.Location = new Point(806, 12);
            outputRichTextBox.Name = "outputRichTextBox";
            outputRichTextBox.Size = new Size(374, 459);
            outputRichTextBox.TabIndex = 2;
            outputRichTextBox.Text = "";
            // 
            // btnCloseCamera
            // 
            btnCloseCamera.Location = new Point(1027, 477);
            btnCloseCamera.Name = "btnCloseCamera";
            btnCloseCamera.Size = new Size(149, 46);
            btnCloseCamera.TabIndex = 3;
            btnCloseCamera.Text = "Close Camera";
            btnCloseCamera.UseVisualStyleBackColor = true;
            btnCloseCamera.Click += btnCloseCamera_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1188, 953);
            Controls.Add(btnCloseCamera);
            Controls.Add(outputRichTextBox);
            Controls.Add(btnExit);
            Controls.Add(pictureBox1);
            Name = "Form1";
            Text = "Form1";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox1;
        private Button btnExit;
        private RichTextBox outputRichTextBox;
        private Button btnCloseCamera;
    }
}
