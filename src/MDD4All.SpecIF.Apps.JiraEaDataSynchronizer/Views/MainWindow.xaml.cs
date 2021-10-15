using MDD4All.EAFacade.ModelTree.ViewModels;
using MDD4All.EAFacade.DataAccess.Cached;
using System;
using System.Windows;
using EAAPI = EA;
using EADM = MDD4All.EAFacade.DataModels.Contracts;
using System.Windows.Controls;
using System.Windows.Media;

namespace MDD4All.SpecIF.Apps.JiraEaDataSynchronizer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EAAPI.Repository _originalRepo;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = InitializeViewModel();
        }

        private RootNodeViewModel InitializeViewModel()
        {
            RootNodeViewModel result = null;

            if(OpenEA())
            {
                CachedRepository cachedRepository = new CachedRepository(_originalRepo);

                cachedRepository.InitializePackageCache();

                EADM.Collection models = cachedRepository.Models;

                RootNodeViewModel rootNodeViewModel = new RootNodeViewModel();

                foreach(EADM.Package model in models)
                {
                    rootNodeViewModel.Children.Add(new PackageNodeViewModel(model));
                }

                result = rootNodeViewModel;
            }

            return result;
        }

        private bool OpenEA()
        {
            string progId = "EA.Repository";
            Type type = Type.GetTypeFromProgID(progId);
            EAAPI.Repository repository = Activator.CreateInstance(type) as EAAPI.Repository;

            bool openResult = repository.OpenFile(@"d:\alto_daten\EA\TestEA.eapx");

            if (openResult)
            {
                _originalRepo = repository;
                repository.ShowWindow(1);

            }

            return openResult;
        }

        private void ModelTreeView_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
                TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

                if (treeViewItem != null)
                {
                    treeViewItem.Focus();
                    e.Handled = true;
                }
            

            
        }

        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }
    }
}
