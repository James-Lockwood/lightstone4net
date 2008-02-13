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

namespace LightStone4net.Core
{
	public sealed class BeatsPerMinuteCalculator : ISink<TimeStampedValue<double>>, IOutput<int>
	{
		public static readonly BeatsPerMinuteCalculator Instance = new BeatsPerMinuteCalculator();

		// Explicit static constructor to tell C# compiler
		// not to mark type as beforefieldinit
		static BeatsPerMinuteCalculator()
		{
		}

		private TimeSpanBuffer<double> m_TimeSpanBuffer;
		private TimeSpan m_IntegrationPeriod;
		private Output<int> m_Output;

		private BeatsPerMinuteCalculator()
		{
			m_Output = new Output<int>();
			m_IntegrationPeriod = TimeSpan.FromSeconds(15);
			m_TimeSpanBuffer = new TimeSpanBuffer<double>(m_IntegrationPeriod, 200);

			HeartPeakDetector.Instance.PeakOutput.Add(this);
		}

		#region ISink<TimeStampedValue<double>> Members

		void ISink<TimeStampedValue<double>>.Accept(TimeStampedValue<double> value)
		{
			m_TimeSpanBuffer.Add(value);
			TimeSpan coveredTimeSpan;
			if (m_TimeSpanBuffer.IsReady)
			{
				// We have data for the full time span that is covered by the buffer
				coveredTimeSpan = m_IntegrationPeriod;
			}
			else
			{
				// we don't yet have data for the full specified time span
				if (m_TimeSpanBuffer.Count > 1)
				{
					coveredTimeSpan =
						m_TimeSpanBuffer[m_TimeSpanBuffer.Count - 1].TimeStamp -
						m_TimeSpanBuffer[0].TimeStamp;
				}
				else
				{
					return;
				}
			}
			double beatsPerMinute = (m_TimeSpanBuffer.Count - 1) * ((double)TimeSpan.FromSeconds(60).Ticks / (double)coveredTimeSpan.Ticks);
			int roundedBeatsPerMinute = (int)Math.Round(beatsPerMinute);

			m_Output.WriteOutput(roundedBeatsPerMinute);
		}

		#endregion

		#region IOutput<int> Members

		public ISource<int> Output
		{
			get { return m_Output; }
		}

		#endregion
	}
}
