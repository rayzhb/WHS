﻿#region Copyright information
// <copyright file="LocBinding.cs">
//     Licensed under Microsoft Public License (Ms-PL)
//     https://github.com/XAMLMarkupExtensions/WPFLocalizationExtension/blob/master/LICENSE
// </copyright>
// <author>Uwe Mayer</author>
#endregion

namespace WPFLocalizeExtension.Deprecated.Engine
{
    #region Usings
    using System;
    using System.Windows;
    using System.Windows.Data;
    using WPFLocalizeExtension.Extensions;
    #endregion

    /// <summary>
    /// A binding proxy class that accepts bindings and forwards them to the LocExtension.
    /// Based on: http://www.codeproject.com/Articles/71348/Binding-on-a-Property-which-is-not-a-DependencyPro
    /// </summary>
    [Obsolete("LocBinding is deprecated and will be removed in version 4.0, because lex:Loc supports now direct Binding, see documentation", false)]
    public class LocBinding : FrameworkElement
    {
        #region Source DP
        /// <summary>
        /// We don't know what will be the Source/target type so we keep 'object'.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(object), typeof(LocBinding),
            new FrameworkPropertyMetadata(OnPropertyChanged)
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });

        /// <summary>
        /// The source.
        /// </summary>
        public object Source
        {
            get => GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }
        #endregion

        #region Target LocExtension
        private LocExtension _target;
        /// <summary>
        /// The target extension.
        /// </summary>
        public LocExtension Target
        {
            get => _target;
            set
            {
                _target = value;
                if (_target != null && Source != null)
                    _target.Key = Source.ToString();
            }
        }
        #endregion

        #region OnPropertyChanged
        private static void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is LocBinding locBinding && args.Property == SourceProperty)
            {
                if (!ReferenceEquals(locBinding.Source, locBinding._target) && locBinding._target != null && locBinding.Source != null)
                    locBinding._target.Key = locBinding.Source.ToString();
            }
        }
        #endregion
    }
}
