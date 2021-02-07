using System;

namespace TranslationMemory
{
    class TranslationFactory
    {
        public static AbstractTranslation GetTranslation(Language language, string translation, string id, string author)
        {
            if (translation.Length > 0)
            {
                return new Translation(language, translation, id, author);
            }
            else
            {
                return new DefaultTranslation(language, "(Keine)", id, author);
            }
        }
    }
}
