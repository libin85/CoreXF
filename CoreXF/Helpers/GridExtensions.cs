using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CoreXF
{
    public static class GridExtensions
    {
        public static Grid AddView(this Grid grid,View view,int Row = 0,int Column = 0, int RowSpan = 0,int ColumnSpan = 0,bool FillRows = false,bool FillColumns = false)
        {
            if (FillRows)
            {
                Row = 0;
                RowSpan = grid.RowDefinitions.Count;
            }
            if (FillColumns)
            {
                Column = 0;
                ColumnSpan = grid.ColumnDefinitions.Count;
            }

            grid.Children.Add(view,
                left: Column,
                right: Column + (ColumnSpan > 1 ? ColumnSpan :  1),
                top: Row,
                bottom: Row + (RowSpan > 1 ? RowSpan : 1)
                );
            
            return grid;
        }
    }
}
