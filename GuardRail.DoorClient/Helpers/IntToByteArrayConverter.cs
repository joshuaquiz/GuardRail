using System;
using System.ComponentModel;
using System.Globalization;

namespace GuardRail.DoorClient.Helpers;

public sealed class IntToByteArrayConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(int) || base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        object result = null;

        if (value is int intValue)
        {
            result = new[] { Convert.ToByte(intValue) };
        }

        return result ?? base.ConvertFrom(context, culture, value);
    }
}