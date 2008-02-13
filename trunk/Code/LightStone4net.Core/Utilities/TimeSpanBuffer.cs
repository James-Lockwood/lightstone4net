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

using LightStone4net.Core.Filter;
using LightStone4net.Core.Data;
using LightStone4net.Core.Internal;

namespace LightStone4net.Core.Utilities
{
	/// <summary>
	/// A circular buffer for time stamped data. It keeps the newest data
	/// up to a given age.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class TimeSpanBuffer<T> : ITimeSpanBuffer<T>
	{
		public static ITimeSpanBuffer<T> Synchronized(TimeSpan maximalAge, int initialBufferCapacity)
		{
			TimeSpanBuffer<T> timeSpanBuffer = new TimeSpanBuffer<T>(maximalAge, initialBufferCapacity);
			return new SynchronizedWrapper(timeSpanBuffer);
		}

		private TimeStampedValue<T>[] m_DataBuffer;
		private TimeSpan m_MaximalAge;
		private int m_SizeIncrement = 50;
		private int m_StartIndex;
		private int m_EndIndex;
		private bool m_IsReady = false;
		private object m_SyncObject = new object();

		public TimeSpanBuffer(TimeSpan maximalAge, int initialBufferCapacity)
		{
			if (maximalAge.Ticks < 0)
			{
				throw new ArgumentException("The specified maximal age must not be negative.", "maximalAge");
			}

			m_MaximalAge = maximalAge;
			m_DataBuffer = new TimeStampedValue<T>[initialBufferCapacity];
			m_StartIndex = 0;
			m_EndIndex = 0;
		}

		public object SyncObject
		{
			get
			{
				return m_SyncObject;
			}
		}

		public TimeSpan MaximalAge
		{
			get { return m_MaximalAge; }
		}

		public bool IsReady
		{
			get { return m_IsReady; }
		}

		public virtual void AddRange(IEnumerable<TimeStampedValue<T>> values)
		{
			foreach (TimeStampedValue<T> value in values)
			{
				Add(value);
			}
		}

		#region IList<TimeStampedValue<T>> Members

		public virtual int IndexOf(TimeStampedValue<T> item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public virtual void Insert(int index, TimeStampedValue<T> item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public virtual void RemoveAt(int index)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public virtual TimeStampedValue<T> this[int index]
		{
			get
			{
				return m_DataBuffer[TranslateIndex(index)];
			}
			set
			{
				m_DataBuffer[TranslateIndex(index)] = value;
			}
		}

		#endregion

		#region ICollection<T> Members

		public virtual void Add(TimeStampedValue<T> value)
		{
			RemoveOldEntries();
			m_DataBuffer[m_EndIndex] = value;
			m_EndIndex = (m_EndIndex + 1) % m_DataBuffer.Length;

			if (m_EndIndex == m_StartIndex)
			{
				IncreaseDataBuffer();
			}
		}

		public virtual void Clear()
		{
			m_StartIndex = m_EndIndex = 0;
		}

		public virtual bool Contains(TimeStampedValue<T> item)
		{
			throw new NotImplementedException();
		}

		public virtual void CopyTo(TimeStampedValue<T>[] array, int arrayIndex)
		{
			for (int i = arrayIndex; i < this.Count; i++)
			{
				array[i] = this[i];
			}
		}

		public virtual int Count
		{
			get
			{
				return (m_EndIndex - m_StartIndex + m_DataBuffer.Length) % m_DataBuffer.Length;
			}
		}

		public virtual bool IsReadOnly
		{
			get { return false; }
		}

		public virtual bool Remove(TimeStampedValue<T> item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IEnumerable<TimeStampedValue<T>> Members

		public virtual IEnumerator<TimeStampedValue<T>> GetEnumerator()
		{
			int currentIndex = m_StartIndex;
			while (currentIndex != m_EndIndex)
			{
				yield return m_DataBuffer[currentIndex];
				currentIndex = (currentIndex + 1) % m_DataBuffer.Length;
			}
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion

		protected virtual void RemoveOldEntries()
		{
			DateTime now = DateTime.Now;
			long cutOffAgeTicks = now.Ticks - m_MaximalAge.Ticks;

			while (this.Count > 0 && m_DataBuffer[m_StartIndex].TimeStamp.Ticks < cutOffAgeTicks)
			{
				//Debug.WriteLine("Removing old entry, current count: " + this.Count);
				m_IsReady = true; // the buffer is full

				TimeStampedValue<T> removedValue = m_DataBuffer[m_StartIndex];
				m_StartIndex = (m_StartIndex + 1) % m_DataBuffer.Length;
				OnOldEntryRemoved(removedValue);
				//Debug.WriteLine("After removing old entry, current count: " + this.Count);
				//Debug.WriteLine("Age of next entry: " + new TimeSpan(now.Ticks - m_DataBuffer[m_StartIndex].TimeStamp.Ticks).TotalSeconds.ToString());
			}
		}

		protected virtual void OnOldEntryRemoved(TimeStampedValue<T> removedValue)
		{
		}

		private void IncreaseDataBuffer()
		{
			TimeStampedValue<T>[] newDataBuffer = new TimeStampedValue<T>[m_DataBuffer.Length + m_SizeIncrement];
			for (int i = 0; i < m_DataBuffer.Length; i++)
			{
				newDataBuffer[i] = this[i];
			}

			m_StartIndex = 0;
			m_EndIndex = m_DataBuffer.Length;

			m_DataBuffer = newDataBuffer;
		}


		private int TranslateIndex(int index)
		{
			return (m_StartIndex + index) % m_DataBuffer.Length;
		}

		#region ISink<TimeStampedValue<T>> Members

		public virtual void Accept(TimeStampedValue<T> value)
		{
			Add(value);
		}

		#endregion

		#region nested class SynchronizedWrapper

		private class SynchronizedWrapper : ITimeSpanBuffer<T>
		{
			private TimeSpanBuffer<T> m_TimeSpanBuffer;

			public SynchronizedWrapper(TimeSpanBuffer<T> timeSpanBuffer)
			{
				m_TimeSpanBuffer = timeSpanBuffer;
			}

			#region ITimeSpanBuffer<T> Members

			public object SyncObject
			{
				get { return m_TimeSpanBuffer.SyncObject; }
			}

			public TimeSpan MaximalAge
			{
				get { return m_TimeSpanBuffer.MaximalAge; }
			}

			public bool IsReady
			{
				get 
				{
					lock (this.SyncObject)
					{
						return m_TimeSpanBuffer.IsReady;
					}
				}
			}

			public void AddRange(IEnumerable<TimeStampedValue<T>> values)
			{
				lock (this.SyncObject)
				{
					m_TimeSpanBuffer.AddRange(values);
				}
			}

			#endregion

			#region IList<TimeStampedValue<T>> Members

			public int IndexOf(TimeStampedValue<T> item)
			{
				lock (this.SyncObject)
				{
					return m_TimeSpanBuffer.IndexOf(item);
				}
			}

			public void Insert(int index, TimeStampedValue<T> item)
			{
				lock (this.SyncObject)
				{
					m_TimeSpanBuffer.Insert(index, item);
				}
			}

			public void RemoveAt(int index)
			{
				lock (this.SyncObject)
				{
					m_TimeSpanBuffer.RemoveAt(index);
				}
			}

			public TimeStampedValue<T> this[int index]
			{
				get
				{
					lock (this.SyncObject)
					{
						return m_TimeSpanBuffer[index];
					}
				}
				set
				{
					lock (this.SyncObject)
					{
						m_TimeSpanBuffer[index] = value;
					}
				}
			}

			#endregion

			#region ICollection<TimeStampedValue<T>> Members

			public void Add(TimeStampedValue<T> item)
			{
				lock (this.SyncObject)
				{
					m_TimeSpanBuffer.Add(item);
				}
			}

			public void Clear()
			{
				lock (this.SyncObject)
				{
					m_TimeSpanBuffer.Clear();
				}
			}

			public bool Contains(TimeStampedValue<T> item)
			{
				lock (this.SyncObject)
				{
					return m_TimeSpanBuffer.Contains(item);
				}
			}

			public void CopyTo(TimeStampedValue<T>[] array, int arrayIndex)
			{
				lock (this.SyncObject)
				{
					m_TimeSpanBuffer.CopyTo(array, arrayIndex);
				}
			}

			public int Count
			{
				get
				{
					lock (this.SyncObject)
					{
						return m_TimeSpanBuffer.Count;
					}
				}
			}

			public bool IsReadOnly
			{
				get { return m_TimeSpanBuffer.IsReadOnly; }
			}

			public bool Remove(TimeStampedValue<T> item)
			{
				lock (this.SyncObject)
				{
					return m_TimeSpanBuffer.Remove(item);
				}
			}

			#endregion

			#region IEnumerable<TimeStampedValue<T>> Members

			public IEnumerator<TimeStampedValue<T>> GetEnumerator()
			{
				lock (this.SyncObject)
				{
					return m_TimeSpanBuffer.GetEnumerator();
				}
			}

			#endregion

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			#endregion

			#region ISink<TimeStampedValue<T>> Members

			public void Accept(TimeStampedValue<T> value)
			{
				lock (this.SyncObject)
				{
					m_TimeSpanBuffer.Accept(value);
				}
			}

			#endregion
		}

		#endregion
	}
}
