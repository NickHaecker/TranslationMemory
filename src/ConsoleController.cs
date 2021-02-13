using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
namespace TranslationMemory
{
    class ConsoleController
    {
        private List<Command> _commands = new List<Command>();

        private string _userType = null;

        private bool IsUserContaining(string[] command)
        {
            bool isusercontaining = false;
            if (command != null && command.Length > 0)
                foreach (string type in command)
                {
                    if (type == _userType)
                    {
                        isusercontaining = true;
                    }
                }
            return isusercontaining;
        }
        public void InitCommands(string usertype)
        {
            _userType = usertype;
            _commands.Add(new Command("/logout", null));
            _commands.Add(new Command("/search-word", new string[] { "TranslationMemory.User", "TranslationMemory.Translator" }));
            _commands.Add(new Command("/get-my-words", new string[] { "TranslationMemory.User", "TranslationMemory.Translator" }));
            _commands.Add(new Command("/get-count-of-all-words-in-database", new string[] { "TranslationMemory.User", "TranslationMemory.Translator" }));
            _commands.Add(new Command("/give-translator-a-language", new string[] { "TranslationMemory.Admin" }));
            _commands.Add(new Command("/create-new-language", new string[] { "TranslationMemory.Admin" }));
            _commands.Add(new Command("/list-words-with-uncompletet-translations", new string[] { "TranslationMemory.Translator" }));
            _commands.Add(new Command("/add-translation", new string[] { "TranslationMemory.Translator" }));
            _commands.Add(new Command("/show-my-translations", new string[] { "TranslationMemory.Translator" }));
        }
        public List<string> GetUserSpecificCommands()
        {
            List<string> commands = new List<string>();
            foreach (Command command in _commands)
            {
                if (IsUserContaining(command._userType) || command._userType == null)
                {
                    commands.Add(command._command);
                }
            }
            return commands;

        }
        public void WriteStringList(List<string> commands)
        {
            foreach (string command in commands)
            {
                Console.WriteLine(command);
            }
        }
        public void WriteStringList(List<string> commands, string prefix, string suffix)
        {
            Console.WriteLine(prefix);
            foreach (string command in commands)
            {
                Console.WriteLine(command);
            }
            Console.WriteLine(suffix);
        }

        public string GetStringAnswer(string question)
        {
            Console.WriteLine(question);
            string answer = Console.ReadLine();
            return answer;
        }
        public string GetStringAnswer()
        {
            string answer = Console.ReadLine();
            return answer;
        }
        public int GetIntAnswer(string question)
        {
            Console.WriteLine(question);
            int answer = Convert.ToInt32(Console.ReadLine());
            return answer;
        }
        public void WriteString(string answer)
        {
            Console.WriteLine(answer);
        }
        public void WriteInt(int answer)
        {
            Console.WriteLine(answer);
        }
        public int GetIntAnswer()
        {
            Regex rgx = new Regex("[0-9]{4}");
            Console.WriteLine("Bitte gib dein Passwort ein: ");
            string answer = Console.ReadLine();
            if (rgx.IsMatch(answer))
            {
                return Convert.ToInt32(answer);
            }
            Console.WriteLine("Error! Leider haben sie keinen gültigen Zahlencode eingeben. Versuchen sie es erneut! Es muss eine vierstellige Zahl sein!");
            return GetIntAnswer();
        }
        public Gender GetGender(string gender)
        {
            if (gender.ToUpper() == Gender.MALE.ToString())
            {
                return Gender.MALE;
            }
            else if (gender.ToUpper() == Gender.FEMALE.ToString())
            {
                return Gender.FEMALE;
            }
            else
            {
                return Gender.DIVERS;
            }
        }
        public bool IsInputCorrectAt(string answer)
        {
            bool isanswerinlist = false;
            foreach (string command in GetUserSpecificCommands())
            {
                if (command == answer)
                {
                    isanswerinlist = true;
                }
            }
            return isanswerinlist;
        }
        public void WriteAddedWords(List<Word> words)
        {
            List<string> commands = new List<string>();
            foreach (Word word in words)
            {
                string command = word._word;
                commands.Add(command);
            }
            string prefix = "Du hast " + commands.Count + " erstellt: ";
            WriteStringList(commands, prefix, null);
        }
        public void WriteErrorMessage()
        {
            Console.WriteLine("Sie müssen einen gültigen Befehl eingeben!");
        }
    }
}