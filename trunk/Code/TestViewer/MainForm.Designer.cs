using LightStone4net.WinUI.ThreadSafeControls;
namespace TestViewer
{
	partial class MainForm
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
			this.components = new System.ComponentModel.Container();
			this.m_BottomPanel = new System.Windows.Forms.Panel();
			this.m_Timer = new System.Windows.Forms.Timer(this.components);
			this.beatsPerMinuteControl1 = new LightStone4net.WinUI.BeatsPerMinuteControl();
			this.heartSignalControl1 = new LightStone4net.WinUI.HeartSignalControl();
			this.m_BottomPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_BottomPanel
			// 
			this.m_BottomPanel.Controls.Add(this.beatsPerMinuteControl1);
			this.m_BottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.m_BottomPanel.Location = new System.Drawing.Point(0, 316);
			this.m_BottomPanel.Name = "m_BottomPanel";
			this.m_BottomPanel.Size = new System.Drawing.Size(562, 49);
			this.m_BottomPanel.TabIndex = 0;
			// 
			// m_Timer
			// 
			this.m_Timer.Tick += new System.EventHandler(this.OnTimerTick);
			// 
			// beatsPerMinuteControl1
			// 
			this.beatsPerMinuteControl1.Location = new System.Drawing.Point(13, 7);
			this.beatsPerMinuteControl1.Name = "beatsPerMinuteControl1";
			this.beatsPerMinuteControl1.Size = new System.Drawing.Size(242, 30);
			this.beatsPerMinuteControl1.TabIndex = 0;
			// 
			// heartSignalControl1
			// 
			this.heartSignalControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.heartSignalControl1.Location = new System.Drawing.Point(0, 0);
			this.heartSignalControl1.Name = "heartSignalControl1";
			this.heartSignalControl1.Size = new System.Drawing.Size(562, 316);
			this.heartSignalControl1.TabIndex = 1;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(562, 365);
			this.Controls.Add(this.heartSignalControl1);
			this.Controls.Add(this.m_BottomPanel);
			this.Name = "MainForm";
			this.Text = "Form1";
			this.m_BottomPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel m_BottomPanel;
		private System.Windows.Forms.Timer m_Timer;
		private LightStone4net.WinUI.BeatsPerMinuteControl beatsPerMinuteControl1;
		private LightStone4net.WinUI.HeartSignalControl heartSignalControl1;
	}
}

