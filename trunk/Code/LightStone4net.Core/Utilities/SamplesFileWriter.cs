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
using System.Globalization;

namespace LightStone4net.Core.Utilities
{
	public class SamplesFileWriter : IDisposable
	{
		private LightStoneDevice m_LightStoneDevice;
		private StreamWriter m_StreamWriter = null;

		public SamplesFileWriter(LightStoneDevice lightStoneDevice, string filePath)
		{
			m_LightStoneDevice = lightStoneDevice;
			m_StreamWriter = File.CreateText(filePath);
			m_StreamWriter.WriteLine("TimeStamp\tSCL\tHeart Data\tAuto Gain");

			m_LightStoneDevice.NewSample += new EventHandler<SampleEventArgs>(OnNewSample);
		}

		private void OnNewSample(object sender, SampleEventArgs e)
		{
			Sample sample = e.Sample;
			m_StreamWriter.WriteLine(
				"{0}\t{1}::{2}\t{3}\t{4}\t{5}",
				sample.TimeSlotIndex,
				sample.TimeStamp.ToString("HH:mm:ss", CultureInfo.InvariantCulture),
				sample.TimeStamp.Millisecond.ToString("000"),
				sample.SkinConductivity,
				sample.Heart,
				String.Empty);
		}

		#region IDisposable Members
		/// <summary>
		/// Dispose method
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Disposer called by both dispose and finalise
		/// </summary>
		/// <param name="disposing">True if disposing</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)	// if we are disposing, need to close the managed resources
			{
				m_LightStoneDevice.NewSample -= new EventHandler<SampleEventArgs>(OnNewSample);
				if (m_StreamWriter != null)
				{
					m_StreamWriter.Close();
					m_StreamWriter = null;
				}
			}
		}
		#endregion
	}
}
