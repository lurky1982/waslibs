﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

using AppStudio.Uwp.Commands;
using AppStudio.DataProviders.Bing;

namespace AppStudio.Uwp.Samples
{
    [SamplePage(Category = "DataProviders", Name = "Bing", Order = 70)]
    public sealed partial class BingPage : SamplePage
    {
        private const BingCountry DefaultBingCountry = BingCountry.UnitedStates;
        private const int DefaultMaxRecordsParam = 20;
        private const string DefaultBingQueryParam = "Windows App Studio";

        public BingPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        public override string Caption
        {
            get { return "Bing Data Provider"; }
        }

        #region DataProvider Config    
        public int MaxRecordsParam
        {
            get { return (int)GetValue(MaxRecordsParamProperty); }
            set { SetValue(MaxRecordsParamProperty, value); }
        }

        public static readonly DependencyProperty MaxRecordsParamProperty = DependencyProperty.Register("MaxRecordsParam", typeof(int), typeof(BingPage), new PropertyMetadata(DefaultMaxRecordsParam));


        public string BingQueryParam
        {
            get { return (string)GetValue(BingQueryParamProperty); }
            set { SetValue(BingQueryParamProperty, value); }
        }

        public static readonly DependencyProperty BingQueryParamProperty = DependencyProperty.Register("BingQueryParam", typeof(string), typeof(BingPage), new PropertyMetadata(DefaultBingQueryParam));



        public BingCountry BingCountrySelectedItem
        {
            get { return (BingCountry)GetValue(BingCountrySelectedItemProperty); }
            set { SetValue(BingCountrySelectedItemProperty, value); }
        }

        public static readonly DependencyProperty BingCountrySelectedItemProperty = DependencyProperty.Register("BingCountrySelectedItem", typeof(BingCountry), typeof(BingPage), new PropertyMetadata(DefaultBingCountry));

        #endregion

        #region Items
        public ObservableCollection<object> Items
        {
            get { return (ObservableCollection<object>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(ObservableCollection<object>), typeof(BingPage), new PropertyMetadata(null));
        #endregion      

        #region DataProviderRawData
        public string DataProviderRawData
        {
            get { return (string)GetValue(DataProviderRawDataProperty); }
            set { SetValue(DataProviderRawDataProperty, value); }
        }

        public static readonly DependencyProperty DataProviderRawDataProperty = DependencyProperty.Register("DataProviderRawData", typeof(string), typeof(BingPage), new PropertyMetadata(string.Empty));
        #endregion

        #region HasErrors
        public bool HasErrors
        {
            get { return (bool)GetValue(HasErrorsProperty); }
            set { SetValue(HasErrorsProperty, value); }
        }
        public static readonly DependencyProperty HasErrorsProperty = DependencyProperty.Register("HasErrors", typeof(bool), typeof(BingPage), new PropertyMetadata(false));
        #endregion

        #region NoItems
        public bool NoItems
        {
            get { return (bool)GetValue(NoItemsProperty); }
            set { SetValue(NoItemsProperty, value); }
        }
        public static readonly DependencyProperty NoItemsProperty = DependencyProperty.Register("NoItems", typeof(bool), typeof(BingPage), new PropertyMetadata(false));
        #endregion

        #region IsBusy
        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }
        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register("IsBusy", typeof(bool), typeof(BingPage), new PropertyMetadata(false));

        #endregion

        #region ICommands
        public ICommand RefreshDataCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Request();
                });
            }
        }

        public ICommand RestoreConfigCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    RestoreConfig();
                });
            }
        }

        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.Items = new ObservableCollection<object>();
            RestoreConfig();
            Request();

            base.OnNavigatedTo(e);
        }

        protected override void OnSettings()
        {
            AppShell.Current.Shell.ShowRightPane(new BingSettings() { DataContext = this });
        }

        private async void Request()
        {
            try
            {
                IsBusy = true;
                HasErrors = false;
                NoItems = false;
                DataProviderRawData = string.Empty;
                Items.Clear();
                var bingDataProvider = new BingDataProvider();
                var config = new BingDataConfig() { Query = BingQueryParam, Country = BingCountrySelectedItem };

                var rawParser = new RawParser();
                var rawData = await bingDataProvider.LoadDataAsync(config, MaxRecordsParam, rawParser);
                DataProviderRawData = rawData.FirstOrDefault()?.Raw;

                var items = await bingDataProvider.LoadDataAsync(config, MaxRecordsParam);

                NoItems = !items.Any();

                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                DataProviderRawData += ex.Message;
                DataProviderRawData += ex.StackTrace;
                HasErrors = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void RestoreConfig()
        {
            BingQueryParam = DefaultBingQueryParam;
            BingCountrySelectedItem = DefaultBingCountry;
            MaxRecordsParam = DefaultMaxRecordsParam;
        }
    }
}
