// 
//  KeyItem.cs
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

namespace Ananse
{
	public class KeyItem
	{
		public MappingType			MappingType	{ get; private set; }
		public string				Key			{ get; private set; }
		public Type[] 				Signature 	{ get; private set; }
		public string				Category    { get; private set; }
		
		public KeyItem (string category, MappingType mappingType, string key, Type[] signature)
		{
			Category 	= category;
			MappingType = mappingType;
			Key   		= key;
			Signature   = signature;
		}
	}
}

