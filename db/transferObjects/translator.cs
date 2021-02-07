using System;
using System.Collections.Generic;
using System.Collections;
namespace TranslationMemory
{
    class translator
    {
        public Gender Gender { get; set; }
        public Role Role { get; set; }
        public string UUID { get; set; }
        public List<word> AddedWords { get; set; }
        public string _userName { get; set; }
        public int _password { get; set; }
        public language _language { get; set; }
        public List<translation> _addedTranslations { get; set; }
    }
}