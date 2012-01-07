// 
//  Crawler.cs
//  
//  Author:
//       Gunnar Quehl <github@gunnar-quehl.de>
// 
//  Copyright (c) 2011 gecko
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
using System.Collections;

namespace Crawler
{
	internal struct CollItems
	{
		public CollItems(ICollection collection, int start, int stop) : this()
		{
			Collection = collection;
			StartIndex = start;
			StopIndex = stop;
		}
		
		public ICollection Collection {get; private set; }
		public int StartIndex {get; private set; }
		public int StopIndex {get; private set; }
		public override string ToString ()
		{
			return string.Format ("[CollItems: Collection={0}, from={1}, to={2}]", Collection, StartIndex, StopIndex);
		}
	}

	internal struct DicItems
	{
		public DicItems(IDictionary dictionary, int start, int stop) : this()
		{
			Dictionary = dictionary;
			StartIndex = start;
			StopIndex = stop;
		}
		
		public IDictionary Dictionary {get; private set; }
		public int StartIndex {get; private set; }
		public int StopIndex {get; private set; }
		public override string ToString ()
		{
			return string.Format ("[CollItems: Collection={0}, from={1}, to={2}]", Dictionary, StartIndex, StopIndex);
		}
	}

	public struct Tag
	{
		public Tag(object orgvalue, object value, System.Reflection.PropertyInfo propinfo) : this()
		{
			OriginalValue = orgvalue;
			Value = value;
			PropInfo = propinfo;
			
			CanWrite = false;
			if( PropInfo != null
			&&	PropInfo.GetSetMethod() != null
			&& (	PropInfo.PropertyType.IsPrimitive 
				|| 	PropInfo.PropertyType == typeof(string)) )
				CanWrite = true;
		}
		
		public object OriginalValue 					{ get; set; }
		public object Value 							{ get; set; }
		public System.Reflection.PropertyInfo PropInfo 	{ get; private set; }
		public bool CanWrite 							{ get; private set; }
	}
	
	[System.ComponentModel.ToolboxItem(true)]
	public partial class Crawler : Gtk.Bin
	{
		public bool RulesHint
		{
			get { return crawlerView.RulesHint;} 
			set { crawlerView.RulesHint = value;}
		}
		
		
		public bool Editable
		{
			get { return cicon.Visible;  }
			set { cicon.Visible = value; }
		}
		
		public void SetValueOnly()
		{
			cicon.Visible = false; 
			cprop.Visible = false;
			ctype.Visible = false;
		}
		
		private Gtk.TreeViewColumn cicon {get; set; }
		private Gtk.TreeViewColumn cprop {get; set; }
		private Gtk.TreeViewColumn ctype {get; set; }
				
		public bool ZoomIn    {get; set;}		
		public Object CurrentTag { get; private set; }
		
		private Gtk.TreeStore Store;
		
		public Crawler ()
		{	
			this.Build ();
	
			cprop = new Gtk.TreeViewColumn();
			cprop.Title = "Property";

			ctype = new Gtk.TreeViewColumn();
			ctype.Title = "Type";
	
			cicon = new Gtk.TreeViewColumn();
			cicon.Title = "E";

			Gtk.TreeViewColumn cvalue = new Gtk.TreeViewColumn();
			cvalue.Title = "Value";
	
			Gtk.CellRendererText propCell = new Gtk.CellRendererText();
			cprop.PackStart(propCell, true);
	
			Gtk.CellRendererText typeCell = new Gtk.CellRendererText();
			ctype.PackStart(typeCell, true);

			Gtk.CellRendererPixbuf iconCell = new Gtk.CellRendererPixbuf();
			cicon.PackStart(iconCell, true);

			Gtk.CellRendererText valueCell = new Gtk.CellRendererText();
			cvalue.PackStart(valueCell, true);
			
			cprop.AddAttribute(propCell, "text", 1);
			ctype.AddAttribute(typeCell, "text", 2);
			//cvalue.AddAttribute(valueCell, "text", 3);
			
			cicon.SetCellDataFunc(iconCell, ShowIcon);
			cvalue.SetCellDataFunc(valueCell, ShowValue);
			
			Store = new Gtk.TreeStore(typeof(Tag), typeof (string), typeof(string), typeof(string));
			crawlerView.RowCollapsed += HandleCrawlerViewRowCollapsed;
			crawlerView.TestExpandRow += HandleCrawlerViewTestExpandRow;
			crawlerView.RowActivated += HandleCrawlerViewRowActivated;
			crawlerView.Selection.Changed += HandleCrawlerViewSelectionChanged;
				
			crawlerView.Model = Store;
			
			crawlerView.AppendColumn(cprop);
			crawlerView.AppendColumn(ctype);
			crawlerView.AppendColumn(cicon);
			crawlerView.AppendColumn(cvalue);
		}

