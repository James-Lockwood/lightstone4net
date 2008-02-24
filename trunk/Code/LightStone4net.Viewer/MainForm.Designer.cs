namespace LightStone4net.Viewer
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
			this.m_MainSplitContainer = new System.Windows.Forms.SplitContainer();
			this.m_BeatsPerMinPanel = new System.Windows.Forms.Panel();
			this.m_HrvResetButton = new System.Windows.Forms.Button();
			this.m_BeepCheckBox = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.m_PlotsSplitContainer = new System.Windows.Forms.SplitContainer();
			this.m_HeartSignalPlotControl = new LightStone4net.WinUI.HeartSignalPlotControl();
			this.m_HrvSdnnTextBox = new LightStone4net.WinUI.ThreadSafeControls.SafeTextBox();
			this.m_BeatsPerMinTextBox = new LightStone4net.WinUI.ThreadSafeControls.SafeTextBox();
			this.m_RRPlotControl = new LightStone4net.WinUI.RRPlotControl();
			this.m_HrvSdnnPlotControl = new LightStone4net.WinUI.HrvSdnnPlotControl();
			this.m_MainSplitContainer.Panel1.SuspendLayout();
			this.m_MainSplitContainer.Panel2.SuspendLayout();
			this.m_MainSplitContainer.SuspendLayout();
			this.m_BeatsPerMinPanel.SuspendLayout();
			this.m_PlotsSplitContainer.Panel1.SuspendLayout();
			this.m_PlotsSplitContainer.Panel2.SuspendLayout();
			this.m_PlotsSplitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_MainSplitContainer
			// 
			this.m_MainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_MainSplitContainer.Location = new System.Drawing.Point(0, 0);
			this.m_MainSplitContainer.Name = "m_MainSplitContainer";
			this.m_MainSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// m_MainSplitContainer.Panel1
			// 
			this.m_MainSplitContainer.Panel1.Controls.Add(this.m_HeartSignalPlotControl);
			this.m_MainSplitContainer.Panel1.Controls.Add(this.m_BeatsPerMinPanel);
			// 
			// m_MainSplitContainer.Panel2
			// 
			this.m_MainSplitContainer.Panel2.Controls.Add(this.m_PlotsSplitContainer);
			this.m_MainSplitContainer.Size = new System.Drawing.Size(573, 396);
			this.m_MainSplitContainer.SplitterDistance = 120;
			this.m_MainSplitContainer.TabIndex = 0;
			// 
			// m_BeatsPerMinPanel
			// 
			this.m_BeatsPerMinPanel.Controls.Add(this.m_HrvResetButton);
			this.m_BeatsPerMinPanel.Controls.Add(this.m_BeepCheckBox);
			this.m_BeatsPerMinPanel.Controls.Add(this.m_HrvSdnnTextBox);
			this.m_BeatsPerMinPanel.Controls.Add(this.label2);
			this.m_BeatsPerMinPanel.Controls.Add(this.m_BeatsPerMinTextBox);
			this.m_BeatsPerMinPanel.Controls.Add(this.label1);
			this.m_BeatsPerMinPanel.Dock = System.Windows.Forms.DockStyle.Left;
			this.m_BeatsPerMinPanel.Location = new System.Drawing.Point(0, 0);
			this.m_BeatsPerMinPanel.Name = "m_BeatsPerMinPanel";
			this.m_BeatsPerMinPanel.Size = new System.Drawing.Size(256, 120);
			this.m_BeatsPerMinPanel.TabIndex = 0;
			// 
			// m_HrvResetButton
			// 
			this.m_HrvResetButton.Location = new System.Drawing.Point(132, 72);
			this.m_HrvResetButton.Name = "m_HrvResetButton";
			this.m_HrvResetButton.Size = new System.Drawing.Size(95, 23);
			this.m_HrvResetButton.TabIndex = 5;
			this.m_HrvResetButton.Text = "Reset HRV";
			this.m_HrvResetButton.UseVisualStyleBackColor = true;
			this.m_HrvResetButton.Click += new System.EventHandler(this.OnHrvResetButtonClick);
			// 
			// m_BeepCheckBox
			// 
			this.m_BeepCheckBox.AutoSize = true;
			this.m_BeepCheckBox.Location = new System.Drawing.Point(16, 75);
			this.m_BeepCheckBox.Name = "m_BeepCheckBox";
			this.m_BeepCheckBox.Size = new System.Drawing.Size(63, 21);
			this.m_BeepCheckBox.TabIndex = 4;
			this.m_BeepCheckBox.Text = "Beep";
			this.m_BeepCheckBox.UseVisualStyleBackColor = true;
			this.m_BeepCheckBox.CheckedChanged += new System.EventHandler(this.OnBeepCheckBoxCheckedChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(134, 13);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(90, 17);
			this.label2.TabIndex = 2;
			this.label2.Text = "HRV (SDNN)";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(14, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(98, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "Beats / Minute";
			// 
			// m_PlotsSplitContainer
			// 
			this.m_PlotsSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_PlotsSplitContainer.Location = new System.Drawing.Point(0, 0);
			this.m_PlotsSplitContainer.Name = "m_PlotsSplitContainer";
			this.m_PlotsSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// m_PlotsSplitContainer.Panel1
			// 
			this.m_PlotsSplitContainer.Panel1.Controls.Add(this.m_RRPlotControl);
			// 
			// m_PlotsSplitContainer.Panel2
			// 
			this.m_PlotsSplitContainer.Panel2.Controls.Add(this.m_HrvSdnnPlotControl);
			this.m_PlotsSplitContainer.Size = new System.Drawing.Size(573, 272);
			this.m_PlotsSplitContainer.SplitterDistance = 136;
			this.m_PlotsSplitContainer.TabIndex = 1;
			// 
			// m_HeartSignalPlotControl
			// 
			this.m_HeartSignalPlotControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_HeartSignalPlotControl.Location = new System.Drawing.Point(256, 0);
			this.m_HeartSignalPlotControl.Name = "m_HeartSignalPlotControl";
			this.m_HeartSignalPlotControl.Size = new System.Drawing.Size(317, 120);
			this.m_HeartSignalPlotControl.TabIndex = 1;
			// 
			// m_HrvSdnnTextBox
			// 
			this.m_HrvSdnnTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_HrvSdnnTextBox.Location = new System.Drawing.Point(132, 34);
			this.m_HrvSdnnTextBox.Name = "m_HrvSdnnTextBox";
			this.m_HrvSdnnTextBox.Size = new System.Drawing.Size(95, 34);
			this.m_HrvSdnnTextBox.TabIndex = 3;
			// 
			// m_BeatsPerMinTextBox
			// 
			this.m_BeatsPerMinTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_BeatsPerMinTextBox.Location = new System.Drawing.Point(16, 34);
			this.m_BeatsPerMinTextBox.Name = "m_BeatsPerMinTextBox";
			this.m_BeatsPerMinTextBox.Size = new System.Drawing.Size(95, 34);
			this.m_BeatsPerMinTextBox.TabIndex = 1;
			// 
			// m_RRPlotControl
			// 
			this.m_RRPlotControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_RRPlotControl.Location = new System.Drawing.Point(0, 0);
			this.m_RRPlotControl.Name = "m_RRPlotControl";
			this.m_RRPlotControl.Size = new System.Drawing.Size(573, 136);
			this.m_RRPlotControl.TabIndex = 0;
			// 
			// m_HrvSdnnPlotControl
			// 
			this.m_HrvSdnnPlotControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_HrvSdnnPlotControl.Location = new System.Drawing.Point(0, 0);
			this.m_HrvSdnnPlotControl.Name = "m_HrvSdnnPlotControl";
			this.m_HrvSdnnPlotControl.Size = new System.Drawing.Size(573, 132);
			this.m_HrvSdnnPlotControl.TabIndex = 1;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(573, 396);
			this.Controls.Add(this.m_MainSplitContainer);
			this.Name = "MainForm";
			this.Text = "Heart Monitor";
			this.m_MainSplitContainer.Panel1.ResumeLayout(false);
			this.m_MainSplitContainer.Panel2.ResumeLayout(false);
			this.m_MainSplitContainer.ResumeLayout(false);
			this.m_BeatsPerMinPanel.ResumeLayout(false);
			this.m_BeatsPerMinPanel.PerformLayout();
			this.m_PlotsSplitContainer.Panel1.ResumeLayout(false);
			this.m_PlotsSplitContainer.Panel2.ResumeLayout(false);
			this.m_PlotsSplitContainer.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer m_MainSplitContainer;
		private System.Windows.Forms.Panel m_BeatsPerMinPanel;
		private LightStone4net.WinUI.HeartSignalPlotControl m_HeartSignalPlotControl;
		private System.Windows.Forms.CheckBox m_BeepCheckBox;
		private LightStone4net.WinUI.ThreadSafeControls.SafeTextBox m_HrvSdnnTextBox;
		private System.Windows.Forms.Label label2;
		private LightStone4net.WinUI.ThreadSafeControls.SafeTextBox m_BeatsPerMinTextBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button m_HrvResetButton;
		private System.Windows.Forms.SplitContainer m_PlotsSplitContainer;
		private LightStone4net.WinUI.RRPlotControl m_RRPlotControl;
		private LightStone4net.WinUI.HrvSdnnPlotControl m_HrvSdnnPlotControl;
	}
}

