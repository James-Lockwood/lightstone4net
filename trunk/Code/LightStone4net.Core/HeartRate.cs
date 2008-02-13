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
	    public static readonly HeartRate Instance=new HeartRate();

		// Explicit static constructor to tell C# compiler
		// not to mark type as beforefieldinit
		static HeartRate()
		{
		}

		private int m_BeatsPerMinute = 0;
		private HeartPeakInput m_HeartPeakInput;
		private BeatsPerMinuteInput m_BeatsPerMinuteInput;
		private bool m_BeepEnabled = true;

		private HeartRate()
		{
			m_HeartPeakInput = new HeartPeakInput(this);
			HeartPeakDetector.Instance.PeakOutput.Add(m_HeartPeakInput);

			m_BeatsPerMinuteInput = new BeatsPerMinuteInput(this);
			BeatsPerMinuteCalculator.Instance.Output.Add(m_BeatsPerMinuteInput);
		}

		private void OnNewHeartBeat(TimeStampedValue<double> timeStampedValue)
		{
			if (m_BeepEnabled)
			{
				Console.Beep();
			}
		}

		public int BeatsPerMinute
		{
			get
			{
				return m_BeatsPerMinute;
			}
		}

		public ISource<int> BeatsPerMinuteOutput
		{
			get
			{
				return BeatsPerMinuteCalculator.Instance.Output;
			}
		}

		public bool BeepEnabled
		{
			get
			{
				return m_BeepEnabled;
			}
			set
			{
				m_BeepEnabled = value;
			}
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

		private class BeatsPerMinuteInput : ISink<int>
		{
			private HeartRate m_HeartRate;

			internal BeatsPerMinuteInput(HeartRate heartRate)
			{
				m_HeartRate = heartRate;
			}

			#region ISink<TimeStampedValue<double>> Members

			public void Accept(int value)
			{
				m_HeartRate.m_BeatsPerMinute = value;
			}

			#endregion
		}

		#endregion
	}
}
