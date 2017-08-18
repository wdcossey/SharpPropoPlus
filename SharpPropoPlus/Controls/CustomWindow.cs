//
// Copyright(C) MixModes Inc. 2011
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SharpPropoPlus.Annotations;
using SharpPropoPlus.Commands;
using SharpPropoPlus.ViewModels;

namespace SharpPropoPlus.Controls
{
    /// <summary>
    /// Custom Window
    /// </summary>     
    [TemplatePart(Name = "PART_TITLEBAR", Type = typeof(UIElement))]
    [TemplatePart(Name = "PART_MINIMIZE", Type = typeof(Button))]
    [TemplatePart(Name = "PART_MAXIMIZE_RESTORE", Type = typeof(Button))]
    [TemplatePart(Name = "PART_CLOSE", Type = typeof(Button))]
    [TemplatePart(Name = "PART_LEFT_BORDER", Type = typeof(UIElement))]
    [TemplatePart(Name = "PART_RIGHT_BORDER", Type = typeof(UIElement))]
    [TemplatePart(Name = "PART_TOP_BORDER", Type = typeof(UIElement))]
    [TemplatePart(Name = "PART_BOTTOM_BORDER", Type = typeof(UIElement))]
    [TemplatePart(Name = "PART_FAME", Type = typeof(Frame))]
    public partial class CustomWindow : Window, INotifyPropertyChanged
    {

