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
    public int WordId { get; private set; }

    public int Index { get; private set; }

    public virtual SubtitlePhrase? Phrase { get; private set; }
    public virtual Word? Word { get; private set; }

    public static PhraseWords Create(SubtitlePhrase phrase, Word word,int index) 
    {
        return new PhraseWords()
        {
            Phrase = phrase,
            Word = word,
            Index = index
        };
    
    }
}
