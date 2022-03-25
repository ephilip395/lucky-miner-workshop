using System;
using System.ComponentModel;
using System.Reflection;

namespace Lucky.Vms
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void AllPropertyChanged()
        {
            Type type = GetType();
            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                OnPropertyChanged(propertyInfo.Name);
            }
        }
    }
}
