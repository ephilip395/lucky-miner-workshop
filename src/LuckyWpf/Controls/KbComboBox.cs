﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lucky.Controls {
    public class KbComboBox : ComboBox {
        public FrameworkElement Toolbar {
            get { return (FrameworkElement)GetValue(ToolbarProperty); }
            set { SetValue(ToolbarProperty, value); }
        }
        public static readonly DependencyProperty ToolbarProperty =
            DependencyProperty.Register(nameof(Toolbar), typeof(FrameworkElement), typeof(KbComboBox), new PropertyMetadata(null));

        public ContextMenu ContextMenuEx {
            get { return (ContextMenu)GetValue(ContextMenuExProperty); }
            set { SetValue(ContextMenuExProperty, value); }
        }
        public static readonly DependencyProperty ContextMenuExProperty =
            DependencyProperty.Register(nameof(ContextMenuEx), typeof(ContextMenu), typeof(KbComboBox), new PropertyMetadata(null));

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e) {
            if (e.GetPosition(this).Y < this.ActualHeight) {
                this.IsDropDownOpen = false;
            }
            base.OnPreviewMouseRightButtonDown(e);
        }
    }
}
