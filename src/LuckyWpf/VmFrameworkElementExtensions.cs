namespace Lucky {
    public static class VmFrameworkElementExtensions {
        public static void Init<TVm>(this IVmFrameworkElementTpl<TVm> element, TVm vm) {
            element.Vm = vm;
            element.DataContext = vm;
            element.DataContextChanged += (sender, e) => {
                element.Vm = (TVm)e.NewValue;
            };
        }
    }
}
