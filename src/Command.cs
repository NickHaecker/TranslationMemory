using System;
using System.Collections.Generic;
using System.Collections;

namespace TranslationMemory
{
    class Command
    {
        public string _command { get; private set; }
        public string[] _userType = new string[0];
        public Command(string command, string[] usertype)
        {
            _command = command;
            _userType = usertype;
        }
    }
}
