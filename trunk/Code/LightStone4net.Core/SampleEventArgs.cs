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

namespace LightStone4net.Core
{
	/// <summary>
	/// An <see cref="EventArgs"/> class for events that expose a
	/// <see cref="Sample"/>.
	/// </summary>
	public class SampleEventArgs : EventArgs
	{
		private Sample m_Sample;

		/// <summary>
		/// Creates a new <see cref="ItemEventArgs"/> instance.
		/// </summary>
		/// <param name="sample">The involved <see cref="Sample"/>.</param>
		public SampleEventArgs(Sample sample)
		{
			m_Sample = sample;
		}

		/// <summary>
		/// The involved <see cref="Sample"/>.
		/// </summary>
		public Sample Sample
		{
			get { return m_Sample; }
		}
	}
}