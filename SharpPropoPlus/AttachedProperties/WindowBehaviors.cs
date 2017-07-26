using System;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Interop;

namespace SharpPropoPlus.AttachedProperties
{
    public class WindowBehaviors : Behavior<Window>
    {
        public static readonly DependencyProperty
            BlehProperty = 
                DependencyProperty.RegisterAttached(
                    "Bleh", typeof(string), typeof(WindowBehaviors),
                    new PropertyMetadata("", PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            //throw new NotImplementedException();
        }

        public static void SetBleh(DependencyObject element, string value)
        {
            element.SetValue(BlehProperty, value);
        }
        public static string GetBleh(DependencyObject element)
        {
            return (string)element.GetValue(BlehProperty);
        }

        //public string Bleh
        //{
        //    get { return (string) GetValue(BlehProperty); }

        //    set
        //    {
        //        SetValue(BlehProperty, value);
        //    }
        //}

        protected override void OnChanged()
        {
            base.OnChanged();
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            throw new NotImplementedException();
            //DropShadowToWindow(this.AssociatedObject);
        }


        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll")]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMarInset);

        /// <summary>
        /// Drops a standard shadow to a WPF Window, even if the window is borderless. Only works with DWM (Windows Vista or newer).
        /// This method is much more efficient than setting AllowsTransparency to true and using the DropShadow effect,
        /// as AllowsTransparency involves a huge performance issue (hardware acceleration is turned off for all the window).
        /// </summary>
        /// <param name="window">Window to which the shadow will be applied</param>
        public static void DropShadowToWindow(Window window)
        {
            if (!DropShadow(window))
            {
                window.SourceInitialized += new EventHandler(window_SourceInitialized);
            }
        }

        private static void window_SourceInitialized(object sender, EventArgs e)
        {
            Window window = (Window)sender;

            DropShadow(window);

            window.SourceInitialized -= new EventHandler(window_SourceInitialized);
        }

        /// <summary>
        /// The actual method that makes API calls to drop the shadow to the window
        /// </summary>
        /// <param name="window">Window to which the shadow will be applied</param>
        /// <returns>True if the method succeeded, false if not</returns>
        private static bool DropShadow(Window window)
        {
            try
            {
                WindowInteropHelper helper = new WindowInteropHelper(window);
                int val = 2;
                int ret1 = DwmSetWindowAttribute(helper.Handle, 0, ref val, 4);

                if (ret1 == 0)
                {
                    Margins m = new Margins(0, 0, 0, 0);
                    int ret2 = DwmExtendFrameIntoClientArea(helper.Handle, ref m);
                    return ret2 == 0;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Probably dwmapi.dll not found (incompatible OS)
                return false;
            }
        }
    }
}