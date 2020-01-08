using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Utilities.EventQueue
{
    public class EventSource
    {
        public string FilePath { get; set; }

        public int LineNumber { get; set; }

        public string MemberName { get; set; }

        public EventSource(string filePath, int lineNumber, string memberName)
        {
            FilePath = filePath;
            LineNumber = lineNumber;
            MemberName = memberName;
        }

        public override string ToString()
        {
            return $"file '{FilePath}' [Method: '{MemberName}', LineNumber: '{LineNumber}']";
        }
    }
}
