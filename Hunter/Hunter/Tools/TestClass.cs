using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunter
{
    public class TestEnum
    {
        public static int Count = 0;
        public static int ItemA = Count++;
        public static int ItemB = Count++;
        public static int ItemC = Count++;
        public static int ItemD = Count++;
    }

    [Serializable]
    public class SubTClass
    {
        public int sub_a;// { get; set; }
        public SubTClass()
        {
            sub_a = 0;
        }
        public SubTClass(int value)
        {
            sub_a = value;
        }
    }

    [Serializable]
    public class TestClass
    {
        public int a;// { get; set; }
        public int b;// { get; set; }
        public List<SubTClass> child;// { get; set; }
        public SubTClass sc;//{ get; set; }

        public TestClass()
        {
            a = 0;
            b = 0;
            sc = new SubTClass();
            sc.sub_a = 0;
            child = new List<SubTClass>();
        }

        public void printSelf(string name = "")
        {
            Console.WriteLine(name + ":" + " a = " + a + " b = " + b + " sub_a = " + sc.sub_a);
            Console.Write("child =");
            foreach (var it in child)
            {
                Console.Write(" " + it.sub_a);
            }
            Console.Write("\n");
        }

        public static void Test()
        {
            TestClass tc1 = new TestClass();
            TestClass tc2 = new TestClass();

            tc1.a = 1;
            tc1.b = 2;
            tc1.sc.sub_a = 3;
            tc1.child.Add(new SubTClass(4));
            tc1.child.Add(new SubTClass(5));
            tc1.child.Add(new SubTClass(6));

            tc2 = Utility.Clone(tc1);

            tc1.a = 4;
            tc1.b = 5;
            tc1.sc.sub_a = 6;
            tc1.child.Clear();
            tc1.child.Add(new SubTClass(7));
            tc1.child.Add(new SubTClass(8));
            tc1.child.Add(new SubTClass(9));

            tc1.printSelf("tc1");
            tc2.printSelf("tc2");
        }
    }
}
