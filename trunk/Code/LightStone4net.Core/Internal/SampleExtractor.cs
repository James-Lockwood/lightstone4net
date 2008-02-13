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

namespace LightStone4net.Core.Internal
{
	internal class SampleExtractor
	{
		private LightStoneDevice m_LightStoneDevice;
		private MessageBuilder m_MessageBuilder;
		private DateTime m_StartTime;
		private long m_PreviousTimeSlotIndex = -1;

		DataWriter m_DataWriter = new DataWriter();

		internal SampleExtractor(LightStoneDevice lightStoneDevice)
		{
			m_LightStoneDevice = lightStoneDevice;
			m_MessageBuilder = new MessageBuilder(this);
		}

		internal void HandleNewData(byte[] data)
		{
			Debug.WriteLine(BuildHexString(data));
			//m_DataWriter.HandleNewData(data);
			m_MessageBuilder.HandleNewData(data);
		}

		private string BuildHexString(byte[] data)
		{
			StringBuilder stringBuilder = new StringBuilder(data.Length * 2);
			foreach (byte byteValue in data)
			{
				stringBuilder.AppendFormat("{0:x2}", byteValue);
			}
			return stringBuilder.ToString();
		}

		internal void ParseCompletedMessage(string message)
		{
			// Ensure that the start time is initialized
			if (m_StartTime == DateTime.MinValue)
			{
				m_StartTime = DateTime.Now;
			}

			long timeSlotIndex = (DateTime.Now.Ticks - m_StartTime.Ticks) / LightStoneDevice.SamplingInterval.Ticks;

			if (timeSlotIndex == m_PreviousTimeSlotIndex)
			{
				// At the start the light stone returns a number of samples with pretty much the same time stamp
				// We need to skip those
				return;
			}

			m_PreviousTimeSlotIndex = timeSlotIndex;
			DateTime timeStamp = new DateTime(m_StartTime.Ticks + timeSlotIndex * LightStoneDevice.SamplingInterval.Ticks, DateTimeKind.Local);
			
			double skinConductivitySignal = ExtractSignal(message.Substring(5, 4));
			double heartSignal = ExtractSignal(message.Substring(10, 4));

			Sample newSample = new Sample(timeSlotIndex, timeStamp, skinConductivitySignal, heartSignal);
			//Debug.WriteLine(newSample.ToString());
			m_LightStoneDevice.OnNewSample(newSample);
		}

		private double ExtractSignal(string rawAsciiSection)
		{
			string hexNumberString = rawAsciiSection.Substring(0, 2);
			int b1 = Convert.ToByte(hexNumberString, 16);

			hexNumberString = rawAsciiSection.Substring(2, 2);
			int b2 = Convert.ToByte(hexNumberString, 16);

			double signal = ((b1 *= 255) + b2) * .01;
			return signal;
		}
	}
}
