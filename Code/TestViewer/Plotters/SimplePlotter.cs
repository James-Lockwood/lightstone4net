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

namespace TestViewer.Plotters
{
	public class SimplePlotter : Plotter
	{
		private int m_BufferSize;
		private CircularBuffer<Sample> m_PointBuffer;
		private BufferWrapper m_BufferWrapper;
		private LinePlot m_LinePlot;

		public SimplePlotter(NPlot.Windows.PlotSurface2D plotSurface, int bufferSize)
			: base(plotSurface)
		{
			m_BufferSize = bufferSize;
			m_PointBuffer = CircularBuffer<Sample>.Synchronized(bufferSize);
			LightStoneDevice.Instance.RawOutput.Add(m_PointBuffer);
			m_BufferWrapper = new BufferWrapper(m_PointBuffer);

			this.PlotSurface.Clear();

			m_LinePlot = new LinePlot();
			m_LinePlot.Pen = new Pen(Color.Red, 2.0f);
			this.PlotSurface.Add(m_LinePlot);

			this.PlotSurface.Title = "Heart Signal";
			this.PlotSurface.XAxis1.Label = "Time";
			this.PlotSurface.YAxis1.Label = "Magnitude";

			Refresh();
		}

		public int BufferSize
		{
			get { return m_BufferSize; }
		}

		public override void Refresh()
		{
			m_BufferWrapper.Refresh();
			m_LinePlot.AbscissaData = m_BufferWrapper.XValues.Values;
			m_LinePlot.OrdinateData = m_BufferWrapper.YValues.Values;

			this.PlotSurface.XAxis1.WorldMin = m_BufferWrapper.XValues.Min;
			this.PlotSurface.XAxis1.WorldMax = m_BufferWrapper.XValues.Max;

			this.PlotSurface.YAxis1.WorldMin = m_BufferWrapper.YValues.Min;
			this.PlotSurface.YAxis1.WorldMax = m_BufferWrapper.YValues.Max;

			this.PlotSurface.Refresh();
		}
	}
}
