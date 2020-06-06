using System;
using System.Collections.Generic;
using System.Text;

namespace ChineseAppWPF.Service
{
    public static class MyExtensions
    {
        public static bool IsNotExtraCharacter(this char character)
        {
            return character != '〔' &&
                    character != '〕' &&
                    character != '-' &&
                    character != ' ' &&
                    character != '·';
        }
    }
}
