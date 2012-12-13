using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WP_TextBlockPP
{
    public class TexBlock : UserControl
    {
        private System.Windows.Controls.TextBlock measureTextBlock;
        private System.Windows.Controls.TextBlock displayTextBlock;

        private string displayedText ;

        public string DisplayedText 
        {
            get { return displayedText ; }
            set
            {
                displayedText = value;
                if (displayTextBlock != null)
                    displayTextBlock.Text = value;
            }
        }
        


        #region Text

        /// <summary>
        /// Text Dependency Property
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TexBlock),
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
            TexBlock target = (TexBlock)d;
            string oldValue = (string)e.OldValue;
            string newValue = target.Text;
            target.OnTextChanged(oldValue, newValue);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Text property.
        /// </summary>
        protected virtual void OnTextChanged(string oldValue, string newValue)
        {
            var tmp = AutoEllipsis.Ellipsis.Compact(newValue, MeasureText, this.ActualWidth, Ellipsis);
            DisplayedText = tmp;
        }

        #endregion

        #region Ellipsis

        /// <summary>
        /// Ellipsis Dependency Property
        /// </summary>
        public static readonly DependencyProperty EllipsisProperty =
            DependencyProperty.Register("Ellipsis", typeof(AutoEllipsis.EllipsisFormat), typeof(TexBlock),
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
            TexBlock target = (TexBlock)d;
            AutoEllipsis.EllipsisFormat oldValue = (AutoEllipsis.EllipsisFormat)e.OldValue;
            AutoEllipsis.EllipsisFormat newValue = target.Ellipsis;
            target.OnEllipsisChanged(oldValue, newValue);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Ellipsis property.
        /// </summary>
        protected virtual void OnEllipsisChanged(AutoEllipsis.EllipsisFormat oldValue, AutoEllipsis.EllipsisFormat newValue)
        {
            UpdateDisplayText();
        }

        #endregion

        public TexBlock()
        {
            this.Loaded += TexBlock_Loaded;
            measureTextBlock = new System.Windows.Controls.TextBlock();
            displayTextBlock = new System.Windows.Controls.TextBlock();
            this.Content = displayTextBlock;
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

        private void UpdateDisplayText()
        {
            var tmp = AutoEllipsis.Ellipsis.Compact(Text, MeasureText, this.ActualWidth, Ellipsis);
            DisplayedText = tmp;
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
            if (displayTextBlock!=null)
                displayTextBlock.Measure(availableSize);
            return SetWidth(MeasureText(Text),availableSize.Width);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (displayTextBlock != null)
                displayTextBlock.Arrange(new Rect(new Point(), finalSize));
            return finalSize;
        }

        private Size SetWidth(Size size, double newWidth)
        {
            return new Size(newWidth, size.Height);
        }
    }
}
