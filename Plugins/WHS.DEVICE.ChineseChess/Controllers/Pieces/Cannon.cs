using System;
using WHS.DEVICE.ChineseChess.Models;

namespace WHS.DEVICE.ChineseChess.Controller
{
    class Cannon: ProCon
    {
        public bool Pao(int X, int Y, int chozenX, int chozenY, Base[,] Matrix)
        {
            int col, row, temp, count;

            if (chozenX == X)
            {
                col = chozenY < Y ? chozenY : Y;
                row = chozenY > Y ? chozenY : Y;
                count = 0;

                for (temp = col + 1; temp < row; temp++)
                {
                    if (Matrix[X, temp].side != Base.Player.blank)
                    {
                        count++;
                    }
                }

                if (count > 1)
                {
                    return false;
                }
            }
            else if (chozenY == Y)
            {
                col = chozenX < X ? chozenX : X;
                row = chozenX > X ? chozenX : X;
                count = 0;

                for (temp = col + 1; temp < row; temp++)
                {
                    if (Matrix[temp, Y].side != Base.Player.blank)
                    {
                        count++;
                    }
                }

                if (count > 1)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            if (count == 0 && Matrix[X, Y].side != Base.Player.blank)
            {
                return false;
            }

            if (count == 1 && Matrix[X, Y].side == Base.Player.blank)
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
