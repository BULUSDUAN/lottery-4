using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Colin.Lottery.Models;
using Xamarin.Forms;

namespace Colin.Lottery.MobileApp.Expands.Convertors
{
    public class PlanConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value.ToString().Substring(4);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class PeriodConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            $"{long.Parse(value.ToString()) + 1}期";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
    
    public class ForcastCnt2Enable:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (value as IEnumerable<JinMaForcastModel>)?.Any() ?? false;
        

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}