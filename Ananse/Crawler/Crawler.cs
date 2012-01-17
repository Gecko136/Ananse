// 
//  Crawler.cs
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
	public abstract class Crawler
	{
		public Crawler (CrawlerFactory factory, object tag, Type tagType)
		{
			Stack 	= SingleLinkedList<StackItem>.Empty;
			Path  	= SingleLinkedList<CrawlerItem>.Empty;
			Factory = factory;
			Tag   	= tag;
			
			if (tag != null)
				if (tagType == null || !tagType.IsAssignableFrom(tag.GetType()))
				    tagType = tag.GetType();
			TagType = tagType;
		}
		
		public abstract Type[] Signature   				{ get; }
		
		public object Tag  								{ get; private set; }
		public Type	  TagType							{ get; private set; }
		
		public CrawlerFactory 					Factory { get; private set; }
		public SingleLinkedList<StackItem> 		Stack 	{ get; private set; }
		public SingleLinkedList<CrawlerItem>	Path  	{ get; private set; }
		
		public abstract IEnumerable<KeyItem> 	Keys 	{ get; }
		public abstract CrawlerItem this[KeyItem keyItem]{ get; }

	}
}

