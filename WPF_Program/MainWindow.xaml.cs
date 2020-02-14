using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChineseAppWPF.Controllers;
using ChineseAppWPF.Logic;

namespace ChineseAppWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Console.WriteLine("test 9");
            ChineseService.InitializeData();
            Decomposition.BuildDecompositionDict();
            Controller.SetWindow(this);
            Controller.InitializeSentenceExamples();
            Controller.ShowSomeRandomWords();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e) => Controller.ShowResult();

        private void RandomButton_Click(object sender, RoutedEventArgs e) => Controller.ShowSomeRandomWords();

        private void SearchBar_KeyUp(object sender, KeyEventArgs e) => Controller.ShowResult();

        private void CharacterAndPinyin_MouseEnter(object sender, MouseEventArgs e)
        {
            var textBlock = (TextBlock)sender;
            ZoomedCharacterBox.Text = textBlock.Text;
            Controller.ShowWordWithThisCharacter(textBlock.Text[0]);
        }

        private void CharacterAndPinyin_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var textBlock = (TextBlock)sender;
            SearchBar.Text = textBlock.Text;
            Controller.ShowChineseResult();
        }
    }
}
