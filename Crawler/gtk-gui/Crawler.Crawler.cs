
// This file has been generated by the GUI designer. Do not modify.
namespace Crawler
{
	public partial class Crawler
	{
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView crawlerView;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Crawler.Crawler
			global::Stetic.BinContainer.Attach (this);
			this.Name = "Crawler.Crawler";
			// Container child Crawler.Crawler.Gtk.Container+ContainerChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.VscrollbarPolicy = ((global::Gtk.PolicyType)(0));
			this.GtkScrolledWindow.HscrollbarPolicy = ((global::Gtk.PolicyType)(0));
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.crawlerView = new global::Gtk.TreeView ();
			this.crawlerView.CanFocus = true;
			this.crawlerView.Name = "crawlerView";
			this.crawlerView.EnableSearch = false;
			this.GtkScrolledWindow.Add (this.crawlerView);
			this.Add (this.GtkScrolledWindow);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
		}
	}
}