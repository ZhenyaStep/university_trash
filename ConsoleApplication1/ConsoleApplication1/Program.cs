using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    enum typeNode {sentence, subject, predicate, article, noun, verb, obj, error};

    class Tree
    {
        private string value;
        private typeNode lexema;
        private Tree[] sons;
        private typeNode father;

        private string[] nounArr = { "cat", "John", "Meredith", "Sunday" , "book"};
        private string[] verbArr = { "eats", "watches", "makes", "plays", "go", "run"};
        private string[] articleArr = { "a", "an", "the"};

        private bool isNoun(string word)
        {
            for (int i = 0; i < nounArr.Length; i++)
            {
                if (nounArr[i].Equals(word))
                    return true;
            }
            return false;
        }

        private bool isArticle(string word)
        {
            for (int i = 0; i < articleArr.Length; i++)
            {
                if (articleArr[i].Equals(word))
                    return true;
            }
            return false;
        }

        private bool isVerb(string word)
        {
            for (int i = 0; i < verbArr.Length; i++)
            {
                if (verbArr[i].Equals(word))
                    return true;
            }
            return false;
        }

        public typeNode getType(string word)
        {
            if (isArticle(word))
                return typeNode.article;
            if (isVerb(word))
                return typeNode.verb;
            if (isNoun(word))
                return typeNode.noun;
            return typeNode.error;
        }

        public Tree(typeNode type, string word, typeNode father)
        {
            this.lexema = type;
            this.value = word;
            this.father = father;
            this.sons = new Tree[0];
        }

        private bool addNode(typeNode curType, string element, typeNode fatherType, Tree root, ref bool stop)
        {
            if (stop)
            {
                return true;
            }
            else
            {
                if ((root.sons.Length == 0) && (root.lexema != fatherType))
                    return false;
                if (root.lexema == fatherType)
                {
                    int numSons = root.sons.Length + 1;
                    Array.Resize(ref root.sons, numSons);
                    root.sons[numSons - 1] = new Tree(curType, element, fatherType);
                    stop = true;
                    return true;
                }
            }
            for (int i = 0; (i < root.sons.Length) && !stop; i++)
            {
                if (root.sons[i].lexema == fatherType)
                {
                    int numSons = root.sons[i].sons.Length + 1;
                    Array.Resize(ref root.sons[i].sons, numSons);
                    root.sons[i].sons[numSons - 1] = new Tree(curType, element, fatherType);
                    stop = true;
                    return true;
                }
                else
                    addNode(curType, element, fatherType, root.sons[i], ref stop);
            }
            return false;
        }

        public bool findPlace(typeNode myType, string word, typeNode fatherNode, Tree head)
        {
            bool flag = false;
            if (myType == typeNode.error)
                return false;
            return addNode(myType, word, fatherNode, head, ref flag);
        }

        public typeNode getFatherType(string word, int index, out typeNode curType)
        {
            curType = getType(word);
            if (curType == typeNode.article)
            {
                if (index == 0)
                    return typeNode.subject;
                if ((index == 3)|| (index == 2))
                    return typeNode.obj;
            }
            if (curType == typeNode.noun)
            {
                if ((index == 0) || (index == 1))
                    return typeNode.subject;
                if ((index == 2) || (index ==  3) || (index == 4))
                    return typeNode.obj;
            }
            if (curType == typeNode.verb)
            {
                if ((index == 1) || (index == 2))
                    return typeNode.predicate;
            }
            return typeNode.error;
        }

        public bool checkIfRightTree(Tree myTree)
        {
            if ((myTree.sons[0].sons.Length == 0) || (myTree.sons[0].sons.Length > 2))
            {
                return false;
            }

            if ((myTree.sons[0].sons.Length == 1) && (myTree.sons[0].sons[0].lexema == typeNode.article))
            {
                return false;
            }

            if ((myTree.sons[0].sons.Length == 2) && (myTree.sons[0].sons[0].lexema == typeNode.noun))
            {
                return false;
            }

            if (myTree.sons[1].sons.Length != 2)
            {
                return false;
            }

            if ((myTree.sons[1].sons[0].sons.Length == 0) || (myTree.sons[1].sons[0].sons.Length > 2))
            {
                return false;
            }

            if ((myTree.sons[1].sons[0].sons.Length == 2) && (myTree.sons[1].sons[0].sons[0].lexema == typeNode.noun))
            {
                return false;
            }

            return true;
        }

        public void search(Tree root, typeNode x, ref string result, ref bool isFound)
        {
            if (isFound)
            {
                return;
            }
            if (root.lexema == x)
            {
                result = root.value;
                isFound = true;
                return;
            }
            else
            {
                for (int i = 0; i < root.sons.Length; i++)
                {
                    search(root.sons[i], x, ref result, ref isFound);
                }
            }
        }

        public void getUpAnalysis(Tree tree)
        {
            string article1 = "", article2 = "";
            string noun1 = "", noun2 = "";
            string verb = "";
            int step = 0;
          
            bool flag = false;

            Console.WriteLine("Going-Up Analysis:");

            search(tree.sons[0], typeNode.article, ref article1, ref flag);
            if (flag)
            {
                step++;
                Console.WriteLine("{0}) article => {1}", step, article1);
            }
            flag = false;

            search(tree, typeNode.noun, ref noun1, ref flag);
            step++;
            Console.WriteLine("{0}) noun => {1}", step, noun1);
            flag = false;

            step++;
            Console.WriteLine("{0}) subject => {1} {2}", step, article1, noun1);

            search(tree, typeNode.verb, ref verb, ref flag);
            step++;
            Console.WriteLine("{0}) verb => {1}", step, verb);
            flag = false;

            search(tree.sons[1], typeNode.article, ref article2, ref flag);
            if (flag)
            {
                step++;
                Console.WriteLine("{0}) article => {1}", step, article2);
            }
            flag = false;

            search(tree.sons[1], typeNode.noun, ref noun2, ref flag);
            step++;
            Console.WriteLine("{0}) noun => {1}", step, noun2);
            flag = false;

            step++;
            Console.WriteLine("{0}) object => {1} {2}", step, article2, noun2);

            step++;
            Console.WriteLine("{0}) predicate => {1} {2} {3}", step, verb, article2, noun2);

            step++;
            Console.WriteLine("{0}) sentence => {1} {2} {3} {4} {5}", step, article1, noun1, verb, article2, noun2);
 
        }

        public void getDownAnalysis(Tree tree)
        {
            string article1 = "", article2 = "";
            string noun1 = "", noun2 = "";
            string verb = "";
            int step = 0;

            bool flag = false;

            Console.WriteLine("Going-Down Analysis:");

            search(tree.sons[0], typeNode.article, ref article1, ref flag);
            flag = false;

            search(tree, typeNode.noun, ref noun1, ref flag);
            flag = false;

            search(tree, typeNode.verb, ref verb, ref flag);
            flag = false;

            search(tree.sons[1], typeNode.article, ref article2, ref flag);
            flag = false;

            search(tree.sons[1], typeNode.noun, ref noun2, ref flag);
            flag = false;

            step++;
            Console.WriteLine("{0}) sentence => {1} {2} {3} {4} {5}", step, article1, noun1, verb, article2, noun2);

            step++;
            Console.WriteLine("{0}) subject => {1} {2}", step, article1, noun1);

            search(tree.sons[0], typeNode.article, ref article1, ref flag);
            if (flag)
            {
                step++;
                Console.WriteLine("{0}) article => {1}", step, article1);
            }
            flag = false;

            step++;
            Console.WriteLine("{0}) noun => {1}", step, noun1);

            step++;
            Console.WriteLine("{0}) predicate => {1} {2} {3}", step, verb, article2, noun2);

            step++;
            Console.WriteLine("{0}) verb => {1}", step, verb);

            step++;
            Console.WriteLine("{0}) object => {1} {2}", step, article2, noun2);


            search(tree.sons[1], typeNode.article, ref article2, ref flag);
            if (flag)
            {
                step++;
                Console.WriteLine("{0}) article => {1}", step, article2);
            }

            step++;
            Console.WriteLine("{0}) noun => {1}", step, noun2);
   
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            string sentence;
            bool validSent = true;

            Tree lexemTree = new Tree(typeNode.sentence, "-", typeNode.error);
            lexemTree.findPlace(typeNode.subject, "-", typeNode.sentence, lexemTree);
            lexemTree.findPlace(typeNode.predicate, "-", typeNode.sentence, lexemTree);
            lexemTree.findPlace(typeNode.obj, "-", typeNode.predicate, lexemTree);
         
            Console.WriteLine("Grammar of construction:");
            Console.WriteLine("Sentence -> Subject | Predicate");
            Console.WriteLine("Subject -> article + noun| noun");
            Console.WriteLine("article -> a|an|the");
            Console.WriteLine("Predicate -> verb|object");
            Console.WriteLine("object -> article + noun|noun");
            Console.WriteLine();


            Console.WriteLine("Enter your sentence: ");
            sentence = Console.ReadLine();
            string[] tokens = sentence.Split(' ');

            for (int i = 0; i < tokens.Length; i++)
            {
                typeNode myType;
                typeNode father = lexemTree.getFatherType(tokens[i], i, out myType);
                if (father == typeNode.error)
                {
                    validSent = false;
                    break;
                }
                lexemTree.findPlace(myType, tokens[i], father, lexemTree);
            }

            if (!lexemTree.checkIfRightTree(lexemTree))
            {
                validSent = false;
            }

            if (validSent)
            {
                lexemTree.getUpAnalysis(lexemTree);
                Console.WriteLine();
                lexemTree.getDownAnalysis(lexemTree);
            }
            else
            {
                Console.WriteLine("Incorrect sentence!");
            }

            Console.ReadKey();
        }
    }
}
