using ReferenceData.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ReferenceData
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var form = new MainWindow();
            // create a listener of form's events
            var presenter = new Presenter(form);
            form.ShowDialog();
        }
    }
}
