using System;
using WHS.DEVICE.ChineseChess.Models;

namespace WHS.DEVICE.ChineseChess.Controller
{
    class Horse: ProCon
    {
        public bool Ma(int X, int Y, int chozenX, int chozenY, Base[,] Matrix)
        {

            if (Math.Abs(chozenX - X) == 1 && Math.Abs(chozenY - Y) == 2)           //横着走日字
            {
                if (Matrix[chozenX, chozenY + (Y - chozenY) / Math.Abs(Y - chozenY)].side != Base.Player.blank)        //斩马腿
                {
                    return false;
                }
            }
            else if (Math.Abs(chozenX - X) == 2 && Math.Abs(chozenY - Y) == 1)      //竖着走日字
            {
                if (Matrix[chozenX + (X - chozenX) / Math.Abs(X - chozenX), chozenY].side != Base.Player.blank)        //斩马腿
                {
                    return false;
                }
            }
            else
            {                                                       //避免走出日字以外的路径
                return false;
            }

            if (Matrix[chozenX, chozenY].side == Matrix[X, Y].side)             //避免走到自己方的棋子上
            {
                return false;
            }

            getMove(X, Y, chozenX, chozenY, Matrix);

            return true;
        }
    }
}