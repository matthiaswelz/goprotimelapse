using System;
using System.Linq;
using System.Windows;

namespace GoProTimelapse.Extensions
{
    public static class VisibilityExtensions
    {
        public static readonly DependencyProperty IsNotCollapsedProperty = DependencyProperty.RegisterAttached("IsNotCollapsed", typeof(bool?), typeof(VisibilityExtensions), new PropertyMetadata(true, OnIsNotCollapsedChanged));

        public static void SetIsNotCollapsed(UIElement element, bool? value)
        {
            element.SetValue(IsNotCollapsedProperty, value);
        }
        public static bool? GetIsNotCollapsed(UIElement element)
        {
            return (bool?)element.GetValue(IsNotCollapsedProperty);
        }

        private static void OnIsNotCollapsedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var isNotCollapsed = ((bool?) e.NewValue) ?? true;
            ((UIElement) d).Visibility = isNotCollapsed ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
