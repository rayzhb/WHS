using System;

namespace WHS.DEVICE.ChineseChess.Models
{
    public class Base
    {
        public enum Player    //分为红黑两方
        {
            red,
            black,
            blank        //中间方
        };


        public enum PieceType    //分为不同的棋子类型
        {
            blank,        //中间方的，即棋子以外的空格，方便棋子去移动
            jiang,
            che,
            ma,
            pao,
            xiang,
            bing,
            shi
        };


        public enum PiecePath     //能否行走
        {
            yes,
            not
        };


        public Player side;
        public PieceType type;
        public PiecePath path;
    }
}
