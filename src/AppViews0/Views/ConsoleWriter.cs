using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Controls;

namespace Lucky.Views
{
    public class ConsoleWriter : TextWriter
    {
        private TextBox textbox;

        public ConsoleWriter(TextBox textbox)
        {
            this.textbox = textbox;
        }

        public override void Write(char value)
        {
            textbox.Dispatcher.Invoke(new Action(delegate
            {
                textbox.Text += value;

            }));
        }

        public override void Write(string value)
        {
            textbox.Dispatcher.Invoke(new Action(delegate
            {
                textbox.Text += value;

            }));
        }

        public override Encoding Encoding
        {
            get { return Encoding.Unicode; }
        }
    }
}
