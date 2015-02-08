using System;

namespace Sample01CSharp
{
    public class Person
    {
        public int m_old;
        public string m_name;

        public Person(){
            m_old = 10;
            m_name = "Hoge";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Person[] hoges = new Person[10];

            for (int i = 0; i < 10; ++i)
                hoges[i] = new Person(); //各要素のコンストラクタを明示的に呼び出す

            for (int i = 0; i < 10; ++i)
                Console.WriteLine(i + " : Old = " + hoges[i].m_old + "  Names = " + hoges[i].m_name);
        }
    }
}
