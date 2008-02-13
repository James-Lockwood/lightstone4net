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

namespace TestViewer
{
	/// <summary>
	/// Wraps an array and provides min / max values
	/// </summary>
	public class ArrayWithMinMax
	{
		private double[] m_Values;
		private double m_Min = double.MaxValue;
		private double m_Max = double.MinValue;

		internal ArrayWithMinMax(int size)
		{
			m_Values = new double[size];
		}

		public void Add(int index, double value)
		{
			m_Values[index] = value;
			if (value < m_Min)
			{
				m_Min = value;
			}
			if (value > m_Max)
			{
				m_Max = value;
			}
		}

		public double Min
		{
			get { return m_Min; }
		}

		public double Max
		{
			get { return m_Max; }
		}

		public double[] Values
		{
			get { return m_Values; }
		}
	}
}