﻿using System;
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
    public class Color
    {
        public const int Spade = 1;
        public const int Heart = 2;
        public const int Club = 3;
        public const int Diamond = 4;
        public const int Count = 4;
    }
    
    public class Card
    {
        public static int iTotalCount = 52;
        public int id;
        public int iColor;
        public int iNumber;
        public Card(int _iColor, int _iNumber)
        {
            iColor = _iColor;
            iNumber = _iNumber;
            id = GetId();
        }
        public int GetId()
        {
            return (iColor - 1) * 13 + iNumber;
        }
        public static int CountId(int iColor, int iNumber)
        {
            return (iColor - 1) * 13 + iNumber;
        }
        public static string ToString(Card inCard)
        {
            string strCard = inCard.Print();
            return strCard;
        }
        public string Print()
        {
            string sColor = "";
            string sNumber = "";
            string ret = "";
            switch (iColor)
            {
                case Color.Spade:
                    sColor = "S";
                    break;
                case Color.Heart:
                    sColor = "H";
                    break;
                case Color.Club:
                    sColor = "C";
                    break;
                case Color.Diamond:
                    sColor = "D";
                    break;
                default:
                    break;
            }
            switch (iNumber)
            {
                case 1:
                    sNumber = "A";
                    break;
                case 10:
                    sNumber = "T";
                    break;
                case 11:
                    sNumber = "J";
                    break;
                case 12:
                    sNumber = "Q";
                    break;
                case 13:
                    sNumber = "K";
                    break;
                default:
                    sNumber = Convert.ToString(iNumber);
                    break;
            }
            ret = sColor + sNumber;
            return ret;
        }
    }

    public class AgainstResult
    {
        public int id;
        public List<Card> mCardList;
        public float fWinRate;
        public float fDrawRate;
        public float fLoseRate;

    }

    public class HandCard
    {
        public const int SameColor = 1;
        public const int DiffColor = 2;
        public int id;
        public List<Card> mCardList;
        //public Card mCard1;
        //public Card mCard2;
        public HandCard(List<Card> _CardList)
        {
            mCardList = _CardList;
        }
        public HandCard(Card _card1, Card _card2)
        {
            //mCard1 = _card1;
            //mCard1 = _card1;
            //mCardList = new List<Card>();
            //mCardList.Add(mCard1);
            //mCardList.Add(mCard2);
        }
    }

    public class AgainstType
    {
        public static int Count = 0;
        public static int None = 0;
        //1 color
        public static int SpadeSpade_VS_SpadeSpade = Count++;

        //2 color
        public static int SpadeSpade_VS_SpadeHeart = Count++;
        public static int SpadeSpade_VS_HeartHeart = Count++;
        public static int SpadeHeart_VS_SpadeSpade = Count++;
        public static int SpadeHeart_VS_SpadeHeart = Count++;
        public static int SpadeHeart_VS_HeartHeart = Count++;

        //3 color
        public static int SpadeSpade_VS_HeartClub = Count++;
        public static int SpadeHeart_VS_SpadeClub = Count++;

        //4 color
        public static int SpadeHeart_VS_ClubDiamond = Count++;

    }

    public class CardType
    {
        public static int Count = 100;
        public static int StraightFlush = Count--;//同花顺
        public static int FourKind = Count--;//四条
        public static int FullHouse = Count--;//葫芦
        public static int Flush = Count--;//同花
        public static int Straight = Count--;//顺子
        public static int ThreeKind = Count--;//三条
        public static int TwoPairs = Count--;//两对
        public static int OnePairs = Count--;//对子
        public static int HighCard = Count--;//高牌
        public static int None = Count--;

        public static int GetCardType(List<Card> _CardList)
        {
            CardTypeCaculator cardType = new CardTypeCaculator(_CardList);
            return 0;
        }
    }
}
