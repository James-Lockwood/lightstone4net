using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace UsbLib
{
	/// <summary>
	/// Generic HID device exception
	/// </summary>
	[global::System.Serializable]
	public sealed class HidDeviceException : Exception, ISerializable
	{
		public static HidDeviceException GenerateWithWinError(string strMessage)
		{
			return new HidDeviceException(string.Format("Msg:{0} WinEr:{1:X8}", strMessage, Marshal.GetLastWin32Error()));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HIDDeviceException"/>class.
		/// </summary>
		public HidDeviceException()
			: base() // Call base constructor 
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HIDDeviceException"/>class.
		/// </summary>
		/// <param name="message">Message that describes the current exception</param>
		public HidDeviceException(String message)
			: base(message) // Call base constructor
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HIDDeviceException"/>class.
		/// </summary>
		/// <param name="message">Message that describes the current exception</param>
		/// <param name="innerException">The exception that triggered the creation of this exception.</param>
		public HidDeviceException(String message, Exception innerException)
			: base(message, innerException) // Call base constructor
		{
		}
	}
}
