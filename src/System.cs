using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
namespace TranslationMemory
{
    class System
    {
        private InterfaceUser _registeredUser = null;
        private DataTransferObject _dataTransferObject = null;
        private ConsoleController _inputController = null;

        public System()
        {
            _dataTransferObject = new DataTransferObject();
            _inputController = new ConsoleController();
            // CreateAdminAndTranslator();
        }
        // private void CreateAdminAndTranslator()
        // {
        //     InterfaceUser translator = (Translator)_dataTransferObject.CreateNewUser(Role.TRANSLATOR, Gender.MALE, "translator1234", 1234, new List<Word>(), new List<AbstractTranslation>());
        //     InterfaceUser admin = (Admin)_dataTransferObject.CreateNewUser(Role.ADMIN, Gender.MALE, "admin1234", 1234, new List<Word>(), new List<AbstractTranslation>());
        //     _dataTransferObject.SaveUser(translator, Role.TRANSLATOR);
        //     _dataTransferObject.SaveUser(admin, Role.ADMIN);
        // }
        // private void RegisterUser()
        // {
        //     string username = _inputController.GetStringAnswer("Bitte erstellen sie ihren Benutzernamen: ");
        //     int password = _inputController.GetIntAnswer();
        // }
        private void EnterAsUser()
        {
            string answer = _inputController.GetStringAnswer("Bitte geben sie ihr geschlecht ein. Sie können wählen zwischen 'Male', 'Female' oder 'Divers'");
            if (answer.ToUpper() != _inputController.GetGender(answer).ToString())
            {
                _inputController.WriteString("Tut mir leid, aber ihre Eingabe war leider nicht richtig");
                EnterAsUser();
            }
            Role role = Role.USER;
            Gender gender = _inputController.GetGender(answer);
            _registeredUser = (User)_dataTransferObject.CreateNewUser(role, gender, null, 0, new List<Word>(), null);
        }
        private void Login()
        {
            string username = _inputController.GetStringAnswer("Bitte geben Sie ihren Benutzernamen ein: ");
            int password = _inputController.GetIntAnswer();
            if (_dataTransferObject.LoginUser(username, password) != null)
            {
                InterfaceUser u = _dataTransferObject.LoginUser(username, password);
                string name = "";
                switch (u.Role)
                {
                    case Role.TRANSLATOR:
                        Translator t = (Translator)u;
                        name = t._userName;
                        _registeredUser = (Translator)u;
                        break;
                    default:
                        Admin a = (Admin)u;
                        name = a._userName;
                        _registeredUser = (Admin)u;
                        break;
                }
                _inputController.WriteString("####################### Herzlich Willkommen ################## \nGuten Tag " + name + ", Sie haben sich erfolgreich angmeldet!");
            }
            else
            {
                _inputController.WriteErrorMessage();
                _inputController.WriteString("Sie müssen Ihren richtigen Usernamen und ihr richtiges Passwort eingeben um sich anmelden zu könenn!");
                Login();
            }
        }
        public void MainLifeCycle()
        {
            WelcomeView();
            _inputController.InitCommands(_registeredUser.GetType().ToString());
            while (_registeredUser != null)
            {
                MainLifeCycleHandleInput();
            }
            SayingGoodbye();
        }
        private void WelcomeView()
        {
            string answer = _inputController.GetStringAnswer("Willst du dich Registrieren oder hast du bereits ein Konto bei uns? Tippe '/guest' um als User fortzufahren oder '/login' um dich anzumelden.");
            _inputController.WriteString(answer);
            switch (answer)
            {
                case "/login":
                    Login();
                    return;
                case "/guest":
                    EnterAsUser();
                    return;
                default:
                    WelcomeView();
                    return;
            }
        }
        private void MainLifeCycleHandleInput()
        {
            _inputController.WriteStringList(_inputController.GetUserSpecificCommands(), "\n\n\nZwischen folgende Konsolenbefehle können sie auswählen: ", null);
            string answer = _inputController.GetStringAnswer();
            if (_inputController.IsInputCorrectAt(answer))
            {
                switch (answer)
                {
                    case "/logout":
                        Logout();
                        break;
                    case "/search-word":
                        SearchWord();
                        break;
                    case "/get-my-words":
                        GetMyWords();
                        break;
                    case "/get-count-of-all-words-in-database":
                        GetCountOfAllWordsInDatabase();
                        break;
                    case "/give-translator-a-language":
                        GiveTranslatorALanguage();
                        break;
                    case "/create-new-language":
                        CreateNewLanguage();
                        break;
                    case "/list-words-with-uncompletet-translations":
                        ListWords();
                        break;
                    case "/add-translation":
                        AddTranslation();
                        break;
                    case "/show-my-translations":
                        ShowMyTranslations();
                        break;
                    default:
                        MainLifeCycleHandleInput();
                        break;
                }
            }
            else
            {
                _inputController.WriteErrorMessage();
                MainLifeCycleHandleInput();
            }

        }
        private void Logout()
        {
            _inputController.WriteString("Aufwiedersehen und bis zum nächsten mal \nWollen Sie sich erneut anmelden oder das Programm beenden?");
            switch (_registeredUser.Role)
            {
                case Role.USER:
                    _dataTransferObject.SaveUser((User)_registeredUser, _registeredUser.Role);
                    break;
                case Role.TRANSLATOR:
                    _dataTransferObject.SaveUser((Translator)_registeredUser, _registeredUser.Role);
                    break;
                default:
                    _dataTransferObject.SaveUser((Admin)_registeredUser, _registeredUser.Role);
                    break;
            }
            _registeredUser = null;
        }
        private void SearchWord()
        {
            string answer = _inputController.GetStringAnswer("Nach welchem Wort wollen Sie suchen?");
            Word WORD = _dataTransferObject.GetWord(answer);
            if (WORD == null)
            {
                _inputController.WriteString("Tut uns leid, das Wort " + answer + " haben wir leider nicht in unserem System gefunden.");
                string boolanswer = _inputController.GetStringAnswer("Wollen Sie das Wort erstellen? Tippe /ja um das Wort zu ertsellen, oder /nein um den Prozess zu beenden");
                if (boolanswer == "/ja")
                {
                    Word word = _dataTransferObject.CreateWord(answer);
                    switch (_registeredUser.Role)
                    {
                        case Role.TRANSLATOR:
                            Translator t = (Translator)_registeredUser;
                            CreateTranslations(word._UUID, t.UUID);
                            t.SaveWord(word);
                            break;
                        default:
                            User u = (User)_registeredUser;
                            CreateTranslations(word._UUID, u.UUID);
                            u.SaveWord(word);
                            break;
                    }
                }
            }
            else
            {
                List<string> commands = new List<string>();
                foreach (AbstractTranslation translation in _dataTransferObject.GetTranslationByWord(WORD))
                {
                    string command = translation.LANGUAGE._name + ": " + translation.Translation;
                    commands.Add(command);
                }
                _inputController.WriteStringList(commands, WORD._word, null);
            }
        }
        private void GetMyWords()
        {
            List<Word> createdWords = new List<Word>();
            switch (_registeredUser.Role)
            {
                case Role.TRANSLATOR:
                    Translator translator = (Translator)_registeredUser;
                    createdWords = translator._addedWords;
                    break;
                default:
                    User user = (User)_registeredUser;
                    createdWords = user._addedWords;
                    break;
            }
            List<string> commands = new List<string>();
            foreach (Word word in createdWords)
            {
                string command = word._word;
                commands.Add(command);
            }
            string prefix = "Du hast " + commands.Count + " erstellt: ";
            _inputController.WriteStringList(commands, prefix, null);
        }
        private void GetCountOfAllWordsInDatabase()
        {
            string countMessage = "Zurzeit sind " + _dataTransferObject.GetWordsInDatabaseLength() + " in der Datenbank gespeichert";
            List<string> percentages = _dataTransferObject.GetPercentageOfCorrectTranslatetWords();
            _inputController.WriteStringList(percentages, countMessage, null);
        }
        private void GiveTranslatorALanguage()
        {
            List<Translator> translators = _dataTransferObject.GetAllTranslator();
            _inputController.WriteString("Diesen Übersetzter wurde noch keine Sprache zugewiesen: ");
            foreach (Translator t in translators)
            {
                if (t._language == null)
                {
                    _inputController.WriteString(t._userName);
                }
            }
            List<Language> languages = _dataTransferObject.GetLanguages();
            _inputController.WriteString("Diese Sprachen sind im System Hinterlegt: ");
            foreach (Language language in languages)
            {
                _inputController.WriteString(language._name);
            }
            Translator translator = (Translator)_dataTransferObject.GetTranslator(_inputController.GetStringAnswer("Welchem Übersetzer möchten Sie eine Sprache zuweisen ?"));
            if (translator != null)
            {
                Language language = _dataTransferObject.GetLanguage(_inputController.GetStringAnswer("Welche Sprache möchten Sie Ihm zuweisen?"));
                if (language != null)
                {
                    translator.SetLanguage(language);
                    _dataTransferObject.SaveUser(translator, translator.Role);
                }
                else
                {
                    _inputController.WriteErrorMessage();
                }
            }
            else
            {
                _inputController.WriteErrorMessage();
            }
        }
        private void CreateNewLanguage()
        {
            string answer = _inputController.GetStringAnswer("Welche Sprache möchten sie hinzufügen?");
            if (_dataTransferObject.GetLanguage(answer) == null)
            {
                _dataTransferObject.CreateLanguage(answer, _registeredUser.UUID);
            }
            else
            {
                _inputController.WriteString("Die Sprache " + answer + " wurde leider bereits angelegt.");
                _inputController.WriteErrorMessage();
            }
        }
        private void AddTranslation()
        {
            ListWords();
            Translator translator = (Translator)_registeredUser;
            Language language = translator._language;
            if (language != null)
            {
                _inputController.WriteString("Ihre Sprache ist " + language._name);
                string answer = _inputController.GetStringAnswer("Geben Sie das entsprechende Wort ein: ");
                AbstractTranslation abstracttranslation = _dataTransferObject.GetWordWithMissingTranslation(answer, language);
                if (abstracttranslation != null)
                {
                    string translation = _inputController.GetStringAnswer("Geben Sie die Übersetzung ein: ");
                    abstracttranslation = translator.SetTranslation(abstracttranslation, translation);
                    _dataTransferObject.SaveUser(translator, translator.Role);
                    _dataTransferObject.CreateTranslation(null, null, false, abstracttranslation);
                }
                else
                {
                    _inputController.WriteString("Für dieses Wort gibt es bereits eine Übersetzung");
                }

            }
            else
            {
                _inputController.WriteString("Ihnen wurde leider noch keine Sprache zugewiesen");
            }
        }
        private void SayingGoodbye()
        {
            string answer = _inputController.GetStringAnswer("Geben Sie entweder /start oder /end ein um fortzufahren.");
            switch (answer)
            {
                case "/start":
                    MainLifeCycle();
                    break;
                case "/end":
                    break;
                default:
                    _inputController.WriteErrorMessage();
                    SayingGoodbye();
                    break;
            }
        }
        private void CreateTranslations(string wordUUID, string userUUID)
        {
            _dataTransferObject.CreateTranslation(wordUUID, userUUID, true, null);
        }
        private void ListWords()
        {
            List<string> uncorrectTranslatetWords = _dataTransferObject.GetUncompleteTranslatetWords();
            _inputController.WriteStringList(uncorrectTranslatetWords, "Diese Wörter haben keine vollständige Übersetzung\n", null);
        }
        private void ShowMyTranslations()
        {
            Translator translator = (Translator)_registeredUser;
            int countedtranslations = translator._addedTranslations.Count;
            _inputController.WriteString("Du hast " + countedtranslations + " Übersetzungen angelegt.");
        }
    }
}