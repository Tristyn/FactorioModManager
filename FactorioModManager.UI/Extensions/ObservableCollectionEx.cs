using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FactorioModManager.UI.Extensions
{
    public static class ObservableCollectionEx
    {
        public static void AddRange<T>(this ObservableCollection<T> list, IEnumerable<T> add)
        {
            
            foreach (var item in add)
                list.Add(item);
        }
    }
}
