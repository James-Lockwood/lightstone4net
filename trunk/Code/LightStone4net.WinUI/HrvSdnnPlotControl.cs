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
using System.Diagnostics;

namespace LightStone4net.WinUI
{
	public partial class HrvSdnnPlotControl : UserControl
	{
		private static readonly TimeSpan c_MaxAllowedTimeSpan = TimeSpan.FromMinutes(30);
		private TimeSpan m_DisplayPeriod;
		private ITimeSpanBuffer<double> m_PointBuffer;
		private LinePlot m_LinePlot;

		private class Range
		{
			private const double c_MinDelta = 0.00001;

			private double m_Min;
			private double m_Max;

			public Range(double min, double max)
			{
				Update(min, max);
			}

			internal void Update(double min, double max)
			{
				Debug.Assert(max >= min);

				if (max == min)
				{
					m_Min = min - c_MinDelta;
					m_Max = max + c_MinDelta;
				}
				else
				{
					m_Min = min;
					m_Max = max;
				}
			}

			public double Min
			{
				get { return m_Min; }
			}

			public double Max
			{
				get { return m_Max; }
			}

			public double Delta
			{
				get { return m_Max - m_Min; }
			}
		}

		private Range m_YAxisRange = null;

		public HrvSdnnPlotControl()
		{
			this.DisplayPeriod = TimeSpan.FromMinutes(1);
			InitializeComponent();
		}

		protected override void OnCreateControl()
		{
			if (!this.DesignMode)
			{
				HeartRate heartRate = HeartRate.Instance;

				int initialBufferSize = (int)m_DisplayPeriod.TotalSeconds * 60;
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

		public void Reset()
		{
			HeartRate.Instance.ResetHrv();
			lock (m_PointBuffer.SyncObject)
			{
				m_PointBuffer.Clear();
			}

			m_LinePlot.AbscissaData = new double[0];
			m_LinePlot.OrdinateData = new double[0];
			m_PlotSurface.Refresh();
		}

		private void RefreshGraph()
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
				foreach (TimeStampedValue<double> timeStampedValue in m_PointBuffer)
				{
					xValues[index] = timeStampedValue.TimeStamp.Ticks;
					double y = timeStampedValue.Value;
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

			AdjustRange(minY, maxY);

			m_LinePlot.AbscissaData = xValues;
			m_LinePlot.OrdinateData = yValues;

			m_PlotSurface.XAxis1.WorldMin = now.Ticks - m_DisplayPeriod.Ticks;
			m_PlotSurface.XAxis1.WorldMax = now.Ticks;

			m_PlotSurface.YAxis1.WorldMin = m_YAxisRange.Min;
			m_PlotSurface.YAxis1.WorldMax = m_YAxisRange.Max;

			m_PlotSurface.Refresh();
		}

		private void AdjustRange(double min, double max)
		{
			const double c_IncreaseIfLessThanLeft = 0.01;
			const double c_DecreaseIfMoreThanLeft = 0.15;
			const double c_ChangeBy = 0.1;

			if (m_YAxisRange == null)
			{
				m_YAxisRange = new Range(min, max);
			}
			else
			{
				double newMax = m_YAxisRange.Max;
				double newMin = m_YAxisRange.Min;

				// First we make sure that min and max are not outside the range
				if (max > newMax)
				{
					newMax = max;
				}
				if (min < newMin)
				{
					newMin = min;
				}

				double delta = max - min;

				// Second, enlarge if necessary
				if ((newMax - max) / delta < c_IncreaseIfLessThanLeft)
				{
					newMax = max + c_ChangeBy * delta;
				}
				if ((min - newMin) / delta < c_IncreaseIfLessThanLeft)
				{
					newMin = min - c_ChangeBy * delta;
				}

				// Third, decrease if necessary
				if ((newMax - max) / delta > c_DecreaseIfMoreThanLeft)
				{
					newMax = max + c_ChangeBy * delta;
				}
				if ((min - newMin) / delta > c_IncreaseIfLessThanLeft)
				{
					newMin = min - c_ChangeBy * delta;
				}

				m_YAxisRange.Update(newMin, newMax);
			}
		}

		private void OnTimerTick(object sender, EventArgs e)
		{
			RefreshGraph();
		}
	}
}
