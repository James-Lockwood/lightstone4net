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
using System.Threading;
using System.Diagnostics;

using NUnit.Framework;
using LightStone4net.Core.Utilities;
using LightStone4net.Core.Data;

namespace UnitTests
{
	[TestFixture]
	public class TimeSpanBufferTests
	{
		[Test]
		public void TestAging()
		{
			TimeSpan maximalAge = TimeSpan.FromSeconds(10);
			TimeSpanBuffer<double> circularBuffer = new TimeSpanBuffer<double>(maximalAge, 100);

			for (int i = 0; i < 10; i++)
			{
				circularBuffer.Add(new TimeStampedValue<double>(DateTime.Now, i));
				Assert.AreEqual(circularBuffer.Count, i + 1);
				Thread.Sleep(TimeSpan.FromSeconds(1));
			}

			Thread.Sleep(TimeSpan.FromSeconds(5));
			circularBuffer.Add(new TimeStampedValue<double>(DateTime.Now, 10));
			Assert.IsTrue(circularBuffer.Count < 9);

			//for (int i = 10; i < 20; i++)
			//{
			//    circularBuffer.Add(new TimeStampedValue<double>(DateTime.Now, i));
			//    Assert.AreEqual(circularBuffer.Count, 10);
			//    Thread.Sleep(TimeSpan.FromSeconds(1));
			//}
		}

		[Test]
		public void TestSizeIncrease()
		{
			TimeSpan maximalAge = TimeSpan.FromSeconds(12);
			TimeSpanBuffer<double> circularBuffer = new TimeSpanBuffer<double>(maximalAge, 10);

			for (int i = 0; i < 100; i++)
			{
				circularBuffer.Add(new TimeStampedValue<double>(DateTime.Now, i));
				Debug.WriteLine("***" + circularBuffer.Count);
				Assert.AreEqual(circularBuffer.Count, i + 1);
				Thread.Sleep(TimeSpan.FromMilliseconds(100));
			}

			Assert.IsTrue(circularBuffer.Count > 10);
		}

		[Test]
		public void TestAging2()
		{
			TestingTimeSpanBuffer testingTimeSpanBuffer = new TestingTimeSpanBuffer();
			Thread.Sleep(TimeSpan.FromSeconds(16));
		}

		#region nested classes

		private class TestingTimeSpanBuffer : TimeSpanBuffer<double>
		{
			private double[] m_Values;
			private int m_CurrentIndex = -1;

			public TestingTimeSpanBuffer() : base(TimeSpan.FromSeconds(2.5), 20)
			{
				m_Values = new double[] { 1, 2, 3, 2, 3, 4, 3, 4, 5, 4, 5, 6 };
				TimedValuesProvider timedValuesProvider = new TimedValuesProvider(m_Values, TimeSpan.FromSeconds(1), this);
				timedValuesProvider.Start();
			}

			public override void Accept(TimeStampedValue<double> timeStampedValue)
			{
				m_CurrentIndex++;
				PrintContent(String.Format("Before accepting {0} [count = {1}]: ", timeStampedValue.Value, this.Count));
				base.Accept(timeStampedValue);
				PrintContent(String.Format("After accepting {0} [count = {1}]: ", timeStampedValue.Value, this.Count));
			}

			protected override void RemoveOldEntries()
			{
				PrintContent("Before RemoveOldEntries: ");
				base.RemoveOldEntries();
				PrintContent("After RemoveOldEntries: ");
			}

			protected override void OnOldEntryRemoved(TimeStampedValue<double> removedTimeStampedValue)
			{
				TimeSpan age = new TimeSpan(DateTime.Now.Ticks - removedTimeStampedValue.TimeStamp.Ticks);
				Console.WriteLine("OnOldEntryRemoved: {0}, age: {1}", removedTimeStampedValue.Value, age.TotalSeconds);
			}

			private void PrintContent(string prefix)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(prefix);
				bool isFirst = true;
				foreach (TimeStampedValue<double> timeStampedDouble in this)
				{
					if (isFirst)
					{
						isFirst = false;
					}
					else
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(timeStampedDouble.Value);
				}

				Console.WriteLine(stringBuilder.ToString());
			}
		}

		private class TimedValuesProvider
		{
			private double[] m_Values;
			private TimeSpan m_TimeInterval;
			private TestingTimeSpanBuffer m_TestingTimeSpanBuffer;
			private Timer m_Timer;
			private int m_CurrentIndex = 0;

			public TimedValuesProvider(double[] values, TimeSpan timeInterval, TestingTimeSpanBuffer testingTimeSpanBuffer)
			{
				m_Values = values;
				m_TimeInterval = timeInterval;
				m_TestingTimeSpanBuffer = testingTimeSpanBuffer;

				m_Timer = new Timer(
					new TimerCallback(this.OnTimer),
					null, // we don't need a state object 
					Timeout.Infinite, // don't start yet
					Timeout.Infinite  // no periodic firing
					);
			}

			public void Start()
			{
				m_Timer.Change(TimeSpan.Zero, m_TimeInterval);
			}

			/// <summary>
			/// Callback for the timer event
			/// </summary>
			/// <param name="state"></param>
			private void OnTimer(object state)
			{
				ServeNextValue();
			}

			private void ServeNextValue()
			{
				if (m_CurrentIndex > m_Values.Length - 1)
				{
					// we reached the end of the values list
					m_Timer.Change(Timeout.Infinite, Timeout.Infinite);
				}
				else
				{
					double value = m_Values[m_CurrentIndex];
					m_TestingTimeSpanBuffer.Accept(new TimeStampedValue<double>(DateTime.Now, value));
					m_CurrentIndex++;
				}
			}
		}
		#endregion
	}
}