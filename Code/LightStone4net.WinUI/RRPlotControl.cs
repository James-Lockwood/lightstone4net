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
using System.Diagnostics;

using NPlot;
using NPlot.Windows;
using LightStone4net.Core;
using LightStone4net.Core.Utilities;
using LightStone4net.Core.Filter;
using LightStone4net.Core.Data;
using LightStone4net.WinUI.Internal;

namespace LightStone4net.WinUI
{
	public partial class RRPlotControl : BasePlotControl
	{
		private ITimeSpanBuffer<TimeSpan> m_PointBuffer;
		private LinePlot m_LinePlot;
		private AutoRange m_YAxisRange = new AutoRange();

		public RRPlotControl()
		{
			this.DisplayPeriod = TimeSpan.FromMinutes(1);
			InitializeComponent();
		}

		protected override void OnCreateControl()
		{
			if (!this.DesignMode)
			{
				HeartRate heartRate = HeartRate.Instance;

				int initialBufferSize = (int)base.DisplayPeriod.TotalSeconds * 60;
				m_PointBuffer = TimeSpanBuffer<TimeSpan>.Synchronized(base.DisplayPeriod, initialBufferSize);
				HeartRate.Instance.RRIntervalOutput.Add(m_PointBuffer);

				base.PlotSurface.Clear();

				m_LinePlot = new LinePlot();
				m_LinePlot.Pen = new Pen(Color.Red, 2.0f);
				base.PlotSurface.Add(m_LinePlot);

				base.PlotSurface.XAxis1 = new DateTimeAxis(base.PlotSurface.XAxis1);
				base.PlotSurface.XAxis1.NumberFormat = "mm:ss";

				base.PlotSurface.Title = "R-R Interval Over Time";
				base.PlotSurface.XAxis1.Label = "Time";
				base.PlotSurface.YAxis1.Label = "R-R [msec]";

				RefreshGraph();

				base.StartTimer(200);
			}
			base.OnCreateControl();
		}

		protected override void RefreshGraph()
		{
			DateTime now = DateTime.Now;

			double[] xValues;
			double[] yValues;

			double minY = double.MaxValue;
			double maxY = double.MinValue;

			lock (m_PointBuffer.SyncObject)
			{
				if (m_PointBuffer.Count < 3)
				{
					// not enough data
					return;
				}

				xValues = new double[m_PointBuffer.Count];
				yValues = new double[m_PointBuffer.Count];

				int index = 0;
				foreach (TimeStampedValue<TimeSpan> timeStampedValue in m_PointBuffer)
				{
					xValues[index] = timeStampedValue.TimeStamp.Ticks;
					double y = timeStampedValue.Value.TotalMilliseconds;
					yValues[index] = y;

					if (y > maxY)
					{
						maxY = y;
					}
					if (y < minY)
					{
						minY = y;
					}

					index++;
				}
			}

			m_YAxisRange.AdjustFor(minY, maxY);

			m_LinePlot.AbscissaData = xValues;
			m_LinePlot.OrdinateData = yValues;

			base.PlotSurface.XAxis1.WorldMin = now.Ticks - base.DisplayPeriod.Ticks;
			base.PlotSurface.XAxis1.WorldMax = now.Ticks;

			base.PlotSurface.YAxis1.WorldMin = m_YAxisRange.Min;
			base.PlotSurface.YAxis1.WorldMax = m_YAxisRange.Max;

			base.PlotSurface.Refresh();
		}

		private void OnTimerTick(object sender, EventArgs e)
		{
			RefreshGraph();
		}
	}
}
