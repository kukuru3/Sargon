using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace Sargon {
    public static class TextUtility {

        #region Constant definitions;
        public static readonly char[] LineSeparators = { '\n' };
        public static readonly char[] WordSeparators = { ' ', ',', '.', ';', ':', '-', '\n' };
        #endregion

        //static private SFML.Graphics.Text propObject;

        static public IEnumerable<string> BreakText(string source, float maxWidth, Assets.Font font, int characterSize) {
            var cursor = 0;
            var lines = new List<string>();
            var currentLine = new StringBuilder("");
            if (font == null) font = GameContext.Current.Assets.DefaultFont;
            var propObject = new Text("", font.NativeFont, (uint)characterSize);
                        
            while (cursor < source.Length) {
                var old = cursor;
                CopyWord(source, ref cursor, currentLine);
                var w = GetWidth(currentLine.ToString(), propObject);
                if (w > maxWidth) {
                    LineBreakBackTrack(currentLine, out var remainder);
                    lines.Add(currentLine.ToString().Trim());
                    currentLine.Clear();
                    currentLine.Append(remainder);
                }
            }
            lines.Add(currentLine.ToString());
            return lines;
        }
        static private float GetWidth(string str, Text propObject) {
            propObject.DisplayedString = str;
            return propObject.GetLocalBounds().Width * propObject.Scale.X;
        }
        static private void CopyWord(string source, ref int startingAt, StringBuilder destination) {
            var at = source.IndexOfAny(WordSeparators, startingAt + 1);
            if (at < 0) at = source.Length;
            destination.Append(source.Substring(startingAt, at - startingAt));
            startingAt = at;            
        }

        static private void LineBreakBackTrack(StringBuilder text, out string remaining) {
            remaining = "";
            var str = text.ToString();
            var at = str.LastIndexOfAny(WordSeparators);
            if (at >= 0) {
                text.Remove(at+1, text.Length - at - 1);
                remaining = str.Substring(at + 1);
            }
        }
    }
}
