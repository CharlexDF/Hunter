using System;
using System.Collections.Generic;
using System.Text;

namespace Hunter.TexasHoldem
{
    public class CardType
    {
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
    }

    public class MaxCardType
    {
        public int iCardType;
        public List<Card> cardList;
    }

    public class CardCompare
    {
        #region #Compare
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
            int iMaxSameNumCount = 0;
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

            if (iMaxSameNumCount == 4)
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

        #endregion #Compare
    }
}
