using System;
using System.Windows;

namespace SharpPropoPlus.ViewModels
{
    public class ToolBarButton : DependencyObject
    {
        public bool IsChecked
        {
            get { return (bool) this.GetValue(StateProperty); }
            set { this.SetValue(StateProperty, value); }
        }

        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
            "IsChecked", typeof(bool), typeof(ToolBarButton), new PropertyMetadata(false));

        public object Content
        {
            get { return (object)this.GetValue(ContentProperty); }
            set { this.SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content", typeof(object), typeof(ToolBarButton), new PropertyMetadata(null));

        public Type Type
        {
            get { return (Type)this.GetValue(TypeProperty); }
            set { this.SetValue(TypeProperty, value); }
        }

        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(
            "Type", typeof(Type), typeof(ToolBarButton), new PropertyMetadata(null));

    }
}