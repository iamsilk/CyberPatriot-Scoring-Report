using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Configuration_Tool.Controls
{
    public static class BindingUpdater
    {
        /* Fix for an issue where text in a binded text box
         * would be modified, but if focus was not lost before
         * saving/clicking the menu item (File) the binding would
         * not update, and the latest modified property would
         * not be saved. Thanks to the following link for an explanation
         * and functions to help fix this.
         * https://blogs.msdn.microsoft.com/ryantrem/2010/04/08/globally-updating-binding-sources-in-wpf/ */

        public static IEnumerable<DependencyObject> EnumerateVisualChildren(this DependencyObject dependencyObject)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
            {
                yield return VisualTreeHelper.GetChild(dependencyObject, i);
            }
        }


        public static IEnumerable<DependencyObject> EnumerateVisualDescendents(this DependencyObject dependencyObject)
        {
            yield return dependencyObject;

            foreach (DependencyObject child in dependencyObject.EnumerateVisualChildren())
            {
                foreach (DependencyObject descendent in child.EnumerateVisualDescendents())
                {
                    yield return descendent;
                }
            }
        }

        public static void UpdateBindingSources(this DependencyObject dependencyObject)
        {
            foreach (DependencyObject element in dependencyObject.EnumerateVisualDescendents())
            {
                LocalValueEnumerator localValueEnumerator = element.GetLocalValueEnumerator();
                while (localValueEnumerator.MoveNext())
                {
                    BindingExpressionBase bindingExpression = BindingOperations.GetBindingExpressionBase(element, localValueEnumerator.Current.Property);
                    if (bindingExpression != null)
                    {
                        bindingExpression.UpdateSource();
                    }
                }
            }
        }
    }
}
