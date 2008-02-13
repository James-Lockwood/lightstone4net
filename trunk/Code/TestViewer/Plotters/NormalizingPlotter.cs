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
using System.Text;
using System.Drawing;

using NPlot;
using NPlot.Windows;

using LightStone4net.Core;
using LightStone4net.Core.Utilities;
using LightStone4net.Core.Filter;
using LightStone4net.Core.Data;

namespace TestViewer.Plotters
{
	public class NormalizingPlotter : Plotter
	{
		private static readonly TimeSpan c_MaxAllowedTimeSpan = TimeSpan.FromMinutes(30);
		private TimeSpan m_DisplayPeriod;
		private ITimeSpanBuffer<double> m_PointBuffer;
		private LinePlot m_LinePlot;

		public NormalizingPlotter(NPlot.Windows.PlotSurface2D plotSurface, TimeSpan displayPeriod)
			: base(plotSurface)
		{
			this.DisplayPeriod = displayPeriod;
			HeartRate heartRate = HeartRate.Instance;

			NormalizerFilter normalizerFilter = new NormalizerFilter(TimeSpan.FromSeconds(1.5));
			LightStoneDevice.Instance.RawHeartSignalOutput.Add(normalizerFilter);

			int bufferSize = (int)(m_DisplayPeriod.Ticks / LightStoneDevice.SamplingInterval.Ticks);
			m_PointBuffer = TimeSpanBuffer<double>.Synchronized(m_DisplayPeriod, bufferSize);
			normalizerFilter.Output.Add(m_PointBuffer);

			this.PlotSurface.Clear();

			m_LinePlot = new LinePlot();
			m_LinePlot.Pen = new Pen(Color.Red, 2.0f);
			this.PlotSurface.Add(m_LinePlot);

			this.PlotSurface.XAxis1 = new DateTimeAxis(this.PlotSurface.XAxis1);
			this.PlotSurface.XAxis1.NumberFormat = "hh:mm:ss";

			this.PlotSurface.Title = "Normalized Heart Signal";
			this.PlotSurface.XAxis1.Label = "Time";
			this.PlotSurface.YAxis1.Label = "Magnitude";

			Refresh();
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

		public override void Refresh()
		{
			DateTime now = DateTime.Now;

			double[] xValues;
			double[] yValues;
			lock (m_PointBuffer.SyncObject)
			{
				xValues = new double[m_PointBuffer.Count];
				yValues = new double[m_PointBuffer.Count];

				int index = 0;
				foreach(TimeStampedValue<double> timeStampedValue in m_PointBuffer)
				{
					xValues[index] = timeStampedValue.TimeStamp.Ticks;
					yValues[index] = timeStampedValue.Value;
					index++;
				}
			}

			m_LinePlot.AbscissaData = xValues;
			m_LinePlot.OrdinateData = yValues;

			this.PlotSurface.XAxis1.WorldMin = now.Ticks - m_DisplayPeriod.Ticks;
			this.PlotSurface.XAxis1.WorldMax = now.Ticks;

			this.PlotSurface.YAxis1.WorldMin = 0;
			this.PlotSurface.YAxis1.WorldMax = 1;

			this.PlotSurface.Refresh();
		}
	}
}
