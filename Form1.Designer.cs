namespace ShadowSocksRUpdate
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.State = new System.Windows.Forms.Label();
            this.ReLink = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // State
            // 
            this.State.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.State.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.State.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.State.Location = new System.Drawing.Point(12, 9);
            this.State.Name = "State";
            this.State.Size = new System.Drawing.Size(463, 188);
            this.State.TabIndex = 0;
            this.State.Tag = "State";
            this.State.Text = "State(Times:0)..\r\n小心心么么哒.";
            this.State.Click += new System.EventHandler(this.State_Click);
            // 
            // ReLink
            // 
            this.ReLink.BackColor = System.Drawing.SystemColors.HotTrack;
            this.ReLink.Enabled = false;
            this.ReLink.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.ReLink.Location = new System.Drawing.Point(71, 218);
            this.ReLink.Name = "ReLink";
            this.ReLink.Size = new System.Drawing.Size(104, 22);
            this.ReLink.TabIndex = 1;
            this.ReLink.Text = "重试";
            this.ReLink.UseVisualStyleBackColor = false;
            this.ReLink.Click += new System.EventHandler(this.ReLink_Click);
            // 
            // Cancel
            // 
            this.Cancel.BackColor = System.Drawing.SystemColors.HotTrack;
            this.Cancel.Enabled = false;
            this.Cancel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.Cancel.Location = new System.Drawing.Point(298, 218);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(104, 22);
            this.Cancel.TabIndex = 2;
            this.Cancel.Text = "跳过更新";
            this.Cancel.UseVisualStyleBackColor = false;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(487, 276);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.ReLink);
            this.Controls.Add(this.State);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "配置文件更新程序";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label State;
        private System.Windows.Forms.Button ReLink;
        private System.Windows.Forms.Button Cancel;
    }
}

