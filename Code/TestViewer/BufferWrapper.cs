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

using LightStone4net.Core;
using LightStone4net.Core.Utilities;
using LightStone4net.Core.Filter;

namespace TestViewer
{
	public class BufferWrapper
	{
		private CircularBuffer<Sample> m_PointBuffer;
		private ArrayWithMinMax m_XValues;
		private ArrayWithMinMax m_YValues;
		private AutoGainFilter m_AutoGainFilter = new AutoGainFilter();
		private bool m_UseAutoGain = false;

		public BufferWrapper(CircularBuffer<Sample> pointBuffer)
		{
			m_PointBuffer = pointBuffer;
			Refresh();
		}

		public void Refresh()
		{
			lock (m_PointBuffer.SyncObject)
			{
				m_XValues = new ArrayWithMinMax(m_PointBuffer.Count);
				m_YValues = new ArrayWithMinMax(m_PointBuffer.Count);

				int index = 0;
				foreach (Sample sample in m_PointBuffer)
				{
					m_XValues.Add(index, (double)sample.TimeSlotIndex);

					double y = sample.Heart;
					if (this.UseAutoGain)
					{
						y = m_AutoGainFilter.ApplyAutoGain(sample.Heart);
					}
					m_YValues.Add(index, y);
					index++;
				}
			}
		}

		public bool UseAutoGain
		{
			get
			{
				return m_UseAutoGain;
			}
			set
			{
				m_UseAutoGain = value;
			}
		}

		public ArrayWithMinMax XValues
		{
			get
			{
				return m_XValues;
			}
		}

		public ArrayWithMinMax YValues
		{
			get
			{
				return m_YValues;
			}
		}
	}
}