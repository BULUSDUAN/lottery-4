using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Colin.Lottery.Models;
using Xamarin.Forms;

namespace Colin.Lottery.MobileApp.Expands.Convertors
{
    public class PlanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value.ToString().Substring(4);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class PeriodConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            $"{long.Parse(value.ToString()) + 1}期";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class ForcastCnt2EnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (value as IEnumerable<JinMaForcastModel>)?.Any() ?? false;


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class IsWinConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isWin = value as bool?;
            return isWin == null ? "开奖中" : ((bool) isWin ? "中" : "挂");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
    
    public class IsWin2ColorConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isWin = value as bool?;
            return isWin == false ? Color.Green : Color.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}