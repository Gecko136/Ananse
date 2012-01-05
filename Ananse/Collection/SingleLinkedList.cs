// 
//  SingleLinkedList.cs
//  
//  Author:
//       Gunnar Quehl <github@gunnar-quehl.de>
// 
//  Copyright (c) 2012 Gunnar Quehl
// 
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
// 
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
using System;
using System.Collections.Generic;

namespace Ananse 
{
	public class SingleLinkedList<T> : IEnumerable<T>
	{
		public T 				   Head 			{ get; private set; }
		public SingleLinkedList<T> Tail 			{ get; private set; }
		
		private SingleLinkedList ()
		{
		}
		
		private SingleLinkedList (T head, SingleLinkedList<T> tail) 
		{	
			Head = head;
			Tail = tail;
		}
		
		public SingleLinkedList<T> Add(T item)
		{
			return new SingleLinkedList<T>(item, this);
		}
		
		public static SingleLinkedList<T> Empty		{ get; private set; }
		static SingleLinkedList () 
		{
			Empty = new SingleLinkedList<T>();
			Empty.Tail = Empty;
		}
		
		#region IEnumerable[T] implementation
		public IEnumerator<T> GetEnumerator ()
		{
			SingleLinkedList<T> current = this;
			while (current != Empty)
			{ 
				yield return Head;
				current = Tail;
			}
		}
		#endregion

		#region IEnumerable implementation
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
		#endregion
	}
}

