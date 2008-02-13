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
using System.Globalization;

namespace LightStone4net.Core
{
	public struct Sample
	{
		public Sample(long timeSlotIndex, DateTime timeStamp, double skinConductivity, double heart)
		{
			TimeSlotIndex = timeSlotIndex;
			TimeStamp = timeStamp;
			SkinConductivity = skinConductivity;
			Heart = heart;
		}

		public readonly long TimeSlotIndex;

		public readonly DateTime TimeStamp;

		public readonly double SkinConductivity;

		public readonly double Heart;

		public override string ToString()
		{
			string timeStampString = this.TimeStamp.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
			return String.Format("[{0}] {1} - SCL: {2}, Heart: {3}", this.TimeSlotIndex, timeStampString, this.SkinConductivity, this.Heart);
		}
	}
}
