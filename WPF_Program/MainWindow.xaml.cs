
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
    using CSharp_scripts.Models;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<Word> words;
        private readonly Dictionary<char, List<char>> dict;
        private readonly List<DetailedWord> detailedWords;

        public MainWindow()
        {
            Console.WriteLine("Beginning of constructor");
            InitializeComponent();
            words = ChineseService.GetWordsFromDatabase();
            detailedWords = ChineseService.GetDetailedWords();
            dict = ChineseService.GetCharacterDecomposition();
            Console.WriteLine($"number of words: {words.Count}");
            Console.WriteLine($"number of lines(dict): {dict.Count}");
            InitializeWordsPanel();
            InitializeExamples();
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
                //sBox.MouseEnter += (s, e) => SBox_MouseEnter(s, e, sBox.Text);
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
            //MiddleWordBox.Items.Clear();
            ExamplesList.Items.Clear();
            ExamplesList.Items.Add(StatisticsBox); //todo ??

            TextBlock block = new TextBlock
            {
                Text = c,
                FontSize = 400
            };

            //MiddleWordBox.Items.Add(block);
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
                item.MouseLeftButtonUp += (s, e) =>
                {
                    Decompose(sentence);
                };
                ExamplesList.Items.Add(item);
            }
        }

        private void Decompose(string sentence)
        {
            //MiddleWordBox.Items.Clear();


            string constructedWord = String.Empty;
            var resultedWord = new List<Word>();
            var toInsert = new List<Word>();
            var result = new List<Word>();

            foreach (char curr in sentence)
            {
                resultedWord = GetResultedWord(constructedWord + curr);
                if (resultedWord.Count > 0)
                {
                    toInsert = resultedWord;
                    constructedWord += curr;
                }
                else
                {
                    if (toInsert.Count > 0)
                        result.Add(toInsert[0]);
                        //toInsert.ForEach(w => result.Add(w));
                    toInsert = GetResultedWord(curr.ToString());
                    constructedWord = curr.ToString();
                }
            }
            if (toInsert.Count > 0)
                result.Add(toInsert[0]);
                //toInsert.ForEach(w => result.Add(w));

            foreach (Word w in result)
            {
                Console.Write($"{w.Simplified} , ");
            }
            Console.WriteLine("");


            //TODO separate into function
            UpdateShownWords(result);

            MiddleWordBox.Children.Clear();
            foreach (Word w in result)
            {
                //todo use default
                DetailedWord detailedWord = detailedWords.FirstOrDefault(dw => dw.Simplified == w.Simplified);

                if (detailedWord == null)
                {
                    detailedWord = new DetailedWord();
                }

                Console.WriteLine($"{w.Simplified} {detailedWord.DominantPos}");

                //TODO combine switches
                (SolidColorBrush, string) posTuple = detailedWord.DominantPos switch
                {
                    "a" => (Brushes.Purple, "adjective"),
                    "ad" => (Brushes.Purple, "adj as adv"),
                    "ag" => (Brushes.Purple, "adj morpheme"),
                    "an" => (Brushes.Purple, "adj w. nom fun"),
                    "b" => (Brushes.Purple, "non-pred adje"),
                    "c" => (Brushes.Pink, "conjunction"),
                    "cc" => (Brushes.Pink, "conjunction"),
                    "d" => (Brushes.LightBlue, "adverb"),
                    "dg" => (Brushes.LightBlue, "adv morpheme"),
                    "e" => (Brushes.DarkBlue, "interjection"),
                    "f" => (Brushes.DarkBlue, "dir. locality"),
                    "g" => (Brushes.DarkBlue, "morpheme"),
                    "h" => (Brushes.DarkBlue, "prefix"),
                    "i" => (Brushes.Cyan, "idiom"),
                    "j" => (Brushes.DarkBlue, "interjection"),
                    "k" => (Brushes.DarkBlue, "suffix"),
                    "l" => (Brushes.DarkBlue, "fixed expr"),
                    "m" => (Brushes.Orange, "numeral"),
                    "mg" => (Brushes.Orange, "num morpheme"),
                    "mq" => (Brushes.Orange, "num classifier"),
                    "n" => (Brushes.Red, "noun"),
                    "ng" => (Brushes.Red, "noun morpheme"),
                    "nr" => (Brushes.Red, "pers name"),
                    "ns" => (Brushes.Red, "place name"),
                    "nt" => (Brushes.Red, "org name"),
                    "nx" => (Brushes.Red, "nom. string"),
                    "nz" => (Brushes.Red, "other prop. noun"),
                    "o" => (Brushes.LightBlue, "onomatopoeia"),
                    "p" => (Brushes.Aquamarine, "preposition"),
                    "q" => (Brushes.DarkKhaki, "classifier"),
                    "r" => (Brushes.DarkOrange, "pronoun"),
                    "rg" => (Brushes.DarkOrange, "pron. morpheme"),
                    "s" => (Brushes.Beige, "space word"),
                    "t" => (Brushes.SandyBrown, "time word"),
                    "tg" => (Brushes.SandyBrown, "time morpheme"),
                    "u" => (Brushes.DarkBlue, "auxiliary"),
                    "v" => (Brushes.LightGreen, "verb"),
                    "vd" => (Brushes.LightGreen, "vb as adv"),
                    "vg" => (Brushes.LightGreen, "vb morpheme"),
                    "vn" => (Brushes.LightGreen, "vb nom fun"),
                    "w" => (Brushes.DarkBlue, "symbol.."),
                    "x" => (Brushes.Gold, "unclassififed"),
                    "y" => (Brushes.Brown, "modal part."),
                    "z" => (Brushes.Honeydew, "descriptive"),
                    _ => (Brushes.Gray, "_")
                };

                TextBlock wBlock = new TextBlock
                {
                    Text = w.Simplified,
                    Foreground = posTuple.Item1,
                    FontSize = 32,
                };
                Border wordBorder = new Border
                {
                    BorderBrush = posTuple.Item1,
                    BorderThickness = new Thickness(2),
                    CornerRadius = new CornerRadius(5)
                };
                Thickness wBlockMarginPBox = wBlock.Margin;
                wBlockMarginPBox.Top = 5;
                wBlockMarginPBox.Bottom = 5;
                wBlockMarginPBox.Left = 5;
                wBlockMarginPBox.Right = 5;
                wBlock.Margin = wBlockMarginPBox;

                TextBlock posBox = new TextBlock
                {
                    Text = posTuple.Item2,
                    FontSize = 12,
                    Foreground = posTuple.Item1,
                };
                //posBox.Inlines.Add(new Bold(new Run(posTuple.Item2)));
                Thickness posMarginPBox = posBox.Margin;
                posMarginPBox.Top = -5;
                posBox.Margin = posMarginPBox;


                StackPanel wordPanel = new StackPanel();
                
                wBlock.HorizontalAlignment = HorizontalAlignment.Center;
                posBox.HorizontalAlignment = HorizontalAlignment.Center;

                wordPanel.Orientation = Orientation.Vertical;
                wordPanel.Children.Add(wBlock);
                wordPanel.Children.Add(posBox);
                Thickness wordPadding = wordBorder.Padding;
                wordPadding.Bottom = 5;
                wordPadding.Right = 5;
                wordPadding.Top = 5;
                wordPadding.Left = 5;
                wordBorder.Padding = wordPadding;

                wordBorder.Child = wordPanel;


                //wordBorder.Child = wBlock;
                //MiddleWordBox.Children.Add(wordPanel);
                MiddleWordBox.Children.Add(wordBorder);

            }
        }

        private List<Word> GetResultedWord(string simpl)
        {
            //TODO modify
            return words.Where(w => w.Simplified == simpl).ToList();
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
