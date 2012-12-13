using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WP_TextBlockPP
{
    public class TexBlockPP_notused : ContentControl
    {
        private System.Windows.Controls.TextBlock displayTextBlock;
        private System.Windows.Controls.TextBlock measureTextBlock;

        public string DisplayedText { get; private set; }

        #region Text

        /// <summary>
        /// Text Dependency Property
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TexBlockPP_notused),
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
            TexBlockPP_notused target = (TexBlockPP_notused)d;
            string oldValue = (string)e.OldValue;
            string newValue = target.Text;
            target.OnTextChanged(oldValue, newValue);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Text property.
        /// </summary>
        protected virtual void OnTextChanged(string oldValue, string newValue)
        {
            DisplayedText = AutoEllipsis.Ellipsis.Compact(newValue, MeasureText, this.ActualWidth, Ellipsis);
        }

        #endregion

        #region Ellipsis

        /// <summary>
        /// Ellipsis Dependency Property
        /// </summary>
        public static readonly DependencyProperty EllipsisProperty =
            DependencyProperty.Register("Ellipsis", typeof(AutoEllipsis.EllipsisFormat), typeof(TexBlockPP_notused),
                new PropertyMetadata(AutoEllipsis.EllipsisFormat.None,
                    new PropertyChangedCallback(OnEllipsisChanged)));

        /// <summary>
        /// Gets or sets the Ellipsis property. This dependency property 
        /// indicates ....
        /// </summary>
        public AutoEllipsis.EllipsisFormat Ellipsis
        {
            get { return (AutoEllipsis.EllipsisFormat)GetValue(EllipsisProperty); }
            set { SetValue(EllipsisProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Ellipsis property.
        /// </summary>
        private static void OnEllipsisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TexBlockPP_notused target = (TexBlockPP_notused)d;
            AutoEllipsis.EllipsisFormat oldValue = (AutoEllipsis.EllipsisFormat)e.OldValue;
            AutoEllipsis.EllipsisFormat newValue = target.Ellipsis;
            target.OnEllipsisChanged(oldValue, newValue);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Ellipsis property.
        /// </summary>
        protected virtual void OnEllipsisChanged(AutoEllipsis.EllipsisFormat oldValue, AutoEllipsis.EllipsisFormat newValue)
        {
        }

        #endregion

        public TexBlockPP_notused()
        {
            displayTextBlock = new System.Windows.Controls.TextBlock();
            measureTextBlock = new System.Windows.Controls.TextBlock();
            Content = displayTextBlock;
            SetStyleToTextBlock(displayTextBlock);
            SetStyleToTextBlock(measureTextBlock);
            Text = "no text yet";
        }

        private void SetStyleToTextBlock(System.Windows.Controls.TextBlock tb)
        {            
            tb.FontFamily = this.FontFamily;
            tb.FontSize = this.FontSize;
            tb.FontStretch = this.FontStretch;
            tb.FontStyle = this.FontStyle;
            tb.FontWeight = this.FontWeight;
        }

        private Size MeasureText(string text)
        {
            measureTextBlock.Text = text;
            measureTextBlock.InvalidateMeasure();
            measureTextBlock.InvalidateArrange();
            return new Size(measureTextBlock.ActualWidth, measureTextBlock.ActualHeight);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            displayTextBlock.Measure(availableSize);
            return MeasureText(Text);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            displayTextBlock.Arrange(new Rect(new Point(), finalSize));
            return MeasureText(Text);
        }
    }
}
