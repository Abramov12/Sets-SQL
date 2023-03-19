using System;
using System.Collections.Generic;
using System.Collections;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;

namespace MyApp
{
    public class Set<T> : IEnumerable
    {
        private List<T> items = new List<T>();

        public Set()
        {
        }
        public Set(T item)
        {
            items.Add(item);
        }
        public Set(IEnumerable<T> items)
        {
            this.items = items.ToList();
        }
        public void Add(T item)
        {
            foreach (T i in items)
            {
                if (i.Equals(item))
                {
                    return;
                }
            }
            items.Add(item);

        }
        public void Remove(T item)
        {
            items.Remove(item);
        }
        public int Count(Set<T> set)
        {
            return set.items.Count;
        }

        public Set<T> Union(Set<T> set)
        {
            Set<T> result = new Set<T>();

            foreach (T item in items)
            {
                result.Add(item);
            }
            foreach (T item in set.items)
            {
                result.Add(item);
            }
            return result;
        }
        public Set<T> Intersection(Set<T> set)
        {
            Set<T> result = new Set<T>();
            Set<T> big;
            Set<T> small;

            if (items.Count >= set.items.Count)
            {
                big = this;
                small = set;
            }
            else
            {
                small = this;
                big = set;
            }


            foreach (T item1 in small.items)
            {
                foreach (T item2 in big.items)
                {
                    if (item1.Equals(item2))
                    {
                        result.Add(item1);
                    }
                }
            }
            return result;
        }
        public Set<T> Difference(Set<T> set)
        {
            Set<T> result = new Set<T>(items);
            foreach (T item in set.items)
            {
                result.Remove(item);
            }

            return result;
        }
        public bool Subset(Set<T> set)
        {
            if (items.Count > set.items.Count) return false;

            foreach (T item1 in items)
            {
                bool check = false;
                foreach (T item2 in set.items)
                {

                    if (item1.Equals(item2))
                    {
                        check = true;
                        break;
                    }

                }
                if (!check)
                {
                    return false;
                }
            }
            return true;

        }
        public Set<T> MutualDifferrence(Set<T> set)
        {
            Set<T> result = new Set<T>(items);
            Set<T> result2 = new Set<T>(set.items);
            foreach (T item1 in items)
            {
                result2.Remove(item1);
            }


            foreach (T item1 in set.items)
            {
                result.Remove(item1);
            }
            result.Union(set);
            return result.Union(result2);

        }
        public IEnumerator GetEnumerator()
        {
            return items.GetEnumerator();
        }

    }

    internal class Program
    {
        static Set<string> ReadData(string connectionString, string query)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Set<String> set = new Set<String>();
                    while (reader.Read())
                    {
                        set.Add("Название игры: " + reader[0] + " и её жанр: " + reader[1].ToString() + " и разработчик: " + reader[2].ToString());
                    }
                    reader.Close();
                    return set;
                }
            }
        }

        static void Main(string[] args)
        {
             var first = new Set<int>(new int[] { 1, 2, 3, 4, 5 });
             var second = new Set<int>(new int[] { 4, 5, 6, 7, 8 });
             var third = new Set<int>(new int[] { 1, 2 });
             Console.WriteLine("Union: ");
             foreach (var item in first.Union(second))
             {
                 Console.Write(item + " ");
             }
             Console.WriteLine("");
             Console.WriteLine("Intersection: ");
             foreach (var item in first.Intersection(second))
             {
                 Console.Write(item + " ");
             }
             Console.WriteLine("");
             Console.WriteLine(" A Differrence B");
             foreach (var item in first.Difference(second))
             {
                 Console.Write(item + " ");
             }
             Console.WriteLine();
             Console.WriteLine(" B Differrence A");
             foreach (var item in second.Difference(first))
             {
                 Console.Write(item + " ");
             }
             Console.WriteLine("");
             Console.WriteLine("A SubSet B");
             Console.Write(first.Subset(second));
             Console.WriteLine("");
             Console.WriteLine("C SubSet A");
             Console.Write(third.Subset(first));
             Console.WriteLine("");
             Console.WriteLine("C SubSet B");
             Console.Write(third.Subset(second));
             Console.WriteLine();
             Console.WriteLine("MutualDifference: ");
             foreach (var item in second.MutualDifferrence(first))
             {
                 Console.Write(item + " ");
             }
            

            string connectionString = "Server=.;Database=KURSOVAYA;Trusted_Connection=True;";
            Console.WriteLine();
            string query = "Select  trim (Название_игры), trim (Название_жанра), trim(Название_студии) FROM Жанр_игры JOIN Игра On Жанр_игры.Код_жанра=Игра.Код_жанра JOIN Разработчик ON Игра.Код_разработчика=Разработчик.Код_разработчика where Название_жанра='Экшн'";
            string query2 = "Select  trim (Название_игры), trim (Название_жанра), trim(Название_студии) FROM Жанр_игры JOIN Игра On Жанр_игры.Код_жанра=Игра.Код_жанра JOIN Разработчик ON Игра.Код_разработчика=Разработчик.Код_разработчика where Название_студии='Rockstar'";
            Set<String> set1 = ReadData(connectionString, query);
            Console.WriteLine("Множество 1");
            foreach (var item in set1)
            {
                Console.WriteLine(item);
            }
            Set<String> set2 = ReadData(connectionString, query2);
            Console.WriteLine();
            Console.WriteLine("Множество 2");
            foreach (var item in set2)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine();
            Console.WriteLine("Пример объединения: ");
            foreach (var item in set1.Union(set2))
            {
                Console.WriteLine(item);
            }
            Console.WriteLine();
            Console.WriteLine("Пример пересечения: ");
            foreach (var item in set1.Intersection(set2))
            {
                Console.WriteLine(item);
            }
            Console.WriteLine();
            Console.WriteLine("Пример вычитания: ");
            foreach (var item in set1.Difference(set2))
            {
                Console.WriteLine(item);
            }
            Console.WriteLine();
            Console.WriteLine("Пример двустороннего вычитания: ");
            foreach (var item in set1.MutualDifferrence(set2))
            {
                Console.WriteLine(item);
            }




        }
    }
}