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
        public static List<Card> AllCardList;
        public static List<Card> GetAllCardList()
        {
            if (AllCardList == null)
            {
                AllCardList = new List<Card>();
                for (int iColor = 1; iColor <= Color.Count; iColor++)
                {
                    for (int iNum = 1; iNum <= 13; iNum++)
                    {
                        Card card = new Card(iColor, iNum);
                        AllCardList.Add(card);
                    }
                }
            }
            return CopyCardList(AllCardList);
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

        public static List<Card> CombineList(List<Card> _CardListA, List<Card> _CardListB)
        {
            List<Card> retList = new List<Card>();
            foreach (var card in _CardListA)
            {
                retList.Add(card);
            }
            foreach (var card in _CardListB)
            {
                Debug.Assert(retList.Contains(card));
                retList.Add(card);
            }
            retList.Concat(_CardListA);
            return retList;
        }

        public static List<Card> RemoveList(List<Card> retList, List<Card> tarList)
        {
            foreach (var card in tarList)
            {
                if (retList.Contains(card))
                {
                    retList.Remove(card);
                }
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

        public static void PrintList(List<Card> _CardList)
        {
            Utility.Log("" + ToString(_CardList));
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

        public static void AppendCardList(List<Card> retList, List<Card> appendList)
        {
            foreach (var card in appendList)
            {
                Debug.Assert(!retList.Contains(card));
                retList.Add(card);
            }
        }

        public static List<Card> CombineCardList(List<Card> _CardListA, List<Card> _CardListB)
        {
            List<Card> retList = new List<Card>();
            foreach (var card in _CardListA)
            {
                Debug.Assert(!retList.Contains(card));
                retList.Add(card);
            }
            foreach (var card in _CardListB)
            {
                Debug.Assert(!retList.Contains(card));
                retList.Add(card);
            }
            return retList;
        }






        public static int GetColorCount(List<Card> _CardList)
        {
            List<int> colorList = new List<int>();
            foreach (var card in _CardList)
            {
                if (!colorList.Contains(card.iColor))
                {
                    colorList.Add(card.iColor);
                }
            }
            return colorList.Count;
        }

        public static int GetAgainstType(List<Card> _CardList)
        {
            Debug.Assert(_CardList != null);
            Debug.Assert(_CardList.Count == 4);

            int iAgainstType = AgainstType.None;
            int iColorCount = CardList.GetColorCount(_CardList);
            if (iColorCount == 1)
            {
                return AgainstType.SpadeSpade_VS_SpadeSpade;
            }
            else if (iColorCount == 2)
            {
                if (_CardList[0].iColor == _CardList[1].iColor && _CardList[2].iColor == _CardList[3].iColor)
                {
                    return AgainstType.SpadeSpade_VS_HeartHeart;
                }
                if (_CardList[0].iColor == _CardList[1].iColor || _CardList[2].iColor == _CardList[3].iColor)
                {
                    return AgainstType.SpadeSpade_VS_SpadeHeart;
                }
                return AgainstType.SpadeSpade_VS_SpadeHeart;
            }
            else if (iColorCount == 3)
            {
                if (_CardList[0].iColor == _CardList[1].iColor || _CardList[2].iColor == _CardList[3].iColor)
                {
                    return AgainstType.SpadeSpade_VS_HeartClub;
                }
                return AgainstType.SpadeHeart_VS_SpadeClub;
            }
            else if (iColorCount == 4)
            {
                return AgainstType.SpadeHeart_VS_ClubDiamond;
            }

            return iAgainstType;
        }

        public static int GetHandId(List<Card> _HandCardList)
        {
            Debug.Assert(_HandCardList != null);
            Debug.Assert(_HandCardList.Count == 2);
            Card handCard1 = _HandCardList[0];
            Card handCard2 = _HandCardList[1];
            int iHandId = 0;
            if (handCard1.iColor == handCard2.iColor)
            {
                Card tranCard1 = new Card(Color.Spade, handCard1.iNumber);
                Card tranCard2 = new Card(Color.Spade, handCard2.iNumber);
                iHandId = tranCard1.GetId() * 100 + tranCard2.GetId();
            }
            else
            {
                Card tranCard1 = new Card(Color.Spade, handCard1.iNumber);
                Card tranCard2 = new Card(Color.Heart, handCard2.iNumber);
                iHandId = tranCard1.GetId() * 100 + tranCard2.GetId();
            }
            return iHandId;
        }

        public static int GetOppoId(List<Card> _CardList)
        {
            Debug.Assert(_CardList != null);
            Debug.Assert(_CardList.Count == 4);

            List<Card> handCard = new List<Card>();
            handCard.Add(_CardList[0]);
            handCard.Add(_CardList[1]);

            List<Card> oppoCard = new List<Card>();
            oppoCard.Add(_CardList[2]);
            oppoCard.Add(_CardList[3]);

            int iOppoCardId = CardList.GetOppoId(handCard, oppoCard);

            return iOppoCardId;
        }

        public static int GetOppoId(List<Card> _HandCardList, List<Card> _OppoCardList)
        {
            Debug.Assert(_HandCardList != null);
            Debug.Assert(_HandCardList.Count == 2);
            Debug.Assert(_OppoCardList != null);
            Debug.Assert(_OppoCardList.Count == 2);

            int iHandId = 0;
            Card handCard1 = _OppoCardList[0];
            Card handCard2 = _OppoCardList[1];
            Card tranCard1 = null;
            Card tranCard2 = null;

            List<Card> combineList = CombineCardList(_HandCardList, _OppoCardList);
            int iAgainstType = GetAgainstType(combineList);
            if (iAgainstType == AgainstType.SpadeSpade_VS_SpadeSpade)
            {
                tranCard1 = new Card(Color.Spade, handCard1.iNumber);
                tranCard2 = new Card(Color.Spade, handCard2.iNumber);
            }
            else if (iAgainstType == AgainstType.SpadeSpade_VS_SpadeHeart)
            {
                tranCard1 = new Card(Color.Spade, handCard1.iNumber);
                tranCard2 = new Card(Color.Heart, handCard2.iNumber);
            }
            else if (iAgainstType == AgainstType.SpadeSpade_VS_HeartHeart)
            {
                tranCard1 = new Card(Color.Heart, handCard1.iNumber);
                tranCard2 = new Card(Color.Heart, handCard2.iNumber);
            }
            else if (iAgainstType == AgainstType.SpadeHeart_VS_SpadeHeart)
            {
                tranCard1 = new Card(Color.Spade, handCard1.iNumber);
                tranCard2 = new Card(Color.Heart, handCard2.iNumber);
            }
            else if (iAgainstType == AgainstType.SpadeSpade_VS_HeartClub)
            {
                tranCard1 = new Card(Color.Heart, handCard1.iNumber);
                tranCard2 = new Card(Color.Club, handCard2.iNumber);
            }
            else if (iAgainstType == AgainstType.SpadeHeart_VS_SpadeClub)
            {
                tranCard1 = new Card(Color.Spade, handCard1.iNumber);
                tranCard2 = new Card(Color.Club, handCard2.iNumber);
            }
            else if (iAgainstType == AgainstType.SpadeHeart_VS_ClubDiamond)
            {
                tranCard1 = new Card(Color.Club, handCard1.iNumber);
                tranCard2 = new Card(Color.Diamond, handCard2.iNumber);
            }

            iHandId = tranCard1.GetId() * 100 + tranCard2.GetId();

            return iHandId;
        }

    }
}
