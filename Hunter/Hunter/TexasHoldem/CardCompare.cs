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

        public static int CompareCardList(List<Card> _CardListA, List<Card> _CardListB)
        {
            CardTypeCaculator cardTypeA = new CardTypeCaculator(_CardListA);
            CardTypeCaculator cardTypeB = new CardTypeCaculator(_CardListB);
            cardTypeA.iCardType = cardTypeA.CalcCardType();
            cardTypeB.iCardType = cardTypeB.CalcCardType();

            int iRet = 0;
            if (cardTypeA.iCardType > cardTypeB.iCardType)
            {
                iRet = 1;
            }
            else if (cardTypeA.iCardType < cardTypeB.iCardType)
            {
                iRet = 2;
            }
            else if(cardTypeA.iCardType == cardTypeB.iCardType)
            {
                if (cardTypeA.iCardType == CardType.StraightFlush)
                {
                    iRet = CompareHighCard(cardTypeA.iHighCard, cardTypeB.iHighCard);
                }
                else if (cardTypeA.iCardType == CardType.FourKind)
                {
                    return CompareHighCard(cardTypeA.iHighCard, cardTypeB.iHighCard);
                }
                else if (cardTypeA.iCardType == CardType.FullHouse)
                {
                    if (cardTypeA.iFullHouse_Three != cardTypeB.iFullHouse_Three)
                    {
                        iRet = CompareHighCard(cardTypeA.iFullHouse_Three, cardTypeB.iFullHouse_Three);
                    }
                    else
                    {
                        iRet = CompareHighCard(cardTypeA.iFullHouse_Two, cardTypeB.iFullHouse_Two);
                    }
                }
                else if (cardTypeA.iCardType == CardType.Flush)
                {
                    iRet = CompareHighCard(cardTypeA.iHighCard, cardTypeB.iHighCard);
                }
                else if (cardTypeA.iCardType == CardType.Straight)
                {
                    iRet = CompareHighCard(cardTypeA.iHighCard, cardTypeB.iHighCard);
                }
                else if (cardTypeA.iCardType == CardType.ThreeKind)
                {
                    if (cardTypeA.iThreeKind_Three != cardTypeB.iThreeKind_Three)
                    {
                        iRet = CompareHighCard(cardTypeA.iThreeKind_Three, cardTypeB.iThreeKind_Three);
                    }
                    else
                    {
                        if (cardTypeA.iThreeKind_High1 != cardTypeB.iThreeKind_High1)
                        {
                            iRet = CompareHighCard(cardTypeA.iThreeKind_High1, cardTypeB.iThreeKind_High1);
                        }
                        else
                        {
                            iRet = CompareHighCard(cardTypeA.iThreeKind_High2, cardTypeB.iThreeKind_High2);
                        }
                    }
                }
                else if (cardTypeA.iCardType == CardType.TwoPairs)
                {
                    if (cardTypeA.iTwoPair_Pair1 != cardTypeB.iTwoPair_Pair1)
                    {
                        iRet = CompareHighCard(cardTypeA.iTwoPair_Pair1, cardTypeB.iTwoPair_Pair1);
                    }
                    else
                    {
                        if (cardTypeA.iTwoPair_Pair2 != cardTypeB.iTwoPair_Pair2)
                        {
                            iRet = CompareHighCard(cardTypeA.iTwoPair_Pair2, cardTypeB.iTwoPair_Pair2);
                        }
                        else
                        {
                            iRet = CompareHighCard(cardTypeA.iTwoPair_High, cardTypeB.iTwoPair_High);
                        }
                    }
                }
                else if (cardTypeA.iCardType == CardType.OnePairs)
                {
                    if (cardTypeA.iOnePair_Pair != cardTypeB.iOnePair_Pair)
                    {
                        iRet = CompareHighCard(cardTypeA.iOnePair_Pair, cardTypeB.iOnePair_Pair);
                    }
                    else
                    {
                        iRet = CompareHighCard(cardTypeA.iOnePair_High, cardTypeB.iOnePair_High);
                    }
                }
                else if (cardTypeA.iCardType == CardType.HighCard)
                {
                    iRet = CompareHighCard(cardTypeA.iHighCard, cardTypeB.iHighCard);
                }
            }

            return iRet;
        }

        public static int CompareHighCard(int iNum1, int iNum2)
        {
            if (iNum1 > iNum2)
            {
                return 1;
            }
            else if (iNum1 < iNum2)
            {
                return 2;
            }
            else if (iNum1 == iNum2)
            {
                return 3;
            }
            return 0;
        }
        
        public int CalcCardType()//PreProcess
        {
            foreach (var item in mCardList)
            {
                arrNumCount[item.iNumber] += 1;
                arrColorCount[item.iColor] += 1;
            }

            iCardType = CardType.HighCard;

            if (IsStraightFlush())
            {
                iCardType = CardType.StraightFlush;
            }
            if (IsFourKind())
            {
                iCardType = CardType.FourKind;
            }
            if (IsFullHouse())
            {
                iCardType = CardType.FullHouse;
            }
            if (IsFlush())
            {
                iCardType = CardType.Flush;
            }
            if (IsStraight())
            {
                iCardType = CardType.Straight;
            }
            if (IsThreeKind())
            {
                iCardType = CardType.ThreeKind;
            }
            if (IsTwoPairs())
            {
                iCardType = CardType.TwoPairs;
            }
            if (IsOnePairs())
            {
                iCardType = CardType.OnePairs;
            }

            return iCardType;
        }

        public List<Card> GetMaxCards(List<Card> srcCards)
        {
            List<Card> retCards = new List<Card>();

            return retCards;
        }

        #region #is type
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
        #endregion #is type
    }

    public partial class CardCompare
    {
        public static int RWin = 1;
        public static int RLose = 1;
        public static int REqual = 2;
        public static int CompareCards(List<Card> handCards, List<Card> opponentCards)
        {
            return REqual;
        }
        
    }
}
