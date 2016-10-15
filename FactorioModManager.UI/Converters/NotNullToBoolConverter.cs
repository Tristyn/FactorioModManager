using System;
using Avalonia.Controls;
using OmniXaml;

namespace FactorioModManager.UI.Converters
{
    class NotNullToBoolConverter : IValueConverter<object, bool>
    {
        public bool Convert(object source)
        {
            return source != null;
        }

        /// <exception cref="NotImplementedException"></exception>
        public object Convert(bool source)
        {
            throw new NotImplementedException();
        }
    }
}
