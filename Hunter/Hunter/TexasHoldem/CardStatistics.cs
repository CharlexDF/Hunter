using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Xml;

namespace Hunter.TexasHoldem
{
    public class CardList
    {
        public List<Card> mCardList = new List<Card>();
        public static string ToString(List<Card> inCardList)
        {
            string strCardList = "";
            foreach (var item in inCardList)
            {
                strCardList += Card.ToString(item) + ",";
            }
            return strCardList;
        }
    }

    public class CardStatistics
    {
        public static List<List<Card>> sAllType = new List<List<Card>>();
        public static List<List<Card>> sHandCardList = new List<List<Card>>();
        public List<Card> mCardList = new List<Card>();

        public bool IsSameSituation()
        {
            return false;
        }

        public int GetSituationID()
        {
            return 0;
        }

        public static void WriteHandCardList()
        {
            string folderName = "/Data";
            string fileName = "HandCard.cs";
            string path = Utility.CheckWriteFilePath(folderName, fileName);

            FileStream fs = new FileStream(path, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);

            sw.WriteLine("PlayerACard");

            foreach (var item in sHandCardList)
            {
                sw.WriteLine(CardList.ToString(item));
            }

            sw.Close();
            fs.Close();
        }

        public static void RecordPlayerACard(List<Card> inCardList)
        {
            List<Card> tCardList = new List<Card>();
            foreach (var item in inCardList)
            {
                tCardList.Add(item);
            }
            sHandCardList.Add(tCardList);
        }
    }
}
