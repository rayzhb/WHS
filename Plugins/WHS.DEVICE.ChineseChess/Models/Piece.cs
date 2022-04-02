using System;
using System.Collections.Generic;
using System.Text;

namespace WHS.DEVICE.ChineseChess.Models
{
    public class Piece
    {
        static void Main(string[] args)
        {
        }


        public Base[,] setGround()   //设置棋子位置
        {
            Base[,] Matrix = new Base[10, 9];
            for (int col = 0; col < 10; col++)
            {
                for (int row = 0; row < 9; row++)
                {
                    Matrix[col, row] = new Base
                    {
                        side = Base.Player.blank,
                        type = Base.PieceType.blank
                    };
                }
            }
            for (int row = 0; row < 9; row++)
            {
                Matrix[0, row].side = Base.Player.black;
                Matrix[9, row].side = Base.Player.red;
                if (row == 1 || row == 7)
                {
                    Matrix[2, row].side = Base.Player.black;
                    Matrix[7, row].side = Base.Player.red;
                }
                else if (row % 2 == 0)
                {
                    Matrix[3, row].side = Base.Player.black;
                    Matrix[6, row].side = Base.Player.red;
                }
            }
            for (int col = 0; col < 10; col++)
            {
                if (col == 0 || col == 9)
                {
                    Matrix[col, 0].type = Base.PieceType.che;
                    Matrix[col, 1].type = Base.PieceType.ma;
                    Matrix[col, 2].type = Base.PieceType.xiang;
                    Matrix[col, 3].type = Base.PieceType.shi;
                    Matrix[col, 4].type = Base.PieceType.jiang;
                    Matrix[col, 5].type = Base.PieceType.shi;
                    Matrix[col, 6].type = Base.PieceType.xiang;
                    Matrix[col, 7].type = Base.PieceType.ma;
                    Matrix[col, 8].type = Base.PieceType.che;
                }
                else if (col == 2 || col == 7)
                {
                    Matrix[col, 1].type = Base.PieceType.pao;
                    Matrix[col, 7].type = Base.PieceType.pao;
                }
                else if (col == 3 || col == 6)
                {
                    for (int row = 0; row < 9; row++)
                    {
                        if (row % 2 == 0)
                        {
                            Matrix[col, row].type = Base.PieceType.bing;
                        }
                    }
                }
            }
            return Matrix;
        }

        public Base[,] setRoad()          //初始化并设置棋子路径
        {
            Base[,] road = new Base[10, 9];

            for (int col = 0; col < 10; col++)
            {
                for (int row = 0; row < 9; row++)
                {
                    road[col, row] = new Base
                    {
                        path = Base.PiecePath.not
                    };
                }
            }

            return road;
        }
    }
}
