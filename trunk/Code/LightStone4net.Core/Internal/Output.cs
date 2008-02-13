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

using LightStone4net.Core.Filter;

namespace LightStone4net.Core.Internal
{
	public class Output<T> : ISource<T>
	{
		private List<ISink<T>> m_Sinks = new List<ISink<T>>();

		public void WriteOutput(T value)
		{
			foreach (ISink<T> sink in m_Sinks)
			{
				sink.Accept(value);
			}
		}

		#region ISource<T> Members

		void ISource<T>.Add(ISink<T> sink)
		{
			m_Sinks.Add(sink);
		}

		void ISource<T>.Remove(ISink<T> sink)
		{
			m_Sinks.Remove(sink);
		}

		#endregion
	}
}
