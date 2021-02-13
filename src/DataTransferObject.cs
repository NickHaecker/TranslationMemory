using System;
using System.Collections.Generic;
using System.Collections;
namespace TranslationMemory
{
    class DataTransferObject
    {
        private UserFactory _userFactory = null;

        private TranslationFactory _translationFactory = null;

        public DataTransferObject()
        {
            _userFactory = new UserFactory();
            _translationFactory = new TranslationFactory();
        }
        public InterfaceUser CreateNewUser(Role role, Gender gender, string username, int password, List<Word> words, List<AbstractTranslation> translations)
        {
            switch (role)
            {
                case Role.ADMIN:
                    return (Admin)_userFactory.GetUser(role, gender, username, password, words, translations, GetUUID(), null);
                case Role.TRANSLATOR:
                    return (Translator)_userFactory.GetUser(role, gender, username, password, words, translations, GetUUID(), null);
                default:
                    return (User)_userFactory.GetUser(role, gender, username, password, words, translations, GetUUID(), null);
            }
        }
        public InterfaceUser LoginUser(string username, int password)
        {
            List<admin> admins = Database.Instance.GetAllAdmins();
            List<translator> translators = Database.Instance.GetAllTranslator();
            InterfaceUser user = null;
            foreach (admin admin in admins)
            {
                if (admin._userName == username && admin._password == password)
                {
                    user = (Admin)_userFactory.GetUser(admin.Role, admin.Gender, admin._userName, admin._password, null, null, admin.UUID, null);
                    break;
                }
            }
            foreach (translator translator in translators)
            {
                if (translator._userName == username && translator._password == password)
                {
                    user = (Translator)_userFactory.GetUser(translator.Role, translator.Gender, translator._userName, translator._password, GetWords(translator.AddedWords), GetTranslations(translator._addedTranslations), translator.UUID, GetLanguage(translator._language));
                    break;
                }
            }
            if (user != null)
            {
                switch (user.Role)
                {
                    case Role.ADMIN:
                        return (Admin)user;
                    default:
                        return (Translator)user;
                }
            }
            else
            {
                return null;
            }
        }
        public InterfaceUser GetTranslator(string username)
        {
            foreach (Translator translator in GetAllTranslator())
            {
                if (translator._userName == username)
                {
                    return translator;
                }
            }
            return null;
        }
        public List<Translator> GetAllTranslator()
        {
            List<translator> translator = Database.Instance.GetAllTranslator();
            List<Translator> translators = new List<Translator>();
            foreach (translator t in translator)
            {
                Translator trans = (Translator)_userFactory.GetUser(t.Role, t.Gender, t._userName, t._password, GetWords(t.AddedWords), GetTranslations(t._addedTranslations), t.UUID, GetLanguage(t._language));
                translators.Add(trans);
            }
            return translators;
        }
        public List<Language> GetLanguages()
        {
            List<Language> languages = new List<Language>();
            List<language> lang = Database.Instance.GetLanguages();
            foreach (language l in lang)
            {
                Language language = new Language(l._name, l.ID);
                languages.Add(language);
            }
            return languages;
        }
        public Language GetLanguage(string language)
        {
            foreach (Language lang in GetLanguages())
            {
                if (language == lang._name)
                {
                    return lang;
                }
            }
            return null;
        }
        public List<Word> GetWords()
        {
            List<Word> words = new List<Word>();
            List<word> wo = Database.Instance.GetWords();
            foreach (word w in wo)
            {
                Word word = new Word(w._word, w._UUID);
                words.Add(word);
            }
            return words;
        }
        public List<Word> GetWords(List<word> words)
        {
            List<Word> newwordlist = new List<Word>();
            foreach (word w in words)
            {
                Word word = new Word(w._word, w._UUID);
                newwordlist.Add(word);
            }
            return newwordlist;
        }
        public string GetUUID()
        {
            return Guid.NewGuid().ToString();
        }
        public void SaveUser(InterfaceUser user, Role role)
        {
            switch (role)
            {
                case Role.USER:
                    Database.Instance.SaveUser((User)user, Role.USER);
                    break;
                case Role.TRANSLATOR:
                    Database.Instance.SaveUser((Translator)user, Role.TRANSLATOR);
                    break;
                default:
                    Database.Instance.SaveUser((Admin)user, Role.ADMIN);
                    break;
            }
        }
        public Word GetWord(string word)
        {
            foreach (Word w in GetWords())
            {
                if (w._word.ToLower() == word.ToLower())
                {
                    return w;
                }
            }
            return null;
        }
        public AbstractTranslation GetWordWithMissingTranslation(string word, Language language)
        {
            Word w = GetWord(word);
            if (w != null)
            {
                if (FilteredTranslationByWord(w, language) != null && FilteredTranslationByWord(w, language).Translation == "(Keine)")
                {
                    return FilteredTranslationByWord(w, language);
                }
            }
            return null;
        }
        public Word CreateWord(string word)
        {
            Word newWord = new Word(word, GetUUID());
            Database.Instance.SaveWord(newWord);
            return newWord;
        }
        public void CreateTranslation(string wordUUID, string userUUID, bool createInitialTranslation, AbstractTranslation abstractTranslation)
        {
            if (createInitialTranslation)
            {
                foreach (Language language in GetLanguages())
                {
                    SetTranslation(language, "", wordUUID, userUUID);
                }
            }
            else
            {
                Database.Instance.CreateTranslation(abstractTranslation);
            }
        }
        private void SetTranslation(Language language, string translation, string wordUUID, string userUUID)
        {
            AbstractTranslation t = _translationFactory.GetTranslation(language, "", wordUUID, userUUID);
            Database.Instance.CreateTranslation(t);
        }
        public void CreateLanguage(string language, string uuid)
        {
            Language l = new Language(language, language);
            Database.Instance.AddLanguage(l);
            UpdateTranslationByCreatingNewLanguage(l, uuid);
        }
        private void UpdateTranslationByCreatingNewLanguage(Language language, string uuid)
        {
            List<Word> words = GetWords();
            foreach (Word word in words)
            {
                SetTranslation(language, "", word._UUID, uuid);
            }
        }
        public List<AbstractTranslation> GetTranslations()
        {
            List<translation> translations = Database.Instance.GetTranslations();
            List<AbstractTranslation> transLation = new List<AbstractTranslation>();
            foreach (translation trans in translations)
            {
                AbstractTranslation translation = _translationFactory.GetTranslation(new Language(trans.LANGUAGE._name, trans.LANGUAGE.ID), trans.Translation, trans.WORD_ID, trans.AUTHOR);
                transLation.Add(translation);
            }
            return transLation;
        }
        public List<AbstractTranslation> GetTranslations(List<translation> translations)
        {
            List<AbstractTranslation> transLation = new List<AbstractTranslation>();
            foreach (translation trans in translations)
            {
                AbstractTranslation translation = _translationFactory.GetTranslation(new Language(trans.LANGUAGE._name, trans.LANGUAGE.ID), trans.Translation, trans.WORD_ID, trans.AUTHOR);
                transLation.Add(translation);
            }
            return transLation;
        }
        public List<AbstractTranslation> GetTranslationByWord(Word word)
        {
            List<AbstractTranslation> wordTranslations = new List<AbstractTranslation>();
            foreach (AbstractTranslation translation in GetTranslations())
            {
                if (translation.WORD_ID == word._UUID)
                {
                    wordTranslations.Add(translation);
                }
            }
            return wordTranslations;
        }
        public AbstractTranslation FilteredTranslationByWord(Word word, Language language)
        {
            foreach (AbstractTranslation abstractTranslationin in GetTranslationByWord(word))
            {
                if (abstractTranslationin.LANGUAGE._name == language._name)
                {
                    return abstractTranslationin;
                }
            }
            return null;
        }
        public int GetWordsInDatabaseLength()
        {
            return GetWords().Count;
        }
        public List<string> GetPercentageOfCorrectTranslatetWords()
        {
            List<string> counts = new List<string>();
            foreach (Word word in GetWords())
            {
                int count = 0;
                foreach (AbstractTranslation translation in GetTranslationByWord(word))
                {
                    if (translation.Translation != "(Keine)")
                    {
                        count++;
                    }
                }
                string wordCount = word._word + " (" + CalculatePercentage(GetTranslationByWord(word).Count, count) + "%" + ")";
                counts.Add(wordCount);
            }
            return counts;
        }
        public List<string> GetUncompleteTranslatetWords()
        {
            List<string> counts = new List<string>();
            foreach (Word word in GetWords())
            {
                int count = 0;
                foreach (AbstractTranslation translation in GetTranslationByWord(word))
                {
                    if (translation.Translation != "(Keine)")
                    {
                        count++;
                    }
                }
                if (CalculatePercentage(GetTranslationByWord(word).Count, count) < 100)
                {
                    string wordCount = word._word + " (" + CalculatePercentage(GetTranslationByWord(word).Count, count) + "%" + ")";
                    counts.Add(wordCount);
                }
            }
            return counts;
        }
        private double CalculatePercentage(double length, double count)
        {
            double sum = ((count / length) * 100);
            return sum;
        }
        private Language GetLanguage(language l)
        {
            if (l != null)
            {
                Language language = new Language(l._name, l.ID);
                return language;
            }
            return null;
        }
    }
}