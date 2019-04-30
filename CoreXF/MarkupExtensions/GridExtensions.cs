
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoreXF
{

    [ContentProperty(nameof(Rows))]
    public class RowsExtension : IMarkupExtension
    {
        public string Rows { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            RowDefinitionCollection rowCollection = new RowDefinitionCollection();

            if (!string.IsNullOrEmpty(Rows))
            {
                var col = Rows.ToLower().Split(',');
                foreach (var cl in col)
                {
                    if (cl == "*")
                        rowCollection.Add(new RowDefinition { Height = GridLength.Star });

                    if (cl == "auto")
                        rowCollection.Add(new RowDefinition { Height = GridLength.Auto });

                    if(int.TryParse(cl,out int integer))
                    {
                        rowCollection.Add(new RowDefinition { Height = integer });
                    }
                }
            }

            return rowCollection;
        }
    }

    [ContentProperty(nameof(Columns))]
    public class ColumnsExtension : IMarkupExtension
    {
        public string Columns { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            ColumnDefinitionCollection columnCollection = new ColumnDefinitionCollection();

            if (!string.IsNullOrEmpty(Columns))
            {
                var col = Columns.ToLower().Split(',');
                foreach (var cl in col)
                {
                    if (cl == "*")
                        columnCollection.Add(new ColumnDefinition { Width = GridLength.Star });

                    if (cl == "auto")
                        columnCollection.Add(new ColumnDefinition { Width = GridLength.Auto });

                    if (int.TryParse(cl, out int integer))
                    {
                        columnCollection.Add(new ColumnDefinition { Width = integer });
                    }

                }
            }

            return columnCollection;
        }
    }

}
