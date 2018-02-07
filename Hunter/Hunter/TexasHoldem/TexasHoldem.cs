using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Hunter.Tools;

namespace Hunter.TexasHoldem
{
    public class Result
    {
        public int id;
        public int iWinRate;
    }
    
    public class HandCardWinRate
    {
        public List<HandCard> mHandCardList;
        public static void Statistics()
        {

        }
        public static void PrintHandWinRate()
        {
        }
    }
    
    class Player
    {
        public List<Card> handCards = new List<Card>();
    }

    public class TexasHoldem
    {
        public static SortedList<int, Card> AllCardList = new SortedList<int, Card>();
        public static SortedList<int, Result> ResultList = new SortedList<int, Result>();
        
        public static SortedList<int, Card> CreateAllCardList()
        {
            if (AllCardList.Count != 0)
            {
                return AllCardList;
            }
            int id = 1;
            for (int iColor = 1; iColor <= Color.Count; iColor++)
            {
                for (int iNum = 1; iNum <= 13; iNum++)
                {
                    Card card = new Card();
                    card.id = id++;
                    card.iColor = iColor;
                    card.iNumber = iNum;
                    AllCardList.Add(card.id, card);
                }
            }
            //PrintAllCard();
            return AllCardList;
        }

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
        public static void PrintAllCard()
        {
            foreach (var item in AllCardList)
            {
                Console.WriteLine("index = " + item.Value.id
                    + " color = " + item.Value.iColor
                    + " number = " + item.Value.iNumber);
            }
        }
        
        public static void Permutation(List<Card> poolList, int iNum, List<Card> retList = null)
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
                    GetHandResult(retList);
                    retList.Remove(item);
                }
                return;
            }

            List<Card> next_poolList = new List<Card>();
            foreach (var item in poolList)
            {
                next_poolList.Add(item);
            }
            foreach (var item in poolList)
            {
                retList.Add(item);
                next_poolList.Remove(item);

                Permutation(next_poolList, iNum - 1, retList);

                retList.Remove(item);
                next_poolList.Add(item);
            }
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

        public static int IterationCount = 0;
        public static void GetHandResult(List<Card> inCardList)
        {
            IterationCount++;
            /*
            List<Card> playerA_Cards = new List<Card>();
            List<Card> playerB_Cards = new List<Card>();

            playerA_Cards.Add(in_CardList[0]);
            playerA_Cards.Add(in_CardList[1]);

            playerA_Cards.Add(in_CardList[4]);
            playerA_Cards.Add(in_CardList[5]);
            playerA_Cards.Add(in_CardList[6]);
            playerA_Cards.Add(in_CardList[7]);
            playerA_Cards.Add(in_CardList[8]);

            playerB_Cards.Add(in_CardList[2]);
            playerB_Cards.Add(in_CardList[3]);

            playerB_Cards.Add(in_CardList[4]);
            playerB_Cards.Add(in_CardList[5]);
            playerB_Cards.Add(in_CardList[6]);
            playerB_Cards.Add(in_CardList[7]);
            playerB_Cards.Add(in_CardList[8]);

            CompareCards(playerA_Cards, playerB_Cards);
            */

            CardStatistics.RecordPlayerACard(inCardList);
            bool bNeedPrint = false;
            if (bNeedPrint)
            {
                foreach (var item in inCardList)
                {
                    Console.Write(" " + item.Print());
                }
                Console.Write("\n\n");
            }
        }

        public static void CalcBeginWinRate()
        {
            List<List<Card>> AllHandCard = new List<List<Card>>();
            List<Card> poolList = GetAllCardList();
            List<Card> hands = new List<Card>();
            //Card cSA = poolList.Find(p => p.iColor == Color.Spade && p.iNumber == 1);
            //Card cHA = poolList.Find(p => p.iColor == Color.Heart && p.iNumber == 1);
            //poolList.Remove(cSA);
            //poolList.Remove(cHA);
            //hands.Add(cSA);
            //hands.Add(cHA);

            //Card cCA = poolList.Find(p => p.iColor == Color.Club && p.iNumber == 1);
            //Card cDA = poolList.Find(p => p.iColor == Color.Diamond && p.iNumber == 1);
            //poolList.Remove(cCA);
            //poolList.Remove(cDA);

            Combination(poolList, 2, hands);
            CardStatistics.WriteHandCardList();
            //Permutation(poolList, 9);
            Console.WriteLine("CalcBeginWinRate IterationCount = " + IterationCount);
        }



        public static void Test()
        {
            //CalcBeginWinRate();
            CardStatistics.CalcHandCardWinRate();
        }
    }
}
