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

namespace LightStone4net.Core.Internal
{
	internal class MessageBuilder
	{
		private string m_Message = String.Empty;
		private SampleExtractor m_SampleExtractor;

		internal MessageBuilder(SampleExtractor sampleExtractor)
		{
			m_SampleExtractor = sampleExtractor;
		}

		internal void HandleNewData(byte[] data)
		{
			// Complete messages look like this
			// <RAW>0002 03C8<\RAW>
			for (int i = 2; i < data[1] + 2; i++)
			{
				if (data[i] != 10 && data[i] != 13)
				{
					m_Message += Convert.ToChar(data[i]) + "";
				}

				// check for newline in output from lightstone
				if (data[i] == 13)
				{
					if (m_Message.Length == 20)
					{
						//Debug.WriteLine(m_Message);
						m_SampleExtractor.ParseCompletedMessage(m_Message);
						BeginNewMessage();
					}
					else
					{
						BeginNewMessage();
					}
				}
			}
		}

		private void BeginNewMessage()
		{
			m_Message = String.Empty;
		}
	}
}
