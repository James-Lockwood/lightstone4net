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

using UsbLib;
using LightStone4net.Core.Internal;
using LightStone4net.Core.Filter;
using LightStone4net.Core.Data;

namespace LightStone4net.Core
{
	public class LightStoneDevice : HidDevice
	{
		private static LightStoneDevice s_LightStoneDevice = null;

		public static LightStoneDevice Instance
		{
			get
			{
				if (s_LightStoneDevice == null)
				{
					s_LightStoneDevice = FindLightStoneDevice();
				}
				return s_LightStoneDevice;
			}
		}

		/// <summary>
		/// Finds the Light Stone device. 
		/// </summary>
		/// <returns>A new <see cref="LightStoneDevice"/> or null if not found.</returns>
		private static LightStoneDevice FindLightStoneDevice()
		{
			// Constants as hard coded in version 0.7 of lsm:
			//     Vendor ID: 1155 (0x0483), Product ID: 53 (0x0035)
			// LightStone bought December 2006:
			//     Vendor ID: 5370 (0x14FA), Product ID: 1 (0x0001) [Version number: 9281 (0x2441)]
			// VID and PID for Lightstone device are 0x054c and 0x1000 respectively
			return (LightStoneDevice)FindDevice(
				0x14FA,
				0x0001,
				delegate(string devicePath) { return new LightStoneDevice(devicePath); }
				);
		}

		public static readonly TimeSpan SamplingInterval = TimeSpan.FromMilliseconds(32); // interval between samples taken by lighstone

		private SampleExtractor m_SampleExtractor;
		private DateTime m_StartTime;

		private LightStoneDevice(string devicePath)
			: base(devicePath)
		{
			m_SampleExtractor = new SampleExtractor(this);
			m_StartTime = DateTime.Now;
			base.Start();
		}

		public event EventHandler<SampleEventArgs> NewSample;

		protected override void OnDataReceived(byte[] data)
		{
			if (data != null)
			{
				m_SampleExtractor.HandleNewData(data);
			}
		}

		protected internal virtual void OnNewSample(Sample newSample)
		{
			m_RawOutput.WriteOutput(newSample);
			
			TimeStampedValue<double> heartSignal = new TimeStampedValue<double>(newSample.TimeStamp, newSample.Heart);
			m_RawHeartSignal.WriteOutput(heartSignal);

			EventHandler<SampleEventArgs> handler = this.NewSample;
			if (handler != null)
			{
				try
				{
					handler(this, new SampleEventArgs(newSample));
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.ToString());
					//s_Log.Fatal("One of the subscribers of the NewSample event threw an exception.");
				}
			}
		}

		private Output<Sample> m_RawOutput = new Output<Sample>();

		public ISource<Sample> RawOutput
		{
			get
			{
				return m_RawOutput;
			}
		}

		private Output<TimeStampedValue<double>> m_RawHeartSignal = new Output<TimeStampedValue<double>>();

		public ISource<TimeStampedValue<double>> RawHeartSignalOutput
		{
			get
			{
				return m_RawHeartSignal;
			}
		}

	
		/// <summary>
		/// Virtual method to create an input report for this device. Override to use.
		/// </summary>
		/// <returns>A shiny new input report</returns>
		public override InputReport CreateInputReport()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Fired when data has been received over USB
		/// </summary>
		/// <param name="inputReport">Input report received</param>
		protected override void HandleDataReceived(InputReport inputReport)
		{
			Debug.WriteLine("inputReport received.");
		}

		#region Dispose

		/// <summary>
		/// Dispose.
		/// </summary>
		/// <param name="disposing">True if object is being disposed - else is being finalised</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				// just a place holder for now
			}
			base.Dispose(disposing);
		}

		#endregion
	}
}
