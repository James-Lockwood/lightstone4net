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
using LightStone4net.WinUI.Internal;

namespace LightStone4net.WinUI
{
	public partial class HeartSignalPlotControl : BasePlotControl
	{
		private ITimeSpanBuffer<double> m_PointBuffer;
		private LinePlot m_LinePlot;

		public HeartSignalPlotControl()
		{
			this.DisplayPeriod = TimeSpan.FromSeconds(5);
			InitializeComponent();
		}

		protected override void OnCreateControl()
		{
			if (!this.DesignMode)
			{
				HeartRate heartRate = HeartRate.Instance;

				NormalizerFilter normalizerFilter = new NormalizerFilter(TimeSpan.FromSeconds(1.5));
				LightStoneDevice.Instance.RawHeartSignalOutput.Add(normalizerFilter);

				int bufferSize = (int)(this.DisplayPeriod.Ticks / LightStoneDevice.SamplingInterval.Ticks);
				m_PointBuffer = TimeSpanBuffer<double>.Synchronized(this.DisplayPeriod, bufferSize);
				normalizerFilter.Output.Add(m_PointBuffer);

				base.PlotSurface.Clear();

				m_LinePlot = new LinePlot();
				m_LinePlot.Pen = new Pen(Color.Red, 2.0f);
				base.PlotSurface.Add(m_LinePlot);

				base.PlotSurface.XAxis1 = new DateTimeAxis(base.PlotSurface.XAxis1);
				base.PlotSurface.XAxis1.NumberFormat = "mm:ss";

				base.PlotSurface.Title = "Normalized Heart Signal";
				base.PlotSurface.XAxis1.Label = "Time";
				base.PlotSurface.YAxis1.Label = "Magnitude";

				RefreshGraph();

				base.StartTimer(100);
			}
			base.OnCreateControl();
		}

		protected override void RefreshGraph()
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

			base.PlotSurface.XAxis1.WorldMin = now.Ticks - this.DisplayPeriod.Ticks;
			base.PlotSurface.XAxis1.WorldMax = now.Ticks;

			base.PlotSurface.YAxis1.WorldMin = 0;
			base.PlotSurface.YAxis1.WorldMax = 1;

			base.PlotSurface.Refresh();
		}
	}
}
