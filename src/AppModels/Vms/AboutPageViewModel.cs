using System;

namespace Lucky.Vms
{
    public class AboutPageViewModel : ViewModelBase
    {
        public AboutPageViewModel()
        {
        }

        public int ThisYear => DateTime.Now.Year;
    }
}
