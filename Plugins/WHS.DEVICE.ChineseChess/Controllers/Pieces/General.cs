using System;
using WHS.DEVICE.ChineseChess.Models;

namespace WHS.DEVICE.ChineseChess.Controller
{
    class General: ProCon
    {
        public bool Jiang(int X, int Y, int chozenX, int chozenY, Base[,] Matrix)
        {
            int col, row, temp;

            if (Matrix[X, Y].type == Base.PieceType.jiang && chozenY == Y)         //飞将
            {
                col = chozenX < X ? chozenX : X;
                row = chozenX > X ? chozenX : X;

                for (temp = col + 1; temp < row; temp++)                             //遍历当前列，当双方将军直接有其他棋子时不能飞将
                {
                    if (Matrix[temp, Y].side != Base.Player.blank)
                    {
                        return false;
                    }
                }

                if (Matrix[chozenX, chozenY].side == Matrix[X, Y].side)         //避免走到自己方的棋子上（其实这在飞将时有点多余）
                {
                    return false;
                }

                getMove(X, Y, chozenX, chozenY, Matrix);

                return true;        //先一个setmove，为了能飞将时能移动，避免被下面的条件限制
            }

            if (Matrix[chozenX, chozenY].side == Base.Player.black)        //限制黑方将军在九宫格内移动
            {
                if (Y < 3 || Y > 5 || X > 2)
                {
                    return false;
                }
            }
            else
            {                                                       //限制红方将军在九宫格内移动
                if (Y < 3 || Y > 5 || X < 7)
                {
                    return false;
                }
            }

            if ((chozenX - X) * (chozenX - X) + (chozenY - Y) * (chozenY - Y) != 1)
            {
                return false;                           //水平移动时，移动距离仅为1格 ｜｜ 竖直移动时，移动距离为1格 ｜｜ 避免第一次移动将时可移动距离大于一个格
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