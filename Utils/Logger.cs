using System;
using System.Collections.Generic;
using System.Linq;

namespace Sargon.Utils {
    public class Logger {

        internal struct LogItem {
            public string Content;
            public ConsoleColor Color;
            int Priority;
            public LogItem(string content, ConsoleColor color = ConsoleColor.Gray, int priority = 1) {
                Content = content;
                Color = color;
                Priority = priority;
            }
        }

        const int HISTORY_SIZE = 20;

        private Queue<LogItem> history;

        public Logger() {
            history = new Queue<LogItem>();
            LogAdded += OnLogAdded;
        }

        private void OnLogAdded(LogItem obj) {
            Console.ForegroundColor = obj.Color;
            Console.WriteLine(obj.Content);
        }

        public void Clear() {
            history.Clear();
        }

        public IEnumerable<string> History => history.Select(h => h.Content);

        public void Add(string str, ConsoleColor color = ConsoleColor.Gray) {
            lock(history) { 
                var item = new LogItem(str, color);
                history.Enqueue(item);
                if (history.Count > HISTORY_SIZE) history.Dequeue();
                LogAdded?.Invoke(item);
            }
        }

        internal event Action<LogItem> LogAdded;

    }
}
