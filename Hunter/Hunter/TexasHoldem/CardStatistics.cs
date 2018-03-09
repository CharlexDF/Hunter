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
    public class CardStatistics
    {
        public static List<List<Card>> sAllType = new List<List<Card>>();
        public static List<List<Card>> sHandCardList = new List<List<Card>>();
        public List<Card> mCardList = new List<Card>();

        
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

        public static void CombinePreList(List<Card> preList, int iNum, DeleCardList deleCardList = null)
        {
            List<Card> retList = new List<Card>();
            List<Card> poolList = CardList.GetAllCardList();

            for (int i = 0; i < 1; i++)
            {
                if (preList == null)
                {
                    break;
                }
                if (preList.Count == 0)
                {
                    break;
                }
                foreach (var card in preList)
                {
                    Debug.Assert(poolList.Contains(card));
                    poolList.Remove(card);
                    retList.Add(card);
                }
            }

            Combination(poolList, 2, retList, deleCardList);
        }

        public static void CombineAllCard(int iNum, DeleCardList deleCardList = null)
        {
            List<Card> allCardList = CardList.GetAllCardList();
            Combination(allCardList, 2, null, deleCardList);
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








        public static List<List<Card>> GetAllHandCardList()
        {
            List<Card> allCardList = CardList.GetAllCardList();
            List<List<Card>> allHandCardList = new List<List<Card>>();

            Combination(allCardList, 2, null, (_CardList) => 
            {
                allHandCardList.Add(_CardList);
            });

            Utility.Log("allHandCardList.Count = " + allHandCardList.Count);

            return null;
        }


        public static List<List<Card>> GetAllPreFlopList()
        {
            List<Card> allCardList = CardList.GetAllCardList();
            
            List<Card> colorList = new List<Card>();
            foreach (var card in allCardList)
            {
                if (card.iNumber == 1)
                {
                    colorList.Add(card);
                }
            }
            
            List<List<Card>> againstList = new List<List<Card>>();
            foreach (var card1 in colorList)
            {
                foreach (var card2 in colorList)
                {
                    foreach (var card3 in colorList)
                    {
                        foreach (var card4 in colorList)
                        {
                            List<Card> cardList = new List<Card>();
                            cardList.Add(card1);
                            cardList.Add(card2);
                            cardList.Add(card3);
                            cardList.Add(card4);
                            againstList.Add(cardList);
                        }
                    }
                }
            }
            Utility.Log("againstList.Count = " + againstList.Count);
            return againstList;
        }


        public static SortedList<int, List<Card>> AgainstList = new SortedList<int, List<Card>>();
        public static int GetHandCardAgainstId(List<Card> _CardList)
        {
            Debug.Assert(_CardList != null);
            Debug.Assert(_CardList.Count == 0);

            int iAgainstId = 0;
            int iAgainstType = CardList.GetAgainstType(_CardList);

            int iColorCount = CardList.GetColorCount(_CardList);
            foreach (var card in _CardList)
            {
                iAgainstId = iAgainstType * 100 + card.GetId();
            }
            return iAgainstId;
        }
        
        
        public static int GetSortedAgainstId(List<Card> _CardList)
        {
            int iAgainstType = CardList.GetAgainstType(_CardList);
            if (iAgainstType == AgainstType.SpadeSpade_VS_SpadeSpade)
            {

            }
            else if (iAgainstType == AgainstType.SpadeSpade_VS_SpadeHeart)
            {

            }
            else if (iAgainstType == AgainstType.SpadeSpade_VS_HeartHeart)
            {

            }
            else if (iAgainstType == AgainstType.SpadeHeart_VS_SpadeHeart)
            {

            }
            else if (iAgainstType == AgainstType.SpadeSpade_VS_HeartClub)
            {

            }
            else if (iAgainstType == AgainstType.SpadeHeart_VS_SpadeClub)
            {

            }
            else if (iAgainstType == AgainstType.SpadeHeart_VS_ClubDiamond)
            {

            }

            return 0;
        }

        public static void GetHandCardAgainstList()
        {
            List<Card> allCardList = CardList.GetAllCardList();
            List<List<Card>> againstList = new List<List<Card>>();

            Combination(allCardList, 4, null, (_CardList) =>
            {
                againstList.Add(_CardList);
            });

            Utility.Log("againstList.Count = " + againstList.Count);
        }

        public static void GetAllOppoCardList()
        {
            List<Card> allCardList = CardList.GetAllCardList();
            List<List<Card>> allHandCardList = GetAllHandCardList();
            List<List<Card>> allOppoCardList = new List<List<Card>>();

            foreach (var handCardList in allHandCardList)
            {
                List<Card> poolList = CardList.CopyCardList(allCardList);
                foreach (var card in handCardList)
                {
                    if (poolList.Contains(card) == false)
                    {
                        Utility.Log("poolList.Contains(card) == false");
                    }
                    poolList.Remove(card);
                }

                Combination(allCardList, 2, null, (_CardList) =>
                {
                    allHandCardList.Add(_CardList);
                });
            }
            //spade spade VS spade spade
            //spade spade VS heart heart //equal to (club club) (diamond diamond)
            //spade spade VS spade heart //equal to (spade club) (spade diamond)
            //spade spade vs heart club //equal to (heart diamond) (club diamond)
            //spade heart vs club diamond
        }

        




        public static SortedList<int, SortedList<int, AgainstResult>> AllAgainstResultList = new SortedList<int, SortedList<int, AgainstResult>>();
        public static int CalcAllHandCardAgainstResult()
        {
            List<List<Card>> allHandCardList = GetPruningHandCardList();
            int iCount = 0;
            foreach (var handCard in allHandCardList)
            {
                SortedList<int, AgainstResult> againstResultList = new SortedList<int, AgainstResult>();

                int iHandCardId = CardList.GetHandId(handCard);
                Debug.Assert(!AllAgainstResultList.ContainsKey(iHandCardId));
                AllAgainstResultList.Add(iHandCardId, againstResultList);

                CombinePreList(handCard, 2, (_CardList) =>
                {
                    Debug.Assert(_CardList != null);
                    Debug.Assert(_CardList.Count == 4);
                    List <Card> oppoCard = new List<Card>();
                    oppoCard.Add(_CardList[2]);
                    oppoCard.Add(_CardList[3]);
                    
                    int iOppoCardId = CardList.GetOppoId(handCard, oppoCard);
                    //check is exist in againstResultList
                    if (againstResultList.ContainsKey(iOppoCardId))
                    {
                        return;
                    }
                    AgainstResult againstResult = CalcAgainstResult(handCard, oppoCard);
                    againstResultList.Add(iOppoCardId, againstResult);
                    iCount++;
                });

            }

            Utility.Log("CalcAllHandCardAgainstResult iCount = " + iCount);

            return 0;
        }

        public static List<List<Card>> GetPruningHandCardList()
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

        public static int GetAgainstId(List<Card> _HandCardList, List<Card> _OppoCardList)
        {
            return 0;
        }

        public static AgainstResult CalcAgainstResult(List<Card> _HandCardList, List<Card> _OppoCardList)
        {
            AgainstResult againstResult = new AgainstResult();
            return againstResult;
        }

        public static void GetAgainstResult(List<Card> _CardList)
        {

        }







        public static void DrawCard()
        {
            
        }

    }
}
