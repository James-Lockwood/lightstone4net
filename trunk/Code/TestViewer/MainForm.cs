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

using NPlot;

using LightStone4net.Core;
using LightStone4net.Core.Utilities;
using LightStone4net.Core.Filter;
using TestViewer.Plotters;

namespace TestViewer
{
	public partial class MainForm : Form
	{
		//private Plotter m_Plotter;

		public MainForm()
		{
			InitializeComponent();

			//HeartRate.Instance.BeatsPerMinuteOutput.Add(new BeatsPerMinuteInput(this));
			//HeartRate.Instance.BeepEnabled = m_BeepCheckBox.Checked;

			//m_Plotter = new SimplePlotter(m_PlotSurface, bufferSize);
			//m_Plotter = new NormalizingPlotter(m_PlotSurface, TimeSpan.FromMinutes(1));
			
			//m_UseAutoGainCheckBox.Checked = m_BufferWrapper.UseAutoGain;

			m_Timer.Interval = 200;
			m_Timer.Enabled = true;
		}

		private void OnTimerTick(object sender, EventArgs e)
		{
			//m_Plotter.Refresh();
		}

		private void OnUseAutoGainCheckBoxChanged(object sender, EventArgs e)
		{
			//m_BufferWrapper.UseAutoGain = m_UseAutoGainCheckBox.Checked;
		}
	}
}