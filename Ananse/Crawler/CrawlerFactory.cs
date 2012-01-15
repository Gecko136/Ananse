// 
//  CrawlerFactory.cs
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
using System.Reflection;

namespace Ananse
{
	public class CrawlerFactory
	{
		private Dictionary<Type, Type> CrawlerBaseMap  		= new Dictionary<Type, Type>();	
		private Dictionary<Type, Type> CrawlerInterfaceMap 	= new Dictionary<Type, Type>();	

		
		public CrawlerFactory ()
		{
			Register (typeof(object), typeof(ObjectCrawler));
			Register (typeof(Input), typeof(InputCrawler));
		}	
		
		public void Register(Type tagType, Type crawlerType)
		{
			if (tagType == null) return;
			
			if (tagType.IsInterface)
				CrawlerInterfaceMap[tagType] = crawlerType;
			else 
				CrawlerBaseMap[tagType] = crawlerType;		
		}
		
		public Crawler FindCrawler(Crawler parent, object tag, Type tagType)
		{
			if (tag != null)
				if (tagType == null || !tagType.IsAssignableFrom(tag.GetType()))
					tagType = tag.GetType();
			
		    Type basetype 			= 	tagType;
			Type basecrawlertype 	= 	null;
			
			while(basetype != null)
			{
				if (CrawlerBaseMap.ContainsKey(basetype))
				{
					basecrawlertype = CrawlerBaseMap[basetype];
					break;
				}
				basetype = basetype.BaseType;
			}
			
			if (basecrawlertype != null)
				return Activator.CreateInstance(basecrawlertype, new object[] {this, tag, tagType}) as Crawler;
			
			return null;
		}
	}
}


