﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LengthConverter.cs" company="PropertyTools">
//   Copyright (c) 2014 PropertyTools contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace WHS.DEVICE.MAPDESIGN.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    using WHS.DEVICE.MAPDESIGN.Models;

    [ValueConversion(typeof(Length), typeof(string))]
    public class LengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            try
            {
                var length = (Length)value;
                return length.ToString(string.Empty, culture);
            }

            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            try
            {
                var s = (string)value;
                return Length.Parse(s, culture);
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}