// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalPropertyItemFactory.cs" company="PropertyTools">
//   Copyright (c) 2014 PropertyTools contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace WHS.DEVICE.MAPDESIGN
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using PropertyTools.Wpf;
    using WHS.DEVICE.MAPDESIGN.Controls;

    public class CustomOperator : PropertyGridOperator
    {
        public override IEnumerable<Tab> CreateModel(object instance, bool isEnumerable, IPropertyGridOptions options)
        {
            return base.CreateModel(instance, isEnumerable, options);
        }

        public override PropertyItem CreatePropertyItem(PropertyDescriptor pd, PropertyDescriptorCollection propertyDescriptors, object instance)
        {
            return base.CreatePropertyItem(pd, propertyDescriptors, instance);
        }

        protected override IEnumerable<PropertyItem> CreatePropertyItems(object instance, IPropertyGridOptions options)
        {
            return base.CreatePropertyItems(instance, options);
        }

        protected override PropertyItem CreateCore(PropertyDescriptor pd, PropertyDescriptorCollection propertyDescriptors)
        {
            //if (pd.ComponentType == typeof(CustomLine) || pd.ComponentType == typeof(CustomEllipse))
            //{
            //    return new PropertyItem(pd, propertyDescriptors);
            //}
            //return null;
            return base.CreateCore(pd, propertyDescriptors);
        }
    }
}