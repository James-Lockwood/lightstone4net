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
using System.IO;

namespace LightStone4net.Core.Utilities
{
	internal class DataWriter : IDisposable
	{
		private FileStream fStream;
		BinaryWriter bw;
		int counter = 0;
		List<byte> m_Data = new List<byte>();
		
		public DataWriter()
		{
			fStream = new FileStream("byteArray8.dat", FileMode.Create);
			bw = new BinaryWriter(fStream);
		}

		internal void HandleNewData(byte[] data)
		{
			const int StartCount = 2000;
			const int MaxCount = 100;
			counter++;
			if (counter == StartCount + MaxCount)
			{
				bw.Write(m_Data.ToArray());
				Dispose();
			}
			else if (counter < StartCount || counter > StartCount + MaxCount)
			{
				return;
			}
			else
			{
				m_Data.AddRange(data);
			}
		}


		#region IDisposable Members

		public void Dispose()
		{
			bw.Close();
			fStream.Close();
		}

		#endregion
	}
}
