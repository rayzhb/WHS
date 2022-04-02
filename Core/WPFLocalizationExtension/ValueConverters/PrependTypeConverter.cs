﻿#region Copyright information
// <copyright file="BLoc.cs">
//     Licensed under Microsoft Public License (Ms-PL)
//     https://github.com/XAMLMarkupExtensions/WPFLocalizationExtension/blob/master/LICENSE
// </copyright>
// <author>Konrad Mattheis</author>
#endregion

namespace WPFLocalizeExtension.ValueConverters
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Data;
    using System.Windows.Markup;
    #endregion

    /// <summary>
    /// PrependTypeConverter allows to prepend the type of the value as string with the default _ separator. To change the default separator just us the converterparamater
    /// </summary>
    public class PrependTypeConverter : TypeValueConverterBase, IValueConverter
    {
        #region IValueConverter
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var separator = "_";
                if (parameter != null && parameter.GetType() == typeof(string))
                    separator = parameter.ToString();
                return value.GetType().Name + separator + value.ToString();
            }

            return null;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        } 
        #endregion
    }
}
