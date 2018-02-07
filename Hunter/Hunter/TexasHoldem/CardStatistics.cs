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

        public static List<Card> GetAllCardList()
        {
            List<Card> cardList = new List<Card>();
            int id = 1;
            for (int iColor = 1; iColor <= Color.Count; iColor++)
            {
                for (int iNum = 1; iNum <= 13; iNum++)
                {
                    Card card = new Card();
                    card.id = id++;
                    card.iColor = iColor;
                    card.iNumber = iNum;
                    cardList.Add(card);
                }
            }
            //PrintAllCard();
            return cardList;
        }

        public static List<Card> CopyCardList(List<Card> inCardList)
        {
            List<Card> retList = new List<Card>();
            foreach (var item in inCardList)
            {
                retList.Add(item);
            }
            return retList;
        }

        public static void Combination(List<Card> poolList, int iNum)
        {
            for (int i = 0; i < iNum; i++)
            {
                for (int j = 0; j < poolList.Count; j++)
                {
                    Card card = poolList[j];
                }
            }
        }

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

        public static void CalcHandCardWinRate()
        {
            GetAllHandCardList();
        }

        public static void GetAllHandCardList()
        {
            List<List<Card>> allHandCardList = new List<List<Card>>();
            List<Card> allCardList = GetAllCardList();
            List<Card> poolList = CopyCardList(allCardList);
            List<Card> retList = new List<Card>();
            foreach (var card1 in allCardList)
            {
                if (card1.iColor != Color.Spade)
                {
                    continue;
                }
                poolList.Remove(card1);
                retList.Add(card1);

                if (poolList.Count == 0)
                {
                    continue;
                }

                foreach (var card2 in poolList)
                {
                    if (card2.iColor == Color.Club || card2.iColor == Color.Diamond)
                    {
                        continue;
                    }
                    retList.Add(card1);
                    allHandCardList.Add(retList);
                    retList.Remove(card1);
                }

                retList.Remove(card1);
                //poolList.Add(card1);
            }

            Utility.Log("allHandCardList.Count = " + allHandCardList.Count);

        }

        public static void DrawCard()
        {
            
        }

    }
}
