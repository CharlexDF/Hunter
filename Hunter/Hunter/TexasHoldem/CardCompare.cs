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
    public class MaxCardType
    {
        public int iCardType;
        public List<Card> cardList;
    }


    public class CardTypeCaculator
    {
        public int iCardType;
        public List<Card> mCardList;
        public List<Card> mMaxCardList;
        public int[] arrNumCount;
        public int[] arrColorCount;
        public int iHighCard;
        public int iPairA;
        public int iPairB;

        public CardTypeCaculator(List<Card> _CardList)
        {
            Debug.Assert(_CardList != null);

            iCardType = CardType.None;
            mCardList = _CardList;
            mMaxCardList = null;
            arrNumCount = new int[14];
            arrColorCount = new int[5];
            iHighCard = 0;
        }

        public void UpdateCardType(int _iCardType)
        {
            if (_iCardType > iCardType)
            {
                iCardType = _iCardType;
            }
            iHighCard = 0;
        }
        public void UpdateCardType(int _iCardType, int _iHighCard)
        {
            if (_iCardType > iCardType)
            {
                iCardType = _iCardType;
            }
            iHighCard = _iHighCard;
        }

        public void ProcessSameNumber()
        {
            int iThree = 0;
            int iTwo = 0;
            int iPairsCount = 0;

            int iMaxSameNumCount = 0;
            arrNumCount = new int[14];

            foreach (var item in mCardList)
            {
                arrNumCount[item.iNumber] += 1;
            }

            for (int i = 1; i < arrNumCount.Length; i++)
            {
                var iCount = arrNumCount[i];
                if (iCount == 4)
                {
                    UpdateCardType(CardType.FourKind, i);
                    return;
                }
                if (iCount == 3 && i > iThree)
                {
                    iThree = i;
                    UpdateCardType(CardType.FourKind, i);
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

            foreach (var item in arrNumCount)
            {
                if (item == 4)
                {
                    
                }
            }

            if (iMaxSameNumCount == 4)
            {
            }

            UpdateCardType(CardType.HighCard);
        }
        public int CalcCardType()//PreProcess
        {
            foreach (var item in mCardList)
            {
                arrNumCount[item.iNumber] += 1;
                arrColorCount[item.iColor] += 1;
            }
            
            if (IsStraightFlush())
            {
                return CardType.StraightFlush;
            }
            if (IsFourKind())
            {
                return CardType.FourKind;
            }
            if (IsFullHouse())
            {
                return CardType.FullHouse;
            }
            if (IsFlush())
            {
                return CardType.Flush;
            }
            if (IsStraight())
            {
                return CardType.Straight;
            }
            if (IsThreeKind())
            {
                return CardType.ThreeKind;
            }
            if (IsTwoPairs())
            {
                return CardType.TwoPairs;
            }
            if (IsOnePairs())
            {
                return CardType.OnePairs;
            }

            return CardType.HighCard;
        }
        
        public int CalcCardType(List<Card> cardList)
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

        public List<Card> GetMaxCards(List<Card> srcCards)
        {
            List<Card> retCards = new List<Card>();

            return retCards;
        }

        public bool IsStraightFlush()
        {
            if (IsStraight() && IsFlush())
            {
                return true;
            }
            return false;
        }

        public bool IsFourKind()
        {
            foreach (var count in arrNumCount)
            {
                if (count == 4)
                {
                    UpdateCardType(CardType.FourKind);
                    return true;
                }
            }
            return false;
        }

        public bool IsFullHouse()
        {
            int iThree = 0;
            int iTwo = 0;
            for (int i = 1; i < arrNumCount.Length; i++)
            {
                var iCount = arrNumCount[i];
                if (iCount == 3 && i > iThree)
                {
                    iThree = i;
                }
                else if (iCount == 2 && i > iTwo)
                {
                    iTwo = i;
                }
                if (iThree > 0 && iTwo > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsFlush()
        {
            foreach (var count in arrColorCount)
            {
                if (count == 5)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsStraight()
        {
            int iCombo = 0;
            for (int i = 1; i < arrNumCount.Length; i++)
            {
                var iCount = arrNumCount[i];
                if (iCount == 0)
                {
                    iCombo = 0;
                    continue;
                }
                iCombo++;
                if (iCombo == 5)
                {
                    return true;
                }
            }
            return false;
        }
        
        public bool IsThreeKind()
        {
            foreach (var count in arrNumCount)
            {
                if (count == 2)
                {
                    UpdateCardType(CardType.FourKind);
                    return true;
                }
            }
            return false;
        }

        public bool IsTwoPairs()
        {
            return false;
        }

        public bool IsOnePairs()
        {
            return false;
        }

    }

    public partial class CardCompare
    {
        public static int REqual = 0;
        public static int RWin = 1;
        public static int RLose = -1;
        public static int CompareCards(List<Card> handCards, List<Card> opponentCards)
        {
            return REqual;
        }
        
    }
}
