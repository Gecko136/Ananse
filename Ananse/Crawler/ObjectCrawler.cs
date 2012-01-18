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
		private Dictionary<string, MethodInfo>      Methods 	{ get; set; }
		
		public ObjectCrawler (CrawlerFactory factory, object tag, Type tagType)
			: base(factory, tag, tagType)
		{
			Properties = new Dictionary<string, PropertyInfo>();
			Methods    = new Dictionary<string, MethodInfo>();
			
			if (Tag == null) return;
			
			foreach (var propinfo in Tag.GetType().GetProperties())
				Properties.Add(propinfo.Name, propinfo);
		
			foreach (var methinfo in Tag.GetType().GetMethods())
			{
				ParameterInfo[] parameters = methinfo.GetParameters();
					
				Type[] types = new Type[parameters.Length];
				for(int i = 0; i<types.Length; i++)
					types[i] = parameters[i].ParameterType;
				
				List<Type> stackTypes = new List<Type>();
				foreach(var si in Stack)
					stackTypes.Add(si.Crawler.TagType);
				
				// check if the parameter type of methinfo are compatible with the stacktypes
				if (stackTypes.Count < types.Length) continue; // not compatible
				for (int i = 0; i <types.Length; i++)
					if (!types[i].IsAssignableFrom(stackTypes[i]))
					    continue; // not compatible
				// compatible
				Methods[methinfo.Name] = methinfo;		
			}
		}
	
		
		#region implemented abstract members of Ananse.Crawler
		public override Type[] Signature {
			get {
				return null;
			}
		}

		public override IEnumerable<KeyItem> Keys {
			get {
				foreach (var name in Properties.Keys)
				{
					yield return new KeyItem(MappingType.Property, name, Type.EmptyTypes);
				}
				
				foreach (var name in Methods.Keys)
				{
					MethodInfo mi = Methods[name];
					ParameterInfo[] parameters = mi.GetParameters();
					
					Type[] types = new Type[parameters.Length];
					for(int i = 0; i<types.Length; i++)
						types[i] = parameters[i].ParameterType;
					
					yield return new KeyItem(MappingType.Method, name, types);
				}
			}
		}

		public override CrawlerItem this[KeyItem keyItem] {
			get {
				if (Properties.ContainsKey(keyItem.Key))
				{
					var 	propInfo 	= Properties[keyItem.Key];
			
					object 	val;
					try {
						val = propInfo.GetValue (Tag, new object[] {});
					}
					catch(Exception ex)
					{
						Crawler 	next 		= Factory.FindCrawler(this, ex, ex.GetType());
						CrawlerItem nextItem 	= new CrawlerItem(next, MappingType.Property, keyItem.Signature);
						return nextItem;
					}
					
					if (Factory != null)
					{
						Crawler 	next 		= Factory.FindCrawler(this, val, propInfo.PropertyType);
						CrawlerItem nextItem 	= new CrawlerItem(next, MappingType.Property, keyItem.Signature);
						return nextItem;
					}
					
					else
					return new CrawlerItem(null, MappingType.Property, null);
				}
				
				if (Methods.ContainsKey(keyItem.Key))
				{
					var 			methInfo 	= Methods[keyItem.Key];
					List<object> 	parameter 	= new List<object>();
					var				currstack   = Stack;
					for(int i=0; i<keyItem.Signature.Length; i++)
					{
						parameter.Add(currstack.Head.Crawler.Tag);
						currstack = currstack.Tail;
					}
					
					object 			val;

					try {
						val 		= methInfo.Invoke(Tag, parameter.ToArray());
					}
					catch(Exception ex)
					{
						Crawler 	next 		= Factory.FindCrawler(this, ex, ex.GetType());
						CrawlerItem nextItem 	= new CrawlerItem(next, MappingType.Property, keyItem.Signature);
						return nextItem;
					}
					
					if (Factory != null)
					{
						Crawler 	next 		= Factory.FindCrawler(this, val, methInfo.ReturnType );
						CrawlerItem nextItem 	= new CrawlerItem(next, MappingType.Property, keyItem.Signature);
						return nextItem;
					}
					
					else
					return new CrawlerItem(null, MappingType.Property, null);
				}				
				return null;
			}
		}
		
		#endregion

		// temporary hack because the GtkCrawler does not handle IEnumables correctly
		public List<KeyItem> KeyCollection
		{
			get{ 
				return new List<KeyItem>(Keys);
			}
		}
		
	}
}

