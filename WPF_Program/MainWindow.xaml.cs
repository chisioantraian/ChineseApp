
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

    using Chinese;
    using WPF_program.Controllers;
    using static MyTypes;


    public partial class MainWindow : Window
    {
        private readonly List<Word> allWords;
        private readonly List<DetailedWord> allDetailedWords;

        private readonly Dictionary<char, List<char>> dict;

        public MainWindow()
        {
            InitializeComponent();
            Console.WriteLine("reorganized code");
            allWords = ChineseService.getAllWords();
            allDetailedWords = ChineseService.getAllDetailedWords().ToList();
            dict = Logic.ChineseService.GetCharacterDecomposition();
            InitializeExamples();

            ResultsPanel.setWindow(this);

            ResultsPanel.ShowSomeRandomWords();
        }

        // Get the entered text, and show the results the user wants to see
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ResultsPanel.ShowEnglishResult();
            //List<Word> filteredWords = ChineseService.getEnglishResult(SearchBar.Text).ToList();
            //UpdateShownWords(filteredWords);
        }

        // Populate the results panel with some random words
        private void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            ResultsPanel.ShowSomeRandomWords();
            //List<Word> randomWords = ChineseService.getRandomWords();
            //UpdateShownWords(randomWords);
        }

        // Choose what happens when the user presses enter, in the search bar
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

        // When the user wants to analyze the chinese text, or just find translation
        private void ShowChineseResult()
        {
            ResultsPanel.ShowChineseResult();
            //ShowDecomposed(SearchBar.Text);
        }

        // When the user enters english words, to be translated
        private void ShowEnglishResult()
        {
            ResultsPanel.ShowEnglishResult();
            //List<Word> filteredWords = ChineseService.getEnglishResult(SearchBar.Text).ToList();
            //UpdateShownWords(filteredWords);
        }

        // When the user wants to find chinese words, by entering their romanized pronounciation (pynin)
        private void ShowPronounciationResult()
        {
            ResultsPanel.ShowPronounciationResults();
            //List<Word> filteredWords = ChineseService.getEnglishResult(SearchBar.Text).ToList();
            //UpdateShownWords(filteredWords);
        }

        // From a single character, show all other chinese characters which contain it as a component
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
            ResultsPanel.UpdateShownWords(filteredWords);
            Console.WriteLine("ShowComposeResult - end");
        }

        // Get a character's decomposition, till radicals/strokes level
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

        // Add some sentences to the app, which will be used as examples
        private void InitializeExamples()
        {
            foreach (string sentence in SentenceExamples.getAllSentences())
            {
                ListBoxItem item = new ListBoxItem
                {
                    Content = sentence,
                };
                item.MouseLeftButtonUp += (s, e) => ShowDecomposed(sentence);
                ExamplesList.Items.Add(item);
            }
        }

        // Split a sentence into words, show these words and analyze the sentence
        private void ShowDecomposed(string sentence)
        {
            List<Word> result = GetWordsFromSentence(sentence);
            ResultsPanel.UpdateShownWords(result);

            MiddleWordBox.Children.Clear();
            foreach (Word w in result)
            {
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


        // Split a sentence represented as a string into a list of words
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


        // From a word(DetailedWord) pos tag, get its full pos name , and also return a color, unique to it
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

        // Get all chinese words which have the simplified form equal to a particular string
        private List<Word> GetResultedWord(string simpl)
        {
            return allWords.Where(w => w.Simplified == simpl).ToList();
        }

    }
}
