using System;
using System.Collections.Generic;
using System.Text;

namespace UsbLib
{
	public interface IDataReceiver
	{
		void HandleNewData(byte[] data);
	}
}
