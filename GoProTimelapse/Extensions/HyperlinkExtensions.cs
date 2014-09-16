using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace journeyofcode.GoProTimelapse.Extensions
{
    public static class HyperlinkExtensions
    {
        public static readonly DependencyProperty WebUrlProperty = DependencyProperty.RegisterAttached("WebUrl", typeof(string), typeof(HyperlinkExtensions), new PropertyMetadata(OnWebUrlChanged));

        public static void SetWebUrl(Hyperlink element, string value)
        {
            element.SetValue(WebUrlProperty, value);
        }
        public static string GetWebUrl(Hyperlink element)
        {
            return (string)element.GetValue(WebUrlProperty);
        }

        private static void OnWebUrlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                ((Hyperlink)d).Click -= HyperlinkExtensions_Click;
            if (e.NewValue != null)
                ((Hyperlink)d).Click += HyperlinkExtensions_Click;
        }

        static void HyperlinkExtensions_Click(object sender, RoutedEventArgs e)
        {
            var hyperlink = (Hyperlink)sender;
            var url = GetWebUrl(hyperlink);

            Process.Start(url);
            e.Handled = true;
        }
    }
}
