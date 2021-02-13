using System;
using System.Collections.Generic;
using System.Collections;
namespace TranslationMemory
{
    class User : InterfaceUser
    {
        protected Gender _gender;
        protected Role _role;
        protected string _UUID;

        public List<Word> _addedWords { get; protected set; }
        public Gender Gender
        {
            get { return _gender; }
            set { _gender = value; }
        }
        public Role Role
        {
            get { return _role; }
            set { _role = value; }
        }
        public string UUID
        {
            get { return _UUID; }
            set
            {
                UUID = value;
            }
        }
        public User(Gender gender, Role role, List<Word> words, string uuid)
        {
            _gender = gender;
            _role = role;
            _UUID = uuid;
            _addedWords = words;

        }
        public void SaveWord(Word word)
        {
            _addedWords.Add(word);
        }
    }
}