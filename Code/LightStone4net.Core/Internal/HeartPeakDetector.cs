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

namespace LightStone4net.Core.Internal
{
	internal sealed class HeartPeakDetector : ISink<TimeStampedValue<double>>
	{
		private enum State
		{
			//Initial,
			GoingUp,
			GoingUpBeyondThreshold,
			GoingDown,
			GoingDownBelowThreshold
		}

		public static readonly HeartPeakDetector Instance = new HeartPeakDetector();

		// Explicit static constructor to tell C# compiler
		// not to mark type as beforefieldinit
		static HeartPeakDetector()
		{
		}

		private const double c_Threshold = 0.6;
		private TimeStampedValue<double> m_PreviousPeak;
		private TimeStampedValue<double> m_PreviousTimeStampedValue;
		private State m_State = State.GoingUp;
		private Output<TimeStampedValue<double>> m_PeakOutput;
		private Output<TimeStampedValue<TimeSpan>> m_RRIntervalOutput;

		private HeartPeakDetector()
		{
			m_PreviousPeak = new TimeStampedValue<double>(DateTime.MinValue, 0);
			m_PreviousTimeStampedValue = new TimeStampedValue<double>(DateTime.MinValue, 0);

			m_PeakOutput = new Output<TimeStampedValue<double>>();
			m_RRIntervalOutput = new Output<TimeStampedValue<TimeSpan>>();

			NormalizerFilter normalizerFilter = new NormalizerFilter(TimeSpan.FromSeconds(1.5));
			LightStoneDevice.Instance.RawHeartSignalOutput.Add(normalizerFilter);

			normalizerFilter.Output.Add(this);
		}

		#region ISink<TimeStampedValue<double>> Members

		public void Accept(TimeStampedValue<double> timeStampedValue)
		{
			//Debug.Write(m_State.ToString() + " -> ");
			switch (m_State)
			{
				case State.GoingUp:
					if (timeStampedValue.Value > c_Threshold)
					{
						m_State = State.GoingUpBeyondThreshold;
					}
					break;
				case State.GoingUpBeyondThreshold:
					if (timeStampedValue.Value < m_PreviousTimeStampedValue.Value)
					{
						// We are going down
						if (timeStampedValue.Value < c_Threshold)
						{
							m_State = State.GoingDown;
						}
						else
						{
							m_State = State.GoingDownBelowThreshold;
						}

						// the previous value is a heart peak candidate
						if (IsTimeSinceLastPeakOk(m_PreviousTimeStampedValue))
						{
							OnNewPeakDetected(m_PreviousTimeStampedValue);
							m_PreviousPeak = m_PreviousTimeStampedValue; 
						}
					}
					break;
				case State.GoingDown:
					if (timeStampedValue.Value < m_PreviousTimeStampedValue.Value)
					{
						if (timeStampedValue.Value < c_Threshold)
						{
							m_State = State.GoingDownBelowThreshold;
						}
					}
					break;
				case State.GoingDownBelowThreshold:
					if (timeStampedValue.Value > m_PreviousTimeStampedValue.Value)
					{
						if (timeStampedValue.Value > c_Threshold)
						{
							m_State = State.GoingUpBeyondThreshold;
						}
						else
						{
							m_State = State.GoingUp;
						}
					}
					break;
				default:
					break;
			}

			m_PreviousTimeStampedValue = timeStampedValue;
			//Debug.WriteLine("state after Accept: " + m_State.ToString());
		}

		#endregion

		private void OnNewPeakDetected(TimeStampedValue<double> newPeakValue)
		{
			m_PeakOutput.WriteOutput(newPeakValue);
			if (m_PreviousPeak.TimeStamp.Ticks > DateTime.MinValue.Ticks)
			{
				TimeSpan rrInterval = new TimeSpan(newPeakValue.TimeStamp.Ticks - m_PreviousPeak.TimeStamp.Ticks);
				m_RRIntervalOutput.WriteOutput(
					new TimeStampedValue<TimeSpan>(newPeakValue.TimeStamp, rrInterval));
			}
		}

		public ISource<TimeStampedValue<double>> PeakOutput
		{
			get { return m_PeakOutput; }
		}

		/// <summary>
		/// Delivers the R-R intervals (intervals between ventricular depolarizations)
		/// </summary>
		public ISource<TimeStampedValue<TimeSpan>> RRIntervalOutput
		{
			get { return m_RRIntervalOutput; }
		}

		private bool IsAboveThreshold(TimeStampedValue<double> timeStampedValue)
		{
			return (timeStampedValue.Value > c_Threshold);
		}

		private bool IsTimeSinceLastPeakOk(TimeStampedValue<double> timeStampedValue)
		{
			return (timeStampedValue.TimeStamp.Ticks - m_PreviousPeak.TimeStamp.Ticks > TimeSpan.FromSeconds(0.2).Ticks);
		}
	}
}
