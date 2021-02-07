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
        private void CreateAdminAndTranslator()
        {
            InterfaceUser translator = (Translator)_dataTransferObject.CreateNewUser(Role.TRANSLATOR, Gender.MALE, "translator1234", 1234, new List<Word>(), new List<AbstractTranslation>());
            InterfaceUser admin = (Admin)_dataTransferObject.CreateNewUser(Role.ADMIN, Gender.MALE, "admin1234", 1234, new List<Word>(), new List<AbstractTranslation>());
            _dataTransferObject.SaveUser(translator, Role.TRANSLATOR);
            _dataTransferObject.SaveUser(admin, Role.ADMIN);
        }
        private void RegisterUser()
        {
            string username = _inputController.GetStringAnswer("Bitte erstellen sie ihren Benutzernamen: ");
            int password = _inputController.GetIntAnswer();
        }
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
                // case "/register":
                //     RegisterUser();
                //     break;
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
                        break;
                    case "/search-word":
                        string word = _inputController.GetStringAnswer("Nach welchem Wort wollen Sie suchen?");
                        Word WORD = _dataTransferObject.GetWord(word);
                        if (WORD == null)
                        {
                            _inputController.WriteString("Tut uns leid, das Wort " + word + " haben wir leider nicht in unserem System gefunden.");
                            string boolanswer = _inputController.GetStringAnswer("Wollen Sie das Wort erstellen? Tippe /ja um das Wort zu ertsellen, oder /nein um den Prozess zu beenden");
                            if (boolanswer == "/ja")
                            {
                                Word _word = _dataTransferObject.CreateWord(word);
                                switch (_registeredUser.Role)
                                {
                                    case Role.TRANSLATOR:
                                        Translator t = (Translator)_registeredUser;
                                        CreateTranslations(_word._UUID, t.UUID);
                                        t.SaveWord(_word);
                                        break;
                                    default:
                                        User u = (User)_registeredUser;
                                        CreateTranslations(_word._UUID, u.UUID);
                                        u.SaveWord(_word);
                                        break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            _inputController.WriteTranslationsByWord(_dataTransferObject.GetTranslationByWord(WORD), WORD);
                        }
                        break;
                    case "/get-my-words":
                        List<Word> createdWords = new List<Word>();
                        switch (_registeredUser.Role)
                        {
                            case Role.TRANSLATOR:
                                Translator t = (Translator)_registeredUser;
                                createdWords = t.GetAddedWords();
                                break;
                            default:
                                User u = (User)_registeredUser;
                                createdWords = u.GetAddedWords();
                                break;
                        }
                        _inputController.WriteAddedWords(createdWords);
                        break;
                    case "/get-count-of-all-words-in-database":
                        string count = "Zurzeit sind " + _dataTransferObject.GetWordsInDatabaseLength() + " in der Datenbank gespeichert";
                        List<string> percentages = _dataTransferObject.GetPercentageOfCorrectTranslatetWords();
                        _inputController.WriteStringList(percentages, count, null);
                        break;
                    case "/give-translator-a-language":
                        List<Translator> translator = _dataTransferObject.GetAllTranslator();
                        _inputController.WriteString("Diesen Übersetzter wurde noch keine Sprache zugewiesen: ");
                        foreach (Translator t in translator)
                        {
                            if (t._language == null)
                            {
                                _inputController.WriteString(t._userName);
                            }
                        }
                        List<Language> languages = _dataTransferObject.GetLanguages();
                        _inputController.WriteString("Diese Sprachen sind im System Hinterlegt: ");
                        foreach (Language l in languages)
                        {
                            _inputController.WriteString(l._name);
                        }
                        Translator trans = (Translator)_dataTransferObject.GetTranslator(_inputController.GetStringAnswer("Welchem Übersetzer möchten Sie eine Sprache zuweisen ?"));
                        if (trans != null)
                        {
                            Language gotlanguage = _dataTransferObject.GetLanguage(_inputController.GetStringAnswer("Welche Sprache möchten Sie Ihm zuweisen?"));
                            if (gotlanguage != null)
                            {
                                trans.SetLanguage(gotlanguage);
                                _dataTransferObject.SaveUser(trans, trans.Role);
                            }
                            else
                            {
                                _inputController.WriteErrorMessage();
                                break;
                            }
                        }
                        else
                        {
                            _inputController.WriteErrorMessage();
                            break;
                        }

                        break;
                    case "/create-new-language":
                        string answerlanguage = _inputController.GetStringAnswer("Welche Sprache möchten sie hinzufügen?");
                        if (_dataTransferObject.GetLanguage(answerlanguage) == null)
                        {
                            _dataTransferObject.CreateLanguage(answerlanguage, _registeredUser.UUID);
                        }
                        else
                        {
                            _inputController.WriteString("Die Sprache " + answerlanguage + " wurde leider bereits angelegt.");
                            _inputController.WriteErrorMessage();
                        }
                        break;
                    case "/list-words-with-uncompletet-translations":
                        ListWords();
                        break;
                    case "/add-translation":
                        ListWords();
                        Translator translat = (Translator)_registeredUser;
                        Language language = translat._language;
                        _inputController.WriteString("Ihre Sprache ist " + language._name);
                        if (language != null)
                        {
                            string w = _inputController.GetStringAnswer("Geben Sie das entsprechende Wort ein: ");
                            AbstractTranslation abstracttranslation = _dataTransferObject.GetWordWithMissingTranslation(w, language);
                            if (abstracttranslation != null)
                            {
                                string translation = _inputController.GetStringAnswer("Geben Sie die Übersetzung ein: ");
                                abstracttranslation = translat.SetTranslation(abstracttranslation, translation);
                                _dataTransferObject.SaveUser(translat, translat.Role);
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
                        break;
                    case "/show-my-translations":
                        Translator tra = (Translator)_registeredUser;
                        int countedtranslations = tra._addedTranslations.Count;
                        _inputController.WriteString("Du hast " + countedtranslations + " Übersetzungen angelegt.");
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
        private void CreateTranslations(string wordUuid, string userUuid)
        {
            _dataTransferObject.CreateTranslation(wordUuid, userUuid, true, null);
        }
        private void ListWords()
        {
            List<string> uncorrecttranslatetwords = _dataTransferObject.GetUncompleteTranslatetWords();
            _inputController.WriteStringList(uncorrecttranslatetwords, "Diese Wörter haben keine vollständige Übersetzung\n", null);
        }
    }
}