using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;

namespace YangpaH
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //integrated SQLite dll : unmanaged code
            AppDomain.CurrentDomain.AssemblyResolve += (sender1, args) =>
            {
                string resourceName = new System.Reflection.AssemblyName(args.Name).Name + ".dll";
                string resource = Array.Find(this.GetType().Assembly.GetManifestResourceNames(), element => element.EndsWith(resourceName));
                EmbeddedAssembly.Load(resource, resourceName);

                return EmbeddedAssembly.Get(args.Name);
            };
        }


        private void StartupHandler(object sender, System.Windows.StartupEventArgs e)
        {
            Elysium.Manager.Apply(this, Elysium.Theme.Light, new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0xFF, 0x5F, 0x7D)), SystemColors.WindowBrush);
        }
    }
}