		void HandleCrawlerViewSelectionChanged (object sender, EventArgs e)
		{
			if (crawlerView.Selection.CountSelectedRows() != 1)
			{
				this.CurrentTag = null;
				return;
			}
		
			Gtk.TreeIter iter;
			crawlerView.Selection.GetSelected(out iter);
			
			this.CurrentTag = ((Tag)Store.GetValue(iter, 0)).Value;
		}

	
		public void ShowValue (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			Tag val = (Tag) Store.GetValue(iter, 0);
			
			Gtk.CellRendererText textRenderer = cell as Gtk.CellRendererText;
			textRenderer.Text = (string)Store.GetValue (iter,3);
		}

		public void ShowIcon (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			Tag val = (Tag) Store.GetValue(iter, 0);
			
			Gtk.CellRendererPixbuf pixRenderer = cell as Gtk.CellRendererPixbuf;
			//pixRenderer.Pixbuf = new Gdk.Pixbuf();
			
			if (!val.CanWrite)
			{
				pixRenderer.Pixbuf = null;
			}
			else
			{
				pixRenderer.Pixbuf = new Gdk.Pixbuf("Icons/Actions-edit-select-all-icon.png");
			}
		}

		void HandleCrawlerViewRowActivated (object o, Gtk.RowActivatedArgs args)
		{

			Gtk.TreeIter iter;
			Store.GetIter(out iter, args.Path);
			Tag tag = (Tag)Store.GetValue (iter,0);

			if (args.Column.Title == "Value" && (tag.Value ==null || !(tag.Value.GetType() == typeof(String))))
				return;
			if (args.Column.Title == "Value")
			{
				StringDialog sd = new StringDialog(true);
				sd.Text = (tag.Value ?? "(null)").ToString() ;
				sd.ShowNow();
				sd.Run ();
				sd.Destroy();
				return;
			}
			
			if (args.Column.Title == "E")
			{
				
				
				if (!tag.CanWrite) return;
				
				StringDialog sd = new StringDialog(false);
				sd.Text = (tag.Value ?? "(null)").ToString() ;
				sd.ShowNow();
			    int pressed = sd.Run();
				string result = sd.Text;
				sd.Destroy();
				
				if ((Gtk.ResponseType) pressed == Gtk.ResponseType.Ok)
				{
					object newval;
					
					if (tag.PropInfo.PropertyType == typeof(string))
						newval = result;
					else 
					{	try 
						{
							//get parse method
							System.Reflection.MethodInfo parse = tag.PropInfo.PropertyType.GetMethod("Parse", new Type[]{typeof(string)});
							newval =  parse.Invoke (tag.OriginalValue, new object[]{result});
						}
						catch
						{
							return;
						}
					}	
					
					try {
					// set original property
					tag.PropInfo.SetValue(tag.OriginalValue, newval, null);
					tag.Value = tag.PropInfo.GetValue(tag.OriginalValue, System.Type.EmptyTypes);
					SetObject(iter, tag.Value);
					}
					catch 
					{
					}
				}
				
				return;
			}
			if (ZoomIn)	SetObject (((Tag)Store.GetValue(iter,0)).Value);
		}

