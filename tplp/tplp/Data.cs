/*
   _._
 o|- -|o this file is licensed under cc4-by-nc-sa international license.
  ( l )  to view a copy of this license, visit http://creativecommons.org/licenses/by-nc-sa/4.0/
    =    author: jean-marc "jihem" quere 2022 - metalab@sonaliwan.fr
*/
using System.Text.Json;

namespace tplp;

public class Data
{
    public List<Term> Terms;
    public int TermsIndex=0;
    private static readonly Lazy<Data> _instance =
        new Lazy<Data>(() => new Data());
 
    public static Data Instance
    {
        get { return _instance.Value; }
    }

    private Data() {
        /* 
        Terms=new List<Term>();
        Terms.Add(new Term {
            entity="a",
            classes=new Dictionary<string, string> 
            {
                { "I", "ah!" } 
            },
            index=1
        });
        */
        Terms=JsonSerializer.Deserialize<List<Term>>(File.ReadAllText("./data/terms.json")) 
            ?? new List<Term>();
    }
}