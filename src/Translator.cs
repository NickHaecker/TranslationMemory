using System;
using System.Collections.Generic;
using System.Collections;
namespace TranslationMemory
{
    class Translator : User
    {
        public string _userName { get; private set; }
        public int _password { get; private set; }
        public Language _language { get; private set; }

        public List<AbstractTranslation> _addedTranslations { get; private set; }

        public Translator(Gender gender, Role role, string username, int password, Language language, List<AbstractTranslation> translations, List<Word> words, string uuid) : base(gender, role, words, uuid)
        {
            _userName = username;
            _password = password;
            _language = language;
            _addedTranslations = translations;
        }
        public void SetLanguage(Language language)
        {
            _language = language;
        }
        public AbstractTranslation SetTranslation(AbstractTranslation abstracttranslation, string translation)
        {
            abstracttranslation.SetTranslation(translation);
            abstracttranslation.SetAuthor(_userName);
            _addedTranslations.Add(abstracttranslation);
            return abstracttranslation;
        }
    }
}