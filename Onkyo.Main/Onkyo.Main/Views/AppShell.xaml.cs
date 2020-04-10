using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Onkyo.Main.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        Dictionary<string, Type> routes = new Dictionary<string, Type>();
        public AppShell()
        {
            InitializeComponent();
            //RegisterRoutes();
            BindingContext = this;
        }

        void RegisterRoutes()
        {
            routes.Add("onkyo", typeof(ItemsPage));
            routes.Add("onkyodetails", typeof(DetailsPage));
            //routes.Add("beardetails", typeof(BearDetailPage));
            //routes.Add("catdetails", typeof(CatDetailPage));
            //routes.Add("dogdetails", typeof(DogDetailPage));
            //routes.Add("elephantdetails", typeof(ElephantDetailPage));

            foreach (var item in routes)
            {
                Routing.RegisterRoute(item.Key, item.Value);
            }
        }

        void OnNavigating(object sender, ShellNavigatingEventArgs e)
        {
            // Cancel any back navigation
            //if (e.Source == ShellNavigationSource.Pop)
            //{
            //    e.Cancel();
            //}
        }

        void OnNavigated(object sender, ShellNavigatedEventArgs e)
        {
        }
    }
}