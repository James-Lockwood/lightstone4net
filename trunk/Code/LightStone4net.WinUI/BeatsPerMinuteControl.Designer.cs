namespace LightStone4net.WinUI
{
	partial class BeatsPerMinuteControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.m_BeatsPerMinuteTextBox = new LightStone4net.WinUI.ThreadSafeControls.SafeTextBox();
			this.m_BeepCheckBox = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// m_BeatsPerMinuteTextBox
			// 
			this.m_BeatsPerMinuteTextBox.Enabled = false;
			this.m_BeatsPerMinuteTextBox.Location = new System.Drawing.Point(112, 3);
			this.m_BeatsPerMinuteTextBox.Name = "m_BeatsPerMinuteTextBox";
			this.m_BeatsPerMinuteTextBox.Size = new System.Drawing.Size(49, 22);
			this.m_BeatsPerMinuteTextBox.TabIndex = 6;
			// 
			// m_BeepCheckBox
			// 
			this.m_BeepCheckBox.AutoSize = true;
			this.m_BeepCheckBox.Checked = true;
			this.m_BeepCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.m_BeepCheckBox.Location = new System.Drawing.Point(176, 6);
			this.m_BeepCheckBox.Name = "m_BeepCheckBox";
			this.m_BeepCheckBox.Size = new System.Drawing.Size(63, 21);
			this.m_BeepCheckBox.TabIndex = 5;
			this.m_BeepCheckBox.Text = "Beep";
			this.m_BeepCheckBox.UseVisualStyleBackColor = true;
			this.m_BeepCheckBox.CheckedChanged += new System.EventHandler(this.OnBeepCheckBoxCheckedChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(4, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(102, 17);
			this.label1.TabIndex = 4;
			this.label1.Text = "Beats / Minute:";
			// 
			// BeatsPerMinuteControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_BeatsPerMinuteTextBox);
			this.Controls.Add(this.m_BeepCheckBox);
			this.Controls.Add(this.label1);
			this.Name = "BeatsPerMinuteControl";
			this.Size = new System.Drawing.Size(242, 30);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private LightStone4net.WinUI.ThreadSafeControls.SafeTextBox m_BeatsPerMinuteTextBox;
		private System.Windows.Forms.CheckBox m_BeepCheckBox;
		private System.Windows.Forms.Label label1;
	}
}
