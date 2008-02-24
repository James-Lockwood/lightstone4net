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

namespace LightStone4net.Core.Utilities
{
	/// <summary>
	/// A helper class that automatically adjusts a 
	/// </summary>
	public class AutoRange
	{
		private const double c_IncreaseIfLessThanLeft = 0.01;
		private const double c_DecreaseIfMoreThanLeft = 0.15;
		private const double c_ChangeBy = 0.1;

		private double m_RangeMin = 0;
		private double m_RangeMax = 0;

		public AutoRange()
		{
		}

		public void AdjustFor(double minValue, double maxValue)
		{
			Debug.Assert(maxValue >= minValue);

			double newRangeMin = m_RangeMin;
			double newRangeMax = m_RangeMax;

			// First we make sure that minValue and maxValue are not outside the range
			if (maxValue > newRangeMax)
			{
				newRangeMax = maxValue;
			}
			if (minValue < newRangeMin)
			{
				newRangeMin = minValue;
			}

			double delta = maxValue - minValue;
			if (delta == 0.0)
			{
				delta = 0.000001;
			}

			// Second, enlarge if necessary
			if ((newRangeMax - maxValue) / delta < c_IncreaseIfLessThanLeft)
			{
				newRangeMax = maxValue + c_ChangeBy * delta;
			}
			if ((minValue - newRangeMin) / delta < c_IncreaseIfLessThanLeft)
			{
				newRangeMin = minValue - c_ChangeBy * delta;
			}

			// Third, decrease if necessary
			if ((newRangeMax - maxValue) / delta > c_DecreaseIfMoreThanLeft)
			{
				newRangeMax = maxValue + c_ChangeBy * delta;
			}
			if ((minValue - newRangeMin) / delta > c_IncreaseIfLessThanLeft)
			{
				newRangeMin = minValue - c_ChangeBy * delta;
			}

			m_RangeMax = newRangeMax;
			m_RangeMin = newRangeMin;
		}

		public double Min
		{
			get { return m_RangeMin; }
		}

		public double Max
		{
			get { return m_RangeMax; }
		}

		public double Delta
		{
			get { return m_RangeMax - m_RangeMin; }
		}
	}
}