		void HandleCrawlerViewTestExpandRow (object o, Gtk.TestExpandRowArgs args)
		{
			Gtk.TreeIter iter = args.Iter;
			Tag tag = (Tag)Store.GetValue (iter,0);
			object obj = tag.Value;
			
			RemoveAllChildren (iter);
			
			if (obj == null) return;
			
			// check for IDictionary
			if (obj is IDictionary  || obj is DicItems)
			{
				ExpandDictionary (iter, obj);
				return;
			}
			
			// check for ICollections
			Type gIDictionary = null;
			System.Type objtype = obj.GetType();

			foreach (var i in objtype.GetInterfaces ())
			    if (i.IsGenericType && i.GetGenericTypeDefinition () == typeof(System.Collections.Generic.IDictionary<,>))
				{
					gIDictionary = i;
					break;
				}
			
			if (gIDictionary != null) 
			{
				ExpandGenericIDictionary (iter, gIDictionary, obj);
				return;
			}
			
			// check for ICollections
			if (obj is ICollection  || obj is CollItems)
			{
				ExpandCollection (iter, obj);
				return;
			}
			
			
			
			// if its plain object follow public properties
			
			foreach (System.Reflection.PropertyInfo prop in obj.GetType().GetProperties(
				System.Reflection.BindingFlags.Instance
			|	System.Reflection.BindingFlags.Public
			|   System.Reflection.BindingFlags.GetProperty))
			{
				string name = prop.Name;
				string type = prop.PropertyType.FullName;
				
				var attributes = prop.GetCustomAttributes(typeof(CrawlerDisplayAttribute), true);
				if (attributes.Length==1)
					if (!((CrawlerDisplayAttribute) attributes[0]).Show)
						continue;
				
				try {
					object value = prop.GetValue (obj, System.Type.EmptyTypes);
					Gtk.TreeIter citer = Store.AppendValues(iter, new Tag(obj, null, prop), name, type, null);
					SetObject(citer, value);
				}
				catch(Exception ex)
				{
					Gtk.TreeIter citer = Store.AppendValues(iter, new Tag(obj, null, null), name, type, null);
					SetObject (citer, ex);
				}
			}
		
		}


		void ExpandCollection (Gtk.TreeIter iter, object obj)
		{
			const int maxshow = 10;
			int start = 0;
			int stop  = 0;
			
			ICollection coll;
			
			if (obj is ICollection)
			{
				coll = obj as ICollection;
				start = 0;
				stop  = coll.Count;
			}
			else 
			{
				CollItems colli = (CollItems) obj;
				coll = colli.Collection;
				start = colli.StartIndex;
				stop = colli.StopIndex;
			}
			
			int count = stop - start;
			int chunk = 1;
			
			if (count > maxshow)
			{
				chunk = Math.Max (maxshow, (int)Math.Ceiling((double)count / (double)maxshow));					
			}
			
			IEnumerator enumerator = coll.GetEnumerator();
			enumerator.Reset();
			enumerator.MoveNext();
			for (int i = 0; i < start; i++) 
				enumerator.MoveNext();
			
			int current = start;
			while (current < stop)
			{
				if (chunk == 1)
				{
					string name = string.Format("[{0}]", current);
					
					try 
					{
						object value = enumerator.Current;
						Gtk.TreeIter citer = Store.AppendValues(iter, new Tag(obj, null, null), name, null, null);
						SetObject(citer, value);
					}
					catch(Exception ex)
					{
						Gtk.TreeIter citer = Store.AppendValues(iter, new Tag(obj, null, null), name, null, null);
						SetObject (citer, ex);
					}
				
					current = current + chunk;
					enumerator.MoveNext();
				}
				else
				{
					int cstart = current;
					int cstop =  Math.Min (current + chunk-1, stop-1);
					string name = string.Format("[{0} - {1}]", cstart , cstop);
					
					try 
					{
						object value = new CollItems(coll, cstart, cstop+1);
						Gtk.TreeIter citer = Store.AppendValues(iter, new Tag(obj, null, null), name, null, null);
						SetObject(citer, value);
					}
					catch(Exception ex)
					{
						Gtk.TreeIter citer = Store.AppendValues(iter, new Tag(obj, null, null), name, null, null);
						SetObject (citer, ex);
					}
					
					current = current + chunk;
				}
		
			}
		}

		void ExpandGenericIDictionary(Gtk.TreeIter iter, Type gIDictionary, object obj)
		{
			IEnumerable myCollection = obj as IEnumerable;
			int counter = 0;
			
			
			foreach (var i in myCollection)
			{
					try 
					{
						object value = i;
						Gtk.TreeIter citer = Store.AppendValues(iter, new Tag(obj, null, null), null, null, null);
						SetObject(citer, value);
					}
					catch(Exception ex)
					{
						Gtk.TreeIter citer = Store.AppendValues(iter, new Tag(obj, null, null), null, null, null);
						SetObject (citer, ex);
					}
			}
		}

