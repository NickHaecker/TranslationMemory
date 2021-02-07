using System;
using System.Collections.Generic;
using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace TranslationMemory
{
    class Database
    {
        private static Database _instance;
        public static Database Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Database();
                return _instance;
            }
        }
        private string TRANSLATION_PATH = "./db/translation";
        private string USER_PATH = "./db/user/user";
        private string TRANSLATOR_PATH = "./db/user/translator";
        private string ADMIN_PATH = "./db/user/admin";
        private string WORD_PATH = "./db/word";
        private string LANGUAGE_PATH = "./db/language/";

        public void SaveUser(InterfaceUser user, Role role)
        {
            string jsonString;
            string file;
            switch (role)
            {
                case Role.ADMIN:
                    user = (Admin)user;
                    jsonString = JsonSerializer.Serialize((Admin)user);
                    file = ADMIN_PATH + "/" + user.UUID + ".json";
                    break;
                case Role.TRANSLATOR:
                    user = (Translator)user;
                    jsonString = JsonSerializer.Serialize((Translator)user);
                    file = TRANSLATOR_PATH + "/" + user.UUID + ".json";
                    break;
                default:
                    user = (User)user;
                    jsonString = JsonSerializer.Serialize((User)user);
                    file = USER_PATH + "/" + user.UUID + ".json";
                    break;
            }
            WriteFile(file, jsonString);
        }

        private void WriteFile(string file, string jsonString)
        {
            File.WriteAllText(file, jsonString);
        }
        public void CreateTranslation(AbstractTranslation translation)
        {
            string jsonString = JsonSerializer.Serialize(translation);
            string file = TRANSLATION_PATH + "/" + translation.LANGUAGE._name + translation.WORD_ID + ".json";
            WriteFile(file, jsonString);
            Console.WriteLine("Übersetzung: " + translation.LANGUAGE._name + " wurde ins System gestellt");
        }
        public void SaveWord(Word word)
        {
            string jsonString = JsonSerializer.Serialize(word);
            string file = WORD_PATH + "/" + word._UUID + ".json";
            WriteFile(file, jsonString);
            Console.WriteLine("Wort: " + word._word + " wurde ins System gestellt");
        }
        public void AddLanguage(Language language)
        {
            string jsonString = JsonSerializer.Serialize(language);
            string file = LANGUAGE_PATH + "/" + language.ID + ".json";
            File.WriteAllText(file, jsonString);
        }
        public List<language> GetLanguages()
        {
            List<language> languages = new List<language>();
            foreach (string jsonString in GetFileNames(LANGUAGE_PATH))
            {
                language l;
                l = JsonSerializer.Deserialize<language>(jsonString);
                languages.Add(l);
            }
            return languages;
        }
        public List<word> GetWords()
        {
            List<word> words = new List<word>();
            foreach (string jsonString in GetFileNames(WORD_PATH))
            {
                word w;
                w = JsonSerializer.Deserialize<word>(jsonString);
                words.Add(w);
            }
            return words;
        }
        private List<string> GetFileNames(string path)
        {
            List<string> strings = new List<string>();
            foreach (string fileName in Directory.GetFiles(path))
            {
                string jsonString = File.ReadAllText(fileName);
                strings.Add(jsonString);
            }
            return strings;
        }
        public List<translation> GetTranslations()
        {
            List<translation> translations = new List<translation>();
            foreach (string jsonString in GetFileNames(TRANSLATION_PATH))
            {
                translation t;
                t = JsonSerializer.Deserialize<translation>(jsonString);
                translations.Add(t);
            }
            return translations;
        }
        public List<translator> GetAllTranslator()
        {
            List<translator> translator = new List<translator>();
            foreach (string jsonString in GetFileNames(TRANSLATOR_PATH))
            {
                translator t;
                t = JsonSerializer.Deserialize<translator>(jsonString);
                translator.Add(t);
            }
            return translator;
        }
        public List<admin> GetAllAdmins()
        {
            List<admin> admins = new List<admin>();
            foreach (string jsonString in GetFileNames(ADMIN_PATH))
            {
                admin a;
                a = JsonSerializer.Deserialize<admin>(jsonString);
                admins.Add(a);
            }
            return admins;
        }
    }
}
