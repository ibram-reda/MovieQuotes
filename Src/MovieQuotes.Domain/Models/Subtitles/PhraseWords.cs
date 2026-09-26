using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MovieQuotes.Domain.Models;

public class PhraseWords
{
    public int PhraseId { get; private set; }
    public int WordId { get;  set; }

    public int Index { get;  set; }

    public virtual SubtitlePhrase? Phrase { get; private set; }
    public virtual Word? Word { get;  set; }

    public static PhraseWords Create(SubtitlePhrase phrase, Word word,int index) 
    {
        return new PhraseWords()
        {
            WordId = word.Id,
            Phrase = phrase,
            Word = word,
            Index = index
        };
    
    }

    public static PhraseWords Create(int phraseID, int wordID, int index)
    {
        return new PhraseWords()
        {
            WordId = wordID,
            PhraseId = phraseID, 
            Index = index
        };

    }
    public override string ToString() => $"{Index}, {Word?.Text}";
}
