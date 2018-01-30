using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Hunter.Tools;

namespace Hunter
{
    public class Result
    {
        public int id;
        public Card card1;
        public Card card2;
        public int iWinRate;
    }
    
    public class HandRateStatistics
    {
        public List<HandRateData> DataList;
        public static void Statistics()
        {

        }
        public static void PrintHandWinRate()
        {
        }
    }

    public class HandRateData
    {
        public List<Card> CardList;//nine cards
    }

    class Player
    {
        public List<Card> handCards = new List<Card>();
    }

    public class TexasHoldem
    {
        public static SortedList<int, Card> AllCardList = new SortedList<int, Card>();
        public static SortedList<int, Result> ResultList = new SortedList<int, Result>();
        public static string sResult = "";
        
        public static SortedList<int, Card> GetAllCardList()
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
                    card.bUsed = false;
                    AllCardList.Add(card.id, card);
                }
            }
            //PrintAllCard();
            return AllCardList;
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

        public static void CalcBeginWinRate()
        {
            GetAllCardList();
            List<Card> poolList = new List<Card>();
            foreach (var item in AllCardList)
            {
                poolList.Add(item.Value);
            }
            PermutationCombination(poolList, null, 9);
        }
        
        public static void PermutationCombination(List<Card> poolList, List<Card> retList, int iNum)
        {
            Debug.Assert(iNum > 0);

            if (iNum == 1)
            {
                Debug.Assert(retList != null);

                foreach (var item in poolList)
                {
                    retList.Add(item);
                    GetHandResult(retList);
                    retList.Remove(item);
                }
                return;
            }

            if (retList == null)
            {
                retList = new List<Card>();
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

                PermutationCombination(next_poolList, retList, iNum - 1);

                retList.Remove(item);
                next_poolList.Add(item);
            }
        }

        public static void GetHandResult(List<Card> retList)
        {
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

            /*
            foreach (var item in retList)
            {
                Console.Write(" " + item.Print());
            }
            Console.Write("\n\n");
            */
        }

        public static int REqual = 0;
        public static int RWin = 1;
        public static int RLose = -1;
        public static int CompareCards(List<Card> handCards, List<Card> opponentCards)
        {
            return REqual;
        }

        /*
        public static int Count = 100;
        public static int StraightFlush = Count--;
        public static int FourKind = Count--;
        public static int FullHouse = Count--;
        public static int Flush = Count--;
        public static int Straight = Count--;
        public static int ThreeKind = Count--;
        public static int TwoPairs = Count--;
        public static int OnePairs = Count--;
        public static int HighCard = Count--;
        */
        public static bool IsStraight(List<Card> cardList)
        {
            return false;
        }
        public static bool IsFlush(List<Card> cardList)
        {
            return false;
        }
        public static bool IsStraightFlush(List<Card> cardList)
        {
            return false;
        }
        public static bool IsFourKind(List<Card> cardList)
        {
            int iMaxSameNumCount = GetMaxSameNumCount(cardList);
            return iMaxSameNumCount == 4;
        }
        public static bool IsThreeKind(List<Card> cardList)
        {
            return false;
        }
        public static bool IsFullHouse(List<Card> cardList)
        {
            return false;
        }
        public static bool IsTwoPairs(List<Card> cardList)
        {
            return false;
        }
        public static bool IsOnePairs(List<Card> cardList)
        {
            return false;
        }
        public static int GetMaxSameCardType(List<Card> cardList)
        {
            int iFour = 0;
            int iThree = 0;
            int iTwo = 0;
            int iPairsCount = 0;

            int iMaxSameNumCount = 0;
            int[] arrCount = new int[14];

            foreach (var item in cardList)
            {
                arrCount[item.iNumber] += 1;
            }
            for (int i = 1; i < arrCount.Length; i++)
            {
                var iCount = arrCount[i];
                if (iCount == 4)
                {
                    iFour = i;
                    break;
                }
                if (iCount == 3 && i > iThree)
                {
                    iThree = i;
                }
                else if (iCount == 2)
                {
                    iPairsCount++;
                    if (i > iTwo)
                    {
                        iTwo = i;
                    }
                }
            }

            foreach (var item in arrCount)
            {
                if (item == 4)
                {
                    return CardType.FourKind;
                }
            }

            if (iMaxSameNumCount == 4)
            {
                return CardType.FourKind;
            }

            return CardType.HighCard;
        }

        public static int GetCardType(List<Card> cardList)
        {
            int iMaxSameNumCount = 0;
            int[] arrCount = new int[14];

            foreach (var item in cardList)
            {
                arrCount[item.iNumber] += 1;
            }

            foreach (var item in arrCount)
            {
                if (item > iMaxSameNumCount)
                {
                    iMaxSameNumCount = item;
                }
            }

            if (iMaxSameNumCount==4)
            {
                return CardType.FourKind;
            }

            return CardType.HighCard;
        }
        public static List<Card> GetMaxCards(List<Card> srcCards)
        {
            List<Card> retCards = new List<Card>();

            return retCards;
        }

        public static List<List<T>> Combination<T>(List<T> srcList, int iNum)
        {
            int iCount = srcList.Count;
            List<List<T>> ret = new List<List<T>>();

            Debug.Assert(srcList.Count > 0);
            Debug.Assert(srcList.Count >= iNum);

            if (iNum == 1)
            {
                for (int i = 0; i < srcList.Count; i++)
                {
                    List<T> pro = new List<T>();
                    pro.Add(srcList[i]);
                    ret.Add(pro);
                }
            }
            else if (iNum > 1)
            {
                for (int i = 0; i < srcList.Count; i++)
                {
                    List<T> subList = new List<T>();
                    foreach (var item in srcList)
                    {
                        subList.Add(item);
                    }

                    T t = subList[i];
                    subList.RemoveAt(i);

                    List<List<T>> subRet = Combination(subList, iNum - 1);
                    foreach (var item in subRet)
                    {
                        item.Add(t);
                        ret.Add(item);
                    }
                }
            }

            return ret;
        }

        public static void Test()
        {
            CalcBeginWinRate();
        }
    }
}
