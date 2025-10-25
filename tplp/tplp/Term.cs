/*
   _._
 o|- -|o this file is licensed under cc4-by-nc-sa international license.
  ( l )  to view a copy of this license, visit http://creativecommons.org/licenses/by-nc-sa/4.0/
    =    author: jean-marc "jihem" quere 2022 - metalab@sonaliwan.fr
*/
namespace tplp;
    
public class Term
{
    public String? entity { get; set; }
    public Dictionary<string,string> classes { get; set; } = null!;
    public int index { get; set; }
}
/*
 N noun
 V verb
IV intransitive verb
TV transitive verb
AJ adjective
AV adverb 
   adposition
 P pronoun
   conjunction
   determiner
 I interjection
*/