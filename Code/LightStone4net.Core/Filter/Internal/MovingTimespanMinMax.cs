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
using LightStone4net.Core.Utilities;
using LightStone4net.Core.Data;

namespace LightStone4net.Core.Filter.Internal
{
	/// <summary>
	/// Calculates the min / max values of time stamped double values
	/// based on a sliding  timespan
	/// </summary>
	internal class MovingTimespanMinMax : TimeSpanBuffer<double>
	{
		private double m_Min;
		private double m_Max;

		public MovingTimespanMinMax(TimeSpan maximalAge)
			: base(maximalAge, 200)
		{
			Reset();
		}

		private void Reset()
		{
			m_Min = double.MaxValue;
			m_Max = double.MinValue;
		}

		#region ISink<TimeStampedValue<double>> Members

		public override void Accept(TimeStampedValue<double> timeStampedValue)
		{
			base.Accept(timeStampedValue);

			AdjustMin(timeStampedValue);
			AdjustMax(timeStampedValue);

			//Debug.WriteLine(String.Format("{0} - {1}", m_Min, m_Max));
		}

		#endregion

		public double Min
		{
			get { return m_Min; }
		}

		public double Max
		{
			get { return m_Max; }
		}

		protected override void OnOldEntryRemoved(TimeStampedValue<double> removedTimeStampedValue)
		{
			if (removedTimeStampedValue.Value == m_Min)
			{
				FindMin();
			}
			if (removedTimeStampedValue.Value == m_Max)
			{
				FindMax();
			}
		}

		private void FindMin()
		{
			m_Min = double.MaxValue;
			foreach (TimeStampedValue<double> timeStampedValue in this)
			{
				AdjustMin(timeStampedValue);
			}
		}

		private void FindMax()
		{
			m_Max = double.MinValue;
			foreach (TimeStampedValue<double> timeStampedValue in this)
			{
				AdjustMax(timeStampedValue);
			}
		}

		private void AdjustMin(TimeStampedValue<double> timeStampedValue)
		{
			if (timeStampedValue.Value < m_Min)
			{
				m_Min = timeStampedValue.Value;
			}
		}

		private void AdjustMax(TimeStampedValue<double> timeStampedValue)
		{
			if (timeStampedValue.Value > m_Max)
			{
				m_Max = timeStampedValue.Value;
			}
		}
	}
}
