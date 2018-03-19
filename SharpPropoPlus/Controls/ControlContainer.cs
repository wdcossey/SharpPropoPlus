using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SharpPropoPlus.Controls
{

    //[TemplatePart(Name = "PART_TITLE_ELEMENT", Type = typeof(TextBlock))]
    //[TemplatePart(Name = "PART_DESCRIPTION_ELEMENT", Type = typeof(TextBlock))]
    //[TemplatePart(Name = "PART_CONTROL_ELEMENT", Type = typeof(UIElement))]
    //[TemplatePart(Name = "PART_BOTTOM_BORDER", Type = typeof(Border))]
    public class ControlContainer : UserControl

    {

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(object), typeof(ControlContainer), new PropertyMetadata());

        public object Title
        {
            get => (object)this.GetValue(TitleProperty);
            set => this.SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleStringFormatProperty = DependencyProperty.Register(
            "TitleStringFormat", typeof(object), typeof(ControlContainer), new PropertyMetadata());

        public string TitleStringFormat
        {
            get => (string)this.GetValue(TitleStringFormatProperty);
            set => this.SetValue(TitleStringFormatProperty, value);
        }

        private static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            "Description", typeof(object), typeof(ControlContainer), new PropertyMetadata());

        public object Description
        {
            get => (object)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public static readonly DependencyProperty DescriptionStringFormatProperty = DependencyProperty.Register(
            "DescriptionStringFormat", typeof(object), typeof(ControlContainer), new PropertyMetadata());

        public string DescriptionStringFormat
        {
            get => (string)this.GetValue(DescriptionStringFormatProperty);
            set => this.SetValue(DescriptionStringFormatProperty, value);
        }

        private static readonly DependencyProperty HasBorderProperty = DependencyProperty.Register(
            "HasBorder", typeof(bool), typeof(ControlContainer), new PropertyMetadata(true));

        public bool HasBorder
        {
            get => (bool)GetValue(HasBorderProperty);
            set => SetValue(HasBorderProperty, value);
        }

    }
}