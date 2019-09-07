using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace App1
{
    public class TooManyItems: ObservableCollection<int>, ISupportIncrementalLoading
    {
        const int maxCount = 100000;

        IAsyncOperation<LoadMoreItemsResult> ISupportIncrementalLoading.LoadMoreItemsAsync(uint count)
        {
            var dispatcher = Window.Current.Dispatcher;

            return Task.Run(async () =>
            {
                Debug.WriteLine($"Load more {count}!");

                int limit = maxCount - Count;
                int increment = Math.Min((int)count, limit);

                // This should be awaitable and not block UI
                List<int> items = Enumerable.Range(this.LastOrDefault() + 1, (int)increment).ToList();

                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => items.ForEach(Add));

                return new LoadMoreItemsResult() { Count = (uint)items.Count };
            }).AsAsyncOperation();
        }

        bool ISupportIncrementalLoading.HasMoreItems => this.LastOrDefault() < maxCount;
    }
}
