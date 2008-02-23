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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using NPlot;
using NPlot.Windows;
using LightStone4net.Core;
using LightStone4net.Core.Utilities;
using LightStone4net.Core.Filter;
using LightStone4net.Core.Data;

namespace LightStone4net.WinUI
{
	public partial class HrvSdnnPlotControl : UserControl
	{
		private static readonly TimeSpan c_MaxAllowedTimeSpan = TimeSpan.FromMinutes(30);
		private TimeSpan m_DisplayPeriod;
		private ITimeSpanBuffer<double> m_PointBuffer;
		private LinePlot m_LinePlot;

		public HrvSdnnPlotControl()
		{
			this.DisplayPeriod = TimeSpan.FromSeconds(10);
			InitializeComponent();
		}

		protected override void OnCreateControl()
		{
			if (!this.DesignMode)
			{
				HeartRate heartRate = HeartRate.Instance;

				int initialBufferSize = m_DisplayPeriod.Seconds * 60;
				m_PointBuffer = TimeSpanBuffer<double>.Synchronized(m_DisplayPeriod, initialBufferSize);
				HeartRate.Instance.HrvSdnnOutput.Add(m_PointBuffer);

				m_PlotSurface.Clear();

				m_LinePlot = new LinePlot();
				m_LinePlot.Pen = new Pen(Color.Red, 2.0f);
				m_PlotSurface.Add(m_LinePlot);

				m_PlotSurface.XAxis1 = new DateTimeAxis(m_PlotSurface.XAxis1);
				m_PlotSurface.XAxis1.NumberFormat = "mm:ss";

				m_PlotSurface.Title = "HRV Over Time";
				m_PlotSurface.XAxis1.Label = "Time";
				m_PlotSurface.YAxis1.Label = "HRV";

				RefreshGraph();

				m_Timer.Interval = 200;
				m_Timer.Enabled = true;
			}
			base.OnCreateControl();
		}

		public TimeSpan DisplayPeriod
		{
			get
			{
				return m_DisplayPeriod;
			}
			private set
			{
				if (value.Ticks <= 0)
				{
					throw new ArgumentException("The display period must be greater than zero.");
				}
				if (value.Ticks > c_MaxAllowedTimeSpan.Ticks)
				{
					throw new ArgumentException("The display period must be greater than " + c_MaxAllowedTimeSpan.ToString());
				}
				m_DisplayPeriod = value;
			}
		}

		private void RefreshGraph()
		{
			DateTime now = DateTime.Now;

			double[] xValues;
			double[] yValues;
			lock (m_PointBuffer.SyncObject)
			{
				xValues = new double[m_PointBuffer.Count];
				yValues = new double[m_PointBuffer.Count];

				int index = 0;
				foreach (TimeStampedValue<double> timeStampedValue in m_PointBuffer)
				{
					xValues[index] = timeStampedValue.TimeStamp.Ticks;
					yValues[index] = timeStampedValue.Value;
					index++;
				}
			}

			m_LinePlot.AbscissaData = xValues;
			m_LinePlot.OrdinateData = yValues;

			m_PlotSurface.XAxis1.WorldMin = now.Ticks - m_DisplayPeriod.Ticks;
			m_PlotSurface.XAxis1.WorldMax = now.Ticks;

			m_PlotSurface.YAxis1.WorldMin = 0;
			m_PlotSurface.YAxis1.WorldMax = 1;

			m_PlotSurface.Refresh();
		}

		private void OnTimerTick(object sender, EventArgs e)
		{
			RefreshGraph();
		}
	}
}
