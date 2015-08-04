using System.Windows;
using System;
using System.Collections.Generic;
using ReferenceData.DAL.Model;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Diagnostics;
using System.Threading;

namespace ReferenceData
{
    public partial class MainWindow : Window
    {
        List<Country> countries;
        List<Subdivision> subdivisions;
        List<Location> locations;
        List<UserData> users;
        
        // pairs of <CountryId, Subdivisions>
        Dictionary<int, List<Subdivision>> subdivisionsByCountry = new Dictionary<int, List<Subdivision>>();
        // pairs of <SubdivisionId, Locations>
        Dictionary<int, List<Location>> locationsBySubdivision = new Dictionary<int, List<Location>>();

        // UserData's id, fname, lname, countryId, subdivisionId, locationId
        private List<BindingExpression> determinantUserExpressions = new List<BindingExpression>();
        // Contains binding expressions for all UserData properties
        private BindingGroup UserDetail;

        public MainWindow()
        {
            InitializeComponent();

            GroupBindingExpressions();

            countries = new List<Country> { EmptyItem<Country>.Create() };
            
            subdivisionsByCountry.Add(ReferenceDataConsts.NullId, 
                new List<Subdivision> { EmptyItem<Subdivision>.Create() });

            locationsBySubdivision.Add(ReferenceDataConsts.NullId, 
                new List<Location> { EmptyItem<Location>.Create() });

            this.ContentRendered += MainWindow_ContentRendered;
        }

        private void GroupBindingExpressions()
        {
            UserDetail = new BindingGroup();
            var txts = new List<TextBox> { TxtFirstName, TxtSecondName };
            var txtDP = TextBox.TextProperty;
            var allExps = UserDetail.BindingExpressions;
            txts.ForEach(txt =>
            {
                var exp = txt.GetBindingExpression(txtDP);
                allExps.Add(exp);
                determinantUserExpressions.Add(exp);
            });

            var combos = new List<ComboBox> { cbCountry, cbSubdivision, cbLocation };
            var valDP = ComboBox.SelectedValueProperty;
            txtDP = ComboBox.TextProperty;
            combos.ForEach(cb =>
            {
                var exp = cb.GetBindingExpression(valDP);
                allExps.Add(exp);
                determinantUserExpressions.Add(exp);

                exp = cb.GetBindingExpression(txtDP);
                allExps.Add(exp);
            });
        }

        void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            var loadDataThread = new Thread(() =>
            {
                // load all neccessary data from database
                OnLoadCountries();
                OnLoadSubdivisions();
                OnLoadLocations();
                OnLoadUserList();

                Dispatcher.Invoke(() => SetSources());
            });

            loadDataThread.IsBackground = true;
            loadDataThread.Start();      
        }

        private void SetSources()
        {
            cbCountry.ItemsSource = countries;
            dgUsers.ItemsSource = users;
            gButtons.IsEnabled = true;
            gbUserDetails.IsEnabled = true;
            pbBusyIndicator.Visibility = Visibility.Hidden;
        }

        private void filterSubdivisions(int countryId, bool defaultSelect)
        {
            cbSubdivision.ItemsSource = subdivisionsByCountry[countryId];
            if (!defaultSelect) return;

            SelectFirstItem(cbSubdivision);
        }

        private void filterLocations(int subId, bool defaultSelect)
        {
            cbLocation.ItemsSource = locationsBySubdivision[subId];
            if (!defaultSelect) return;

            SelectFirstItem(cbLocation);
        }

        private void SelectFirstItem(ComboBox cb)
        {
            cb.SelectedIndex = 0;
        }

        private void cbCountry_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCountry.SelectedItem == null) return;
            
            var country = cbCountry.SelectedItem as Country;
            Debug.Assert(country != null);
            // if this change is caused by switching country, select default item 
            filterSubdivisions(country.Id, cbCountry.IsDropDownOpen);
        }

        private void cbSubdivision_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbSubdivision.SelectedItem == null) return;

            var sub = cbSubdivision.SelectedItem as Subdivision;
            Debug.Assert(sub != null);
            // if this change is caused by switching country or subdivision, 
            // select default item 
            filterLocations(sub.Id, cbCountry.IsDropDownOpen || cbSubdivision.IsDropDownOpen);
        }

        private void BtnNewClick(object sender, RoutedEventArgs e)
        {
            if (!UserDetail.ValidateWithoutUpdate()) return;

            var newItemData = new UserData(-1, UserDetail);

            try
            {
                // add the user into database
                OnAddUser((User)newItemData);

                users.Add(newItemData);
                dgUsers.Items.Refresh();
                // select the new-added item
                dgUsers.SelectedItem = newItemData;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        // check if source needs to be updated
        private bool HasDifferentValue(BindingExpression b)
        {
            var originObj = b.ResolvedSource;
            var propName = b.ResolvedSourcePropertyName;
            
            var originValue = originObj.GetType().GetProperty(propName).GetValue(originObj);
            var currentValue = b.Target.GetValue(b.TargetProperty);
            return originValue != currentValue;
        }

        private void BtnSaveClick(object sender, RoutedEventArgs e)
        {
            // check if there is an item to update
            if (determinantUserExpressions.Any(b => b.ResolvedSource == null))
            {
                MessageBox.Show("Firstly, please, specify an element you would like to update", "No selected element");
                return;
            }

            // check if there is any change to submit
            if (!determinantUserExpressions.Any(b => HasDifferentValue(b)))
            {
                MessageBox.Show("Saving will not be done", "No made changes");
                return;
            }


            if (!UserDetail.UpdateSources()) return;

            dgUsers.Items.Refresh();
            var selectedItem = dgUsers.SelectedItem as UserData;
            try
            {
                // update the user in database
                OnChangeUser((User)selectedItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void BtnCancelClick(object sender, RoutedEventArgs e)
        {
            // reset all unsaved changes
            determinantUserExpressions.ForEach(b => b.UpdateTarget());
        }
    }
}
