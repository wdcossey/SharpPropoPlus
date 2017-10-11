using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace SharpPropoPlus.Behaviors
{
    public class LeftClickContextMenuBehavior : Behavior<Button>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.Click += ButtonRoutedEventHandlerOnClick;
        }

        private void ButtonRoutedEventHandlerOnClick(object o, RoutedEventArgs routedEventArgs)
        {
            if (this.AssociatedObject.ContextMenu != null)
            {
                this.AssociatedObject.ContextMenu.PlacementTarget = this.AssociatedObject;
                this.AssociatedObject.ContextMenu.Placement = PlacementMode.Bottom;
                this.AssociatedObject.ContextMenu.IsOpen = true;
            }
        }

        protected override void OnDetaching()
        {

            this.AssociatedObject.Click -= ButtonRoutedEventHandlerOnClick;

            base.OnDetaching();
        }


    }
}