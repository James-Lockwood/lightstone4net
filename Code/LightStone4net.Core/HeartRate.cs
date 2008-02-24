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
using System.Diagnostics;

using LightStone4net.Core.Filter;
using LightStone4net.Core.Data;
using LightStone4net.Core.Internal;

namespace LightStone4net.Core
{
	public sealed class HeartRate
	{
	    public static readonly HeartRate Instance = new HeartRate();

		// Explicit static constructor to tell C# compiler
		// not to mark type as beforefieldinit
		static HeartRate()
		{
		}

		private int m_BeatsPerMinute = 0;
		private double m_HrvSdnn = 0.0;
		private TimeSpan m_RRInterval = TimeSpan.Zero;
		private HeartPeakInput m_HeartPeakInput;
		private RRIntervalInput m_RRIntervalInput;
		private BeatsPerMinuteInput m_BeatsPerMinuteInput;
		private HrvSdnnInput m_HrvSdnnInput;
		private bool m_BeepEnabled = true;

		private HeartRate()
		{
			m_HeartPeakInput = new HeartPeakInput(this);
			HeartPeakDetector.Instance.PeakOutput.Add(m_HeartPeakInput);

			m_RRIntervalInput = new RRIntervalInput(this);
			HeartPeakDetector.Instance.RRIntervalOutput.Add(m_RRIntervalInput);

			m_BeatsPerMinuteInput = new BeatsPerMinuteInput(this);
			BeatsPerMinuteCalculator.Instance.Output.Add(m_BeatsPerMinuteInput);

			m_HrvSdnnInput = new HrvSdnnInput(this);
			HrvSdnnCalculator.Instance.Output.Add(m_HrvSdnnInput);
		}

		public bool BeepEnabled
		{
			get { return m_BeepEnabled;	}
			set { m_BeepEnabled = value; }
		}

		private void OnNewHeartBeat(TimeStampedValue<double> timeStampedValue)
		{
			if (m_BeepEnabled)
			{
				Console.Beep();
			}
		}

		/// <summary>
		/// The current R-R interval (interval between ventricular depolarizations)
		/// </summary>
		public TimeSpan RRInterval
		{
			get { return m_RRInterval; }
		}

		/// <summary>
		/// Delivers time stamped R-R intervals (intervals between ventricular depolarizations)
		/// </summary>
		public ISource<TimeStampedValue<TimeSpan>> RRIntervalOutput
		{
			get { return HeartPeakDetector.Instance.RRIntervalOutput; }
		}

		/// <summary>
		/// Current beats per minute
		/// </summary>
		public int BeatsPerMinute
		{
			get { return m_BeatsPerMinute; }
		}

		/// <summary>
		/// Beats per minute output
		/// </summary>
		public ISource<TimeStampedValue<int>> BeatsPerMinuteOutput
		{
			get { return BeatsPerMinuteCalculator.Instance.Output; }
		}

		/// <summary>
		/// The standard deviation between R-R intervals
		/// </summary>
		public double HrvSdnn
		{
			get { return m_HrvSdnn; }
		}

		/// <summary>
		/// Delivers time stamped standard deviation values between R-R intervals
		/// </summary>
		public ISource<TimeStampedValue<double>> HrvSdnnOutput
		{
			get { return HrvSdnnCalculator.Instance.Output; }
		}

		public void ResetHrv()
		{
			HrvSdnnCalculator.Instance.Reset();
		}

		#region nested Input classes

		private class HeartPeakInput : ISink<TimeStampedValue<double>>
		{
			private HeartRate m_HeartRate;

			internal HeartPeakInput(HeartRate heartRate)
			{
				m_HeartRate = heartRate;
			}

			#region ISink<TimeStampedValue<double>> Members

			public void Accept(TimeStampedValue<double> value)
			{
				m_HeartRate.OnNewHeartBeat(value);
			}

			#endregion
		}

		private class RRIntervalInput : ISink<TimeStampedValue<TimeSpan>>
		{
			private HeartRate m_HeartRate;

			internal RRIntervalInput(HeartRate heartRate)
			{
				m_HeartRate = heartRate;
			}

			#region ISink<TimeStampedValue<TimeSpan>> Members

			public void Accept(TimeStampedValue<TimeSpan> timeStampedValue)
			{
				m_HeartRate.m_RRInterval = timeStampedValue.Value;
			}

			#endregion
		}

		private class BeatsPerMinuteInput : ISink<TimeStampedValue<int>>
		{
			private HeartRate m_HeartRate;

			internal BeatsPerMinuteInput(HeartRate heartRate)
			{
				m_HeartRate = heartRate;
			}

			#region ISink<int> Members

			public void Accept(TimeStampedValue<int> timeStampedValue)
			{
				m_HeartRate.m_BeatsPerMinute = timeStampedValue.Value;
			}

			#endregion
		}

		private class HrvSdnnInput : ISink<TimeStampedValue<double>>
		{
			private HeartRate m_HeartRate;

			internal HrvSdnnInput(HeartRate heartRate)
			{
				m_HeartRate = heartRate;
			}

			#region ISink<TimeStampedValue<double>> Members

			public void Accept(TimeStampedValue<double> timeStampedValue)
			{
				m_HeartRate.m_HrvSdnn = timeStampedValue.Value;
			}

			#endregion
		}

		#endregion
	}
}
