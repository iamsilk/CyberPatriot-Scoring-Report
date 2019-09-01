using Configuration_Tool.Controls.Comparisons;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Configuration_Tool.Controls.CustomRegistry
{
    /// <summary>
    /// Interaction logic for ControlCustomRegistryValue.xaml
    /// </summary>
    public partial class ControlCustomRegistryValue : UserControl
    {
        public ControlCustomRegistryValue()
        {
            InitializeComponent();
        }

        public static List<string> HivesNames => new List<string>(Hives.Keys);

        public static Dictionary<string, RegistryHive> Hives = new Dictionary<string, RegistryHive>()
        {
            { "Classes Root", RegistryHive.ClassesRoot },
            { "Current User", RegistryHive.CurrentUser },
            { "Local Machine", RegistryHive.LocalMachine },
            { "Users", RegistryHive.Users },
            { "Performance Data", RegistryHive.PerformanceData },
            { "Current Config", RegistryHive.CurrentConfig },
            { "Dynamic Data", RegistryHive.DynData },
        };

        public RegistryHive Hive
        {
            get { return Hives[(string)comboBoxHive.SelectedItem]; }
            set
            {
                foreach (KeyValuePair<string, RegistryHive> pairs in Hives)
                {
                    if (pairs.Value == value)
                    {
                        comboBoxHive.SelectedItem = pairs.Key;
                        break;
                    }
                }
            }
        }

        public string KeyPath
        {
            get { return txtKeyPath.Text; }
            set
            {
                txtKeyPath.Text = value;
            }
        }

        public string ValueName
        {
            get { return txtValueName.Text; }
            set
            {
                txtValueName.Text = value;
            }
        }

        public bool ValueEquals
        {
            get
            {
                if (!checkBoxMatches.IsChecked.HasValue)
                    return false;

                return checkBoxMatches.IsChecked.Value;
            }
            set
            {
                checkBoxMatches.IsChecked = value;
            }
        }

        public ItemCollection Comparisons => itemsComparisons.Items;

        public string CustomOutput
        {
            get { return txtCustomOutput.Text; }
            set
            {
                txtCustomOutput.Text = value;
            }
        }

        public bool IsScored
        {
            get
            {
                if (!checkBoxIsScored.IsChecked.HasValue)
                    return false;

                return checkBoxIsScored.IsChecked.Value;
            }
            set
            {
                checkBoxIsScored.IsChecked = value;
            }
        }

        public static ControlCustomRegistryValue Parse(BinaryReader reader)
        {
            // Create instance of policy storage
            ControlCustomRegistryValue registryKey = new ControlCustomRegistryValue();

            // Read registry identifing fields
            registryKey.Hive = (RegistryHive)reader.ReadInt32();
            registryKey.KeyPath = reader.ReadString();
            registryKey.ValueName = reader.ReadString();

            // Read comparison values
            registryKey.ValueEquals = reader.ReadBoolean();

            int comparisonCount = reader.ReadInt32();

            for (int i = 0; i < comparisonCount; i++)
            {
                IComparison comparison = null;

                EComparison type = (EComparison)reader.ReadInt32();
                switch (type)
                {
                    case EComparison.Simple:
                        comparison = new ControlComparisonSimple();
                        break;
                    case EComparison.Regex:
                        comparison = new ControlComparisonRegex();
                        break;
                    case EComparison.Range:
                        comparison = new ControlComparisonRange();
                        break;
                }

                comparison.Load(reader);

                // Add comparison to list
                registryKey.Comparisons.Add(comparison);
            }

            // Read scoring values
            registryKey.CustomOutput = reader.ReadString();
            registryKey.IsScored = reader.ReadBoolean();

            return registryKey;
        }

        public void Write(BinaryWriter writer)
        {
            // Write properties
            writer.Write((Int32)Hive);
            writer.Write(KeyPath);
            writer.Write(ValueName);
            writer.Write(ValueEquals);

            writer.Write(Comparisons.Count);
            foreach (IComparison comparison in Comparisons)
            {
                writer.Write((Int32)comparison.Type);
                comparison.Write(writer);
            }

            writer.Write(CustomOutput);
            writer.Write(IsScored);
        }

        private void btnAddSimple_Click(object sender, RoutedEventArgs e)
        {
            itemsComparisons.Items.Add(new ControlComparisonSimple());
        }

        private void btnAddRegex_Click(object sender, RoutedEventArgs e)
        {
            itemsComparisons.Items.Add(new ControlComparisonRegex());
        }

        private void btnAddRange_Click(object sender, RoutedEventArgs e)
        {
            itemsComparisons.Items.Add(new ControlComparisonRange());
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            // Get items control parent
            ItemsControl parent = (ItemsControl)this.Parent;

            // Remove self from items control
            parent.Items.Remove(this);
        }
    }
}
