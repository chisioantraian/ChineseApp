
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
    using WpfApp2.Logic;
    using WPF_program.Logic;

    using Chinese;
    using static MyTypes;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ChineseWords chineseWords;
        private readonly List<Word> allWords;
        private readonly List<DetailedWord> allDetailedWords;

        private readonly Dictionary<char, List<char>> dict;

        public MainWindow()
        {
            InitializeComponent();
            chineseWords = new ChineseWords();
            allWords = chineseWords.GetAll();
            allDetailedWords = chineseWords.GetAllDetailed();
            dict = Logic.ChineseService.GetCharacterDecomposition();
            InitializeWordsPanel();
            InitializeExamples();
        }

        /// <summary>
        /// Get the entered text, and show the results the user wants to see
        /// </summary>
        /// <param name="sender">The Search Button</param>
        /// <param name="e">The clicked event args</param> 
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            List<Word> filteredWords = chineseWords.EnglishResult(SearchBar.Text);
            UpdateShownWords(filteredWords);
        }

        /// <summary>
        /// Populate the results panel with some random words
        /// </summary>
        /// <param name="sender">The Random Button</param>
        /// <param name="e">The clicked event args</param>
        private void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            List<Word> randomWords = chineseWords.GetRandomWords();
            UpdateShownWords(randomWords);
        }

        /// <summary>
        /// Choose what happens when the user presses enter, in the search bar
        /// </summary>
        /// <param name="sender">the source of the event - the search bar</param>
        /// <param name="e">the event args - it must be enter</param>
        private void SearchBar_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            ComboBoxItem typeItem = (ComboBoxItem)InputComboBox.SelectedItem;

            switch (typeItem.Content.ToString())
            {
                case "Chinese": ShowChineseResult(); break;
                case "English": ShowEnglishResult(); break;
                case "Pronounciation": ShowPronounciationResult(); break;
                case "Compose": ShowComposeResult(); break;
            }
        }

        /// <summary>
        /// After the application starts, show some words
        /// </summary>
        public void InitializeWordsPanel()
        {
            List<Word> wordsExample = chineseWords.EnglishResult("rainforest");
            UpdateShownWords(wordsExample);
        }

        /// <summary>
        /// When the user wants to analyze the chinese text, or just find translation
        /// </summary>
        private void ShowChineseResult()
        {
            ShowDecomposed(SearchBar.Text);
        }

        /// <summary>
        /// When the user enters english words, to be translated
        /// </summary>
        private void ShowEnglishResult()
        {
            List<Word> filteredWords = chineseWords.EnglishResult(SearchBar.Text);
            UpdateShownWords(filteredWords);
        }

        /// <summary>
        /// When the user wants to find chinese words, by entering their romanized pronounciation (pynin)
        /// </summary>
        private void ShowPronounciationResult()
        {
            List<Word> filteredWords = chineseWords.SearchByPinyin(SearchBar.Text);
            UpdateShownWords(filteredWords);
        }

        /// <summary>
        /// From a single character, show all other chinese characters which contain it as a component
        /// </summary>
        private void ShowComposeResult()
        {
            Console.WriteLine("ShowComposeResult - begin");
            string searchInput = SearchBar.Text;
            List<char> simplifiedComponentsFound = new List<char>();

            if (dict.ContainsKey(searchInput[0]))
            {
                string decompositionText = string.Empty;
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
                if (componentsList != null && componentsList.Contains(searchInput[0]))
                {
                    Console.WriteLine(searchInput[0]);
                    simplifiedComponentsFound.Add(decompositionTuple.Key);
                }
            }
            Console.WriteLine($"simplifiedComponentsFound size = {simplifiedComponentsFound.Count}");

            // get complete words(1 char-length) using the above simplified list
            List<Word> filteredWords = new List<Word>();
            foreach (var word in allWords)
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

        /// <summary>
        /// In the results panel, replace the shown words
        /// </summary>
        /// <param name="filteredWords"> The new words to be shown</param>
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

        /// <summary>
        /// Create a Card from the word, and add it to the results panel
        /// </summary>
        /// <param name="word">word to be inserted</param>
        private void AddWordToPanel(Word word)
        {
            StackPanel sPanel = new StackPanel { Orientation = Orientation.Horizontal };

            List<char> singleChar = word.Simplified.ToList();
            List<string> singlePron = word.Pronounciation.Split(" ").ToList();

            for (int i = 0; i < singleChar.Count && i < singlePron.Count; i++)
            {
                TextBlock sBox = new TextBlock
                {
                    FontSize = 48,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Text = singleChar[i].ToString(),
                };
                //sBox.MouseEnter += (s, e) => SBox_MouseEnter(sBox.Text);
                //
                sBox.MouseUp += (e, s) =>
                {
                    SearchBar.Text = sBox.Text;
                    List<Word> filteredWords = chineseWords.SearchBySimplified(sBox.Text);
                    UpdateShownWords(filteredWords);
                };

                TextBlock pBox = new TextBlock
                {
                    FontSize = 12,
                    Foreground = Brushes.DarkGreen,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(-10),
                };
                pBox.Inlines.Add(new Bold(new Run(singlePron[i])));

                StackPanel cPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical
                };

                cPanel.Children.Add(sBox);
                cPanel.Children.Add(pBox);
                sPanel.Children.Add(cPanel);
            }

            StackPanel wordPanel = new StackPanel
            {
                Margin = new Thickness(0, 0, 0, 15),
                Orientation = Orientation.Vertical,
            };
            Border wordBorder = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8)
            };
            TextBlock definitionsBox = new TextBlock
            {
                FontSize = 16,
                Foreground = Brushes.Brown,
                Text = word.Definitions,
            };

            wordPanel.Children.Add(sPanel);
            wordPanel.Children.Add(definitionsBox);
            wordBorder.Child = wordPanel;
            WordsList.Items.Add(wordBorder);
        }

        /// <summary>
        /// When the mouse hovers over a char, show a magnified version of it
        /// </summary>
        /// <param name="c">The character to be magnified</param>
        private void SBox_MouseEnter(string c)
        {
            ExamplesList.Items.Clear();
            ExamplesList.Items.Add(StatisticsBox);

            TextBlock block = new TextBlock
            {
                Text = c,
                FontSize = 400
            };

            //MiddleWordBox.Items.Add(block);
            DecompositionBlock.Text = GetNiceDecomposed(c[0]);
        }

        /// <summary>
        /// Get a character's decomposition, till radicals/strokes level
        /// </summary>
        /// <param name="c">The character to be decomposed</param>
        /// <returns>The decomposition</returns>
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

            foreach (Word w in allWords)
            {
                if (w.Simplified.Length == 1 && w.Simplified[0] == c)
                {
                    string def = w.Definitions.Replace('/', '\n');
                    result += $"{c}: {def}";
                }
            }

            return result;
        }

        /// <summary>
        /// Add some sentences to the app, which will be used as examples
        /// </summary>
        private void InitializeExamples()
        {
            //foreach (string sentence in SentenceExamples.Examples())
            foreach (string sentence in Chinese.SentenceExamples.getAllSentences())
            {
                ListBoxItem item = new ListBoxItem
                {
                    Content = sentence,
                };
                item.MouseLeftButtonUp += (s, e) => ShowDecomposed(sentence);
                ExamplesList.Items.Add(item);
            }
        }

        /// <summary>
        /// Split a sentence into words, show these words and analyze the sentence
        /// </summary>
        /// <param name="sentence">The sentence to be splitted</param>
        private void ShowDecomposed(string sentence)
        {
            List<Word> result = GetWordsFromSentence(sentence);
            UpdateShownWords(result);

            MiddleWordBox.Children.Clear();
            foreach (Word w in result)
            {
                //DetailedWord detailedWord = allDetailedWords.Find(dw => dw.Simplified == w.Simplified) ?? new DetailedWord();
                DetailedWord? detailedWord = allDetailedWords.Find(dw => dw.Simplified == w.Simplified);
                (SolidColorBrush, string) posTuple = GetPosInfo(detailedWord);

                TextBlock wBlock = new TextBlock
                {
                    FontSize = 32,
                    Foreground = posTuple.Item1,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(5),
                    Text = w.Simplified,
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
                MiddleWordBox.Children.Add(wordBorder);
            }
        }

        /// <summary>
        /// Split a sentence represented as a string into a list of words
        /// </summary>
        /// <param name="sentence">The sentence to be splitted</param>
        /// <returns>The list of words</returns>
        private List<Word> GetWordsFromSentence(string sentence)
        {
            string constructedWord = string.Empty;
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
                        result.Add(toInsert[0]); //toInsert.ForEach(w => result.Add(w));
                    toInsert = GetResultedWord(curr.ToString());
                    constructedWord = curr.ToString();
                }
            }
            if (toInsert.Count > 0)
                result.Add(toInsert[0]); //toInsert.ForEach(w => result.Add(w));
            return result;
        }

        /// <summary>
        /// From a word(DetailedWord) pos tag, get its full pos name , and also return a color, unique to it
        /// </summary>
        /// <param name="detailedWord">The (detailed)word which contains the pos tag</param>
        /// <returns>A tuple representing a color and the full name of the tag</returns>
        private static (SolidColorBrush, string) GetPosInfo(DetailedWord? detailedWord)
        {
            if (detailedWord == null)
                return (Brushes.Gray, "_");
            return detailedWord.DominantPos switch
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
        }

        /// <summary>
        /// Get all chinese words which have the simplified form equal to a particular string
        /// </summary>
        /// <param name="simpl">The string to be searched after</param>
        /// <returns>The list of chinese words which satisfy the condition</returns>
        private List<Word> GetResultedWord(string simpl)
        {
            return allWords.Where(w => w.Simplified == simpl).ToList();
        }

    }
}
