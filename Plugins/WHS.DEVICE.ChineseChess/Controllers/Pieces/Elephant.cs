using System;
using WHS.DEVICE.ChineseChess.Models;

namespace WHS.DEVICE.ChineseChess.Controller
{
    class Elephant: ProCon
    {
        public bool Xiang(int X, int Y, int chozenX, int chozenY, Base[,] Matrix)
        {

            if (Matrix[chozenX, chozenY].side == Base.Player.black)            //使象不能过河
            {
                if (X > 4)                                      //4和5为河界的行坐标
                {
                    return false;
                }
            }
            else
            {
                if (X < 5)
                {
                    return false;
                }
            }

            if ((X - chozenX) * (X - chozenX) + (chozenY - Y) * (chozenY - Y) != 8)      //选择的行坐标与移动前的行坐标的差的1/2的绝对值乘选择的列坐标和移动前的列坐标，其积不等于8时，粗略画出田字的路径。筛选条件1
            {
                return false;
            }

            if (Matrix[(X + chozenX) / 2, (Y + chozenY) / 2].side != Base.Player.blank)        //使象走田字时，当要走的田中心有棋子时不能移动    
            {
                return false;
            }


            if (Matrix[chozenX, chozenY].side == Matrix[X, Y].side)                 //避免走到自己方的棋子上
            {
                return false;
            }

            getMove(X, Y, chozenX, chozenY, Matrix);

            return true;
        }
    }
}