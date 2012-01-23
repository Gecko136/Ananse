// 
//  Presenter.cs
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
using Ananse;

namespace AnanseGtk
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class Presenter : Gtk.Bin
	{
		public Ananse.Crawler Crawler { get; private set; }
		
		public Presenter ()
		{
			this.Build ();

			#region stack			
			// add columns to Stack Tree
			Gtk.TreeViewColumn stackColName = new Gtk.TreeViewColumn();
			stackColName.Title = "Name";
			
			Gtk.TreeViewColumn stackColValue= new Gtk.TreeViewColumn();
			stackColValue.Title = "Value";
			
			// add Renderer to Stack Tree
			Gtk.CellRendererText stackColNameRenderer = new Gtk.CellRendererText();
			stackColName.PackStart(stackColNameRenderer, true);
			
			Gtk.CellRendererText stackColValueRenderer = new Gtk.CellRendererText();
			stackColValue.PackStart(stackColValueRenderer, true);
			
			// set data functions
			stackColName.SetCellDataFunc(stackColNameRenderer, SetStackName);
			stackColValue.SetCellDataFunc(stackColValueRenderer, SetStackValue);
			
			stackTree.AppendColumn (stackColName);
			stackTree.AppendColumn (stackColValue);
			#endregion stack

			#region path			
			// add columns to Path Tree
			Gtk.TreeViewColumn pathColName = new Gtk.TreeViewColumn();
			pathColName.Title = "Name";
			
			Gtk.TreeViewColumn pathColValue= new Gtk.TreeViewColumn();
			pathColValue.Title = "Value";
			
			// add Renderer to Path Tree
			Gtk.CellRendererText pathColNameRenderer = new Gtk.CellRendererText();
			pathColName.PackStart(pathColNameRenderer, true);
			
			Gtk.CellRendererText pathColValueRenderer = new Gtk.CellRendererText();
			pathColValue.PackStart(pathColValueRenderer, true);
			
			// set data functions
			pathColName.SetCellDataFunc(pathColNameRenderer, SetPathName);
			pathColValue.SetCellDataFunc(pathColValueRenderer, SetPathValue);
			
			pathTree.AppendColumn (pathColName);
			pathTree.AppendColumn (pathColValue);
			#endregion path

			#region keys			
			// add columns to Stack Tree
			Gtk.TreeViewColumn keyColName = new Gtk.TreeViewColumn();
			keyColName.Title = "Name";
			
			Gtk.TreeViewColumn keyColValue= new Gtk.TreeViewColumn();
			keyColValue.Title = "Value";
			
			// add Renderer to Stack Tree
			Gtk.CellRendererText keyColNameRenderer = new Gtk.CellRendererText();
			keyColName.PackStart(keyColNameRenderer, true);
			
			Gtk.CellRendererText keyColValueRenderer = new Gtk.CellRendererText();
			keyColValue.PackStart(keyColValueRenderer, true);
			
			// set data functions
			keyColName.SetCellDataFunc(keyColNameRenderer, SetKeyName);
			keyColValue.SetCellDataFunc(keyColValueRenderer, SetKeyValue);
			
			keyTree.AppendColumn (keyColName);
			keyTree.AppendColumn (keyColValue);
			#endregion keys

		}
		
		public void SetCrawler (Ananse.Crawler crawler)
		{
			Crawler = crawler;

			SetStack(Crawler.Stack);
			SetPath(Crawler.Path);
			SetKey(Crawler.Keys);
		
			crawler1.SetObject(Crawler);
		}
			
			
		public void SetStack (SingleLinkedList<StackItem> stack)
		{
			Gtk.TreeStore stackTreeStore = new Gtk.TreeStore(typeof(string), typeof(string));
		
			Gtk.TreeIter iter = new Gtk.TreeIter();
			foreach (var stackItem in Crawler.Stack)
			{
				// todo: Crawler, Tag or TagType might be null
				iter = stackTreeStore.AppendValues (
					stackItem.Crawler.TagType.Name,
					stackItem.Crawler.Tag.ToString()
				);
			}			
			stackTree.Model = stackTreeStore;
		}

		public void SetPath (SingleLinkedList<CrawlerItem> path)
		{
			Gtk.TreeStore pathTreeStore = new Gtk.TreeStore(typeof(string), typeof(string));
		
			Gtk.TreeIter iter = new Gtk.TreeIter();
			foreach (var pathItem in Crawler.Path)
			{
				// todo: Crawler, Tag or TagType might be null
				iter = pathTreeStore.AppendValues (
					pathItem.Crawler.TagType.Name,
					pathItem.Crawler.Tag.ToString()
				);
			}			
			pathTree.Model = pathTreeStore;
		}

		public void SetKey (System.Collections.Generic.IEnumerable<KeyItem> keys)
		{
			Gtk.TreeStore keyTreeStore = new Gtk.TreeStore(typeof(string), typeof(string), typeof(KeyItem));
			
			Dictionary<MappingType, Dictionary<String, List<KeyItem>>> map = new Dictionary<MappingType, Dictionary<String, List<KeyItem>>>  ();
			foreach (var keyItem in keys)
			{
				MappingType mt = keyItem.MappingType;
				if(!map.ContainsKey(mt))
					map[mt] = new Dictionary<string, List<KeyItem>>();
				
				var mtd = map[mt];
				string category = keyItem.Category ?? "";
				if (!mtd.ContainsKey(category))
					mtd[category] = new List<KeyItem>();
				
				var list = mtd[category];
				list.Add(keyItem);
			}
			
			
			var mappingTypeList = new List<MappingType> (map.Keys);
			mappingTypeList.Sort();
			foreach (var mtItem in mappingTypeList)
			{
				Gtk.TreeIter mtIter = keyTreeStore.AppendValues (mtItem.ToString(), "", null);
				
				var categoryList = new List<string>(map[mtItem].Keys);
				categoryList.Sort();
				foreach (var catItem in categoryList)
				{
					Gtk.TreeIter catIter;
					if (string.IsNullOrEmpty(catItem))
						catIter = mtIter;
					else 
						catIter =  keyTreeStore.AppendValues (mtIter, catItem);
				
					var morphList = map[mtItem][catItem];
					morphList.Sort((x,y) => x.Key.CompareTo (y.Key));
					
					foreach (var keyItem in morphList)
					{
						keyTreeStore.AppendValues(mtIter, keyItem.Key, "", keyItem);
					}
				}
			}	
			keyTree.Model = keyTreeStore;
		}
		
		
		
		private void SetStackName (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			string name = (string) model.GetValue(iter,0);
			
			Gtk.CellRendererText renderer = cell as Gtk.CellRendererText;
			renderer.Text = name;
		}

		private void SetStackValue (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			//throw new NotImplem	entedException ();
		}

		public void SetPathName (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			string name = (string) model.GetValue(iter,0);
			
			Gtk.CellRendererText renderer = cell as Gtk.CellRendererText;
			renderer.Text = name;
		}
		
		private void SetPathValue (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			//throw new NotImplem	entedException ();
		}

		public void SetKeyName (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			string name = (string) model.GetValue(iter,0);
			
			Gtk.CellRendererText renderer = cell as Gtk.CellRendererText;
			renderer.Text = name;
		}
		
		private void SetKeyValue (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			string name = (string) model.GetValue(iter,1);
			
			Gtk.CellRendererText renderer = cell as Gtk.CellRendererText;
			renderer.Text = name;
		}

		protected void OnKeyTreeRowActivated (object o, Gtk.RowActivatedArgs args)
		{
			Gtk.TreeIter iter;
			
			keyTree.Model.GetIter(out iter, args.Path);
			KeyItem key = keyTree.Model.GetValue(iter,2) as KeyItem;
			
			if (key != null) SetCrawler(Crawler[key].Crawler);		
		}

		protected void OnStackTreeRowActivated (object o, Gtk.RowActivatedArgs args)
		{
			Gtk.TreeIter iter;
			
			stackTree.Model.GetIter(out iter, args.Path);
			KeyItem key = stackTree.Model.GetValue(iter,2) as KeyItem;
			
			if (key != null) SetCrawler(Crawler[key].Crawler);		
		}

		protected void OnPathTreeRowActivated (object o, Gtk.RowActivatedArgs args)
		{
			Gtk.TreeIter iter;

			pathTree.Model.GetIter(out iter, args.Path);
			KeyItem key = pathTree.Model.GetValue(iter,2) as KeyItem;
			
			if (key != null) SetCrawler(Crawler[key].Crawler);		
		}
	}
}

