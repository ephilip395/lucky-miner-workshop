﻿using Lucky.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lucky.Views.Ucs
{
    public partial class OverClockTable : UserControl
    {
        public Visibility IsOverClockVisible
        {
            get { return (Visibility)GetValue(IsOverClockVisibleProperty); }
            set { SetValue(IsOverClockVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsOverClockVisibleProperty =
            DependencyProperty.Register(nameof(IsOverClockVisible), typeof(Visibility), typeof(OverClockTable), new PropertyMetadata(Visibility.Collapsed));

        public SpeedTableViewModel Vm { get; private set; }

        public OverClockTable()
        {
            if (WpfUtil.IsInDesignMode)
            {
                return;
            }
            this.Vm = new SpeedTableViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
            IsOverClockVisible = Visibility.Visible;
        }


        private void ItemsControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Window.GetWindow(this).DragMove();
            }
        }

    }
}
