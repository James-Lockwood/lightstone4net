/*
This file is part of LightStone4net, an access and analysis library for the
LightStone USB device from http://www.wilddivine.com 

Copyright (C) 2006  Dr. Rainer Hessmer

LightStone4net is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

LightStone4net is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with LightStone4net.  If not, see <http://www.gnu.org/licenses/>.
*/

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
			m_HrvSdnnTextBox.Text = String.Empty;
		}
	}
}