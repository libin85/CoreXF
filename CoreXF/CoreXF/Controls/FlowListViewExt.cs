
using CoreXF;
using DLToolkit.Forms.Controls;
using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace CoreXF
{
    public class FlowListViewExt : FlowListView
    {
        bool _subscribtionOnFlowItemAppearing;

        public static BindableProperty FlowLastItemAppearingCommandProperty = BindableProperty.Create(nameof(FlowLastItemAppearingCommand), typeof(ICommand), typeof(FlowListViewExt), null);
        public ICommand FlowLastItemAppearingCommand
        {
            get => (ICommand)GetValue(FlowLastItemAppearingCommandProperty); 
            set => SetValue(FlowLastItemAppearingCommandProperty, value); 
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(FlowListViewExt.FlowLastItemAppearingCommand))
            {
                if (FlowLastItemAppearingCommand == null && _subscribtionOnFlowItemAppearing)
                {
                    FlowItemAppearing -= FlowListViewExt_FlowItemAppearing;
                    return;
                }

                if (!_subscribtionOnFlowItemAppearing)
                {
                    FlowItemAppearing += FlowListViewExt_FlowItemAppearing;
                    _subscribtionOnFlowItemAppearing = true;
                }
            }

        }

        private void FlowListViewExt_FlowItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (FlowLastItemAppearingCommand == null || e.Item == null || FlowItemsSource == null || FlowItemsSource.Count == 0)
                return;

            IList list = FlowItemsSource as IList;
            if (list == null)
                return;

            if(e.Item == list[FlowItemsSource.Count - 1])
            {
                FlowLastItemAppearingCommand?.Execute(e.Item);
            }
        }

        protected override void UnhookContent(Cell content)
        {
            base.UnhookContent(content);

            ViewCell viewCell = content as ViewCell;
            if (viewCell != null)
            {
                viewCell.View?.DisposeAllViews();
            }
                
        }

    }
}
