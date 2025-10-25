/*
   _._
 o|- -|o this file is licensed under cc4-by-nc-sa international license.
  ( l )  to view a copy of this license, visit http://creativecommons.org/licenses/by-nc-sa/4.0/
    =    author: jean-marc "jihem" quere 2022 - metalab@sonaliwan.fr
*/
namespace tplp;
    
public class Node : Term
{
    public Node parent { get; set; } = null!;
    public List<Node> children { get; set; } = new List<Node>();
    public Node()
    {
    }
    public Node (string entity,string classe="")
    {
        var t=Data.Instance.Terms.Where(t=>(t.entity??"").Equals(entity)).FirstOrDefault();
        this.entity=entity;
        if ((t!=null) && string.IsNullOrEmpty(classe)) 
        {
            this.index=t.index;
            this.classes=new Dictionary<string, string>(t.classes);
        }
        else
            this.classes=new Dictionary<string, string>{ { classe, "" } };
    }
   public void PrintPretty(string indent, bool last)
   {
        Console.Write(indent);
        if (last)
        {
            Console.Write("\\_");
            indent += "  ";
        }
        else
        {
            Console.Write("|_");
            indent += "| ";
        }
        if (this.index==0)
        {
            Console.WriteLine($"{this.entity}");
        }
        else
            Console.WriteLine($"{this.entity} [{this.index}] ({String.Join(',',this.classes.Keys)})");

        for (int i = 0; i < this.children.Count; i++)
            this.children[i].PrintPretty(indent, i == this.children.Count - 1);
   }    
}
