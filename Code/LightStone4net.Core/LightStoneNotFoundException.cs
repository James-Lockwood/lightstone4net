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
using System.Runtime.Serialization;

namespace LightStone4net.Core
{
	/// <summary>
	/// An exception that gets raised when the LightStone device cannot be found.
	/// </summary>
	[Serializable]
	public class LightStoneNotFoundException : Exception, ISerializable
	{
		private const string c_DefaultMessage = "LightStone device not found. Please check that the device is plugged in.";

		/// <summary>
		/// Initializes a new instance of the <see cref="LightStoneNotFoundException"/>class.
		/// </summary>
		public LightStoneNotFoundException()
			: this(c_DefaultMessage) 
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LightStoneNotFoundException"/>class.
		/// </summary>
		/// <param name="message">Message that describes the current exception</param>
		public LightStoneNotFoundException(String message)
			: base(message) // Call base constructor
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LightStoneNotFoundException"/>class.
		/// </summary>
		/// <param name="message">Message that describes the current exception</param>
		/// <param name="innerException">The exception that triggered the creation of this exception.</param>
		public LightStoneNotFoundException(String message, Exception innerException)
			: base(message, innerException) // Call base constructor
		{
		}

		/// <summary>
		/// Constructor required for serialization
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private LightStoneNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
