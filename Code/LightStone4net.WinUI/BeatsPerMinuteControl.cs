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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using LightStone4net.Core;
using LightStone4net.Core.Utilities;
using LightStone4net.Core.Filter;
using LightStone4net.Core.Data;

namespace LightStone4net.WinUI
{
	public partial class BeatsPerMinuteControl : UserControl
	{
		public BeatsPerMinuteControl()
		{
			InitializeComponent();
		}

		protected override void OnCreateControl()
		{
			if (!this.DesignMode)
			{
				HeartRate.Instance.BeatsPerMinuteOutput.Add(new BeatsPerMinuteInput(this));
				HeartRate.Instance.BeepEnabled = m_BeepCheckBox.Checked;
			}
			base.OnCreateControl();
		}

		private void OnBeepCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			HeartRate.Instance.BeepEnabled = m_BeepCheckBox.Checked;
		}

		#region nested Input classes

		private class BeatsPerMinuteInput : ISink<TimeStampedValue<int>>
		{
			private BeatsPerMinuteControl m_Parent;

			internal BeatsPerMinuteInput(BeatsPerMinuteControl parent)
			{
				m_Parent = parent;
			}

			#region ISink<TimeStampedValue<int>> Members

			public void Accept(TimeStampedValue<int> timeStampedValue)
			{
				m_Parent.m_BeatsPerMinuteTextBox.Text = timeStampedValue.Value.ToString();
			}

			#endregion
		}

		#endregion
	}
}
