using System;
using WHS.DEVICE.ChineseChess.Models;

namespace WHS.DEVICE.ChineseChess.Controller
{
    class Rook: ProCon
    {
        public bool Che(int X, int Y, int chozenX, int chozenY, Base[,] Matrix)
        {
            int col, row, temp;

            if (chozenX == X)
            {
                col = chozenY < Y ? chozenY : Y;//如果chozenY<Y为ture 返回chozenY 否则Y；
                row = chozenY > Y ? chozenY : Y;

                for (temp = col + 1; temp < row; temp++)
                {
                    if (Matrix[X, temp].side != Base.Player.blank)
                    {
                        return false;
                    }
                }
            }

            if (chozenY == Y)
            {
                col = chozenX < X ? chozenX : X;
                row = chozenX > X ? chozenX : X;

                for (temp = col + 1; temp < row; temp++)
                {
                    if (Matrix[temp, Y].side != Base.Player.blank)
                    {
                        return false;
                    }
                }
            }

            if (chozenX != X && chozenY != Y)
            {
                return false;
            }

            if (Matrix[chozenX, chozenY].side == Matrix[X, Y].side)
            {
                return false;
            }

            getMove(X, Y, chozenX, chozenY, Matrix);

            return true;
        }
    }
}