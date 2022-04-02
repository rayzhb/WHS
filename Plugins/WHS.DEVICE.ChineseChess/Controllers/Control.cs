using System;
using WHS.DEVICE.ChineseChess.Models;

namespace WHS.DEVICE.ChineseChess.Controller
{
    public class ProCon
    {
        static void Main(string[] args)
        {
        }


        public bool checkResult(Base[,] Matrix)       //判断结果
        {
            int count = 0;
            bool result = true;

            for (int col = 0; col < 10; col++)
            {
                for (int row = 0; row < 9; row++)
                {
                    if (Matrix[col, row].type == Base.PieceType.jiang)
                    {
                        count++;
                    }
                }
            }

            if (count == 2)
            {
                return result;
            }
            else
            {
                result = false;
                return result;
            }
        }


        public bool movePiece(int X, int Y, int chozenX, int chozenY, Base[,] Matrix)       //定义每种棋子的移动方式
        {
            Rook che = new Rook();
            Horse ma = new Horse();
            Elephant xiang = new Elephant();
            Guard shi = new Guard();
            General jiang = new General();
            Cannon pao = new Cannon();
            Soldier bing = new Soldier();
            bool re;

            switch (Matrix[chozenX, chozenY].type)
            {
                case Base.PieceType.che:
                    re = che.Che(X, Y, chozenX, chozenY, Matrix);
                    return re;
                case Base.PieceType.ma:
                    re = ma.Ma(X, Y, chozenX, chozenY, Matrix);
                    return re;
                case Base.PieceType.xiang:
                    re = xiang.Xiang(X, Y, chozenX, chozenY, Matrix);
                    return re;
                case Base.PieceType.shi:
                    re = shi.Shi(X, Y, chozenX, chozenY, Matrix);
                    return re;
                case Base.PieceType.jiang:
                    re = jiang.Jiang(X, Y, chozenX, chozenY, Matrix);
                    return re;
                case Base.PieceType.pao:
                    re = pao.Pao(X, Y, chozenX, chozenY, Matrix);
                    return re;
                case Base.PieceType.bing:
                    re = bing.Bing(X, Y, chozenX, chozenY, Matrix);
                    return re;
            }

            return false;
        }


        public Base[,] showRoad(int chozenX, int chozenY, Base[,] Matrix)      // 使棋子显示可行路径时，并使移动前的棋子不移动
        {
            Piece mod = new Piece();                  //实例化，方便使用该类里的方法
            Base[,] road = mod.setRoad();              //用Chess类创建出一个[19，17]的road数组，并一个个实例，再初始化路径，即将Chess.Piecepath.not赋到每个road里
            Base[,] trans = new Base[10, 9];         //用Chess类创建出一个[19，17]的trans数组，方便临时储存信息
            bool cr;

            for (int col = 0; col < 10; col++)                //遍历整个棋盘
            {
                for (int row = 0; row < 9; row++)
                {
                    trans[col, row] = new Base();             //通过循环，一个个地具体实例化每个trans
                }
            }

            for (int col = 0; col < 10; col++)                //遍历整个棋盘
            {
                for (int row = 0; row < 9; row++)
                {
                        trans[col, row].side = Matrix[col, row].side;           //把每个位置的side属性一个个赋到trans上暂时储存
                        trans[col, row].type = Matrix[col, row].type;              //把每个位置的type属性一个个赋到trans上暂时储存
                        trans[chozenX, chozenY].side = Matrix[chozenX, chozenY].side;      //把当前选择的具体位置的side属性赋到trans暂时储存
                        trans[chozenX, chozenY].type = Matrix[chozenX, chozenY].type;       //把当前选择的具体位置的type属性赋到trans暂时储存
                        cr = movePiece(col, row, chozenX, chozenY, Matrix);                 //使用该方法依据棋子类型，通过遍历一个个检查当前选择的棋子能走的格子，通过返回true或false来形成路径

                        if (cr == true)     //如果该格子能走
                        {
                            road[col, row].path = Base.PiecePath.yes;      //把该格子的位置的path属性里赋上Chess.Piecepath.yes,即把之前初始化的not变成了yes
                        }

                        Matrix[col, row].side = trans[col, row].side;           //每遍历了一个位置后及时把该位置的属性再赋回去，避免选择该棋子准备进行移动时棋子就已经提前移动影响整个函数的判断
                        Matrix[col, row].type = trans[col, row].type;
                        Matrix[chozenX, chozenY].side = trans[chozenX, chozenY].side;
                        Matrix[chozenX, chozenY].type = trans[chozenX, chozenY].type;
                    
                }
            }

            return road;            //返回road即可行路径
        }


        public void getMove(int X, int Y, int chozenX, int chozenY, Base[,] Matrix)       //基本移动方式
        {
            Matrix[X, Y].side = Matrix[chozenX, chozenY].side;
            Matrix[X, Y].type = Matrix[chozenX, chozenY].type;
            Matrix[chozenX, chozenY].side = Base.Player.blank;
            Matrix[chozenX, chozenY].type = Base.PieceType.blank;
        }
    }
}
