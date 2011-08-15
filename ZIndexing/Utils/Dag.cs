using System;
using System.Text;

namespace ZIndexing.Utils
{
    public class Dag : NTree<string>
    {
        public static NTree<string> Parse(string input) 
        {
            var dag = new Dag();
            var currentNode = dag;

            for(int i = 0; i < input.Length; i++) 
            {
                switch (input[i]) 
                {
                    case '[':
                        {
                            var newNode = new Dag();
                            currentNode.Add(newNode);
                            currentNode = newNode;
                            break;
                        }
                    case ']':
                        {
                            currentNode = (Dag) currentNode.Parent;
                            break;
                        }
                    case ',':   
                        {
                            var newNode = new Dag();
                            currentNode.Parent.Add(newNode);
                            currentNode = newNode;
                            break;
                        }
                    default:
                        currentNode.Value += input[i];
                        break;
                }
            }

            return dag;
        }

        public override string ToString() 
        {
            var sb = new StringBuilder();
            sb.Append(Value);
            
            if (Children.Count != 0)
                sb.Append('[');

            for(var iLoop = 0; iLoop < Children.Count; iLoop++) 
            {
                if (iLoop != 0)
                    sb.Append(',');
                sb.Append(Children[iLoop].ToString());
            }

            if (Children.Count != 0)
                sb.Append(']');

            return sb.ToString();
        }
    }
}