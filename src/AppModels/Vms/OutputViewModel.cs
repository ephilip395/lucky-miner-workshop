using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Lucky.Collection;

namespace Lucky.Vms
{
    public class OutputViewModel : ViewModelBase, IOutputWriter
    {

        public AsyncObservableCollection<ColorText> OutputContent { get; } = new AsyncObservableCollection<ColorText>(9999);

        public void WriteLine()
        {
            WriteLine("");
        }

        public void WriteLine(string line)
        {
            WriteLine(line, ConsoleColor.White);
        }

        public void WriteLine(string line, ConsoleColor color)
        {
            OutputContent.PressIfFulled(new ColorText(color, line));
        }

    }

    public class ColorText
    {
        public ConsoleColor Color { get; }

        public string Text { get; }

        public ColorText(ConsoleColor color, string text)
        {
            Color = color;
            Text = text;
        }


    }
}
