using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using WPF_program.Models;

namespace WPF_program.Ui_Factory
{
    public static class UiFactory
    {
        public static Border CreateWordBox((SolidColorBrush, string) posTuple, string simp)
        {
            TextBlock wBlock = new TextBlock
            {
                FontSize = 32,
                Foreground = posTuple.Item1,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(5),
                Text = simp,
            };
            Border wordBorder = new Border
            {
                BorderBrush = posTuple.Item1,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(5)
            };
            TextBlock posBox = new TextBlock
            {
                FontSize = 18,
                Foreground = posTuple.Item1,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, -5, 0, 0),
                Text = $"{posTuple.Item2}"
                //Text = $"{posTuple.Item2} \n {detailedWord.AllPos} \n {detailedWord.AllPosFreq}", //modify
            };
            StackPanel wordPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
            };

            wordPanel.Children.Add(wBlock);
            wordPanel.Children.Add(posBox);
            wordBorder.Child = wordPanel;

            return wordBorder;
        }
    }
}
