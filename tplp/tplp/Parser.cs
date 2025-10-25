/*
   _._
 o|- -|o this file is licensed under cc4-by-nc-sa international license.
  ( l )  to view a copy of this license, visit http://creativecommons.org/licenses/by-nc-sa/4.0/
    =    author: jean-marc "jihem" quere 2022 - metalab@sonaliwan.fr
*/
using System.Text.RegularExpressions;

namespace tplp;
public class Parser 
{
    static public string Normalize(string text)
    {
        int lastIndex=0;
        Boolean skip=false;
        string value;
        string normalizedText="";
        var notAllowed=new Regex("[^ aeioujklmnpstw0123456789.:!?\",]");
        text=Regex.Replace(text, ",", " , ");
        text=Regex.Replace(text, @"\s+", " ");

        string NormalizeSubstring(string text) {
            int lastIndex=0;
            //System.Console.WriteLine(":"+text);
            while (lastIndex<text.Length)
            {
                var match=notAllowed.Match(text,lastIndex);
                if (match.Success) {
                    value=match.Value.RemoveDiacritics().ToUpper();
                    if (value.CompareTo("A")>=0 && value.CompareTo("Z")<=0)
                    {
                        text=text.Replace(match.Value,value);
                        lastIndex=match.Index+1;
                    }
                    else
                        text=text.Replace(match.Value,"");
                    //System.Console.WriteLine($"{match.Value} - {text}");
                }
                else
                    lastIndex=text.Length;
            }
            return text;
        }

        for (int j = text.IndexOf('"'); j > -1; j = text.IndexOf('"', j + 1))
        {
                skip=!skip;
                if (skip)
                {
                    normalizedText+=NormalizeSubstring(text.Substring(lastIndex,j-lastIndex));
                }
                else
                {
                    normalizedText+=text.Substring(lastIndex-1,j-lastIndex+2).Replace(' ','_');
                }
                lastIndex=j+1;                
        }
        normalizedText+=NormalizeSubstring(text.Substring(lastIndex));
//        var matches=notAllowed.Matches(text,0);
//        System.Console.WriteLine(matches.Count);
        return normalizedText;
    }
    static public List<Sentence> getSentenceList(string text)
    {
        var sentences=new List<Sentence>();
        var marks=new char[] { '.', ':', '!', '?', '"' };
        var occurences=new List<Occurence>();
        var i=marks.Length;
        int count=0,lastIndex=0;
        Boolean skip=false;
        while (i-->0)
        {
            for (int j = text.IndexOf(marks[i]); j > -1; j = text.IndexOf(marks[i], j + 1))
            {
                occurences.Add(new Occurence { entity=i, index=j});
            }
        }
        occurences.Sort( (o1,o2) => o1.index.CompareTo(o2.index) );
        occurences.ForEach(o => 
        {
            // System.Console.WriteLine($"- {o.index} : {o.entity}");
            if (o.entity!=4)
            {
                if (skip==false) {
                    sentences.Add(new Sentence {
                        entity=text.Substring(lastIndex,o.index-lastIndex).Trim(),
                        mark=o.entity,
                        index=++count
                    });
                    lastIndex=o.index+1;
                }
            }
            else
                skip=!skip;
        });
        return sentences;
    }

    static public Node getWordList(string text)
    {
        var root=new Node();
        bool precededByComma = false;
        text.Split(' ').ToList().ForEach(w=>{
            if (w.Equals(","))
            {
                precededByComma = true;
            }
            else
            {
                if (precededByComma)
                {
                    if (w.Equals("la"))
                        w = ",la";
                    precededByComma = false;
                }
                var t = Data.Instance.Terms.Find(t => t.entity == w);
                if (t == null)
                {
                    t = new Term
                    {
                        entity = w.First() == '\"' ? w : w.First().ToString().ToUpper() + w.Substring(1).ToLower(),
                        classes = new Dictionary<string, string>() {{"N", ""}},
                        index = --Data.Instance.TermsIndex
                    };
                    Data.Instance.Terms.Add(t);
                }

                root.children.Add(new Node
                {
                    parent = root,
                    entity = t.entity,
                    classes = new Dictionary<string, string>(t.classes),
                    index = t.index
                });
                //System.Console.WriteLine($"- {t.entity} [{t.index}]");
            }
        });
        //System.Console.WriteLine(Data.Instance.Terms.Count());
        return root;
    }

