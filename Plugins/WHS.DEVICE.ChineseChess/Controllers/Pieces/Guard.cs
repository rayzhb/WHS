using System;
using WHS.DEVICE.ChineseChess.Models;

namespace WHS.DEVICE.ChineseChess.Controller
{
    class Guard: ProCon
    {
        public bool Shi(int X, int Y, int chozenX, int chozenY, Base[,] Matrix)
        {
            

            if (Matrix[chozenX, chozenY].side == Base.Player.black)
            {
                if (Y < 3 || Y > 5 || X > 2)
                {
                    return false;
                }
            }
            else
            {
                if (Y < 3 || Y > 5 || X < 7)
                {
                    return false;
                }
            }

            if (Math.Abs(X - chozenX) != 1 || Math.Abs(chozenY - Y) != 1)
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