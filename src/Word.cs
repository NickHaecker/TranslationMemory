using System;
using System.Collections.Generic;
using System.Collections;

namespace TranslationMemory
{
    class Word
    {
        public string _UUID { get; private set; }
        public string _word { get; private set; }
        public Word(string word, string uuid)
        {
            _UUID = uuid;
            _word = word;
        }
    }
}