        // Private members
        //private static DependencyPropertyKey MaximizedPropertyKey = DependencyProperty.RegisterReadOnly("Maximized",
        //    typeof(bool),
        //    typeof(CustomWindow),
        //    new PropertyMetadata(false));

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
                return new RelayCommand((o) =>
                {
                    var type = o as Type;

                    if (type == null)
                        return;

                    var root = Application.Instance.Container.Resolve(type, type.FullName);
                    FrameContent.NavigationService.Navigate(root);

                    this.SetActiveMenuButton();
                });
            }
        }

        public Type InitialPage
        {
            get { return (Type)this.GetValue(InitialPageProperty); }
            set { this.SetValue(InitialPageProperty, value); }
        }

        public static readonly DependencyProperty InitialPageProperty = DependencyProperty.Register(
            "InitialPage", typeof(Type), typeof(CustomWindow), new PropertyMetadata(null));


        public object Footer
        {
            get { return (object)this.GetValue(FooterProperty); }
            set { this.SetValue(FooterProperty, value); }
        }

        public static readonly DependencyProperty FooterProperty = DependencyProperty.Register(
            "Footer", typeof(object), typeof(CustomWindow), new PropertyMetadata());


        ///// <summary>
        ///// Maximized property
        ///// </summary>
        //public static DependencyProperty MaximizedProperty = MaximizedPropertyKey.DependencyProperty;

        ///// <summary>
        ///// Gets or sets a value indicating whether this <see cref="CustomWindow"/> is maximized
        ///// </summary>
        ///// <value><c>true</c> if maximized; otherwise, <c>false</c>.</value>
        //public bool Maximized
        //{
        //    get
        //    {
        //        return (bool)GetValue(MaximizedProperty);
        //    }
        //    private set
        //    {
        //        if (value)
        //        {
        //            //UpdateRestoreBounds();

        //            // Maximize hides taskbar, hence workaround
        //            //Top = Left = 0;
        //            //Height = SystemParameters.MaximizedPrimaryScreenHeight - SystemParameters.BorderWidth * 2;
        //            //Width = SystemParameters.MaximizedPrimaryScreenWidth - SystemParameters.BorderWidth * 2;
        //            this.WindowState = WindowState.Maximized;

        //        }
        //        else
        //        {
        //            //ApplyRestoreBounds();
        //            this.WindowState = WindowState.Normal;
        //        }

        //        //Visibility sizerVisibility = value ? Visibility.Hidden : Visibility.Visible;
        //        //UpdateBorderVisibility(RightBorder, sizerVisibility);
        //        //UpdateBorderVisibility(TopBorder, sizerVisibility);
        //        //UpdateBorderVisibility(BottomBorder, sizerVisibility);
        //        //UpdateBorderVisibility(LeftBorder, sizerVisibility);

        //        SetValue(MaximizedPropertyKey, value);
        //    }
        //}

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
                MinimizeCommand,
                (a, b) =>
                {
                    //WindowStyle = WindowStyle.SingleBorderWindow;
                    WindowState = WindowState.Minimized;
                }));

            // Command binding for maximize / restore button
            CommandBindings.Add(new CommandBinding(MaximizeRestoreCommand,
                (a, b) =>
                {
                    WindowState = (WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
                    //Maximized = !Maximized;
                }));
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
            //AttachTitleBar();
            //AttachBorders();
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
                minimizeButton.Command = MinimizeCommand;
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
                maximizeRestoreButton.Command = MaximizeRestoreCommand;
                MaximizeRestoreButton = maximizeRestoreButton;
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
                this.NavigateCommand.Execute(InitialPage);

                this.SetActiveMenuButton();

            }
        }

        private void SetActiveMenuButton()
        {
            if (ToolBarItems.Select(s => s.Type).Contains(InitialPage))
            {
                ToolBarItems.First(f => f.Type == InitialPage).IsChecked = true;
            }
        }

        ///// <summary>
        ///// Attaches the title bar to visual tree
        ///// </summary>
        //private void AttachTitleBar()
        //{
        //    if (TitleBar != null)
        //    {
        //        TitleBar.RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnTitlebarClick));
        //    }

        //    UIElement titleBar = GetChildControl<UIElement>("PART_TITLEBAR");
        //    if (titleBar != null)
        //    {
        //        TitleBar = titleBar;
        //        titleBar.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnTitlebarClick));
        //    }
        //}

        ///// <summary>
        ///// Called when titlebar is clicked or double clicked
        ///// </summary>
        ///// <param name="source">Event source</param>
        ///// <param name="args">The <see cref="System.Windows.Input.MouseLeftButtonEventArgs"/> instance containing the event data</param>
        //private void OnTitlebarClick(object source, MouseButtonEventArgs args)
        //{
        //    switch (args.ClickCount)
        //    {
        //        case 1:
        //            //if (this.WindowState == WindowState.Maximized)
        //            //{
        //            //    this.WindowState = WindowState.Normal;
        //            //}

        //            //if (this.WindowState == WindowState.Normal)
        //            //{
        //            //    DragMove();
        //            //}
                    
        //            break;
        //        case 2:

        //            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                    
        //            //Maximized = !Maximized;
        //            break;
        //    }
        //}

        ///// <summary>
        ///// Attaches the borders to the visual tree
        ///// </summary>
        //private void AttachBorders()
        //{
        //    AttachLeftBorder();
        //    AttachRightBorder();
        //    AttachTopBorder();
        //    AttachBottomBorder();
        //}

        ///// <summary>
        ///// Attaches the left border to the visual tree
        ///// </summary>
        //private void AttachLeftBorder()
        //{
        //    if (LeftBorder != null)
        //    {
        //        LeftBorder.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonDown));
        //        LeftBorder.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonUp));
        //        LeftBorder.MouseMove -= OnLeftBorderMouseMove;
        //    }

        //    UIElement leftBorder = GetChildControl<UIElement>("PART_LEFT_BORDER");
        //    if (leftBorder != null)
        //    {
        //        LeftBorder = leftBorder;
        //        leftBorder.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonDown));
        //        leftBorder.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonUp));
        //        leftBorder.MouseMove += OnLeftBorderMouseMove;
        //    }
        //}

        ///// <summary>
        ///// Called when mouse left button is down on a border
        ///// </summary>
        ///// <param name="source">Event source</param>
        ///// <param name="args">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data</param>
        //private void OnBorderMouseLeftButtonDown(object source, MouseButtonEventArgs args)
        //{
        //    IsResizing = true;
        //}

        ///// <summary>
        ///// Called when mouse left button is up on a border
        ///// </summary>
        ///// <param name="source">Event source</param>
        ///// <param name="args">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data</param>
        //private void OnBorderMouseLeftButtonUp(object source, MouseButtonEventArgs args)
        //{
        //    IsResizing = false;
        //    if (source is UIElement)
        //    {
        //        (source as UIElement).ReleaseMouseCapture();
        //    }
        //}

        ///// <summary>
        ///// Called when mouse moves over left border
        ///// </summary>
        ///// <param name="source">Event source</param>
        ///// <param name="args">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data</param>
        //private void OnLeftBorderMouseMove(object source, MouseEventArgs args)
        //{
        //    if ((!LeftBorder.IsMouseCaptured) && IsResizing)
        //    {
        //        LeftBorder.CaptureMouse();
        //    }

        //    if (IsResizing)
        //    {
        //        double position = args.GetPosition(this).X;

        //        if (Math.Abs(position) < 10)
        //        {
        //            return;
        //        }

        //        if ((position > 0) && ((Width - position) > MinWidth) && (Width > position))
        //        {
        //            Left += position;
        //            Width -= position;
        //        }
        //        else if ((position < 0) && (Left > 0))
        //        {
        //            position = (Left + position > 0) ? position : -1 * Left;
        //            Width = ActualWidth - position;
        //            Left += position;
        //        }
        //    }
        //}

        ///// <summary>
        ///// Attaches the right border to the visual tree
        ///// </summary>
        //private void AttachRightBorder()
        //{
        //    if (RightBorder != null)
        //    {
        //        RightBorder.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonDown));
        //        RightBorder.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonUp));
        //        RightBorder.MouseMove -= OnRightBorderMouseMove;
        //    }

        //    UIElement rightBorder = GetChildControl<UIElement>("PART_RIGHT_BORDER");
        //    if (rightBorder != null)
        //    {
        //        RightBorder = rightBorder;
        //        rightBorder.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonDown));
        //        rightBorder.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonUp));
        //        rightBorder.MouseMove += OnRightBorderMouseMove;
        //    }
        //}

        ///// <summary>
        ///// Called when mouse moves over right border
        ///// </summary>
        ///// <param name="source">Event source</param>
        ///// <param name="args">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data</param>
        //private void OnRightBorderMouseMove(object source, MouseEventArgs args)
        //{
        //    if ((!RightBorder.IsMouseCaptured) && IsResizing)
        //    {
        //        RightBorder.CaptureMouse();
        //    }

        //    if (IsResizing)
        //    {
        //        double position = args.GetPosition(this).X;

        //        if (Math.Abs(position) < 10)
        //        {
        //            return;
        //        }

        //        if (position > 0)
        //        {
        //            Width = position;
        //        }
        //        else if ((position < 0) && (ActualWidth > MinWidth))
        //        {
        //            position = (ActualWidth + position < MinWidth) ? MinWidth - ActualWidth : position;
        //            Width = ActualWidth + position;
        //        }
        //    }
        //}

        ///// <summary>
        ///// Attaches the top border to the visual tree
        ///// </summary>
        //private void AttachTopBorder()
        //{
        //    if (TopBorder != null)
        //    {
        //        TopBorder.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonDown));
        //        TopBorder.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonUp));
        //        TopBorder.MouseMove -= OnRightBorderMouseMove;
        //    }

        //    UIElement topBorder = GetChildControl<UIElement>("PART_TOP_BORDER");
        //    if (topBorder != null)
        //    {
        //        TopBorder = topBorder;
        //        topBorder.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonDown));
        //        topBorder.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonUp));
        //        topBorder.MouseMove += OnTopBorderMouseMove;
        //    }
        //}

        ///// <summary>
        ///// Called when mouse moves over top border
        ///// </summary>
        ///// <param name="source">Event source</param>
        ///// <param name="args">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data</param>
        //private void OnTopBorderMouseMove(object source, MouseEventArgs args)
        //{
        //    if ((!TopBorder.IsMouseCaptured) && IsResizing)
        //    {
        //        TopBorder.CaptureMouse();
        //    }

        //    if (IsResizing)
        //    {
        //        double position = args.GetPosition(this).Y;

        //        if (Math.Abs(position) < 10)
        //        {
        //            return;
        //        }

        //        if (position < 0)
        //        {
        //            position = Top + position < 0 ? -Top : position;
        //            Top += position;
        //            Height = ActualHeight - position;
        //        }
        //        else if ((position > 0) && (ActualHeight - position > MinHeight))
        //        {
        //            position = (ActualHeight - position < MinHeight) ? MinHeight - ActualHeight : position;
        //            Height = ActualHeight - position;
        //            Top += position;
        //        }
        //    }
        //}

        ///// <summary>
        ///// Attaches the bottom border to the visual tree
        ///// </summary>
        //private void AttachBottomBorder()
        //{
        //    if (BottomBorder != null)
        //    {
        //        BottomBorder.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonDown));
        //        BottomBorder.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonUp));
        //        BottomBorder.MouseMove -= OnBottomBorderMouseMove;
        //    }

        //    UIElement bottomBorder = GetChildControl<UIElement>("PART_BOTTOM_BORDER");
        //    if (bottomBorder != null)
        //    {
        //        BottomBorder = bottomBorder;
        //        bottomBorder.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonDown));
        //        bottomBorder.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnBorderMouseLeftButtonUp));
        //        bottomBorder.MouseMove += OnBottomBorderMouseMove;
        //    }
        //}

        ///// <summary>
        ///// Called when mouse moves over bottom border
        ///// </summary>
        ///// <param name="source">Event source</param>
        ///// <param name="args">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data</param>
        //private void OnBottomBorderMouseMove(object source, MouseEventArgs args)
        //{
        //    if ((!BottomBorder.IsMouseCaptured) && IsResizing)
        //    {
        //        BottomBorder.CaptureMouse();
        //    }

        //    if (IsResizing)
        //    {
        //        double position = args.GetPosition(this).Y - ActualHeight;

        //        if (Math.Abs(position) < 10)
        //        {
        //            return;
        //        }

        //        if (position > 0)
        //        {
        //            Height = ActualHeight + position;
        //        }
        //        else if ((position < 0) && (ActualHeight + position > MinHeight))
        //        {
        //            position = (ActualHeight + position < MinHeight) ? MinHeight - ActualHeight : position;
        //            Height = ActualHeight + position;
        //        }
        //    }
        //}

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

        ///// <summary>
        ///// Updates the border visibility.
        ///// </summary>
        ///// <param name="border">Border</param>
        ///// <param name="visibility">Visibility</param>
        //private void UpdateBorderVisibility(UIElement border, Visibility visibility)
        //{
        //    if (border != null)
        //    {
        //        border.Visibility = visibility;
        //    }
        //}

        ///// <summary>
        ///// Updates the restore bounds
        ///// </summary>
        //private void UpdateRestoreBounds()
        //{
        //    RestoreBounds = new Rect(new Point(Left, Top), new Point(Left + ActualWidth, Top + ActualHeight));
        //}

        ///// <summary>
        ///// Applies the restore bounds to the window
        ///// </summary>
        //private void ApplyRestoreBounds()
        //{
        //    Left = RestoreBounds.Left;
        //    Top = RestoreBounds.Top;
        //    Width = RestoreBounds.Width;
        //    Height = RestoreBounds.Height;
        //}

        ///// <summary>
        ///// Gets the size and location of a window before being either minimized or maximized.
        ///// </summary>
        ///// <value></value>
        ///// <returns>A <see cref="T:System.Windows.Rect"/> that specifies the size and location of a window before being either minimized or maximized.</returns>
        //private new Rect RestoreBounds { get; set; }

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

        private Frame FrameContent { get; set; }

        ///// <summary>
        ///// Title bar
        ///// </summary>
        //private UIElement TitleBar { get; set; }

        ///// <summary>
        ///// Left border
        ///// </summary>
        //private UIElement LeftBorder { get; set; }

        ///// <summary>
        ///// Right border
        ///// </summary>
        //private UIElement RightBorder { get; set; }

        ///// <summary>
        ///// Top border
        ///// </summary>
        //private UIElement TopBorder { get; set; }

        ///// <summary>
        ///// Bottom border
        ///// </summary>
        //private UIElement BottomBorder { get; set; }

        ///// <summary>
        ///// Indicates whether window is currently resizing
        ///// </summary>
        ///// <value>
        ///// 	<c>true</c> If window is currently resizing; otherwise, <c>false</c>.
        ///// </value>
        //private bool IsResizing { get; set; }

        /// <summary>
        /// Minimize Command
        /// </summary>
        private readonly RoutedUICommand MinimizeCommand =
            new RoutedUICommand("Minimize", "Minimize", typeof(CustomWindow));

        /// <summary>
        /// Maximize / Restore command
        /// </summary>
        private readonly RoutedUICommand MaximizeRestoreCommand =
            new RoutedUICommand("MaximizeRestore", "MaximizeRestore", typeof(CustomWindow));

        private System.Uri _frameSource;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        { 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}