using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LightStone4net.Core;
using LightStone4net.Core.Data;
using LightStone4net.Core.Filter;

namespace LightStone4net.Viewer
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		protected override void OnCreateControl()
		{
			if (!this.DesignMode)
			{
				HeartRate.Instance.BeatsPerMinuteOutput.Add(new BeatsPerMinuteInput(this));
				HeartRate.Instance.BeepEnabled = m_BeepCheckBox.Checked;
				HeartRate.Instance.HrvSdnnOutput.Add(new HrvSdnnInput(this));
			}
			base.OnCreateControl();
		}

		private void OnBeepCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			HeartRate.Instance.BeepEnabled = m_BeepCheckBox.Checked;
		}

		#region nested Input classes

		private class BeatsPerMinuteInput : ISink<TimeStampedValue<int>>
		{
			private MainForm m_Parent;

			internal BeatsPerMinuteInput(MainForm parent)
			{
				m_Parent = parent;
			}

			#region ISink<TimeStampedValue<int>> Members

			public void Accept(TimeStampedValue<int> timeStampedValue)
			{
				m_Parent.m_BeatsPerMinTextBox.Text = timeStampedValue.Value.ToString();
			}

			#endregion
		}

		private class HrvSdnnInput : ISink<TimeStampedValue<double>>
		{
			private MainForm m_Parent;

			internal HrvSdnnInput(MainForm parent)
			{
				m_Parent = parent;
			}

			#region ISink<TimeStampedValue<double>> Members

			public void Accept(TimeStampedValue<double> timeStampedValue)
			{
				m_Parent.m_HrvSdnnTextBox.Text = timeStampedValue.Value.ToString();
			}

			#endregion
		}

		#endregion

		private void OnHrvResetButtonClick(object sender, EventArgs e)
		{
			m_HrvSdnnPlotControl.Reset();
		}
	}
}