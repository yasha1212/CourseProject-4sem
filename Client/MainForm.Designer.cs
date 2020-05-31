namespace Client
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.pbScreen = new System.Windows.Forms.PictureBox();
            this.bScreenshot = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbScreen)).BeginInit();
            this.SuspendLayout();
            // 
            // pbScreen
            // 
            this.pbScreen.BackColor = System.Drawing.SystemColors.ControlText;
            this.pbScreen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbScreen.Location = new System.Drawing.Point(12, 12);
            this.pbScreen.Name = "pbScreen";
            this.pbScreen.Size = new System.Drawing.Size(822, 554);
            this.pbScreen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbScreen.TabIndex = 0;
            this.pbScreen.TabStop = false;
            // 
            // bScreenshot
            // 
            this.bScreenshot.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bScreenshot.Location = new System.Drawing.Point(360, 572);
            this.bScreenshot.Name = "bScreenshot";
            this.bScreenshot.Size = new System.Drawing.Size(142, 43);
            this.bScreenshot.TabIndex = 1;
            this.bScreenshot.Text = "Screenshot";
            this.bScreenshot.UseVisualStyleBackColor = true;
            this.bScreenshot.Click += new System.EventHandler(this.bScreenshot_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1206, 619);
            this.Controls.Add(this.bScreenshot);
            this.Controls.Add(this.pbScreen);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RemoteScreen";
            ((System.ComponentModel.ISupportInitialize)(this.pbScreen)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbScreen;
        private System.Windows.Forms.Button bScreenshot;
    }
}

