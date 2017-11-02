using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NuGetAnalyzer.View
{
    public static class RichTextBoxExtensions
    {
        public static void WriteLineAsync(this RichTextBox richTextBox, string text, Color color)
        {
            if (richTextBox.InvokeRequired)
            {
                richTextBox.Invoke(new Action<RichTextBox, string, Color>(AppendText), richTextBox, text, color);
            }
            else
            {
                richTextBox.AppendColoredText(text, color);
            }
        }

        public static void WriteLineAsync(this RichTextBox richTextBox, IEnumerable<string> text, Color color)
        {
            foreach (string textLine in text)
            {
                if (richTextBox.InvokeRequired)
                {
                    richTextBox.Invoke(new Action<RichTextBox, string, Color>(AppendText), richTextBox, textLine, color);
                }
                else
                {
                    richTextBox.AppendColoredText(textLine, color);
                }
            }
        }

        public static void WriteLineAsync(this RichTextBox richTextBox, IEnumerable<string> text)
        {
            foreach (string textLine in text)
            {
                if (richTextBox.InvokeRequired)
                {
                    richTextBox.Invoke(new Action<RichTextBox, string, Color>(AppendText), richTextBox, textLine, Color.Lime);
                }
                else
                {
                    richTextBox.AppendColoredText(textLine, Color.Lime);
                }
            }
        }

        public static void WriteLineAsync(this RichTextBox richTextBox, string text)
        {
            if (richTextBox.InvokeRequired)
            {
                richTextBox.Invoke(new Action<RichTextBox, string, Color>(AppendText), richTextBox, text, Color.Lime);
            }
            else
            {
                //richTextBox.SelectionStart = richTextBox.TextLength;
                //richTextBox.SelectionLength = 0;

                richTextBox.AppendColoredText(text, Color.Lime);
                //richTextBox.SelectionAlignment = HorizontalAlignment.Right;
            }

        }

        private static void AppendColoredText(this RichTextBox richTextBox, string text, Color color)
        {
            AppendText(richTextBox, text, color);
        }

        private static void AppendText(RichTextBox richTextBox, string text, Color color)
        {
            richTextBox.SelectionStart = richTextBox.TextLength;
            richTextBox.SelectionLength = 0;
            richTextBox.SelectionColor = color;
            richTextBox.AppendText("> " + text + "\r\n");
            richTextBox.SelectionColor = richTextBox.ForeColor;
            richTextBox.ScrollToCaret();
        }
    }
}
