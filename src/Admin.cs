using System;
using System.Collections.Generic;
using System.Collections;
namespace TranslationMemory
{
    class Admin : InterfaceUser
    {
        private Gender _gender;
        private Role _role;
        private string _UUID;
        public string _userName { get; private set; }
        public int _password { get; private set; }
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

        public Admin(Gender gender, Role role, string username, int password, string uuid)
        {
            _gender = gender;
            _role = role;
            _userName = username;
            _password = password;
            _UUID = uuid;
        }
    }
}