//MIT License

//Copyright (c) 2024 - Tomás Busch

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace DynamicGrid.Avalonia
{

    public enum AutoGrid
    {
        Auto,
        MinContent,
        MaxContent,

    }

    /// <summary>
    /// Responsive auto ajustable Grid based on UniformGrid from Avalonia
    /// </summary>
    public partial class DynamicGrid : Panel
    {
        /// <summary>
        /// Defines the <see cref="AutoRows"/> property.
        /// </summary>
        public static readonly StyledProperty<int> AutoRowsProperty =
            AvaloniaProperty.Register<DynamicGrid, int>(nameof(AutoRows));

        /// <summary>
        /// Defines the <see cref="AutoColumns"/> property.
        /// </summary>
        public static readonly StyledProperty<int> AutoColumnsProperty =
            AvaloniaProperty.Register<DynamicGrid, int>(nameof(AutoColumns));

        /// <summary>
        /// Defines the <see cref="RowGap"/> property.
        /// </summary>
        public static readonly StyledProperty<int> RowGapProperty =
            AvaloniaProperty.Register<DynamicGrid, int>(nameof(RowGap), 0);

        /// <summary>
        /// Defines the <see cref="ColumnGap"/> property.
        /// </summary>
        public static readonly StyledProperty<int> ColumnGapProperty =
            AvaloniaProperty.Register<DynamicGrid, int>(nameof(ColumnGap));

        private int _rows;
        private int _columns;

        private int _autoRows;
        private int _autoColumns;

        private int _rowGap;
        private int _columnGap;

        private float[] _rowHeights = [];
        private float[] _columnWidths = [];


        static DynamicGrid()
        {
            AffectsMeasure<DynamicGrid>(
                AutoRowsProperty,
                AutoColumnsProperty,
                RowGapProperty
                );
        }

        /// <summary>
        /// Specifies the automatic size of all implicit rows. If set to 0 uses the size of the tallest element.
        /// </summary>
        public int AutoRows
        {
            get => GetValue(AutoRowsProperty);
            set => SetValue(AutoRowsProperty, value);
        }

        /// <summary>
        /// Specifies the automatic size of all implicit columns. If set to 0 uses the size of the widest element.
        /// </summary>
        public int AutoColumns
        {
            get => GetValue(AutoColumnsProperty);
            set => SetValue(AutoColumnsProperty, value);
        }

        /// <summary>
        /// Specifies the row gap size between grid items.
        /// </summary>
        public int RowGap
        {
            get => GetValue(RowGapProperty);
            set => SetValue(RowGapProperty, value);
        }

        /// <summary>
        /// Specifies the column gap size between grid items.
        /// </summary>
        public int ColumnGap
        {
            get => GetValue(ColumnGapProperty);
            set => SetValue(ColumnGapProperty, value);
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
            this.InvalidateMeasure();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            UpdateRowsAndColumns(availableSize);

            var maxWidth = 0d;
            var maxHeight = 0d;

            Size childAvailableSize = new Size(
                ((availableSize.Width - (_columnGap * (_columns - 1))) / _columns),
                ((availableSize.Height - (_rowGap * (_rows - 1))) / _rows)
                );

            foreach (var child in Children)
            {
                if (!child.IsVisible)
                {
                    continue;
                }

                child.Measure(childAvailableSize);

                if (child.DesiredSize.Width > maxWidth)
                {
                    maxWidth = child.DesiredSize.Width;
                }

                if (child.DesiredSize.Height > maxHeight)
                {
                    maxHeight = child.DesiredSize.Height;
                }
            }

            return new Size(childAvailableSize.Width * _columns + (_columnGap * (_columns - 1)), maxHeight * _rows + (_rowGap * (_rows - 1)));
        }

        protected override Size ArrangeOverride(Size availableSize)
        {
            var x = 0;
            var y = 0;

            var maxChildWidth = 0d;
            var maxChildHeight = 0d;

            //TODO: Calculate per row
            foreach (var child in Children)
            {
                if (child.IsVisible)
                {
                    if (child.DesiredSize.Width > maxChildWidth)
                    {
                        maxChildWidth = child.DesiredSize.Width;
                    }

                    if (child.DesiredSize.Height > maxChildHeight)
                    {
                        maxChildHeight = child.DesiredSize.Height;
                    }
                }
            }

            var width = ((availableSize.Width - (_columnGap * (_columns - 1))) / _columns);
            var height = maxChildHeight;

            foreach (var child in Children)
            {
                if (!child.IsVisible)
                {
                    continue;
                }

                var colGap = 0f;
                if (x != 0)
                {
                    colGap = _columnGap;
                }

                var rowGap = 0f;
                if (y != 0)
                {
                    rowGap = _rowGap;
                }

                child.Arrange(new Rect(x * (width + colGap), y * (height + rowGap), width, height));

                x++;
                if (x >= _columns)
                {
                    x = 0;
                    y++;
                }
            }

            return new Size(availableSize.Width + 500, availableSize.Height);
        }

        private void UpdateRowsAndColumns(Size availableSize)
        {
            _rows = 1;
            _columns = 1;
            _columnGap = ColumnGap;
            _rowGap = RowGap;

            var itemCount = 0;

            foreach (var child in Children)
            {
                if (child.IsVisible)
                {
                    itemCount++;
                }
            }

            _columns = CalculateColumnCount(availableSize);
            _rows = Math.Max((int)Math.Ceiling((double)itemCount / _columns), 1);
        }

        private int CalculateColumnCount(Size availableSize)
        {

            float maxChildWidth = 0f;
            float maxChildHeight = 0f;

            foreach (var child in Children)
            {
                if (child.IsVisible)
                {
                    if (child.DesiredSize.Width > maxChildWidth)
                    {
                        maxChildWidth = (float)child.DesiredSize.Width;
                    }

                    if (child.DesiredSize.Height > maxChildHeight)
                    {
                        maxChildHeight = (float)child.DesiredSize.Height;
                    }
                }
            }

            int finalColumnCount = Math.Max((int)Math.Floor(availableSize.Width / maxChildWidth), 1);

            bool fits = false;

            do
            {
                float sizeWithGap = ((maxChildWidth + _columnGap) * (finalColumnCount - 1)) + maxChildWidth;

                if (sizeWithGap > availableSize.Width)
                {
                    if (finalColumnCount != 1)
                    {
                        finalColumnCount--;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    fits = true;
                }
            }
            while (!fits);

            return finalColumnCount;
        }


        private int CalculateColumns(Size availableSize)
        {

            float maxChildWidth = 0f;
            float maxChildHeight = 0f;




            foreach (var child in Children)
            {
                if (child.IsVisible)
                {
                    if (child.DesiredSize.Width > maxChildWidth)
                    {
                        maxChildWidth = (float)child.DesiredSize.Width;
                    }

                    if (child.DesiredSize.Height > maxChildHeight)
                    {
                        maxChildHeight = (float)child.DesiredSize.Height;
                    }
                }
            }

            int finalColumnCount = Math.Max((int)Math.Floor(availableSize.Width / maxChildWidth), 1);

            bool fits = false;

            do
            {
                float sizeWithGap = ((maxChildWidth + _columnGap) * (finalColumnCount - 1)) + maxChildWidth;

                if (sizeWithGap > availableSize.Width)
                {
                    if (finalColumnCount != 1)
                    {
                        finalColumnCount--;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    fits = true;
                }
            }
            while (!fits);

            return finalColumnCount;
        }
    }
}


