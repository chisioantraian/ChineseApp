using ChineseAppWPF.Logic;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChineseAppWPF.Controllers
{
    public static partial class Controller
    {
        private static void ShowCharacterDecomposition(char characterToBeDecomposed)
        {
            string decompositionText = Decomposition.DecomposeCharToRadicals(characterToBeDecomposed);
            mainWindow.DecompositionBlock.Text = $"{decompositionText} ";
        }
    }
}
