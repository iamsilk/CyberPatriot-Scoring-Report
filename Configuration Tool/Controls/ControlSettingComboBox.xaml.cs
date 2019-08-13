using Configuration_Tool.Configuration;
using System;
using System.Windows.Controls;

namespace Configuration_Tool.Controls
{
    /// <summary>
    /// Interaction logic for ControlSettingComboBox.xaml
    /// </summary>
    public partial class ControlSettingComboBox : UserControl
    {
        public string Header
        {
            get
            {
                return txtHeader.Text;
            }
            set
            {
                txtHeader.Text = value;
            }
        }
        public bool IsScored
        {
            get
            {
                if (!checkBoxScored.IsChecked.HasValue) return false;

                return checkBoxScored.IsChecked.Value;
            }
            set
            {
                checkBoxScored.IsChecked = value;
            }
        }

        public string Delimiter = ";";

        public string items = "";
        public string Items
        {
            get
            {
                return items;
            }
            set
            {
                if (value != items)
                {
                    items = value;
                    string[] itemsList = ItemsList;

                    comboBox.Items.Clear();

                    foreach (string item in itemsList)
                    {
                        comboBox.Items.Add(item);
                    }

                    if (itemsList.Length > 0)
                    {
                        comboBox.SelectedIndex = 0;
                    }
                }
            }
        }

        public string[] ItemsList => items.Split(new string[] { Delimiter }, StringSplitOptions.None);

        public string SelectedItem
        {
            get { return (string)comboBox.SelectedItem; }
            set
            {
                comboBox.SelectedItem = value;
            }
        }
        

        public int SelectedIndex
        {
            get { return comboBox.SelectedIndex; }
            set
            {
                comboBox.SelectedIndex = value;
            }
        }

        public ScoredItem<int> GetScoredItem()
        {
            return new ScoredItem<int>(SelectedIndex, IsScored);
        }

        public void SetFromScoredItem(ScoredItem<int> scoredItem)
        {
            SelectedIndex = scoredItem.Value;
            IsScored = scoredItem.IsScored;
        }

        public ControlSettingComboBox()
        {
            InitializeComponent();
        }
    }
}
