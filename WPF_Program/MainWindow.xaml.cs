
namespace WpfApp2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using WpfApp2.Models;
    using WpfApp2.Logic;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<Word> words;
        private readonly Dictionary<char, List<char>> dict;

        public MainWindow()
        {
            Console.WriteLine("Beginning of constructor");
            InitializeComponent();
            words = ChineseService.GetWordsFromDatabase();
            dict = ChineseService.GetCharacterDecomposition();
            Console.WriteLine($"number of words: {words.Count}");
            Console.WriteLine($"number of lines(dict): {dict.Count}");
            InitializeWordsPanel();
            //InitializeExamples();
            Console.WriteLine("End of constructor");
        }

        public void InitializeWordsPanel()
        {
            var wordsExample = words
                .Where(w => w.Definitions.Contains("mountain"))
                .ToList();
            UpdateShownWords(wordsExample);
        }

        //TODO change
        private void SearchBar_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            ComboBoxItem typeItem = (ComboBoxItem)InputComboBox.SelectedItem;
            string value = typeItem.Content.ToString();

            switch (value)
            {
                case "Chinese": ShowChineseResult(); break;
                case "English": ShowEnglishResult(); break;
                case "Pronounciation": ShowPronounciationResult(); break;
                case "Compose": ShowComposeResult(); break;
            }
        }

        private void ShowChineseResult()
        {
            string searchInput = SearchBar.Text;
            var filteredWords = words
                .AsParallel()
                .Where(w => w.Simplified.Contains(searchInput))
                .OrderBy(w => w.Simplified.Length)
                .ToList();
            UpdateShownWords(filteredWords);
        }

        private void ShowEnglishResult()
        {
            string searchInput = SearchBar.Text;
            var filteredWords = words
                .Where(w => w.Definitions.Contains(searchInput))
                .OrderBy(w => w.Simplified.Length)
                .ToList();
            UpdateShownWords(filteredWords);

            /*string searchInput = SearchBar.Text;
            //var filteredWords = new List<Word>();

            WordsList.Items.Clear();
            WordsList.Items.Add(ResultCountBlock);
            int count = 0;
            foreach (Word word in words)
            {
                if (word.Definitions.Contains(searchInput))
                {
                    count += 1;
                    AddWordToPanel(word);
                    ResultCountBlock.Text = $"{count} words found";
                }
                //filteredWords.Add(word);
            }*/
            //UpdateShownWords(filteredWords);
        }

        private void AddWordToPanel(Word word)
        {
            Border wordBorder = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8)
            };

            StackPanel wordPanel = new StackPanel();

            Thickness margin = wordPanel.Margin;
            margin.Bottom = 15;

            wordPanel.Orientation = Orientation.Vertical;
            wordPanel.Margin = margin;
            wordBorder.Child = wordPanel;

            StackPanel sPanel = new StackPanel { Orientation = Orientation.Horizontal };

            List<char> singleChar = word.Simplified.ToList();
            List<string> singlePron = word.Pronounciation.Split(" ").ToList();

            for (int i = 0; i < singleChar.Count && i < singlePron.Count; i++)
            {
                TextBlock sBox = new TextBlock
                {
                    Text = singleChar[i].ToString(),
                    FontSize = 48
                };
                sBox.MouseEnter += (s, e) => SBox_MouseEnter(s, e, sBox.Text);
                //
                // TODO can remove
                sBox.MouseUp += (e, s) =>
                {
                    SearchBar.Text = sBox.Text;
                    var filteredWords = words
                        .Where(w => w.Simplified.Contains(sBox.Text))
                        .ToList();
                    UpdateShownWords(filteredWords);
                };

                TextBlock pBox = new TextBlock
                {
                    FontSize = 12,
                    Foreground = Brushes.DarkGreen,
                };
                pBox.Inlines.Add(new Bold(new Run(singlePron[i])));
                Thickness pMarginPBox = pBox.Margin;
                pMarginPBox.Top = -10;
                pBox.Margin = pMarginPBox;

                StackPanel cPanel = new StackPanel { Orientation = Orientation.Vertical };
                sBox.HorizontalAlignment = HorizontalAlignment.Center;
                pBox.HorizontalAlignment = HorizontalAlignment.Center;

                cPanel.Children.Add(sBox);
                cPanel.Children.Add(pBox);
                sPanel.Children.Add(cPanel);
            }

            TextBlock definitionsBox = new TextBlock { Text = word.Definitions, FontSize = 16, Foreground = Brushes.Brown };

            wordPanel.Children.Add(sPanel);
            wordPanel.Children.Add(definitionsBox);

            WordsList.Items.Add(wordBorder);
        }

        private void ShowPronounciationResult()
        {
            string searchInput = SearchBar.Text;
            string[] prons = searchInput.Split(' ');
            List<Word> filteredWords = new List<Word>();
            foreach (var word in words)
            {
                string[] wordProns = word.Pronounciation.Split(' ');
                if (prons.Length != wordProns.Length)
                    continue;
                bool toInsert = true;
                for (int i = 0; i < prons.Length; i++)
                {
                    if (!wordProns[i].StartsWith(prons[i]))
                        toInsert = false;
                    if (wordProns[i].Length != (prons[i].Length + 1))
                        toInsert = false;
                }
                if (toInsert)
                    filteredWords.Add(word);
            }
            UpdateShownWords(filteredWords);
        }

        private void ShowComposeResult()
        {
            Console.WriteLine("ShowComposeResult - begin");
            string searchInput = SearchBar.Text;
            List<char> simplifiedComponentsFound = new List<char>();

            if (dict.ContainsKey(searchInput[0]))
            {
                string decompositionText = String.Empty;
                foreach (char c in dict[searchInput[0]])
                {
                    decompositionText += ("   " + c);
                }
                DecompositionBlock.Text = $"{searchInput[0]} : {decompositionText}";
            }

            // get simplified representation of words which contains character represented by 'searchinput'
            foreach (var decompositionTuple in dict)
            {
                List<char> componentsList = decompositionTuple.Value;
                if (componentsList.Contains(searchInput[0]))
                {
                    Console.WriteLine(searchInput[0]);
                    simplifiedComponentsFound.Add(decompositionTuple.Key);
                }
            }
            Console.WriteLine($"simplifiedComponentsFound size = {simplifiedComponentsFound.Count}");

            
            // get complete words(1 char-length) using the above simplified list
            List<Word> filteredWords = new List<Word>();
            foreach (var word in words)
            {
                if (word.Simplified.Length > 1)
                    continue;
                foreach (char character in word.Simplified)
                {
                    if (simplifiedComponentsFound.Contains(character))
                    {
                        filteredWords.Add(word);
                        break;
                    }
                }
            }
            UpdateShownWords(filteredWords);
            Console.WriteLine("ShowComposeResult - end");
        }

        private void UpdateShownWords(List<Word> filteredWords)
        {
            ResultCountBlock.Text = $"{filteredWords.Count} words found";

            WordsList.Items.Clear();
            WordsList.Items.Add(ResultCountBlock);

            foreach (var word in filteredWords)
            {
                AddWordToPanel(word);
            }
        }

        private void SBox_MouseEnter(object sender, MouseEventArgs e, string c)
        {
            ZoomedWordBox.Items.Clear();
            ExamplesList.Items.Clear();
            ExamplesList.Items.Add(StatisticsBox); //todo ??

            TextBlock block = new TextBlock
            {
                Text = c,
                FontSize = 400
            };

            ZoomedWordBox.Items.Add(block);
            DecompositionBlock.Text = GetNiceDecomposed(c[0]);
        }

        private string GetNiceDecomposed(char c)
        {
            string result = String.Empty;

            Queue<char> qu = new Queue<char>();
            qu.Enqueue(c);

            while (qu.Count > 0)
            {
                char curr = qu.Dequeue();
                if (dict.ContainsKey(curr))
                {
                    List<char> currList = dict[curr];
                    if (currList == null)
                    {
                        result += $"{curr} : Kangxi radical\n";
                    }
                    else if (currList.Count >= 2)
                    {
                        result += $"{curr} : {currList[0]}   {currList[1]}\n";
                        qu.Enqueue(currList[0]);
                        qu.Enqueue(currList[1]);
                    }
                    else if (currList.Count == 1)
                    {
                        result += $"{curr} : {currList[0]}\n";
                        qu.Enqueue(currList[0]);
                    }
                    else
                    {
                        result += $"{curr} : basic stroke\n";
                    }
                }
            }

            result += "\n definitions : \n";

            foreach (Word w in words)
            {
                if (w.Simplified.Length == 1 && w.Simplified[0] == c)
                {
                    string def = w.Definitions.Replace('/', '\n'); //todo \n
                    result += $"{c}: {def}";
                }
            }

            return result;
        }

        private void InitializeExamples()
        {
            foreach (string sentence in SentenceExamples.Examples())
            {
                ListBoxItem item = new ListBoxItem
                {
                    Content = sentence,
                };             
                ExamplesList.Items.Add(item);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchInput = SearchBar.Text;
            var filteredWords = words
                .Where(w => w.Definitions.Contains(searchInput))
                .ToList();

            UpdateShownWords(filteredWords);
        }

        private void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            int index = random.Next(words.Count);
            UpdateShownWords(new List<Word> { words[index] });
        }
    }
}
