using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StudentGraphApplication
{
    public static class ControlAttachedProperties
    {
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached(
                "CornerRadius",
                typeof(CornerRadius),
                typeof(ControlAttachedProperties),
                new PropertyMetadata(new CornerRadius(0), OnCornerRadiusChanged));

        public static void SetCornerRadius(DependencyObject element, CornerRadius value)
        {
            element.SetValue(CornerRadiusProperty, value);
        }

        public static CornerRadius GetCornerRadius(DependencyObject element)
        {
            return (CornerRadius)element.GetValue(CornerRadiusProperty);
        }

        private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Control control)
            {
                control.Loaded += (s, args) =>
                {
                    var border = control.Template?.FindName("border", control) as Border;
                    if (border != null)
                    {
                        border.CornerRadius = (CornerRadius)e.NewValue;
                    }
                };

                if (control.Template == null)
                {
                    control.Template = CreateControlTemplate(control, (CornerRadius)e.NewValue);
                }
            }
        }

        private static ControlTemplate CreateControlTemplate(Control control, CornerRadius cornerRadius)
        {
            var template = new ControlTemplate(control.GetType());

            var borderFactory = new FrameworkElementFactory(typeof(Border));
            borderFactory.SetValue(Border.CornerRadiusProperty, cornerRadius);
            borderFactory.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Control.BackgroundProperty));
            borderFactory.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(Control.BorderBrushProperty));
            borderFactory.SetValue(Border.BorderThicknessProperty, new TemplateBindingExtension(Control.BorderThicknessProperty));

            var contentFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            contentFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            contentFactory.SetValue(ContentPresenter.ContentProperty, new TemplateBindingExtension(ContentControl.ContentProperty));

            borderFactory.AppendChild(contentFactory);
            template.VisualTree = borderFactory;

            return template;
        }
    }
}
