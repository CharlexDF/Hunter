using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunter
{
    public delegate void Dele_Void_Fun();
    class Sudoku
    {
        public int iState;
        public int KnownCount;
        public int UnKnownCount;
        public static int[,] m_known;
        public Dictionary<int, Dictionary<int, List<int>>> dic_possible;

        public Sudoku()
        {
            iState = 0;
            KnownCount = 0;
            UnKnownCount = 9 * 9 * 9;
            m_known = null;
            dic_possible = new Dictionary<int, Dictionary<int, List<int>>>();
        }


        public void SetPuzzle()
        {
            int[,] m_puzzle0 =
            {
                { 0,0,0, 0,0,0, 0,0,0 },
                { 0,0,0, 0,0,0, 0,0,0 },
                { 0,0,0, 0,0,0, 0,0,0 },

                { 0,0,0, 0,0,0, 0,0,0 },
                { 0,0,0, 0,0,0, 0,0,0 },
                { 0,0,0, 0,0,0, 0,0,0 },

                { 0,0,0, 0,0,0, 0,0,0 },
                { 0,0,0, 0,0,0, 0,0,0 },
                { 0,0,0, 0,0,0, 0,0,0 }
            };
            int[,] m_puzzle1 =
            {
                { 0,0,7, 5,0,8, 0,0,0 },
                { 3,0,0, 0,4,0, 6,0,0 },
                { 0,5,0, 3,9,0, 0,0,0 },

                { 2,8,0, 0,0,0, 5,4,0 },
                { 0,6,0, 7,0,5, 0,8,0 },
                { 0,3,5, 0,0,0, 0,1,6 },

                { 0,0,0, 0,7,4, 0,3,0 },
                { 0,0,3, 0,1,0, 0,0,4 },
                { 0,0,0, 6,0,3, 1,0,0 }
            };
            int[,] m_puzzle2 =
            {
                { 0,9,6, 0,7,0, 8,0,3 },
                { 3,8,2, 4,9,6, 1,5,7 },
                { 0,0,1, 0,3,0, 9,0,0 },

                { 9,2,4, 0,5,0, 3,0,0 },
                { 0,0,5, 0,6,3, 4,9,0 },
                { 0,0,3, 0,4,0, 2,7,5 },

                { 2,3,9, 6,1,0, 7,0,0 },
                { 0,5,7, 3,8,0, 6,2,9 },
                { 0,0,8, 0,2,0, 5,3,0 }
            };
            m_known = m_puzzle1;

            int i;
            int j;
            int k;
            for (i = 0; i < 9; ++i)
            {
                Dictionary<int, List<int>> line = new Dictionary<int, List<int>>();
                for (j = 0; j < 9; ++j)
                {
                    List<int> ls_num = new List<int>();
                    if (m_known[i, j] != 0)
                    {
                        ls_num.Add(m_known[i, j]);
                        KnownCount++;
                    }
                    else
                    {
                        for (k = 0; k < 9; ++k)
                        {
                            ls_num.Add(k + 1);
                        }
                    }
                    line.Add(j, ls_num);
                }
                dic_possible.Add(i, line);
            }

            Console.WriteLine("SetPuzzle KnownCount = " + KnownCount);
        }
        public void Solve()
        {
            process();
            CountNum();
            print();
        }

        public void print()
        {
            Console.WriteLine("Print Matrix Count = " + KnownCount);

            int i;
            int j;
            for (i = 0; i < 9; ++i)
            {
                if (i % 3 == 0)
                {
                    Console.WriteLine("");
                }
                for (j = 0; j < 9; ++j)
                {
                    if (j % 3 == 0)
                    {
                        Console.Write(" ");
                    }
                    if (dic_possible[i][j].Count == 1)
                    {
                        Console.Write(" " + dic_possible[i][j][0]);
                    }
                    else if (dic_possible[i][j].Count == 0)
                    {
                        Console.Write(" -");
                    }
                    else
                    {
                        Console.Write(" 0");
                    }
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");
        }

        public int CountNum()
        {
            int i;
            int j;
            KnownCount = 0;
            UnKnownCount = 0;
            for (i = 0; i < 9; ++i)
            {
                for (j = 0; j < 9; ++j)
                {
                    if (dic_possible[i][j].Count == 1)
                    {
                        KnownCount++;
                    }
                    else if (dic_possible[i][j].Count == 0)
                    {
                        iState = -1;
                        //return KnownCount;
                    }
                    else
                    {
                        UnKnownCount += dic_possible[i][j].Count;
                    }
                }
            }
            return KnownCount;
        }
        public int process()
        {
            int ukcount = 9 * 9 * 9;
            CountNum();
            //Console.WriteLine("0 UnKnownCount = " + UnKnownCount);
            while (UnKnownCount < ukcount)
            {
                ukcount = UnKnownCount;
                processKnown();
                processCountOne();
                //processCountMutli();
                CountNum();
                if (iState == -1)
                {
                    break;
                }
                if (UnKnownCount == 0)
                {
                    break;
                }
            }
            //Console.WriteLine("1 UnKnownCount = " + UnKnownCount);
            if (iState == -1)
            {
                return KnownCount;
            }
            if (UnKnownCount > 0)
            {
                processEnum();
            }
            return KnownCount;
        }
        public void processKnown()
        {
            int i;
            int j;
            int row;
            int col;
            int square_row_start;
            int square_row_end;
            int square_col_start;
            int square_col_end;
            for (i = 0; i < 9; ++i)
            {
                for (j = 0; j < 9; ++j)
                {
                    if (dic_possible[i][j].Count == 1)
                    {
                        row = i;
                        for (col = 0; col < 9; ++col)
                        {
                            if (col == j) continue;
                            if (dic_possible[row][col].Contains(dic_possible[i][j][0]))
                            {
                                dic_possible[row][col].Remove(dic_possible[i][j][0]);
                            }
                        }

                        col = j;
                        for (row = 0; row < 9; ++row)
                        {
                            if (row == i) continue;
                            if (dic_possible[row][col].Contains(dic_possible[i][j][0]))
                            {
                                dic_possible[row][col].Remove(dic_possible[i][j][0]);
                            }
                        }

                        square_row_start = (i / 3) * 3;
                        square_row_end = square_row_start + 3;
                        square_col_start = (j / 3) * 3;
                        square_col_end = square_col_start + 3;
                        for (row = square_row_start; row < square_row_end; ++row)
                        {
                            for (col = square_col_start; col < square_col_end; ++col)
                            {
                                if (row == i && col == j) continue;
                                if (dic_possible[row][col].Contains(dic_possible[i][j][0]))
                                {
                                    dic_possible[row][col].Remove(dic_possible[i][j][0]);
                                }
                            }
                        }
                    }
                }
            }
        }//function end
        public void processCountOne()
        {
            int i;
            int j;
            int row;
            int col;
            int square_row_start;
            int square_row_end;
            int square_col_start;
            int square_col_end;
            int count = 0;
            int n = 0;
            for (int num = 1; num <= 9; ++num)
            {
                for (i = 0; i < 9; ++i)
                {
                    row = i;
                    col = -1;
                    count = 0;
                    for (j = 0; j < 9; ++j)
                    {
                        if (dic_possible[i][j].Count == 1)
                        {
                            if (dic_possible[i][j][0] == num)
                            {
                                count = 0;
                                break;
                            }
                            continue;
                        }
                        if (dic_possible[i][j].Contains(num))
                        {
                            count++;
                            row = i;
                            col = j;
                        }
                    }
                    if (count == 1)
                    {
                        for (n = dic_possible[row][col].Count - 1; n >= 0; n--)
                        {
                            if (dic_possible[row][col][n] != num)
                            {
                                dic_possible[row][col].Remove(dic_possible[row][col][n]);
                            }
                        }
                    }
                }

                for (j = 0; j < 9; ++j)
                {
                    row = -1;
                    col = j;
                    count = 0;
                    for (i = 0; i < 9; ++i)
                    {
                        if (dic_possible[i][j].Count == 1)
                        {
                            if (dic_possible[i][j][0] == num)
                            {
                                count = 0;
                                break;
                            }
                            continue;
                        }
                        if (dic_possible[i][j].Contains(num))
                        {
                            count++;
                            row = i;
                            col = j;
                        }
                    }
                    if (count == 1)
                    {
                        for (n = dic_possible[row][col].Count - 1; n >= 0; n--)
                        {
                            if (dic_possible[row][col][n] != num)
                            {
                                dic_possible[row][col].Remove(dic_possible[row][col][n]);
                            }
                        }
                    }
                }

                for (square_row_start = 0; square_row_start < 9; square_row_start += 3)
                {
                    square_row_end = square_row_start + 3;
                    for (square_col_start = 0; square_col_start < 9; square_col_start += 3)
                    {
                        square_col_end = square_col_start + 3;
                        row = -1;
                        col = -1;
                        count = 0;
                        for (i = square_row_start; i < square_row_end; ++i)
                        {
                            for (j = square_col_start; j < square_col_end; ++j)
                            {
                                if (dic_possible[i][j].Count == 1)
                                {
                                    if (dic_possible[i][j][0] == num)
                                    {
                                        i = square_row_end;
                                        count = 0;
                                        break;
                                    }
                                    continue;
                                }
                                if (dic_possible[i][j].Contains(num))
                                {
                                    count++;
                                    row = i;
                                    col = j;
                                }
                            }
                        }
                        if (count == 1)
                        {
                            for (n = dic_possible[row][col].Count - 1; n >= 0; n--)
                            {
                                if (dic_possible[row][col][n] != num)
                                {
                                    dic_possible[row][col].Remove(dic_possible[row][col][n]);
                                }
                            }
                        }
                    }
                }


            }
        }//function end
        public void processCountMutli()
        {
            int i;
            int j;
            int row;
            int col;
            int square_row_start;
            int square_row_end;
            int square_col_start;
            int square_col_end;
            int count = 0;
            //int index = 0;
            List<int> knownList = new List<int>();
            List<int> xList = new List<int>();
            List<int> yList = new List<int>();

            for (square_row_start = 0; square_row_start < 9; square_row_start += 3)
            {
                square_row_end = square_row_start + 3;
                for (square_col_start = 0; square_col_start < 9; square_col_start += 3)
                {
                    square_col_end = square_col_start + 3;

                    for (i = square_row_start; i < square_row_end; ++i)
                    {
                        for (j = square_col_start; j < square_col_end; ++j)
                        {
                            if (dic_possible[i][j].Count == 1)
                            {
                                knownList.Add(dic_possible[i][j][0]);
                            }
                        }
                    }

                    for (int num = 1; num <= 9; ++num)
                    {
                        if (knownList.Contains(num)) continue;
                        count = 0;
                        xList.Clear();
                        yList.Clear();
                        for (i = square_row_start; i < square_row_end; ++i)
                        {
                            for (j = square_col_start; j < square_col_end; ++j)
                            {
                                if (dic_possible[i][j].Count == 0) continue;
                                if (dic_possible[i][j].Count == 1) continue;
                                if (dic_possible[i][j].Contains(num))
                                {
                                    count++;
                                    if (!xList.Contains(i))
                                    {
                                        xList.Add(i);
                                    }
                                    if (!yList.Contains(j))
                                    {
                                        yList.Add(j);
                                    }
                                }
                            }
                        }
                        if (xList.Count == 1)
                        {
                            //clear row
                            row = xList[0];
                            for (col = 0; col < 9; col++)
                            {
                                if (dic_possible[row][col].Count == 1) continue;
                                if (col >= square_col_start && col < square_col_end) continue;
                                if (dic_possible[row][col].Contains(num))
                                {
                                    dic_possible[row][col].Remove(num);
                                }
                            }
                        }
                        if (yList.Count == 1)
                        {
                            //clear col
                            col = yList[0];
                            for (row = 0; row < 9; row++)
                            {
                                if (dic_possible[row][col].Count == 1) continue;
                                if (row >= square_row_start && row < square_row_end) continue;
                                if (dic_possible[row][col].Contains(num))
                                {
                                    dic_possible[row][col].Remove(num);
                                }
                            }
                        }
                    }//num
                }//square_col_start
            }//square_row_start

        }//function end
        public void processEnum()
        {
            //int index;
            int i;
            int j;
            int k;
            int n;
            bool bLoop_i = true;
            for (i = 0; i < 9; ++i)
            {
                for (j = 0; j < 9; ++j)
                {
                    if (dic_possible[i][j].Count > 1)
                    {
                        for (k = 0; k < dic_possible[i][j].Count; k++)
                        {
                            Sudoku sub = new Sudoku();
                            sub.CopyDicPossible(dic_possible);
                            for (n = sub.dic_possible[i][j].Count - 1; n >= 0; n--)
                            {
                                if (sub.dic_possible[i][j][n] != dic_possible[i][j][k])
                                {
                                    sub.dic_possible[i][j].Remove(sub.dic_possible[i][j][n]);
                                }
                            }
                            sub.CountNum();
                            if (sub.KnownCount > KnownCount)
                            {
                            }
                            sub.process();
                            sub.CountNum();
                            if (sub.iState == -1)
                            {
                                continue;
                            }
                            if (sub.UnKnownCount == 0)
                            {
                                CopyDicPossible(sub.dic_possible);
                                break;
                            }
                        }
                        iState = -1;
                        bLoop_i = false;
                        break;
                    }
                }
                if (!bLoop_i)
                {
                    break;
                }
            }
        }//processEnum

        public void CopyDicPossible(Dictionary<int, Dictionary<int, List<int>>> target)
        {
            int i;
            int j;
            int k;
            dic_possible.Clear();
            for (i = 0; i < 9; ++i)
            {
                Dictionary<int, List<int>> line = new Dictionary<int, List<int>>();
                for (j = 0; j < 9; ++j)
                {
                    List<int> ls_num = new List<int>();
                    if (target[i][j].Count == 0)
                    {
                        line.Add(j, ls_num);
                        continue;
                    }
                    for (k = 0; k < target[i][j].Count; ++k)
                    {
                        ls_num.Add(target[i][j][k]);
                    }
                    line.Add(j, ls_num);
                }
                dic_possible.Add(i, line);
            }
        }

        public static void Test()
        {
            Sudoku sudu = new Sudoku();
            sudu.SetPuzzle();
            sudu.Solve();
        }

    }//class end
}
