using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace UsbLib
{
	/// <summary>
	/// Abstract HID device : Derive your new device controller class from this
	/// </summary>
	public abstract class HidDevice : Win32Usb, IDisposable
	{
		#region Static stuff
		/// <summary>
		/// Finds a device given its PID and VID
		/// </summary>
		/// <param name="vendorId">Vendor id for device (VID)</param>
		/// <param name="productId">Product id for device (PID)</param>
		/// <param name="creatorDelegate">A delgate that will be invoked to create the actual device class.</param>
		/// <returns>A new device class of the given type or null</returns>
		public static HidDevice FindDevice(int vendorId, int productId, HidDeviceCreatorDelegate creatorDelegate)
		{
			string searchString = string.Format("vid_{0:x4}&pid_{1:x4}", vendorId, productId); // first, build the path search string
			Guid gHid;
			HidD_GetHidGuid(out gHid);	// next, get the GUID from Windows that it uses to represent the HID USB interface
			IntPtr hInfoSet = SetupDiGetClassDevs(ref gHid, null, IntPtr.Zero, DIGCF_DEVICEINTERFACE | DIGCF_PRESENT);	// this gets a list of all HID devices currently connected to the computer (InfoSet)
			try
			{
				DeviceInterfaceData interfaceData = new DeviceInterfaceData();	// build up a device interface data block
				interfaceData.Size = Marshal.SizeOf(interfaceData);
				// Now iterate through the InfoSet memory block assigned within Windows in the call to SetupDiGetClassDevs
				// to get device details for each device connected
				int index = 0;
				while (SetupDiEnumDeviceInterfaces(hInfoSet, 0, ref gHid, (uint)index, ref interfaceData))	// this gets the device interface information for a device at index 'nIndex' in the memory block
				{
					string devicePath = GetDevicePath(hInfoSet, ref interfaceData);	// get the device path (see helper method 'GetDevicePath')
					if (devicePath.IndexOf(searchString) >= 0)	// do a string search, if we find the VID/PID string then we found our device!
					{
						HidDevice newDevice = creatorDelegate(devicePath);	// create an instance of the class for this device
						return newDevice;	// and return it
					}
					index++;	// if we get here, we didn't find our device. So move on to the next one.
				}
			}
			finally
			{
				// Before we go, we have to free up the InfoSet memory reserved by SetupDiGetClassDevs
				SetupDiDestroyDeviceInfoList(hInfoSet);
			}
			return null;	// oops, didn't find our device
		}

		/// <summary>
		/// Helper method to return the device path given a DeviceInterfaceData structure and an InfoSet handle.
		/// Used in 'FindDevice' so check that method out to see how to get an InfoSet handle and a DeviceInterfaceData.
		/// </summary>
		/// <param name="hInfoSet">Handle to the InfoSet</param>
		/// <param name="interfaceData">DeviceInterfaceData structure</param>
		/// <returns>The device path or null if there was some problem</returns>
		private static string GetDevicePath(IntPtr hInfoSet, ref DeviceInterfaceData oInterface)
		{
			uint nRequiredSize = 0;
			// Get the device interface details
			if (!SetupDiGetDeviceInterfaceDetail(hInfoSet, ref oInterface, IntPtr.Zero, 0, ref nRequiredSize, IntPtr.Zero))
			{
				DeviceInterfaceDetailData oDetail = new DeviceInterfaceDetailData();
				oDetail.Size = 5;	// hardcoded to 5! Sorry, but this works and trying more future proof versions by setting the size to the struct sizeof failed miserably. If you manage to sort it, mail me! Thx
				if (SetupDiGetDeviceInterfaceDetail(hInfoSet, ref oInterface, ref oDetail, nRequiredSize, ref nRequiredSize, IntPtr.Zero))
				{
					return oDetail.DevicePath;
				}
			}
			return null;
		}
		#endregion

		#region Privates variables
		/// <summary>Filestream we can use to read/write from</summary>
		private FileStream m_File;
		/// <summary>Length of input report : device gives us this</summary>
		private int m_InputReportLength;
		/// <summary>Length if output report : device gives us this</summary>
		private int m_OutputReportLength;
		/// <summary>Handle to the device</summary>
		private IntPtr m_hHandle;

		private int m_VendorId;
		private int m_ProductId;
		private int m_VersionNumber;

		private string m_Manufacturer;
		private string m_Product;
		#endregion

		#region Construction related

		/// <summary>
		/// Constructs the device
		/// </summary>
		/// <param name="path">Path to the device</param>
		public HidDevice(string path)
		{
			// Create the file from the device path
			m_hHandle = CreateFile(path, GENERIC_READ | GENERIC_WRITE, 0, IntPtr.Zero, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, IntPtr.Zero);

			if (m_hHandle != InvalidHandleValue)	// if the open worked...
			{
				InitializeAttributes();
				InitializeManufacturer();
				InitializeProduct();
				InitializeCapabilities();


				m_File = new FileStream(m_hHandle, FileAccess.Read | FileAccess.Write, true, m_InputReportLength, true);	// wrap the file handle in a .Net file stream

			}
			else	// File open failed? Chuck an exception
			{
				m_hHandle = IntPtr.Zero;
				throw HidDeviceException.GenerateWithWinError("Failed to create device file");
			}
		}

		/// <summary>
		/// Kick off the first asynchronous read
		/// </summary>
		protected void Start()
		{
			BeginAsyncRead();
		}

		private void InitializeAttributes()
		{
			HIDD_ATTRIBUTES attributes = new HIDD_ATTRIBUTES();
			attributes.Size = Marshal.SizeOf(attributes);

			if (HidD_GetAttributes(m_hHandle, out attributes))
			{
				m_VendorId = attributes.VendorID;
				m_ProductId = attributes.ProductID;
				m_VersionNumber = attributes.VersionNumber;
			}
			else
			{
				throw HidDeviceException.GenerateWithWinError("GetAttributes failed");
			}
		}

		private void InitializeManufacturer()
		{
			int bufferLength = 126;
			byte[] buffer = new byte[bufferLength];

			if (HidD_GetManufacturerString(m_hHandle, buffer, bufferLength))
			{
				m_Manufacturer = GetUnicodeString(buffer);
			}
			else
			{
				throw HidDeviceException.GenerateWithWinError("GetManufacturerString failed");
			}
		}

		private void InitializeProduct()
		{
			int bufferLength = 126;
			byte[] buffer = new byte[bufferLength];

			if (HidD_GetProductString(m_hHandle, buffer, bufferLength))
			{
				m_Product = GetUnicodeString(buffer);
			}
			else
			{
				throw HidDeviceException.GenerateWithWinError("GetProductString failed");
			}
		}

		private void InitializeCapabilities()
		{
			IntPtr lpData;
			if (HidD_GetPreparsedData(m_hHandle, out lpData))	// get windows to read the device data into an internal buffer
			{
				try
				{
					HidCaps caps;
					HidP_GetCaps(lpData, out caps);	// extract the device capabilities from the internal buffer
					m_InputReportLength = caps.InputReportByteLength;	// get the input...
					m_OutputReportLength = caps.OutputReportByteLength;	// ... and output report lengths
				}
				finally
				{
					HidD_FreePreparsedData(ref lpData);	// before we quit the funtion, we must free the internal buffer reserved in GetPreparsedData
				}
			}
			else	// GetPreparsedData failed? Chuck an exception
			{
				throw HidDeviceException.GenerateWithWinError("GetPreparsedData failed");
			}
		}

		#endregion

		#region Privates/protected

		private string GetUnicodeString(byte[] buffer)
		{
			int contentLength = 0;
			for (int i = 0; i < buffer.Length; i += 2)
			{
				if (buffer[i] == 0 && buffer[i + 1] == 0)
				{
					contentLength = i;
					break;
				}
			}
			return new UnicodeEncoding().GetString(buffer, 0, contentLength);
		}

		/// <summary>
		/// Kicks off an asynchronous read which completes when data is read or when the device
		/// is disconnected. Uses a callback.
		/// </summary>
		private void BeginAsyncRead()
		{
			byte[] arrInputReport = new byte[m_InputReportLength];
			// put the buff we used to receive the stuff as the async state then we can get at it when the read completes
			m_File.BeginRead(arrInputReport, 0, m_InputReportLength, new AsyncCallback(ReadCompleted), arrInputReport);
		}
		/// <summary>
		/// Callback for above. Care with this as it will be called on the background thread from the async read
		/// </summary>
		/// <param name="iResult">Async result parameter</param>
		protected void ReadCompleted(IAsyncResult asyncResult)
		{
			byte[] data = (byte[])asyncResult.AsyncState;	// retrieve the read buffer
			try
			{
				m_File.EndRead(asyncResult);	// call end read : this throws any exceptions that happened during the read
				try
				{
					//InputReport inputReport = CreateInputReport();	// Create the input report for the device
					//inputReport.SetData(data);	// and set the data portion - this processes the data received into a more easily understood format depending upon the report type
					//HandleDataReceived(inputReport);	// pass the new input report on to the higher level handler

					OnDataReceived(data);
				}
				finally
				{
					BeginAsyncRead();	// when all that is done, kick off another read for the next report
				}
			}
			catch (IOException)	// if we got an IO exception, the device was removed
			{
				HandleDeviceRemoved();
				if (OnDeviceRemoved != null)
				{
					OnDeviceRemoved(this, new EventArgs());
				}
				Dispose();
			}
		}
		/// <summary>
		/// Write an output report to the device.
		/// </summary>
		/// <param name="outputReport">Output report to write</param>
		protected void Write(OutputReport outputReport)
		{
			try
			{
				m_File.Write(outputReport.Buffer, 0, outputReport.BufferLength);
			}
			catch (IOException)
			{
				// The device was removed!
				throw new HidDeviceException("Device was removed");
			}
		}

		protected virtual void OnDataReceived(byte[] data)
		{
		}

		/// <summary>
		/// virtual handler for any action to be taken when data is received. Override to use.
		/// </summary>
		/// <param name="inputReport">The input report that was received</param>
		protected virtual void HandleDataReceived(InputReport inputReport)
		{
		}

		/// <summary>
		/// Virtual handler for any action to be taken when a device is removed. Override to use.
		/// </summary>
		protected virtual void HandleDeviceRemoved()
		{
		}
		#endregion

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
		/// <param name="bDisposing">True if disposing</param>
		protected virtual void Dispose(bool bDisposing)
		{
			if (bDisposing)	// if we are disposing, need to close the managed resources
			{
				if (m_File != null)
				{
					m_File.Close();
					m_File = null;
				}
			}
			if (m_hHandle != IntPtr.Zero)	// Dispose and finalize, get rid of unmanaged resources
			{
				CloseHandle(m_hHandle);
			}
		}
		#endregion

		#region Publics
		/// <summary>
		/// Event handler called when device has been removed
		/// </summary>
		public event EventHandler OnDeviceRemoved;

		public int VendorId
		{
			get { return m_VendorId; }
		}

		public int ProductId
		{
			get { return m_ProductId; }
		}

		public int VersionNumber
		{
			get { return m_VersionNumber; }
		}

		public string Manufacturer
		{
			get { return m_Manufacturer; }
		}

		public string Product
		{
			get { return m_Product; }
		}

		/// <summary>
		/// Accessor for output report length
		/// </summary>
		public int OutputReportLength
		{
			get
			{
				return m_OutputReportLength;
			}
		}
		/// <summary>
		/// Accessor for input report length
		/// </summary>
		public int InputReportLength
		{
			get
			{
				return m_InputReportLength;
			}
		}
		/// <summary>
		/// Virtual method to create an input report for this device. Override to use.
		/// </summary>
		/// <returns>A shiny new input report</returns>
		public abstract InputReport CreateInputReport();

		#endregion
	}
}
