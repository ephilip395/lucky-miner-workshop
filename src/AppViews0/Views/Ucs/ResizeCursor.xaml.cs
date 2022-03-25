﻿using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Shapes;

namespace Lucky.Views.Ucs
{
    public partial class ResizeCursor : UserControl
    {
        #region SafeNativeMethods

        private static class SafeNativeMethods {
            public enum ResizeDirection
            {
                Left = 1,
                Right = 2,
                Top = 3,
                TopLeft = 4,
                TopRight = 5,
                Bottom = 6,
                BottomLeft = 7,
                BottomRight = 8,
            }

            [DllImport(DllName.User32Dll, CharSet = CharSet.Auto)]
            internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        }

        #endregion

        private HwndSource hwndSource;

        public ResizeCursor() {
            InitializeComponent();
            if (WpfUtil.IsInDesignMode)
            {
                return;
            }
            Loaded += (sender, e) =>
            {
                Window window = Window.GetWindow(this);
                hwndSource = PresentationSource.FromVisual(window) as HwndSource;
            };
        }

        private void Resize(object sender, MouseButtonEventArgs e)
        {
            ResizeWindow(sender);
        }

        private void DisplayResizeCursor(object sender, MouseEventArgs e)
        {
            DisplayResizeCursor(sender);
        }

        private void ResetCursor(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                Cursor = Cursors.Arrow;
            }
        }

        private void ResizeWindow(SafeNativeMethods.ResizeDirection direction)
        {
            const int WM_SYSCOMMAND = 0x112;
            _ = SafeNativeMethods.SendMessage(hwndSource.Handle, WM_SYSCOMMAND, (IntPtr)(61440 + direction), IntPtr.Zero);
        }

        private void ResizeWindow(object sender)
        {
            Rectangle clickedRectangle = sender as Rectangle;

            switch (clickedRectangle.Name)
            {
                case nameof(top):
                    clickedRectangle.Cursor = Cursors.SizeNS;
                    ResizeWindow(SafeNativeMethods.ResizeDirection.Top);
                    break;
                case nameof(bottom):
                    clickedRectangle.Cursor = Cursors.SizeNS;
                    ResizeWindow(SafeNativeMethods.ResizeDirection.Bottom);
                    break;
                case nameof(left):
                    clickedRectangle.Cursor = Cursors.SizeWE;
                    ResizeWindow(SafeNativeMethods.ResizeDirection.Left);
                    break;
                case nameof(right):
                    clickedRectangle.Cursor = Cursors.SizeWE;
                    ResizeWindow(SafeNativeMethods.ResizeDirection.Right);
                    break;
                case nameof(topLeft):
                    clickedRectangle.Cursor = Cursors.SizeNWSE;
                    ResizeWindow(SafeNativeMethods.ResizeDirection.TopLeft);
                    break;
                case nameof(topRight):
                    clickedRectangle.Cursor = Cursors.SizeNESW;
                    ResizeWindow(SafeNativeMethods.ResizeDirection.TopRight);
                    break;
                case nameof(bottomLeft):
                    clickedRectangle.Cursor = Cursors.SizeNESW;
                    ResizeWindow(SafeNativeMethods.ResizeDirection.BottomLeft);
                    break;
                case nameof(bottomRight):
                    clickedRectangle.Cursor = Cursors.SizeNWSE;
                    ResizeWindow(SafeNativeMethods.ResizeDirection.BottomRight);
                    break;
                default:
                    break;
            }
        }

        private void DisplayResizeCursor(object sender)
        {
            Rectangle clickedRectangle = sender as Rectangle;

            switch (clickedRectangle.Name)
            {
                case nameof(top):
                    clickedRectangle.Cursor = Cursors.SizeNS;
                    break;
                case nameof(bottom):
                    clickedRectangle.Cursor = Cursors.SizeNS;
                    break;
                case nameof(left):
                    clickedRectangle.Cursor = Cursors.SizeWE;
                    break;
                case nameof(right):
                    clickedRectangle.Cursor = Cursors.SizeWE;
                    break;
                case nameof(topLeft):
                    clickedRectangle.Cursor = Cursors.SizeNWSE;
                    break;
                case nameof(topRight):
                    clickedRectangle.Cursor = Cursors.SizeNESW;
                    break;
                case nameof(bottomLeft):
                    clickedRectangle.Cursor = Cursors.SizeNESW;
                    break;
                case nameof(bottomRight):
                    clickedRectangle.Cursor = Cursors.SizeNWSE;
                    break;
                default:
                    break;
            }
        }
    }
}