    static public Node Analyze(Node n)
    {
        int current=0,first=0,last=n.children.Count-1;
        var root=new Node();
        if (last>=0)
        { 
            var n_C=new Node("(CX)");
            var n_S=new Node("(SU)");
            var n_P=new Node("(PD)");
            Node element;
            Node nextElement=null!;
            Node pdElement=null!;
            Node ppElement=null!;
            Node cdElement=null;
            string entity;
            int laCount=0;
            root.children.Add(n_C); // ,la
            root.children.Add(n_S);
            root.children.Add(n_P);

            while (current<=last)
            {
                entity = n.children.ElementAt(current).entity ?? "";
                if (entity.Equals("la")||entity.Equals(",la"))
                    laCount++;
                current++;
            }
            current=0;

            while (current<=last)
            {
                element=n.children.ElementAt(current);
                entity=element.entity??"";

                if (entity.Equals("pi"))
                {
                    current+=2;
                }
                else
                {
                    if (nextElement == null)
                    {
                        if (entity.Equals("la") || entity.Equals(",la"))
                        {
                            n_C.children.Add(element); // add from first to current
                            while (first < current)
                            {
                                element.children.Add(n.children.ElementAt(first));
                                first++;
                            }

                            first = current + 1;
                            laCount--;
                        }

                        if (laCount == 0 && (((entity.Equals("mi") || entity.Equals("sina")) && current == first) ||
                            entity.Equals("li") || entity.Equals("o")))
                        {
                            if (current == first)
                            {
                                if (current == last || ((current + 1 <= last) &&
                                                        (new List<string>() {"anu", "la", ",la"}).Where(s =>
                                                            (n.children.ElementAt(current + 1).entity ?? "").Equals(s))
                                                        .Count() == 0))
                                {
                                    if (entity.Equals("o"))
                                    {
                                        nextElement = new Node("o");
                                        pdElement = nextElement;
                                        ppElement = null;
                                        cdElement = null;
                                        n_P.children.Add(nextElement);
                                        first = current + 1;
                                    }
                                    else
                                    {
                                    var en = new Node("en");
                                    n_S.children.Add(en);
                                    en.children.Add(element);
                                    first = current + 1;
                                    if ((current + 1 <= last) && (new List<string>() {"anu", "en", "o"})
                                        .Where(s => (n.children.ElementAt(current + 1).entity ?? "").Equals(s))
                                        .Count() == 0)
                                    {
                                        nextElement = new Node("li");
                                        pdElement = nextElement;
                                        ppElement = null;
                                        cdElement = null;
                                        n_P.children.Add(nextElement);
                                    }
                                    }
                                }
                            }
                            else
                            {
                                var en = new Node("en");
                                n_S.children.Add(en);
                                if ((n.children.ElementAt(first).entity ?? "").Equals("en")) first++;
                                while (first < current)
                                {
                                    if ((n.children.ElementAt(first).entity ?? "").Equals("en"))
                                    {
                                        en = new Node("en");
                                        n_S.children.Add(en);
                                        first++;
                                    }
                                    if (first<current)
                                        en.children.Add(n.children.ElementAt(first));
                                    first++;
                                }

                                first = current + 1;
                                nextElement = new Node(entity);
                                if (entity.Equals("li") || entity.Equals("o"))
                                {
                                    pdElement = nextElement;
                                    ppElement = null;
                                    cdElement = null;
                                }

                                n_P.children.Add(nextElement);
                            }
                        }
                    }
                    else
                    {
                        if (current == last ||
                            (new List<string>() {"e", "li", "o", "lon", "sama", "tan", "tawa", "kepeken"})
                            .Where(s => entity.Equals(s)).Count() == 1)
                        {
                            if (current == last) current++;
                            while (first < current)
                            {
                                nextElement.children.Add(n.children.ElementAt(first));
                                first++;
                            }

                            if (current < last)
                            {
                                first = current + 1;
                                if ((new List<string>() {"li", "o"}).Where(s => entity.Equals(s)).Count() == 1)
                                {
                                    nextElement = new Node(entity);
                                    pdElement = nextElement;
                                    ppElement = null;
                                    cdElement = null;
                                    n_P.children.Add(nextElement);
                                }
                                else
                                {
                                    if (entity.Equals("e"))
                                    {
                                        if (cdElement == null)
                                        {
                                            cdElement = new Node("(CD)");
                                            pdElement.children.Add(cdElement);
                                        }

                                        nextElement = new Node(entity);
                                        cdElement.children.Add(nextElement);
                                    }
                                    else
                                    {
                                        if ((ppElement == null) && (nextElement.children.Count == 0))
                                        {
                                            nextElement.children.Add(new Node(entity));
                                        }
                                        else
                                        {
                                            if (ppElement == null)
                                            {
                                                ppElement = new Node("(PP)");
                                                pdElement.children.Add(ppElement);
                                            }

                                            nextElement = new Node(entity);
                                            ppElement.children.Add(nextElement);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                current++;
            }

            if (n_S.children.Count == 0 && first == 0)
            {
                var en = new Node("en");
                n_S.children.Add(en);
                while (first < current)
                {
                    if ((n.children.ElementAt(first).entity ?? "").Equals("en"))
                    {
                        en = new Node("en");
                        n_S.children.Add(en);
                        first++;
                    }
                    if (first<current)
                        en.children.Add(n.children.ElementAt(first));
                    first++;
                }
            }
           
            if (nextElement != null && nextElement.children.Count == 0)
            {
                while (first <= last)
                {
                    nextElement.children.Add(n.children.ElementAt(first));
                    first++;
                }
            }

            n_C.children.ForEach(c => {
                var cxN=Analyze(c);
                cxN.children.RemoveAt(0);
                c.children.Clear();
                //c.children.Add(cxN);
                c.children=cxN.children;
            });
            
        }

        return root;
    }
}
