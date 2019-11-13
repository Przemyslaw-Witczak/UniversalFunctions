using MojeFunkcjeUniwersalneNameSpace.Files;
using MVVMClasses;
using System;
using System.Collections.ObjectModel;

namespace FolderTreeView
{
    public class WpfTreeNodeModel : ModelBase
    {
        private object tag;
        private string name;

        public string Name
        {
            get => name;
            set
            {
                if (value!=name)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        //private IPathElement _pathElement;
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyPropertyChanged("IsSelected");
                if (_isSelected)
                {
                    SelectedIPathElement = (this.Tag as FolderElement);
                }

            }
        }

        private static FolderElement _selectedItem = null;
        // This is public get-only here but you could implement a public setter which
        // also selects the item.
        // Also this should be moved to an instance property on a VM for the whole tree, 
        // otherwise there will be conflicts for more than one tree.
        public static FolderElement SelectedIPathElement
        {
            get { return _selectedItem; }
            private set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    //NotifyPropertyChanged("Members");
                    OnSelectedItemChanged?.Invoke();
                }
            }
        }

        public static Action OnSelectedItemChanged { get; set; }
        //public static virtual void OnSelectedItemChanged()
        //{
        //    // Raise event / do other things
        //}



        private ObservableCollection<WpfTreeNodeModel> _nodes;

        public WpfTreeNodeModel(string name)
        {
            Name = name;
            Nodes = new ObservableCollection<WpfTreeNodeModel>();
        }

        public WpfTreeNodeModel() : this(string.Empty)
        {
        }

        public ObservableCollection<WpfTreeNodeModel> Nodes
        {
            get => _nodes;
            set
            {
                if (value!=_nodes)
                {
                    _nodes = value;
                    NotifyPropertyChanged("Members");
                }
            }
        }

        public object Tag { get => tag; set => tag = value; }
    }
}