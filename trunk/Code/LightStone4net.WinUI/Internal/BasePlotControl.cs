using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using NPlot.Windows;

namespace LightStone4net.WinUI.Internal
{
	public partial class BasePlotControl : UserControl
	{
		private TimeSpan m_MaxAllowedTimeSpan = TimeSpan.FromMinutes(30);
		private TimeSpan m_DisplayPeriod;

		public BasePlotControl()
		{
			InitializeComponent();
		}

		protected TimeSpan MaxAllowedTimeSpan
		{
			get { return m_MaxAllowedTimeSpan; }
			set { m_MaxAllowedTimeSpan = value; }
		}

		public TimeSpan DisplayPeriod
		{
			get
			{
				return m_DisplayPeriod;
			}
			protected set
			{
				if (value.Ticks <= 0)
				{
					throw new ArgumentException("The display period must be greater than zero.");
				}
				if (value.Ticks > this.MaxAllowedTimeSpan.Ticks)
				{
					throw new ArgumentException("The display period must be greater than " + this.MaxAllowedTimeSpan.ToString());
				}
				m_DisplayPeriod = value;
			}
		}

		protected PlotSurface2D PlotSurface
		{
			get { return m_PlotSurface; }
		}

		protected void StartTimer(int intervalMSec)
		{
			m_Timer.Interval = intervalMSec;
			m_Timer.Enabled = true;
		}

		protected virtual void RefreshGraph()
		{
		}

		private void OnTimerTick(object sender, EventArgs e)
		{
			RefreshGraph();
		}
	}
}
