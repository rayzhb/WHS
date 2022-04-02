﻿#region Copyright information
// <copyright file="RegisterMissingTypeConverters.cs">
//     Licensed under Microsoft Public License (Ms-PL)
//     https://github.com/XAMLMarkupExtensions/WPFLocalizationExtension/blob/master/LICENSE
// </copyright>
// <author>Uwe Mayer</author>
#endregion

namespace WPFLocalizeExtension.TypeConverters
{
    #region Usings
    using System.ComponentModel;
    using System.Windows.Media.Imaging;
    #endregion

    /// <summary>
    /// Register missing type converters here.
    /// </summary>
    public static class RegisterMissingTypeConverters
    {
        /// <summary>
        /// A flag indication if the registration was successful.
        /// </summary>
        private static bool _registered;

        /// <summary>
        /// Registers the missing type converters.
        /// </summary>
        public static void Register()
        {
            if (_registered)
                return;

            TypeDescriptor.AddAttributes(typeof(BitmapSource), new TypeConverterAttribute(typeof(BitmapSourceTypeConverter)));

            _registered = true;
        }
    }
}
