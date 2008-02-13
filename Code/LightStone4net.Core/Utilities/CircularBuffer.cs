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

using LightStone4net.Core.Filter;

namespace LightStone4net.Core.Utilities
{
	public class CircularBuffer<T> : IList<T>, ISink<T>
	{
		public static CircularBuffer<T> Synchronized(int size)
		{
			return new SynchronizedCircularBuffer(size);
		}

		private int m_Size;
		private List<T> m_Content;
		private bool m_IsFull = false;
		private int m_End;
		private object m_SyncObject = new object();

		public CircularBuffer(int size)
		{
			m_Size = size;
			m_Content = new List<T>(m_Size);
			m_End = 0;
		}

		public object SyncObject
		{
			get
			{
				return m_SyncObject;
			}
		}

		public virtual int Size
		{
			get
			{
				return m_Size;
			}
			set
			{
				if (value < 1)
				{
					throw new ArgumentException("Size must be greater than zero.");
				}

				if (value == m_Size)
				{
					// nothing to do
					return;
				}

				if (value > m_Size)
				{
					List<T> newContent = new List<T>(value);
					newContent.AddRange(this);
					m_Content = newContent;
					m_IsFull = false;
					m_Size = value;
				}
				else
				{
					List<T> newContent = new List<T>(value);
					newContent.AddRange(this.GetLast(value));
					m_Content = newContent;
					m_IsFull = m_Content.Count == value;
					m_Size = value;
				}
			}
		}

		public virtual void AddRange(IEnumerable<T> values)
		{
			foreach (T value in values)
			{
				Add(value);
			}
		}

		#region IList<T> Members

		public virtual int IndexOf(T item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public virtual void Insert(int index, T item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public virtual void RemoveAt(int index)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public virtual T this[int index]
		{
			get
			{
				return m_Content[TranslateIndex(index)];
			}
			set
			{
				m_Content[TranslateIndex(index)] = value;
			}
		}

		#endregion

		#region ICollection<T> Members

		public virtual void Add(T value)
		{
			if (!m_IsFull)
			{
				m_Content.Add(value);
				if (m_Content.Count == m_Size)
				{
					m_IsFull = true;
					m_End = m_Size - 1;
				}
			}
			else
			{
				m_End = (m_End + 1) % m_Content.Count;
				m_Content[m_End] = value;
			}
		}

		public virtual void Clear()
		{
			m_Content.Clear();
			m_IsFull = false;
		}

		public virtual bool Contains(T item)
		{
			return m_Content.Contains(item);
		}

		public virtual void CopyTo(T[] array, int arrayIndex)
		{
			for (int i = arrayIndex; i < this.Count; i++)
			{
				array[i] = this[i];
			}
		}

		public virtual int Count
		{
			get { return m_Content.Count; }
		}

		public virtual bool IsReadOnly
		{
			get { return false; }
		}

		public virtual bool Remove(T item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IEnumerable<T> Members

		public virtual IEnumerator<T> GetEnumerator()
		{
			if (!m_IsFull)
			{
				foreach (T value in m_Content)
				{
					yield return value;
				}
			}
			else
			{
				int currentIndex = m_End;
				for (int i = 0; i < m_Content.Count; i++)
				{
					currentIndex = (currentIndex + 1) % m_Content.Count;
					yield return m_Content[currentIndex];
				}
			}
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion

		public IEnumerable<T> GetLast(int count)
		{
			if (count >= m_Content.Count)
			{
				foreach (T value in m_Content)
				{
					yield return value;
				}
			}
			else
			{
				int currentIndex = (m_End + (m_Size - count)) % m_Content.Count;
				for (int i = 0; i < count; i++)
				{
					currentIndex = (currentIndex + 1) % m_Content.Count;
					yield return m_Content[currentIndex];
				}
			}
		}

		private int TranslateIndex(int index)
		{
			if (!m_IsFull)
			{
				return index;
			}
			else
			{
				return (m_End + index + 1) % m_Content.Count;
			}
		}

		#region ISink<T> Members

		public virtual void Accept(T value)
		{
			Add(value);
		}

		#endregion

		#region nested class
		private class SynchronizedCircularBuffer : CircularBuffer<T>
		{
			public SynchronizedCircularBuffer(int size)
				: base(size)
			{
			}

			public override int Size
			{
				get
				{
					lock (this.SyncObject)
					{
						return base.Size;
					}
				}
				set
				{
					lock (this.SyncObject)
					{
						base.Size = value;
					}
				}
			}

			public override void AddRange(IEnumerable<T> values)
			{
				lock (this.SyncObject)
				{
					base.AddRange(values);
				}
			}

			#region IList<T> Members

			public override int IndexOf(T item)
			{
				lock (this.SyncObject)
				{
					return base.IndexOf(item);
				}
			}

			public override void Insert(int index, T item)
			{
				lock (this.SyncObject)
				{
					base.Insert(index, item);
				}
			}

			public override void RemoveAt(int index)
			{
				lock (this.SyncObject)
				{
					base.RemoveAt(index);
				}
			}

			public override T this[int index]
			{
				get
				{
					lock (this.SyncObject)
					{
						return base[index];
					}
				}
				set
				{
					lock (this.SyncObject)
					{
						base[index] = value;
					}
				}
			}

			#endregion

			#region ICollection<T> Members

			public override void Add(T value)
			{
				lock (this.SyncObject)
				{
					base.Add(value);
				}
			}

			public override void Clear()
			{
				lock (this.SyncObject)
				{
					base.Clear();
				}
			}

			public override bool Contains(T item)
			{
				lock (this.SyncObject)
				{
					return base.Contains(item);
				}
			}

			public override void CopyTo(T[] array, int arrayIndex)
			{
				lock (this.SyncObject)
				{
					base.CopyTo(array, arrayIndex);
				}
			}

			public override int Count
			{
				get
				{
					lock (this.SyncObject)
					{
						return base.Count;
					}
				}
			}

			public override bool Remove(T item)
			{
				lock (this.SyncObject)
				{
					return base.Remove(item);
				}
			}

			#endregion

			#region IEnumerable<T> Members

			public override IEnumerator<T> GetEnumerator()
			{
				lock (this.SyncObject)
				{
					return base.GetEnumerator();
				}
			}

			#endregion
		}

		#endregion
	}
}
