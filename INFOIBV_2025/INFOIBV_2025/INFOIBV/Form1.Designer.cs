namespace INFOIBV
{
    partial class INFOIBV
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.LoadImageButton = new System.Windows.Forms.Button();
            this.openImageDialog = new System.Windows.Forms.OpenFileDialog();
            this.imageFileName = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.applyButton = new System.Windows.Forms.Button();
            this.saveImageDialog = new System.Windows.Forms.SaveFileDialog();
            this.saveButton = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.comboBox = new System.Windows.Forms.ComboBox();
            this.numericThreshold = new System.Windows.Forms.NumericUpDown();
            this.numericKernelSize = new System.Windows.Forms.NumericUpDown();
            this.numericSigma = new System.Windows.Forms.NumericUpDown();
            this.numericGrayscaleDilationSize = new System.Windows.Forms.NumericUpDown();
            this.numericBinaryClosingSize = new System.Windows.Forms.NumericUpDown();
            this.numericEdgeMagnitude = new System.Windows.Forms.NumericUpDown();
            this.numericPeakThreshold = new System.Windows.Forms.NumericUpDown();
            this.numericMinSegmentLength = new System.Windows.Forms.NumericUpDown();
            this.numericMaxGap = new System.Windows.Forms.NumericUpDown();
            this.labelThreshold = new System.Windows.Forms.Label();
            this.labelKernelSize = new System.Windows.Forms.Label();
            this.labelSigma = new System.Windows.Forms.Label();
            this.labelBinaryClosingSize = new System.Windows.Forms.Label();
            this.labelGrayscaleDilationSize = new System.Windows.Forms.Label();
            this.labelEdgeMagnitude = new System.Windows.Forms.Label();
            this.labelPeakThreshold = new System.Windows.Forms.Label();
            this.labelMinSegmentLength = new System.Windows.Forms.Label();
            this.labelMaxGap = new System.Windows.Forms.Label();
            
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericKernelSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericSigma)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericGrayscaleDilationSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericBinaryClosingSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericEdgeMagnitude)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPeakThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMinSegmentLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxGap)).BeginInit();
            this.SuspendLayout();
            
            // 
            // LoadImageButton
            // 
            this.LoadImageButton.Location = new System.Drawing.Point(16, 14);
            this.LoadImageButton.Margin = new System.Windows.Forms.Padding(4);
            this.LoadImageButton.Name = "LoadImageButton";
            this.LoadImageButton.Size = new System.Drawing.Size(131, 28);
            this.LoadImageButton.TabIndex = 0;
            this.LoadImageButton.Text = "Load image...";
            this.LoadImageButton.UseVisualStyleBackColor = true;
            this.LoadImageButton.Click += new System.EventHandler(this.loadImageButton_Click);
            // 
            // openImageDialog
            // 
            this.openImageDialog.Filter = "Bitmap files (*.bmp;*.gif;*.jpg;*.png;*.tiff;*.jpeg)|*.bmp;*.gif;*.jpg;*.png;*.ti" +
        "ff;*.jpeg";
            this.openImageDialog.InitialDirectory = "..\\..\\images";
            // 
            // imageFileName
            // 
            this.imageFileName.Location = new System.Drawing.Point(155, 15);
            this.imageFileName.Margin = new System.Windows.Forms.Padding(4);
            this.imageFileName.Name = "imageFileName";
            this.imageFileName.ReadOnly = true;
            this.imageFileName.Size = new System.Drawing.Size(437, 22);
            this.imageFileName.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(17, 55);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(683, 630);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(877, 14);
            this.applyButton.Margin = new System.Windows.Forms.Padding(4);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(137, 28);
            this.applyButton.TabIndex = 3;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // saveImageDialog
            // 
            this.saveImageDialog.Filter = "Bitmap file (*.bmp)|*.bmp";
            this.saveImageDialog.InitialDirectory = "..\\..\\images";
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(1264, 14);
            this.saveButton.Margin = new System.Windows.Forms.Padding(4);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(127, 28);
            this.saveButton.TabIndex = 4;
            this.saveButton.Text = "Save as BMP...";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(708, 55);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(683, 630);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox2.TabIndex = 5;
            this.pictureBox2.TabStop = false;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(1023, 16);
            this.progressBar.Margin = new System.Windows.Forms.Padding(4);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(233, 25);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 6;
            this.progressBar.Visible = false;
            // 
            // comboBox
            // 
            this.comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox.FormattingEnabled = true;
            this.comboBox.Location = new System.Drawing.Point(601, 15);
            this.comboBox.Margin = new System.Windows.Forms.Padding(4);
            this.comboBox.Name = "comboBox";
            this.comboBox.Size = new System.Drawing.Size(267, 24);
            this.comboBox.TabIndex = 7;
            this.comboBox.SelectedIndexChanged += new System.EventHandler(this.comboBox_SelectedIndexChanged);
            // 
            // numericThreshold
            // 
            this.numericThreshold.Location = new System.Drawing.Point(750, 70);
            this.numericThreshold.Name = "numericThreshold";
            this.numericThreshold.Size = new System.Drawing.Size(60, 22);
            this.numericThreshold.Minimum = 0;
            this.numericThreshold.Maximum = 255;
            this.numericThreshold.Value = 127;
            this.numericThreshold.Visible = false;
            // 
            // numericKernelSize
            // 
            this.numericKernelSize.Location = new System.Drawing.Point(820, 80);
            this.numericKernelSize.Name = "numericKernelSize";
            this.numericKernelSize.Size = new System.Drawing.Size(60, 22);
            this.numericKernelSize.Minimum = 3;
            this.numericKernelSize.Maximum = 31;
            this.numericKernelSize.Increment = 2;
            this.numericKernelSize.Value = 5;
            this.numericKernelSize.Visible = false;
            // 
            // numericSigma
            // 
            this.numericSigma.Location = new System.Drawing.Point(900, 80);
            this.numericSigma.Name = "numericSigma";
            this.numericSigma.Size = new System.Drawing.Size(60, 22);
            this.numericSigma.Minimum = 0;
            this.numericSigma.Maximum = 10;
            this.numericSigma.DecimalPlaces = 1;
            this.numericSigma.Increment = 0.1M;
            this.numericSigma.Value = 1.0M;
            this.numericSigma.Visible = false;
            // 
            // numericBinaryClosingSize
            // 
            this.numericBinaryClosingSize.Location = new System.Drawing.Point(980, 80);
            this.numericBinaryClosingSize.Name = "numericBinaryClosingSize";
            this.numericBinaryClosingSize.Size = new System.Drawing.Size(60, 22);
            this.numericBinaryClosingSize.Minimum = 3;
            this.numericBinaryClosingSize.Maximum = 31;
            this.numericBinaryClosingSize.Increment = 2;
            this.numericBinaryClosingSize.Value = 3;
            this.numericBinaryClosingSize.Visible = false;
            // 
            // numericGrayscaleDilationSize
            // 
            this.numericGrayscaleDilationSize.Location = new System.Drawing.Point(1040, 80);
            this.numericGrayscaleDilationSize.Name = "numericGrayscaleDilationSize";
            this.numericGrayscaleDilationSize.Size = new System.Drawing.Size(60, 22);
            this.numericGrayscaleDilationSize.Minimum = 3;
            this.numericGrayscaleDilationSize.Maximum = 31;
            this.numericGrayscaleDilationSize.Increment = 2;
            this.numericGrayscaleDilationSize.Value = 3;
            this.numericGrayscaleDilationSize.Visible = false;
            // 
            // numericEdgeMagnitude
            // 
            this.numericEdgeMagnitude.Location = new System.Drawing.Point(820, 70);
            this.numericEdgeMagnitude.Name = "numericEdgeMagnitude";
            this.numericEdgeMagnitude.Size = new System.Drawing.Size(60, 22);
            this.numericEdgeMagnitude.Minimum = 0;
            this.numericEdgeMagnitude.Maximum = 255;
            this.numericEdgeMagnitude.Value = 100;
            this.numericEdgeMagnitude.Visible = false;
            // 
            // numericPeakThreshold
            // 
            this.numericPeakThreshold.Location = new System.Drawing.Point(930, 70);
            this.numericPeakThreshold.Name = "numericPeakThreshold";
            this.numericPeakThreshold.Size = new System.Drawing.Size(60, 22);
            this.numericPeakThreshold.Minimum = 0;
            this.numericPeakThreshold.Maximum = 1;
            this.numericPeakThreshold.DecimalPlaces = 2;
            this.numericPeakThreshold.Increment = 0.01M;
            this.numericPeakThreshold.Value = 0.5M;
            this.numericPeakThreshold.Visible = false;
            // 
            // numericMinSegmentLength
            // 
            this.numericMinSegmentLength.Location = new System.Drawing.Point(1065, 70);
            this.numericMinSegmentLength.Name = "numericMinSegmentLength";
            this.numericMinSegmentLength.Size = new System.Drawing.Size(60, 22);
            this.numericMinSegmentLength.Minimum = 1;
            this.numericMinSegmentLength.Maximum = 200;
            this.numericMinSegmentLength.Value = 30;
            this.numericMinSegmentLength.Visible = false;
            // 
            // numericMaxGap
            // 
            this.numericMaxGap.Location = new System.Drawing.Point(1150, 70);
            this.numericMaxGap.Name = "numericMaxGap";
            this.numericMaxGap.Size = new System.Drawing.Size(60, 22);
            this.numericMaxGap.Minimum = 0;
            this.numericMaxGap.Maximum = 50;
            this.numericMaxGap.Value = 3;
            this.numericMaxGap.Visible = false;

            // Labels
            // 
            // labelThreshold
            // 
            this.labelThreshold.AutoSize = true;
            this.labelThreshold.Location = new System.Drawing.Point(750, 50);
            this.labelThreshold.Name = "labelThreshold";
            this.labelThreshold.Size = new System.Drawing.Size(70, 16);
            this.labelThreshold.Text = "Threshold";
            this.labelThreshold.Visible = false;
            // 
            // labelKernelSize
            // 
            this.labelKernelSize.AutoSize = true;
            this.labelKernelSize.Location = new System.Drawing.Point(820, 60);
            this.labelKernelSize.Name = "labelKernelSize";
            this.labelKernelSize.Size = new System.Drawing.Size(74, 16);
            this.labelKernelSize.Text = "Kernel Size";
            this.labelKernelSize.Visible = false;
            // 
            // labelSigma
            // 
            this.labelSigma.AutoSize = true;
            this.labelSigma.Location = new System.Drawing.Point(900, 60);
            this.labelSigma.Name = "labelSigma";
            this.labelSigma.Size = new System.Drawing.Size(44, 16);
            this.labelSigma.Text = "Sigma";
            this.labelSigma.Visible = false;
            // 
            // labelBinaryClosingSize
            // 
            this.labelBinaryClosingSize.AutoSize = true;
            this.labelBinaryClosingSize.Location = new System.Drawing.Point(980, 60);
            this.labelBinaryClosingSize.Name = "labelBinaryClosingSize";
            this.labelBinaryClosingSize.Size = new System.Drawing.Size(85, 16);
            this.labelBinaryClosingSize.Text = "Closing Size";
            this.labelBinaryClosingSize.Visible = false;
            // 
            // labelGrayscaleDilationSize
            // 
            this.labelGrayscaleDilationSize.AutoSize = true;
            this.labelGrayscaleDilationSize.Location = new System.Drawing.Point(1040, 60);
            this.labelGrayscaleDilationSize.Name = "labelGrayscaleDilationSize";
            this.labelGrayscaleDilationSize.Size = new System.Drawing.Size(89, 16);
            this.labelGrayscaleDilationSize.Text = "Dilation Size";
            this.labelGrayscaleDilationSize.Visible = false;
            // 
            // labelEdgeMagnitude
            // 
            this.labelEdgeMagnitude.AutoSize = true;
            this.labelEdgeMagnitude.Location = new System.Drawing.Point(820, 50);
            this.labelEdgeMagnitude.Name = "labelEdgeMagnitude";
            this.labelEdgeMagnitude.Size = new System.Drawing.Size(95, 16);
            this.labelEdgeMagnitude.Text = "Edge Threshold";
            this.labelEdgeMagnitude.Visible = false;
            // 
            // labelPeakThreshold
            // 
            this.labelPeakThreshold.AutoSize = true;
            this.labelPeakThreshold.Location = new System.Drawing.Point(930, 50);
            this.labelPeakThreshold.Name = "labelPeakThreshold";
            this.labelPeakThreshold.Size = new System.Drawing.Size(95, 16);
            this.labelPeakThreshold.Text = "Peak Threshold";
            this.labelPeakThreshold.Visible = false;
            // 
            // labelMinSegmentLength
            // 
            this.labelMinSegmentLength.AutoSize = true;
            this.labelMinSegmentLength.Location = new System.Drawing.Point(1065, 50);
            this.labelMinSegmentLength.Name = "labelMinSegmentLength";
            this.labelMinSegmentLength.Size = new System.Drawing.Size(80, 16);
            this.labelMinSegmentLength.Text = "Min Length";
            this.labelMinSegmentLength.Visible = false;
            // 
            // labelMaxGap
            // 
            this.labelMaxGap.AutoSize = true;
            this.labelMaxGap.Location = new System.Drawing.Point(1150, 50);
            this.labelMaxGap.Name = "labelMaxGap";
            this.labelMaxGap.Size = new System.Drawing.Size(62, 16);
            this.labelMaxGap.Text = "Max Gap";
            this.labelMaxGap.Visible = false;
            
            // Add all controls to form
            this.Controls.Add(this.comboBox);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.imageFileName);
            this.Controls.Add(this.LoadImageButton);
            this.Controls.Add(this.numericThreshold);
            this.Controls.Add(this.numericKernelSize);
            this.Controls.Add(this.numericSigma);
            this.Controls.Add(this.numericBinaryClosingSize);
            this.Controls.Add(this.numericGrayscaleDilationSize);
            this.Controls.Add(this.numericEdgeMagnitude);
            this.Controls.Add(this.numericPeakThreshold);
            this.Controls.Add(this.numericMinSegmentLength);
            this.Controls.Add(this.numericMaxGap);
            this.Controls.Add(this.labelThreshold);
            this.Controls.Add(this.labelKernelSize);
            this.Controls.Add(this.labelSigma);
            this.Controls.Add(this.labelBinaryClosingSize);
            this.Controls.Add(this.labelGrayscaleDilationSize);
            this.Controls.Add(this.labelEdgeMagnitude);
            this.Controls.Add(this.labelPeakThreshold);
            this.Controls.Add(this.labelMinSegmentLength);
            this.Controls.Add(this.labelMaxGap);
            
            // 
            // INFOIBV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1403, 709);
            this.Location = new System.Drawing.Point(10, 10);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "INFOIBV";
            this.ShowIcon = false;
            this.Text = "INFOIBV";
            
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericKernelSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericSigma)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericBinaryClosingSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericGrayscaleDilationSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericEdgeMagnitude)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPeakThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMinSegmentLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxGap)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

            // Ensure buttons are on top
            this.LoadImageButton.BringToFront();
            this.applyButton.BringToFront();
            this.saveButton.BringToFront();
            this.comboBox.BringToFront();
            this.imageFileName.BringToFront();

            // Ensure numeric controls are visible when needed
            this.numericThreshold.BringToFront();
            this.numericKernelSize.BringToFront();
            this.numericSigma.BringToFront();
            this.numericBinaryClosingSize.BringToFront();
            this.numericGrayscaleDilationSize.BringToFront();
            this.numericEdgeMagnitude.BringToFront();
            this.numericPeakThreshold.BringToFront();
            this.numericMinSegmentLength.BringToFront();
            this.numericMaxGap.BringToFront();

            // Bring labels to front too
            this.labelThreshold.BringToFront();
            this.labelKernelSize.BringToFront();
            this.labelSigma.BringToFront();
            this.labelBinaryClosingSize.BringToFront();
            this.labelGrayscaleDilationSize.BringToFront();
            this.labelEdgeMagnitude.BringToFront();
            this.labelPeakThreshold.BringToFront();
            this.labelMinSegmentLength.BringToFront();
            this.labelMaxGap.BringToFront();
        }

        #endregion

        private System.Windows.Forms.Button LoadImageButton;
        private System.Windows.Forms.OpenFileDialog openImageDialog;
        private System.Windows.Forms.TextBox imageFileName;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.SaveFileDialog saveImageDialog;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.ComboBox comboBox;
        private System.Windows.Forms.NumericUpDown numericThreshold;
        private System.Windows.Forms.NumericUpDown numericKernelSize;
        private System.Windows.Forms.NumericUpDown numericSigma;
        private System.Windows.Forms.NumericUpDown numericGrayscaleDilationSize;
        private System.Windows.Forms.NumericUpDown numericBinaryClosingSize;
        private System.Windows.Forms.NumericUpDown numericEdgeMagnitude;
        private System.Windows.Forms.NumericUpDown numericPeakThreshold;
        private System.Windows.Forms.NumericUpDown numericMinSegmentLength;
        private System.Windows.Forms.NumericUpDown numericMaxGap;
        private System.Windows.Forms.Label labelThreshold;
        private System.Windows.Forms.Label labelEdgeMagnitude;
        private System.Windows.Forms.Label labelPeakThreshold;
        private System.Windows.Forms.Label labelMinSegmentLength;
        private System.Windows.Forms.Label labelMaxGap;
        private System.Windows.Forms.Label labelKernelSize;
        private System.Windows.Forms.Label labelSigma;
        private System.Windows.Forms.Label labelBinaryClosingSize;
        private System.Windows.Forms.Label labelGrayscaleDilationSize;
    }
}