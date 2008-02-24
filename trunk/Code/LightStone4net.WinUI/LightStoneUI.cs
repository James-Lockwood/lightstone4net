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
using LightStone4net.Core;
using System.Windows.Forms;

namespace LightStone4net.WinUI
{
	public class LightStoneUI
	{
		/// <summary>
		/// Must be called once at the start of a WinUI application that uses the <see cref="LightStoneDevice"/>.
		/// </summary>
		public static void Initialize()
		{
			const string c_InitializationErrorCaption = "Initialization Error";
			string errorMessage;
			try
			{
				LightStoneDevice.Initialize();
			}
			catch (LightStoneNotFoundException)
			{
				errorMessage = @"The LightStone device cannot be found.
Please make sure that the LightStone is plugged in.";
				MessageBox.Show(errorMessage, c_InitializationErrorCaption);
				Environment.Exit(-1);
			}
			catch (LightStoneInUseException)
			{
				errorMessage = @"The LightStone device is in use by another application.
Please close any application that might use the LightStone.";
				MessageBox.Show(errorMessage, c_InitializationErrorCaption);
				Environment.Exit(-2);
			}
			catch (Exception ex)
			{
				errorMessage = @"Unexpected initialization error.

" + ex.Message;
				MessageBox.Show(errorMessage, c_InitializationErrorCaption);
				Environment.Exit(-3);
			}
		}
	}
}
