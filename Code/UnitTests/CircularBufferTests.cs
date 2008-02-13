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

using NUnit.Framework;
using LightStone4net.Core.Utilities;

namespace UnitTests
{
	[TestFixture]
	public class CircularBufferTests
	{
		[Test]
		public void TestRollover()
		{
			int bufferSize = 3;
			CircularBuffer<int> circularBuffer = new CircularBuffer<int>(bufferSize);

			for (int i = 0; i < 3; i++)
			{
				circularBuffer.Add(i);
				VerifyContent(circularBuffer, 0, i);
			}

			for (int i = 3; i < 8; i++)
			{
				circularBuffer.Add(i);
				VerifyContent(circularBuffer, i - bufferSize + 1, i);
			}
		}

		[Test]
		public void TestSizeIncrease()
		{
			int bufferSize = 3;
			CircularBuffer<int> circularBuffer = new CircularBuffer<int>(bufferSize);

			circularBuffer.AddRange(new int[]{0, 1});

			// Increase when not yet full
			bufferSize = 5;
			circularBuffer.Size = bufferSize;
			VerifyContent(circularBuffer, 0 , 1);

			// fill up
			for (int i = 2; i < 5; i++)
			{
				circularBuffer.Add(i);
				VerifyContent(circularBuffer, 0, i);
			}

			// roll over
			for (int i = 5; i < 8; i++)
			{
				circularBuffer.Add(i);
				VerifyContent(circularBuffer, i - bufferSize + 1, i);
			}

			// Increase when full
			bufferSize = 7;
			circularBuffer.Size = bufferSize;
			VerifyContent(circularBuffer, 3, 7);

			// fill up
			for (int i = 8; i < 10; i++)
			{
				circularBuffer.Add(i);
				VerifyContent(circularBuffer, 3, i);
			}

			// roll over
			for (int i = 10; i < 12; i++)
			{
				circularBuffer.Add(i);
				VerifyContent(circularBuffer, i - bufferSize + 1, i);
			}
		}

		private void VerifyContent(CircularBuffer<int> circularBuffer, int min, int max)
		{
			int expectedValue = min;
			foreach (int value in circularBuffer)
			{
				Assert.AreEqual(expectedValue, value);
				expectedValue++;
			}
			Assert.AreEqual(max + 1, expectedValue);
		}

		[Test]
		public void TestAccessByIndex()
		{
			int bufferSize = 3;
			CircularBuffer<int> circularBuffer = new CircularBuffer<int>(bufferSize);

			for (int i = 0; i < 3; i++)
			{
				circularBuffer.Add(i);
			}
			// not rolled over yet
			Assert.AreEqual(1, circularBuffer[1]);

			for (int i = 3; i < 8; i++)
			{
				circularBuffer.Add(i);
			}
			// rolled over 
			Assert.AreEqual(6, circularBuffer[1]);
		}
	}
}
