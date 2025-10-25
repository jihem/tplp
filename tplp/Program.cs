/*
   _._
 o|- -|o this file is licensed under cc4-by-nc-sa international license.
  ( l )  to view a copy of this license, visit http://creativecommons.org/licenses/by-nc-sa/4.0/
    =    author: jean-marc "jihem" quere 2022 - metalab@sonaliwan.fr
*/
tplp.Node root=new tplp.Node();

//var txt=tplp.Parser.Normalize(File.ReadAllText("./data/sample1.txt"));
//var txt=tplp.Parser.Normalize("nimi mi li janh étàn. sina \"ZZ ZZ\".");
//var txt=tplp.Parser.Normalize("tenpo ni en tenpo pini la lon ma mi la meli mi en mi li pilin pona.");
//var txt=tplp.Parser.Normalize("mi la sina wawa lili.");
//var txt=tplp.Parser.Normalize("tenpo ni la mi moku e kili sewi e telo kepeken uta kepeken ilo sina li kalama mute lon tomo.");
//var txt=tplp.Parser.Normalize("sina pilin pona la sina tawa musi kepeken noka.");
//var txt=tplp.Parser.Normalize("sina pilin pona, la sina tawa musi.");
//var txt=tplp.Parser.Normalize("mi pilin e ni: sina pona.");
//var txt=tplp.Parser.Normalize("mi pilin e pona tan tenpo pi musi tawa kepeken kute mi.");
//var txt=tplp.Parser.Normalize("mi pilin e tenpo pi musi tawa.");
//var txt=tplp.Parser.Normalize("ken la sina wile jo e sona pi toki pona.");
//var txt=tplp.Parser.Normalize("tenpo suno pini la mi moku e kala wan lon poka seli.");
//var txt=tplp.Parser.Normalize("o toki ala!");
//var txt = tplp.Parser.Normalize("toki ike ala la kalama musi sina en selo waso sina li sama, la sina waso sewi tan ma pi kasi suli.");
var txt = tplp.Parser.Normalize("jan Etan li kama. ona li pilin pona.");
var msl=tplp.Parser.getSentenceList(txt);
msl.ForEach(s=> {
    System.Console.WriteLine($"{s.index} : {s.entity}{s.mark}");
    root=tplp.Parser.getWordList(s.entity??"");
    root.PrintPretty("",true);
    tplp.Parser.Analyze(root).PrintPretty("", true);    
});
/*
 todo:

 - ,la
 - pi
 - kule x en y
*/