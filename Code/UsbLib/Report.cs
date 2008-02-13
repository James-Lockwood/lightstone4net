using System;

namespace UsbLib
{
	/// <summary>
	/// Base class for report types. Simply wraps a byte buffer.
	/// </summary>
	public abstract class Report
	{
		#region Member variables
		/// <summary>Buffer for raw report bytes</summary>
		private byte[] m_Buffer;
		/// <summary>Length of the report</summary>
		private int m_BufferLength;
		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="hidDevice">Constructing device</param>
		public Report(HidDevice hidDevice)
		{
			// Do nothing
		}

		/// <summary>
		/// Sets the raw byte array.
		/// </summary>
		/// <param name="buffer">Raw report bytes</param>
		protected void SetBuffer(byte[] buffer)
		{
			m_Buffer = buffer;
			m_BufferLength = m_Buffer.Length;
		}

		/// <summary>
		/// Accessor for the raw byte buffer
		/// </summary>
		public byte[] Buffer
		{
			get
			{
				return m_Buffer;
			}
		}
		/// <summary>
		/// Accessor for the buffer length
		/// </summary>
		public int BufferLength
		{
			get
			{
				return m_BufferLength;
			}
		}
	}
	/// <summary>
	/// Defines a base class for output reports. To use output reports, just put the bytes into the raw buffer.
	/// </summary>
	public abstract class OutputReport : Report
	{
		/// <summary>
		/// Construction. Setup the buffer with the correct output report length dictated by the device
		/// </summary>
		/// <param name="hidDevice">Creating device</param>
		public OutputReport(HidDevice hidDevice)
			: base(hidDevice)
		{
			SetBuffer(new byte[hidDevice.OutputReportLength]);
		}
	}

	/// <summary>
	/// Defines a base class for input reports. To use input reports, use the SetData method and override the 
	/// ProcessData method.
	/// </summary>
	public abstract class InputReport : Report
	{
		/// <summary>
		/// Construction. Do nothing
		/// </summary>
		/// <param name="hidDevice">Creating device</param>
		public InputReport(HidDevice hidDevice)
			: base(hidDevice)
		{
		}
		/// <summary>
		/// Call this to set the buffer given a raw input report. Calls an overridable method to
		/// should automatically parse the bytes into meaningul structures.
		/// </summary>
		/// <param name="data">Raw input report.</param>
		public void SetData(byte[] data)
		{
			SetBuffer(data);
			ProcessData();
		}
		/// <summary>
		/// Override this to process the input report into something useful
		/// </summary>
		public abstract void ProcessData();
	}
}
