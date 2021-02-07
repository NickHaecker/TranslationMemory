using System;

namespace TranslationMemory
{
    public class Language
    {
        public string _name { get; private set; }
        public string ID { get; private set; }
        public Language(string name, string id)
        {
            _name = name;
            ID = id;
        }
        public void EditName(string name)
        {
            _name = name;
        }
    }
}
