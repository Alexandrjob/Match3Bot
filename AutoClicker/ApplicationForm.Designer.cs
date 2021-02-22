
namespace AutoClicker
{
    partial class ApplicationForm
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
            this.pictureBoxDefoultImage = new System.Windows.Forms.PictureBox();
            this.buttonStartAlgorithm = new System.Windows.Forms.Button();
            this.pictureBoxContoursImage = new System.Windows.Forms.PictureBox();
            this.labelStateProcces = new System.Windows.Forms.Label();
            this.buttonStopAlgoritm = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDefoultImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxContoursImage)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxDefoultImage
            // 
            this.pictureBoxDefoultImage.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.pictureBoxDefoultImage.Location = new System.Drawing.Point(25, 53);
            this.pictureBoxDefoultImage.Name = "pictureBoxDefoultImage";
            this.pictureBoxDefoultImage.Size = new System.Drawing.Size(312, 352);
            this.pictureBoxDefoultImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxDefoultImage.TabIndex = 0;
            this.pictureBoxDefoultImage.TabStop = false;
            // 
            // buttonStartAlgorithm
            // 
            this.buttonStartAlgorithm.Location = new System.Drawing.Point(25, 8);
            this.buttonStartAlgorithm.Name = "buttonStartAlgorithm";
            this.buttonStartAlgorithm.Size = new System.Drawing.Size(144, 39);
            this.buttonStartAlgorithm.TabIndex = 1;
            this.buttonStartAlgorithm.Text = "Запустить алгоритм";
            this.buttonStartAlgorithm.UseVisualStyleBackColor = true;
            this.buttonStartAlgorithm.Click += new System.EventHandler(this.ButtonStartAlgorithm_Click);
            // 
            // pictureBoxContoursImage
            // 
            this.pictureBoxContoursImage.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.pictureBoxContoursImage.Location = new System.Drawing.Point(386, 53);
            this.pictureBoxContoursImage.Name = "pictureBoxContoursImage";
            this.pictureBoxContoursImage.Size = new System.Drawing.Size(312, 357);
            this.pictureBoxContoursImage.TabIndex = 2;
            this.pictureBoxContoursImage.TabStop = false;
            // 
            // labelStateProcces
            // 
            this.labelStateProcces.AutoSize = true;
            this.labelStateProcces.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelStateProcces.Location = new System.Drawing.Point(382, 23);
            this.labelStateProcces.Name = "labelStateProcces";
            this.labelStateProcces.Size = new System.Drawing.Size(162, 24);
            this.labelStateProcces.TabIndex = 3;
            this.labelStateProcces.Text = "Статус процесса";
            // 
            // buttonStopAlgoritm
            // 
            this.buttonStopAlgoritm.Location = new System.Drawing.Point(175, 8);
            this.buttonStopAlgoritm.Name = "buttonStopAlgoritm";
            this.buttonStopAlgoritm.Size = new System.Drawing.Size(162, 39);
            this.buttonStopAlgoritm.TabIndex = 4;
            this.buttonStopAlgoritm.Text = "Остановить алгоритм";
            this.buttonStopAlgoritm.UseVisualStyleBackColor = true;
            this.buttonStopAlgoritm.Click += new System.EventHandler(this.ButtonStopAlgoritm_Click);
            // 
            // ApplicationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 437);
            this.Controls.Add(this.buttonStopAlgoritm);
            this.Controls.Add(this.labelStateProcces);
            this.Controls.Add(this.pictureBoxContoursImage);
            this.Controls.Add(this.buttonStartAlgorithm);
            this.Controls.Add(this.pictureBoxDefoultImage);
            this.Name = "ApplicationForm";
            this.Text = "ApplicationForm";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDefoultImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxContoursImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.PictureBox pictureBoxDefoultImage;
        public System.Windows.Forms.PictureBox pictureBoxContoursImage;
        private System.Windows.Forms.Button buttonStartAlgorithm;
        public System.Windows.Forms.Label labelStateProcces;
        private System.Windows.Forms.Button buttonStopAlgoritm;
    }
}