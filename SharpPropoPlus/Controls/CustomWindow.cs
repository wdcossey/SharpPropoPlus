//
// Original Implementation by: MixModes Inc.
// 

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.Practices.Unity;
using SharpPropoPlus.Annotations;
using SharpPropoPlus.Commands;

namespace SharpPropoPlus.Controls
{
    /// <summary>
    /// Custom Window
    /// </summary>     
    [TemplatePart(Name = "PART_TITLEBAR", Type = typeof(UIElement))]
    [TemplatePart(Name = "PART_MINIMIZE", Type = typeof(Button))]
    [TemplatePart(Name = "PART_MAXIMIZE_RESTORE", Type = typeof(Button))]
    [TemplatePart(Name = "PART_CLOSE", Type = typeof(Button))]
    [TemplatePart(Name = "PART_SETTINGS", Type = typeof(Button))]
    [TemplatePart(Name = "PART_LEFT_BORDER", Type = typeof(UIElement))]
    [TemplatePart(Name = "PART_RIGHT_BORDER", Type = typeof(UIElement))]
    [TemplatePart(Name = "PART_TOP_BORDER", Type = typeof(UIElement))]
    [TemplatePart(Name = "PART_BOTTOM_BORDER", Type = typeof(UIElement))]
    [TemplatePart(Name = "PART_FAME", Type = typeof(Frame))]
    [TemplatePart(Name = "PART_NAVIGATION", Type = typeof(Button))]
    [TemplatePart(Name = "PART_TOGGLE_BUTTON", Type = typeof(ToggleButton))]
    public partial class CustomWindow : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomWindow"/> class.
        /// </summary>
        public CustomWindow()
        {
            ToolBarItems = new ObservableCollection<ToolBarButton>();

            CreateCommandBindings();

            this.StateChanged += delegate(object sender, EventArgs args)
            {

            };

        }


        public ObservableCollection<ToolBarButton> ToolBarItems
        {
            get { return (ObservableCollection<ToolBarButton>)this.GetValue(MenuItemsProperty); }
            set { this.SetValue(MenuItemsProperty, value); }
        }

        public static readonly DependencyProperty MenuItemsProperty = DependencyProperty.Register(
            nameof(ToolBarItems), typeof(ObservableCollection<ToolBarButton>), typeof(CustomWindow), new FrameworkPropertyMetadata(new ObservableCollection<ToolBarButton>())
            {
                BindsTwoWayByDefault = true,
                AffectsRender = true,
            });

        public ICommand NavigateCommand
        {
            get
            {
                return new RelayCommand<Type>(type =>
                {
                    if (type == null)
                        return;

                    var root = Application.Instance.Container.Resolve(type/*, type.FullName*/);
                    FrameContent.NavigationService.Navigate(root);
                });
            }
        }

        public static readonly DependencyProperty InitialPageProperty = DependencyProperty.Register(
            "InitialPage", typeof(Type), typeof(CustomWindow), new PropertyMetadata(null));

        public Type InitialPage
        {
            get { return (Type)this.GetValue(InitialPageProperty); }
            set { this.SetValue(InitialPageProperty, value); }
        }

        public static readonly DependencyProperty FooterProperty = DependencyProperty.Register(
            "Footer", typeof(object), typeof(CustomWindow), new PropertyMetadata());

        public object Footer
        {
            get { return (object)this.GetValue(FooterProperty); }
            set { this.SetValue(FooterProperty, value); }
        }

        public static readonly DependencyProperty SettingsMenuProperty = DependencyProperty.Register(
            "SettingsMenu", typeof(object), typeof(CustomWindow), new PropertyMetadata());

        public ContextMenu SettingsMenu
        {
            get { return (ContextMenu)this.GetValue(SettingsMenuProperty); }
            set { this.SetValue(SettingsMenuProperty, value); }
        }

        public static readonly DependencyProperty ToggleButtonCommandProperty = DependencyProperty.Register(
            "ToggleButtonCommand", typeof(ICommand), typeof(CustomWindow), new PropertyMetadata());

        public ICommand ToggleButtonCommand
        {
            get { return (ICommand)this.GetValue(ToggleButtonCommandProperty); }
            set { this.SetValue(ToggleButtonCommandProperty, value); }
        }

        public static readonly DependencyProperty IsToggleCheckedProperty = DependencyProperty.Register(
            "IsToggleChecked", typeof(bool), typeof(CustomWindow), new PropertyMetadata(false));

