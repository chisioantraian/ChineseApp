
namespace WpfApp2
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using WPF_program.Controllers;
    using WPF_program.Logic;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Console.WriteLine("test 9");
            ChineseService.InitializeData();
            Controller.setWindow(this);
            Controller.InitializeSentenceExamples();
            Controller.ShowSomeRandomWords();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e) => Controller.ShowResult();
        
        private void RandomButton_Click(object sender, RoutedEventArgs e) => Controller.ShowSomeRandomWords();
        
        private void SearchBar_KeyUp(object sender, KeyEventArgs e) => Controller.ShowResult(e.Key);

        private void caracterPronuntie_MouseEnter(object sender, MouseEventArgs e)
        {
            var textBlock = (TextBlock)sender;
            ZoomedCharacterBox.Text = textBlock.Text;
            Controller.ShowWordWithThisCharacter(textBlock.Text[0]);
        }

        private void caracterPronuntie_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var textBlock = (TextBlock)sender;
            SearchBar.Text = textBlock.Text;
            Controller.ShowChineseResult();
        }


    }
}
