
namespace WpfApp2
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using WPF_program.Controllers;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Console.WriteLine("test 1");
            Controller.setWindow(this);
            Controller.InitializeSentenceExamples();
            Controller.ShowSomeRandomWords();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e) => Controller.ShowResult();
        private void RandomButton_Click(object sender, RoutedEventArgs e) => Controller.ShowSomeRandomWords();
        private void SearchBar_KeyUp(object sender, KeyEventArgs e) => Controller.ShowResult(e.Key);

    }
}
