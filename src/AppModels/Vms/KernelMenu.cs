﻿using System.Windows.Media;

namespace Lucky.Vms {
    public class KernelMenu : ViewModelBase {
        private string _name;
        private Geometry _icon;
        private string _iconName;

        public KernelMenu(string name, string iconName) {
            _name = name;
            _iconName = iconName;
            if (!string.IsNullOrEmpty(iconName)) {
                _icon = AppUtil.GetResource<Geometry>(iconName);
            }
        }

        public string Name {
            get => _name;
            set {
                if (_name != value) {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public string IconName {
            get => _iconName;
            set {
                if (_iconName != value) {
                    _iconName = value;
                    OnPropertyChanged(nameof(IconName));
                }
            }
        }

        public Geometry Icon {
            get { return _icon; }
            private set {
                if (_icon != value) {
                    _icon = value;
                    OnPropertyChanged(nameof(Icon));
                }
            }
        }

        private SolidColorBrush _itemBackground = WpfUtil.TransparentBrush;
        public SolidColorBrush ItemBackground {
            get {
                return _itemBackground;
            }
            set {
                if (_itemBackground != value) {
                    _itemBackground = value;
                    OnPropertyChanged(nameof(ItemBackground));
                }
            }
        }

        private SolidColorBrush _itemForeground = WpfUtil.WhiteBrush;
        public SolidColorBrush ItemForeground {
            get {
                return _itemForeground;
            }
            set {
                if (_itemForeground != value) {
                    _itemForeground = value;
                    OnPropertyChanged(nameof(ItemForeground));
                }
            }
        }

        private SolidColorBrush _borderBrush = WpfUtil.TransparentBrush;
        public SolidColorBrush BorderBrush {
            get {
                return _borderBrush;
            }
            set {
                if (_borderBrush != value) {
                    _borderBrush = value;
                    OnPropertyChanged(nameof(BorderBrush));
                }
            }
        }

        public void SetSelectedBackground() {
            ItemBackground = WpfUtil.SelectedBackground;
            ItemForeground = WpfUtil.SelectedForeground;
            BorderBrush = WpfUtil.SelectedBorderColor;
        }

        public void SetDefaultBackground() {
            ItemBackground = WpfUtil.TransparentBrush;
            ItemForeground = WpfUtil.WhiteBrush;
            BorderBrush = WpfUtil.TransparentBrush;
        }
    }
}
