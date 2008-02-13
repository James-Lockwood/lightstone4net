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

using LightStone4net.Core.Data;
using LightStone4net.Core.Filter.Internal;

namespace LightStone4net.Core.Filter
{
	public class NormalizerFilter : FilterBase<TimeStampedValue<double>>
	{
		private MovingTimespanMinMax m_MovingTimespanMinMax;

		public NormalizerFilter(TimeSpan maximalAge)
		{
			m_MovingTimespanMinMax = new MovingTimespanMinMax(maximalAge);
		}

		#region ISink<TimeStampedValue<T>> Members

		public override void Accept(TimeStampedValue<double> timeStampedValue)
		{
			m_MovingTimespanMinMax.Accept(timeStampedValue);
			if (m_MovingTimespanMinMax.IsReady)
			{
				double normalizedValue = Normalize(timeStampedValue.Value);
				base.WriteOutput(new TimeStampedValue<double>(timeStampedValue.TimeStamp, normalizedValue));
			}
		}

		#endregion

		private double Normalize(double value)
		{
			return (value - m_MovingTimespanMinMax.Min) / (m_MovingTimespanMinMax.Max - m_MovingTimespanMinMax.Min);
		}
	}
}
