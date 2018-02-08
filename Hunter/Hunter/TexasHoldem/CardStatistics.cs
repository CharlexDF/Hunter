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
            return cardList;
        }

        public static List<Card> GetCardListByColor(List<Card> inCardList, int iColor)
        {
            List<Card> retList = new List<Card>();
            foreach (var card in inCardList)
            {
                if (card.iColor != iColor)
                {
                    continue;
                }
                retList.Add(card);
            }
            return retList;
        }

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


        public static List<Card> CopyCardList(List<Card> inCardList)
        {
            List<Card> retList = new List<Card>();
            foreach (var item in inCardList)
            {
                retList.Add(item);
            }
            return retList;
        }


        public delegate void DeleCardList(List<Card> inCardList);
        public static void Combination(List<Card> poolList, int iNum, List<Card> retList = null, DeleCardList deleCardList = null)
        {
            Debug.Assert(iNum > 0);
            Debug.Assert(poolList.Count >= iNum);

            if (retList == null)
            {
                retList = new List<Card>();
            }

            if (iNum == 1)
            {
                foreach (var item in poolList)
                {
                    retList.Add(item);
                    if (deleCardList != null)
                    {
                        deleCardList.Invoke(retList);
                        //GetHandResult(retList);
                    }
                    retList.Remove(item);
                }
                return;
            }

            List<Card> next_poolList = new List<Card>();
            foreach (var item in poolList)
            {
                next_poolList.Add(item);
            }
            for (int i = 0; i < poolList.Count - 1; i++)
            {
                if (next_poolList.Count < iNum)
                {
                    return;
                }
                var item = poolList[i];
                retList.Add(item);
                next_poolList.Remove(item);

                Combination(next_poolList, iNum - 1, retList, deleCardList);

                retList.Remove(item);
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

        public static List<List<Card>> GetAllHandCardList()
        {
            List<List<Card>> allHandCardList = new List<List<Card>>();

            List<Card> allCardList = CardList.GetAllCardList();
            List<Card> spadeCardList = CardList.GetCardListByColor(allCardList, Color.Spade);
            List<Card> heartCardList = CardList.GetCardListByColor(allCardList, Color.Heart);
            
            //same color
            int iSameColor = 0;
            for (int i = 0; i < spadeCardList.Count; i++)
            {
                var card1 = spadeCardList[i];
                for (int j = i + 1; j < spadeCardList.Count; j++)
                {
                    var card2 = spadeCardList[j];
                    List<Card> handCard = new List<Card>();
                    handCard.Add(card1);
                    handCard.Add(card2);
                    allHandCardList.Add(handCard);
                    iSameColor++;
                }
            }
            Utility.Log("iSameColor = " + iSameColor);

            //diff color include same number
            int iDiffColor = 0;
            for (int i = 0; i < spadeCardList.Count; i++)
            {
                var card1 = spadeCardList[i];
                for (int j = i; j < heartCardList.Count; j++)
                {
                    var card2 = heartCardList[j];
                    List<Card> handCard = new List<Card>();
                    handCard.Add(card1);
                    handCard.Add(card2);
                    allHandCardList.Add(handCard);
                    iDiffColor++;
                }
            }
            Utility.Log("iDiffColor = " + iDiffColor);
            
            Utility.Log("allHandCardList.Count = " + allHandCardList.Count);
            Utility.Log("It is gonna be 78, 78+13=91, 169");

            return allHandCardList;
        }

        public static void GetAllOppoCardList()
        {
            List<List<Card>> allHandCardList = GetAllHandCardList();
            List<Card> allCardList = CardList.GetAllCardList();

            //spade spade VS spade spade
            //spade spade VS heart heart //equal to (club club) (diamond diamond)
            //spade spade VS spade heart //equal to (spade club) (spade diamond)
            //spade spade vs heart club //equal to (heart diamond) (club diamond)
            //spade heart vs club diamond
        }

        public static void DrawCard()
        {
            
        }

    }
}
