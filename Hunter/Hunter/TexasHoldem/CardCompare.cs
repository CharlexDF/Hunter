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

        //StraightFlush
        //FourKind
        //Flush
        //Straight
        //ThreeKind
        //HighCard
        public int iHighCard;

        public int iFullHouse_Three;
        public int iFullHouse_Two;

        public int iThreeKind_Three;
        public int iThreeKind_High1;
        public int iThreeKind_High2;

        public int iTwoPair_Pair1;
        public int iTwoPair_Pair2;
        public int iTwoPair_High;

        public int iOnePair_Pair;
        public int iOnePair_High;

        public CardTypeCaculator(List<Card> _CardList)
        {
            Debug.Assert(_CardList != null);

            iCardType = CardType.None;
            mCardList = _CardList;
            mMaxCardList = new List<Card>();
            arrNumCount = new int[14];
            arrColorCount = new int[5];
            iHighCard = 0;
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
            for (int i = 1; i < arrNumCount.Length; i++)
            {
                var iCount = arrNumCount[i];
                if (iCount == 4)
                {
                    iHighCard = i;
                    return true;
                }
            }
            return false;
        }

        public bool IsFullHouse()
        {
            //  iFullHouse_Three;
            //public int iFullHouse_Two;
            iFullHouse_Three = 0;
            iFullHouse_Two = 0;
            for (int i = 1; i < arrNumCount.Length; i++)
            {
                var iCount = arrNumCount[i];
                if (iCount == 3 && i > iFullHouse_Three)
                {
                    iFullHouse_Three = i;
                }
            }

            for (int i = 1; i < arrNumCount.Length; i++)
            {
                if (i == iFullHouse_Three)
                {
                    continue;
                }

                var iCount = arrNumCount[i];
                if (iCount >= 2 && i > iFullHouse_Two)
                {
                    iFullHouse_Two = i;
                }
            }

            if (iFullHouse_Three > 0 && iFullHouse_Two > 0)
            {
                return true;
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
            int iThree = 0;
            for (int i = 1; i < arrNumCount.Length; i++)
            {
                var iCount = arrNumCount[i];
                if (iCount == 3)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsTwoPairs()
        {
            int iBigPairs = 0;
            int iSmallPairs = 0;
            for (int i = arrNumCount.Length - 1; i > 0 ; i++)
            {
                var iCount = arrNumCount[i];
                if (iCount == 2)
                {
                    if (iBigPairs == 0)
                    {
                        iBigPairs = i;
                        continue;
                    }
                    else if (iSmallPairs == 0)
                    {
                        iSmallPairs = i;
                        continue;
                    }
                    return true;
                }
            }
            return false;
        }

        public bool IsOnePairs()
        {
            for (int i = 1; i < arrNumCount.Length; i++)
            {
                var iCount = arrNumCount[i];
                if (iCount == 2)
                {
                    return true;
                }
            }
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
