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

namespace LightStone4net.Core.Filter
{
	// This class is based on Padre's AutoGain class (see http://sourceforge.net/projects/lsm/)
	public class AutoGainFilter : FilterBase<double>
	{
		private double m_DesiredPeak = 1.0;
		private double m_Attack = .9875;
		private double m_Decay = .992;
		private double m_Peak = 0.0;
		private double m_Gain = 0.0;

		public AutoGainFilter()
			: this(1.0, 0.9875, 0.992)
		{
		}

		public AutoGainFilter(double desiredPeak, double attack, double decay)
		{
			m_Attack = attack;
			m_Decay = decay;
			m_DesiredPeak = desiredPeak;
		}

		#region ISink<double> Members

		public override void Accept(double value)
		{
			double outputValue = ApplyAutoGain(value);
			WriteOutput(outputValue);
		}

		#endregion

		public double ApplyAutoGain(double value)
		{
			if (value > m_Peak)
			{
				m_Peak = m_Attack * value;
			}
			else
			{
				m_Peak = m_Decay * m_Peak;
			}

			m_Gain = m_Attack / m_Peak;

			double outputValue = m_Gain * value;
			return outputValue;
		}
	}
}
