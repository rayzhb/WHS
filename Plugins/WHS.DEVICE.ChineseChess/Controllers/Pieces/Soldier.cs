using System;
using WHS.DEVICE.ChineseChess.Models;

namespace WHS.DEVICE.ChineseChess.Controller
{
    class Soldier: ProCon
    {
        public bool Bing(int X, int Y, int chozenX, int chozenY, Base[,] Matrix)
        {

            if (X != chozenX && Y != chozenY)
            {
                return false;
            }

            if (Matrix[chozenX, chozenY].side == Base.Player.black)
            {
                if (chozenX < 5 && X - chozenX != 1)
                {
                    return false;
                }

                if (chozenX > 4)
                {
                    if (X == chozenX && Math.Abs(Y - chozenY) != 1)
                    {
                        return false;
                    }

                    if (Y == chozenY && X - chozenX != 1)
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (chozenX > 4 && chozenX - X != 1)
                {
                    return false;
                }

                if (chozenX < 5)
                {
                    if (X == chozenX && Math.Abs(Y - chozenY) != 1)
                    {
                        return false;
                    }

                    if (Y == chozenY && chozenX - X != 1)
                    {
                        return false;
                    }
                }
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