namespace TranslationMemory
{
    abstract class AbstractTranslation
    {
        public Language LANGUAGE { get; protected set; }
        public string Translation { get; protected set; }
        public string WORD_ID { get; protected set; }
        public string AUTHOR { get; protected set; }

        protected AbstractTranslation(Language language, string translation, string wordID, string author)
        {
            LANGUAGE = language;
            Translation = translation;
            WORD_ID = wordID;
            AUTHOR = author;
        }
        public abstract void SetTranslation(string translation);
        public abstract void SetAuthor(string author);
    }
}