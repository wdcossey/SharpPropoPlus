using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SharpPropoPlus.Controls
{
    /// <summary>
    /// Interaction logic for StripIndicator.xaml
    /// </summary>
    public partial class StripIndicator : UserControl
    {

        //[DllImport("uxtheme.dll", EntryPoint = "#95")]
        //public static extern uint GetImmersiveColorFromColorSetEx(uint dwImmersiveColorSet, uint dwImmersiveColorType, bool bIgnoreHighContrast, uint dwHighContrastCacheMode);
        //[DllImport("uxtheme.dll", EntryPoint = "#96")]
        //public static extern uint GetImmersiveColorTypeFromName(IntPtr pName);
        //[DllImport("uxtheme.dll", EntryPoint = "#98")]
        //public static extern int GetImmersiveUserColorSetPreference(bool bForceCheckRegistry, bool bSkipCheckOnFail);

        public StripIndicator()
        {
            InitializeComponent();

            Loaded += MyProgressBar_Loaded;
        }

        private void MyProgressBar_Loaded(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum",
            typeof(double), typeof(StripIndicator), new PropertyMetadata(100d, OnMaximumChanged));

        public double Maximum
        {
            get => (double) GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }


        private static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum",
            typeof(double), typeof(StripIndicator), new PropertyMetadata(0d, OnMinimumChanged));

        public double Minimum
        {
            get => (double) GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        private static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double),
            typeof(StripIndicator), new PropertyMetadata(50d, OnValueChanged));

        public double Value
        {
            get => (double) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static readonly DependencyProperty ProgressBarWidthProperty =
            DependencyProperty.Register("ProgressBarWidth", typeof(double), typeof(StripIndicator), null);

        private double ProgressBarWidth
        {
            get => (double) GetValue(ProgressBarWidthProperty);
            set => SetValue(ProgressBarWidthProperty, value);
        }

        private new static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(StripIndicator), null);

        public new Thickness BorderThickness
        {
            get => (Thickness) GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }

        private new static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(StripIndicator), null);

        public new Brush BorderBrush
        {
            get => (Brush) GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }

        private static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(StripIndicator), null);

        public Brush Fill
        {
            get => (Brush) GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var stripIndicator = o as StripIndicator;
            stripIndicator?.Update();
        }

        static void OnMinimumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var stripIndicator = o as StripIndicator;
            stripIndicator?.Update();
        }

        static void OnMaximumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var stripIndicator = o as StripIndicator;
            stripIndicator?.Update();
        }


        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            Update();
        }

        void Update()
        {
            // The -2 is for the borders - there are probably better ways of doing this since you
            // may want your template to have variable bits like border width etc which you'd use
            // TemplateBinding for

            //uint colour1 = GetImmersiveColorFromColorSetEx((uint)GetImmersiveUserColorSetPreference(false, false), GetImmersiveColorTypeFromName(Marshal.StringToHGlobalUni("ImmersiveStartSelectionBackground")), false, 0);
            //Color colour = Color.FromArgb((byte)((0xFF000000 & colour1) >> 24), (byte)(0x000000FF & colour1),
            //    (byte)((0x0000FF00 & colour1) >> 8), (byte)((0x00FF0000 & colour1) >> 16));

            //BorderBrush = new SolidColorBrush(colour);
            //Fill = new SolidColorBrush(colour);

            ProgressBarWidth =
                Math.Min((Value / (Maximum + Minimum) * ActualWidth) - (BorderThickness.Left + BorderThickness.Right),
                    ActualWidth - (BorderThickness.Left + BorderThickness.Right));
        }
    }
}
