﻿namespace Lucky.Core {
    public class ConsoleOutLine : IConsoleOutLine {
        public ConsoleOutLine() { }

        public long Timestamp { get; set; }
        public string Line { get; set; }
    }
}
