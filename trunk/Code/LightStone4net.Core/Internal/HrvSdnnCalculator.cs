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
using LightStone4net.Core.Utilities;

namespace LightStone4net.Core.Internal
{
	/// <summary>
	/// Calculates the standard deviation of the time duration between heartbeats (R-R intervals)
	/// </summary>
	internal class HrvSdnnCalculator : ISink<TimeStampedValue<TimeSpan>>, IOutput<TimeStampedValue<double>>
	{
		public static readonly HrvSdnnCalculator Instance = new HrvSdnnCalculator();

		// Explicit static constructor to tell C# compiler
		// not to mark type as beforefieldinit
		static HrvSdnnCalculator()
		{
		}

		private int m_MinSampleCount;
		private int m_MaxSampleCount;
		private CircularBuffer<TimeSpan> m_RRIntervalBuffer;
		private Output<TimeStampedValue<double>> m_Output;

		private HrvSdnnCalculator()
		{
			m_MinSampleCount = 30; // Start calculating the sdnn once we reach this sample count
			m_MaxSampleCount = 90; // use at most this many samples

			m_RRIntervalBuffer = new CircularBuffer<TimeSpan>(m_MaxSampleCount);
			m_Output = new Output<TimeStampedValue<double>>();

			HeartPeakDetector.Instance.RRIntervalOutput.Add(this);
		}

		#region ISink<TimeStampedValue<TimeSpan>> Members

		/// <summary>
		/// Accepts the RRIntervals coming from <see cref="HeartPeakDetector.Instance.RRIntervalOutput"/>.
		/// </summary>
		/// <param name="value"></param>
		void ISink<TimeStampedValue<TimeSpan>>.Accept(TimeStampedValue<TimeSpan> timeStampedValue)
		{
			// TODO: Filter out bad data
			m_RRIntervalBuffer.Add(timeStampedValue.Value);
			if (m_RRIntervalBuffer.Count < m_MinSampleCount)
			{
				// We don't have enough data yet.
				return;
			}

			// The buffer is full
			double sdnn = 0;
			lock(m_RRIntervalBuffer.SyncObject)
			{
				sdnn = CalculateSdnn();
			}

			m_Output.WriteOutput(
				new TimeStampedValue<double>(timeStampedValue.TimeStamp, sdnn)
				);
		}

		#endregion

		private double CalculateSdnn()
		{
			int n = m_RRIntervalBuffer.Count;
			double sum = 0;
			double sumOfSquares = 0;

			for (int i = 0; i < n; i++)
			{
				double rrMSec = m_RRIntervalBuffer[i].TotalMilliseconds;

				sum += rrMSec;
				sumOfSquares += rrMSec * rrMSec;
			}

			double sdSqared = (sumOfSquares - (sum * sum) / n) / (n - 1);
			return Math.Sqrt(sdSqared);
		}

		#region IOutput<TimeStampedValue<double>> Members

		public ISource<TimeStampedValue<double>> Output
		{
			get { return m_Output; }
		}

		#endregion
	}
}