		void ExpandDictionary (Gtk.TreeIter iter, object obj)
		{
			const int maxshow = 10;
			int start = 0;
			int stop  = 0;
			
			IDictionary dic;
			
			if (obj is IDictionary)
			{
				dic = obj as IDictionary;
				start = 0;
				stop  = dic.Count;
			}
			else 
			{
				DicItems dici = (DicItems) obj;
				dic = dici.Dictionary;
				start = dici.StartIndex;
				stop = dici.StopIndex;
			}
			
			int count = stop - start;
			int chunk = 1;
			
			if (count > maxshow)
			{
				chunk = Math.Max (maxshow, (int)Math.Ceiling((double)count / (double)maxshow));					
			}
			
			IEnumerator enumerator = dic.GetEnumerator();
			enumerator.Reset();
			enumerator.MoveNext();
			for (int i = 0; i < start; i++) 
				enumerator.MoveNext();
			
			int current = start;
			while (current < stop)
			{
				if (chunk == 1)
				{
					string name = string.Format("[{0}]", current);
					
					try 
					{
						object value = enumerator.Current;
						Gtk.TreeIter citer = Store.AppendValues(iter, new Tag(obj, null, null), name, null, null);
						SetObject(citer, value);
					}
					catch(Exception ex)
					{
						Gtk.TreeIter citer = Store.AppendValues(iter, new Tag(obj, null, null), name, null, null);
						SetObject (citer, ex);
					}
				
					current = current + chunk;
					enumerator.MoveNext();
				}
				else
				{
					int cstart = current;
					int cstop =  Math.Min (current + chunk-1, stop-1);
					string name = string.Format("[{0} - {1}]", cstart , cstop);
					
					try 
					{
						object value = new CollItems(dic, cstart, cstop+1);
						Gtk.TreeIter citer = Store.AppendValues(iter, new Tag(obj, null, null), name, null, null);
						SetObject(citer, value);
					}
					catch(Exception ex)
					{
						Gtk.TreeIter citer = Store.AppendValues(iter, new Tag(obj, null, null), name, null, null);
						SetObject (citer, ex);
					}
					
					current = current + chunk;
				}
		
			}
		}

		void HandleCrawlerViewRowCollapsed (object o, Gtk.RowCollapsedArgs args)
		{
			RemoveAllChildren(args.Iter);
			object obj = ((Tag)Store.GetValue (args.Iter, 0)).Value;
		    SetObject(args.Iter, obj);		
		}

		void RemoveAllChildren (Gtk.TreeIter iter)
		{
			Gtk.TreeIter citer;
			
			while (Store.IterHasChild(iter))
			{
				Store.IterChildren(out citer, iter);
				Store.Remove (ref citer);
			}	
		}
		
		public void SetExtender (Gtk.TreeIter iter)
		{
			Store.AppendValues(iter, new Tag(null, null, null), null, null,null);
			
			Gtk.TreeIter i;
			Store.GetIterFirst(out i);
		}
    
		public void SetObject(object obj)
		{
			Store.Clear ();
			Gtk.TreeIter iter = Store.AppendValues(new Tag(null, null, null), null, null, null );
			SetObject(iter, obj);
		}

		public void SetObject (Gtk.TreeIter iter, object obj)
		{
			Tag val = (Tag)Store.GetValue (iter, 0);
			val.Value = obj;
			
			Store.SetValue (iter, 0, val);
			if (obj == null)
			{
				Store.SetValue (iter, 2, "Object");
				Store.SetValue (iter, 3, "(null)");
				return;
			}
			
			Store.SetValue (iter, 2, obj.GetType().Name);
			if (obj is String || obj.GetType().IsPrimitive)
			{
				string value = obj.ToString();
				if(obj is String && value.Length > 50) value = value.Substring(0,50) + "...";
				if(obj is String) value = value.Replace(Environment.NewLine, "\\n");
				
				Store.SetValue (iter, 3, value);
				return;
			}
		
			if (obj is System.Collections.ICollection)
			{
				System.Collections.ICollection coll = obj as System.Collections.ICollection;
				string value = coll.Count.ToString();
				
				Store.SetValue (iter, 3, value);
				if (coll.Count > 0)
					SetExtender(iter);
				return;
			}
			
			// check for Key : String => Value
			
			System.Reflection.PropertyInfo keyInfo = obj.GetType().GetProperty("Key", typeof(string));
			System.Reflection.PropertyInfo valInfo = obj.GetType().GetProperty("Value");
			
			if (keyInfo != null && valInfo != null)
			{
				object valValue = valInfo.GetValue (obj,new object[]{});
				object keyValue = keyInfo.GetValue (obj,new object[]{});
				
			    Store.SetValue (iter,1, keyValue.ToString());
				Store.SetValue (iter,2, valValue.GetType().Name);
				Store.SetValue (iter, 0, new Tag(obj, valValue, valInfo));
				SetObject(iter, valValue);
				return;
			}
			
			//Store.SetValue (iter,3, obj.GetType().FullName);
			SetExtender (iter);
		}	
	}
}

