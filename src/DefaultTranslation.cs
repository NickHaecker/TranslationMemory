using System;

namespace TranslationMemory
{
    class DefaultTranslation : AbstractTranslation
    {
        public DefaultTranslation(Language language, string translation, string wordID, string author) : base(language, translation, wordID, author) { }

        public override void SetTranslation(string translation)
        {
            Translation = translation;
        }
        public override void SetAuthor(string author)
        {
            AUTHOR = author;
        }
    }
}
