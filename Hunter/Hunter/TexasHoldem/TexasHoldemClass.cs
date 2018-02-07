using System;
using System.Collections.Generic;
using System.Text;

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

    public class HandCard
    {
        public int id;
        public List<Card> mCardList = new List<Card>();
    }
}