        public bool IsToggleChecked
        {
            get { return (bool)this.GetValue(IsToggleCheckedProperty); }
            set { this.SetValue(IsToggleCheckedProperty, value); }
        }
        
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code 
        /// or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            AttachToVisualTree();
        }

        /// <summary>
        /// Creates the command bindings
        /// </summary>
        private void CreateCommandBindings()
        {
            // Command binding for close button
            CommandBindings.Add(new CommandBinding(
                ApplicationCommands.Close,
                (a, b) => { Close(); }));

            // Command binding for minimize button
            CommandBindings.Add(new CommandBinding(
                _minimizeCommand,
                (a, b) =>
                {
                    WindowState = WindowState.Minimized;
                }));

            // Command binding for maximize / restore button
            CommandBindings.Add(new CommandBinding(_maximizeRestoreCommand,
                (a, b) =>
                {
                    WindowState = (WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
                }));

            // Command binding for navigation button
            CommandBindings.Add(new CommandBinding(_navigationCommand,
                (sender, args) =>
                {
                    FrameContent.NavigationService.GoBack();
                }, 
                (sender, args) => args.CanExecute = FrameContent.NavigationService.CanGoBack));
        }

        /// <summary>
        /// Attaches to visual tree to the template
        /// </summary>
        private void AttachToVisualTree()
        {
            AttachContentFrame();
            AttachCloseButton();
            AttachMinimizeButton();
            AttachMaximizeRestoreButton();
            AttachNavigaionButton();
            AttachSettingsButton();
            AttachToggleButton();
        }

        /// <summary>
        /// Attaches the close button
        /// </summary>
        private void AttachCloseButton()
        {
            if (CloseButton != null)
            {
                CloseButton.Command = null;
            }

            Button closeButton = GetChildControl<Button>("PART_CLOSE");
            if (closeButton != null)
            {
                closeButton.Command = ApplicationCommands.Close;
                CloseButton = closeButton;
            }
        }

        /// <summary>
        /// Attaches the minimize button
        /// </summary>
        private void AttachMinimizeButton()
        {
            if (MinimizeButton != null)
            {
                MinimizeButton.Command = null;
            }

            Button minimizeButton = GetChildControl<Button>("PART_MINIMIZE");
            if (minimizeButton != null)
            {
                minimizeButton.Command = _minimizeCommand;
                MinimizeButton = minimizeButton;
            }
        }

        /// <summary>
        /// Attaches the maximize restore button
        /// </summary>
        private void AttachMaximizeRestoreButton()
        {
            if (MaximizeRestoreButton != null)
            {
                MaximizeRestoreButton.Command = null;
            }

            Button maximizeRestoreButton = GetChildControl<Button>("PART_MAXIMIZE_RESTORE");
            if (maximizeRestoreButton != null)
            {
                maximizeRestoreButton.Command = _maximizeRestoreCommand;
                MaximizeRestoreButton = maximizeRestoreButton;
            }
        }

        /// <summary>
        /// Attaches the settings button
        /// </summary>
        private void AttachSettingsButton()
        {
            if (SettingsButton != null)
            {
                SettingsButton.ContextMenu = null;
            }

            Button settingsButton = GetChildControl<Button>("PART_SETTINGS");
            if (settingsButton != null)
            {
                settingsButton.ContextMenu = SettingsMenu;
                SettingsButton = settingsButton;
            }
        }

        private void AttachNavigaionButton()
        {
            if (NavigationButton != null)
            {
                NavigationButton.Command = null;
            }

            Button navigationButton = GetChildControl<Button>("PART_NAVIGATION");
            if (navigationButton != null)
            {
                navigationButton.Command = _navigationCommand;
                NavigationButton = navigationButton;
            }
        }

        private void AttachToggleButton()
        {
            if (ToggleButton != null)
            {
                ToggleButton.Command = null;
                ToggleButton.IsChecked = null;
            }

            ToggleButton toggleButton = GetChildControl<ToggleButton>("PART_TOGGLE_BUTTON");
            if (toggleButton != null)
            {
                toggleButton.IsChecked = IsToggleChecked;
                ToggleButton = toggleButton;
            }
        }

        /// <summary>
        /// Attaches the maximize restore button
        /// </summary>
        private void AttachContentFrame()
        {
            FrameContent = GetChildControl<Frame>("PART_FAME");

            if (FrameContent != null && InitialPage != null)
            {

                FrameContent.Navigated += (sender, args) =>
                {
                    SetActiveMenuButton(args.Content.GetType());
                };

                this.NavigateCommand.Execute(InitialPage);
            }
        }

        private void SetActiveMenuButton(Type pageType = null)
        {
            foreach (var toolBarButton in ToolBarItems)
            {
                toolBarButton.IsChecked = toolBarButton.Type == (pageType ?? InitialPage);
            }
            //if (ToolBarItems.Select(s => s.Type).Contains(pageType ?? InitialPage))
            //{
            //    ToolBarItems.First(f => f.Type == (pageType ?? InitialPage)).IsChecked = true;
            //}
        }

        /// <summary>
        /// Gets the child control from the template
        /// </summary>
        /// <typeparam name="T">Type of control requested</typeparam>
        /// <param name="controlName">Name of the control</param>
        /// <returns>Control instance if there is one with the specified name; null otherwise</returns>
        private T GetChildControl<T>(string controlName) where T : DependencyObject
        {
            T control = GetTemplateChild(controlName) as T;
            return control;
        }

        /// <summary>
        /// Close button
        /// </summary>
        private Button CloseButton { get; set; }

        /// <summary>
        /// Minimize button
        /// </summary>
        private Button MinimizeButton { get; set; }

        /// <summary>
        /// Maximize / restore button
        /// </summary>
        /// <value>The maximize restore button.</value>
        private Button MaximizeRestoreButton { get; set; }


        private Button SettingsButton { get; set; }

        private Button NavigationButton { get; set; }

        private ToggleButton ToggleButton { get; set; }

        private Frame FrameContent { get; set; }

        /// <summary>
        /// Minimize Command
        /// </summary>
        private readonly RoutedUICommand _minimizeCommand =
            new RoutedUICommand("Minimize", "Minimize", typeof(CustomWindow));

        /// <summary>
        /// Maximize / Restore command
        /// </summary>
        private readonly RoutedUICommand _maximizeRestoreCommand =
            new RoutedUICommand("MaximizeRestore", "MaximizeRestore", typeof(CustomWindow));

        private readonly RoutedUICommand _navigationCommand =
            new RoutedUICommand("FrameNavigation", "FrameNavigation", typeof(CustomWindow));

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        { 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}