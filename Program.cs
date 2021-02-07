using System;

namespace TranslationMemory
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hallo und herzlich Wilkommen zum Translation Memory Programm");
            System translationmemory = new System();
            translationmemory.MainLifeCycle();
            Console.WriteLine("Programm beendet");
        }
    }
}
