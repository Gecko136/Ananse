// 
//  ObjectCrawler.cs
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
using System.Reflection;
using System.Collections.Generic;

namespace Ananse
{
	public class ObjectCrawler : Crawler
	{
		private Dictionary<string, PropertyInfo>	Properties 	{ get; set; }
		
		public ObjectCrawler (object tag)
			: base(tag)
		{
			Properties = new Dictionary<string, PropertyInfo>();
			
			if (Tag != null)
				foreach (var propinfo in Tag.GetType().GetProperties())
					Properties.Add(propinfo.Name, propinfo);	
		}
	
		
		#region implemented abstract members of Ananse.Crawler
		public override Type[] Signature {
			get {
				return null;
			}
		}

		public override IEnumerator<string> Keys {
			get {
				foreach (var name in Properties.Keys)
				{
					yield return name;
				}
			}
		}

		public override Crawler this[string key] {
			get {
				object val = Properties[key].GetValue (Tag, new object[] {});
				return CrawlerFactory.FindCrawler(val);
			}
		}
		#endregion
	}
}

