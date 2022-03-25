using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucky
{
    public interface IOutputWriter
    {
        void WriteLine();
        void WriteLine(string line);
        void WriteLine(string line, ConsoleColor color);
    }
}
