using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WP_TextBlockPP
{
    public partial class PagedTextBlock : UserControl
    {
        private System.Windows.Controls.TextBlock measureTextBlock;
        private System.Windows.Controls.TextBlock displayTextBlock;

        private string displayedText;

        private readonly char[] WordBoundaries = new char[] { ' ', '\t', '.', ',', '!', '?', '\n', '\r' };

        public string DisplayedText
        {
            get { return displayedText; }
            private set
            {
                displayedText = value;
                if (displayTextBlock != null)
                    displayTextBlock.Text = value;
            }
        }

        private int displayPosition = 0;

        #region Text

        /// <summary>
        /// Text Dependency Property
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(PagedTextBlock),
                new PropertyMetadata((string)string.Empty,
                    new PropertyChangedCallback(OnTextChanged)));

        /// <summary>
        /// Gets or sets the Text property. This dependency property 
        /// indicates the text to display.
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Text property.
        /// </summary>
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PagedTextBlock target = (PagedTextBlock)d;
            string oldValue = (string)e.OldValue;
            string newValue = target.Text;
            target.OnTextChanged(oldValue, newValue);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Text property.
        /// </summary>
        protected virtual void OnTextChanged(string oldValue, string newValue)
        {
            UpdateDisplayText();
        }

        #endregion

        public PagedTextBlock()
        {
            this.Loaded += TexBlock_Loaded;
            measureTextBlock = new System.Windows.Controls.TextBlock();
            displayTextBlock = new System.Windows.Controls.TextBlock();
            measureTextBlock.TextWrapping = displayTextBlock.TextWrapping = TextWrapping.Wrap;
            this.Content = displayTextBlock;
        }

        public void NextPage()
        {
            var nextPos = displayPosition + DisplayedText.Length;
            if (nextPos<Text.Length - 1)
            {
                displayPosition = nextPos;
            }
            UpdateDisplayText();
        }

        private void UpdateDisplayText()
        {
            if (this.ActualHeight == 0.0)
                return;

            while (char.IsWhiteSpace(Text[displayPosition]) && displayPosition < Text.Length-1)
            {
                displayPosition++;
            }

            if (displayPosition > Text.Length - 2)
                return;

            int start = displayPosition;
            var dispText = GrowText(Text, start, start);
            bool growing = true;
            while (true)
            {
                var realHeight = MeasureTextHeight(dispText);
                if (realHeight < this.ActualHeight && growing)
                {
                    var oldLength = dispText.Length;
                    dispText = GrowText(Text, start, start + dispText.Length + 1);

                    if (dispText.Length == oldLength)
                        break;
                }
                else if (realHeight > this.ActualHeight)
                {
                    growing = false;
                    dispText = ShortenText(dispText);
                }
                else
                {
                    break;
                }
            }
            DisplayedText = dispText;
        }

        public void PreviousPage()
        {
            UpdateDisplayTextReverse();
        }

        private void UpdateDisplayTextReverse()
        {
            if (this.ActualHeight == 0.0)
                return;

            int end = displayPosition;
            while (char.IsWhiteSpace(Text[end]) && end > 0)
            {
                end--;
            }

            if (end == 0)
                return;

            var dispText = GrowTextLeft(Text, end-1, end);
            bool growing = true;
            while (true)
            {
                var realHeight = MeasureTextHeight(dispText);
                if (realHeight < this.ActualHeight && growing)
                {
                    dispText = GrowTextLeft(Text, end-dispText.Length-1, end);

                    if (displayPosition - dispText.Length == 0)
                        break;
                }
                else if (realHeight > this.ActualHeight)
                {
                    growing = false;
                    dispText = ShortenTextLeft(dispText);
                }
                else
                {
                    break;
                }
            }
            displayPosition -= dispText.Length;
            DisplayedText = dispText;
        }

        private int CalcMaxLettersPerPage()
        {
            return (int)((this.ActualWidth / 2) * (this.ActualHeight / (this.FontSize + 1)));
        }

        private string GrowText(string text, int start, int searchStart)
        {
            if (searchStart >= text.Length)
                return text.Substring(start);
            
            var firstWordSeparator = text.IndexOfAny(WordBoundaries, searchStart);
            return text.Substring(start, firstWordSeparator - start + 1);
        }

        private string GrowTextLeft(string text, int searchStart, int end)
        {
            if (searchStart <= 0)
                return text.Substring(0, end + 1);

            int firstWordSeparator;
            var tmpSearchStart = searchStart;
            do
            {
                firstWordSeparator = text.LastIndexOfAny(WordBoundaries, tmpSearchStart--);
            } while (searchStart == firstWordSeparator);
            return text.Substring(firstWordSeparator + 1, end - (firstWordSeparator + 1));
        }

        private string ShortenText(string text)
        {
            int start = 0;
            int length = text.Length;

            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var lastWordSeparator = text.LastIndexOfAny(WordBoundaries, start + length - 1, length);
            if (lastWordSeparator > 0 && lastWordSeparator + 1 != start + length)
            {
                length = lastWordSeparator - start + 1;
            }
            else
            {
                length = length - 1;
            }
            return text.Substring(start, length);
        }

        private string ShortenTextLeft(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }
            int start;
            var lastWordSeparator = text.IndexOfAny(WordBoundaries, 0);
            if (lastWordSeparator > 0 && lastWordSeparator != text.Length-1)
            {
                start = lastWordSeparator + 1;
            }
            else
            {
                start = 1;
            }
            return text.Substring(start);
        }

        void TexBlock_Loaded(object sender, RoutedEventArgs e)
        {
            //displayTextBlock = (System.Windows.Controls.TextBlock)this.Content;
            //SetStyleToTextBlock(displayTextBlock);
            SetStyleToTextBlock(displayTextBlock);
            SetStyleToTextBlock(measureTextBlock);
            UpdateDisplayText();
        }

        private void SetStyleToTextBlock(System.Windows.Controls.TextBlock tb)
        {
            tb.FontFamily = this.FontFamily;
            tb.FontSize = this.FontSize;
            tb.FontStretch = this.FontStretch;
            tb.FontStyle = this.FontStyle;
            tb.FontWeight = this.FontWeight;
        }

        private double MeasureTextHeight(string text)
        {
            measureTextBlock.Text = text;
            measureTextBlock.Width = this.ActualWidth;
            measureTextBlock.InvalidateMeasure();
            measureTextBlock.InvalidateArrange();
            return measureTextBlock.ActualHeight;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (displayTextBlock != null)
                displayTextBlock.Measure(availableSize);
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (displayTextBlock != null)
                displayTextBlock.Arrange(new Rect(new Point(), finalSize));
            return finalSize;
        }
    }
}
