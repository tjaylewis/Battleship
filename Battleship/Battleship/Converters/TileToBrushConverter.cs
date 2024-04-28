using Battleship.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Battleship.Converters
{
    class TileToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is State)
            {
                switch (value as State?)
                {
                    case State.Water:
                        return new SolidColorBrush(Colors.Blue);
                    case State.Ship:
                        return new SolidColorBrush(Colors.Gray);
                    case State.Hit:
                        return new SolidColorBrush(Colors.Red);
                    case State.Miss:
                        return new SolidColorBrush(Colors.White);
                    case State.Ship & State.Water:
                        return new SolidColorBrush(Colors.Blue);
                }
            }            
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
