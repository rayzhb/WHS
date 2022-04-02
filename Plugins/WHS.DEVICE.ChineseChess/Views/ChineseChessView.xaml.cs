using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WHS.DEVICE.ChineseChess.Controller;
using WHS.DEVICE.ChineseChess.Models;

namespace WHS.DEVICE.ChineseChess.Views
{
    /// <summary>
    /// ChineseChessView.xaml 的交互逻辑
    /// </summary>
    public partial class ChineseChessView : Page
    {
        public int chozenX;                                                     //声明需要的全局变量，与实例化
        public int chozenY;
        public int currentX;
        public int currentY;
        public bool firstClicked = false;
        public int count = 0;
        public Base[,] currentMatrix;
        public Base[,] path;
        Piece proMod = new Piece();
        ProCon proCon = new ProCon();

        public ChineseChessView()
        {
            InitializeComponent();
            currentMatrix = proMod.setGround();                 //初始化当前的矩阵，即实际的棋盘按钮矩阵
            path = proMod.setRoad();                            //初始化棋子的路径
            makeGrid(currentMatrix);                            //打印棋盘与棋子
        }
        public void makeGrid(Base[,] currentMatrix)
        {
            Grid totalGrid = new Grid();                                    //创建背景的boardGrid
            this.Content = totalGrid;
            Grid boardGrid = new Grid();                                     //创建棋盘的boardGrid
            totalGrid.Children.Add(boardGrid);                                  //在totalGrid里添加boardGrid
            boardGrid.HorizontalAlignment = HorizontalAlignment.Left;           //使boardGrid打开在左方

            ImageBrush background = new ImageBrush();                                   //加载背景图
            background.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resource.background.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            totalGrid.Background = background;


            ImageBrush board = new ImageBrush();                                            //加载棋盘图
            board.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resource.gameboard.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            boardGrid.Background = board;

            ColumnDefinition[] col = new ColumnDefinition[9];                               //创建行和列
            for (int i = 0; i < 9; i++)
            {
                col[i] = new ColumnDefinition();
                col[i].Width = new GridLength(75);                                      //设置宽的长度
                boardGrid.ColumnDefinitions.Add(col[i]);
            }

            RowDefinition[] row = new RowDefinition[10];
            for (int i = 0; i < 10; i++)
            {
                row[i] = new RowDefinition();
                row[i].Height = new GridLength(75);                                     //设置高的长度
                boardGrid.RowDefinitions.Add(row[i]);
            }

            showWords(totalGrid);                                                       //打印文字
            makeLayout(currentMatrix, boardGrid);                                                //打印布局
        }


        public void makeLayout(Base[,] currentMatrix, Grid boardGrid)                        //布局，布置棋子
        {
            Button[,] btn = new Button[10, 9];
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resource.box.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            for (int col = 0; col < 10; col++)
            {
                for (int row = 0; row < 9; row++)
                {
                    btn[col, row] = new Button();                                   //设置棋子各种属性
                    btn[col, row].Width = 60;
                    btn[col, row].Height = 60;
                    btn[col, row].Margin = new Thickness(8, 10, 0, 0);
                    btn[col, row].BorderThickness = new Thickness(0, 0, 0, 0);
                    btn[col, row].Background = Brushes.Transparent;         //匹配parent（即前一个）的背景
                    btn[col, row] = loadPiecesPics(currentMatrix, btn[col, row], col, row);
                    btn[col, row].SetValue(Grid.RowProperty, col);
                    btn[col, row].SetValue(Grid.ColumnProperty, row);

                    if (path[col, row].path == Base.PiecePath.yes)
                    {
                        btn[col, row].Background = brush;   //画刷，即使之能画出路径
                    }

                    boardGrid.Children.Add(btn[col, row]);
                }
            }
            btnEvent(btn);         //赋予按钮事件      
        }


        public Button loadPiecesPics(Base[,] currentMatrix, Button btn, int col, int row)           //加载各个棋子的图片
        {
            switch (currentMatrix[col, row].side)
            {
                case Base.Player.red:
                    switch (currentMatrix[col, row].type)
                    {
                        case Base.PieceType.che:
                            ImageBrush Red_che = new ImageBrush();
                            Red_che.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resource.red_che.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            btn.Background = Red_che;
                            return btn;
                        case Base.PieceType.ma:
                            ImageBrush Red_ma = new ImageBrush();
                            Red_ma.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resource.red_ma.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            btn.Background = Red_ma;
                            return btn;
                        case Base.PieceType.xiang:
                            ImageBrush Red_xiang = new ImageBrush();
                            Red_xiang.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resource.red_xiang.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            btn.Background = Red_xiang;
                            return btn;
                        case Base.PieceType.shi:
                            ImageBrush Red_shi = new ImageBrush();
                            Red_shi.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resource.red_shi.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            btn.Background = Red_shi;
                            return btn;
                        case Base.PieceType.jiang:
                            ImageBrush Red_jiang = new ImageBrush();
                            Red_jiang.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resource.red_jiang.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            btn.Background = Red_jiang;
                            return btn;
                        case Base.PieceType.pao:
                            ImageBrush Red_pao = new ImageBrush();
                            Red_pao.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resource.red_pao.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            btn.Background = Red_pao;
                            return btn;
                        case Base.PieceType.bing:
                            ImageBrush Red_bing = new ImageBrush();
                            Red_bing.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resource.red_bing.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            btn.Background = Red_bing;
                            return btn;
                    }
                    return btn;

                case Base.Player.black:
                    switch (currentMatrix[col, row].type)
                    {
                        case Base.PieceType.che:
                            ImageBrush Black_che = new ImageBrush();
                            Black_che.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resource.black_che.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            btn.Background = Black_che;
                            return btn;
                        case Base.PieceType.ma:
                            ImageBrush Black_ma = new ImageBrush();
                            Black_ma.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resource.black_ma.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            btn.Background = Black_ma;
                            return btn;
                        case Base.PieceType.xiang:
                            ImageBrush Black_xiang = new ImageBrush();
                            Black_xiang.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resource.black_xiang.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            btn.Background = Black_xiang;
                            return btn;
                        case Base.PieceType.shi:
                            ImageBrush Black_shi = new ImageBrush();
                            Black_shi.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resource.black_shi.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            btn.Background = Black_shi;
                            return btn;
                        case Base.PieceType.jiang:
                            ImageBrush Black_jiang = new ImageBrush();
                            Black_jiang.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resource.black_jiang.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            btn.Background = Black_jiang;
                            return btn;
                        case Base.PieceType.pao:
                            ImageBrush Black_pao = new ImageBrush();
                            Black_pao.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resource.black_pao.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            btn.Background = Black_pao;
                            return btn;
                        case Base.PieceType.bing:
                            ImageBrush Black_bing = new ImageBrush();
                            Black_bing.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resource.black_bing.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            btn.Background = Black_bing;
                            return btn;
                    }
                    return btn;
            }
            return btn;
        }


        public void showWords(Grid totalGrid)                   //提示的文字
        {
            TextBlock showPlayer = new TextBlock();
            totalGrid.Children.Add(showPlayer);
            showPlayer.FontSize = 50;
            showPlayer.Margin = new Thickness(705, 60, 0, 0);

            TextBlock tips = new TextBlock();
            totalGrid.Children.Add(tips);
            tips.Text = "Round " + ((int)count / 2 + 1);
            tips.FontSize = 21;
            tips.Foreground = Brushes.DarkGray;
            tips.Margin = new Thickness(710, 20, 0, 0);

            TextBlock tipsPlayer = new TextBlock();
            totalGrid.Children.Add(tipsPlayer);
            tipsPlayer.Text = "请选中.";
            tipsPlayer.FontSize = 25;
            tipsPlayer.Margin = new Thickness(710, 150, 0, 0);

            if (count % 2 == 0)
            {
                showPlayer.Foreground = Brushes.Red;
                showPlayer.Text = "红方出手";
                tipsPlayer.Foreground = Brushes.Red;
            }
            else
            {
                showPlayer.Foreground = Brushes.Black;
                showPlayer.Text = "黑房出手";
                tipsPlayer.Foreground = Brushes.Black;
            }
        }


        public void btnClick(object sender, RoutedEventArgs e)              //创建点击事件
        {
            int btnRow = (int)((Button)sender).GetValue(Grid.RowProperty);
            int btnCol = (int)((Button)sender).GetValue(Grid.ColumnProperty);

            ckEvent(btnRow, btnCol);
        }


        public void btnEvent(Button[,] btn)              //给按钮添加点击事件
        {
            for (int col = 0; col < 10; col++)
                for (int row = 0; row < 9; row++)
                    btn[col, row].Click += btnClick;
        }


        public void ckEvent(int btnRow, int btnCol)               //点击发生时进行事件的内容
        {
            bool result;

            if (firstClicked == true)                                                            //选中想移动的棋子
            {
                currentX = btnRow;
                currentY = btnCol;

                if (currentX == chozenX && currentY == chozenY)                 //再次选中到自己
                {
                    msgShow(1, count);
                    makeGrid(currentMatrix);                             //更新boardGrid，即把选中棋子时已显示的路径去掉
                }
                else                                                        //成功选中
                {
                    bool move = proCon.movePiece(currentX, currentY, chozenX, chozenY, currentMatrix);     //检测该位置上的棋子的移动规则并给能走的位置标记

                    if (move == false) msgShow(2, count);                                       //如果选到已被标记不能走的位置
                    else count++;                                                     //选到能走的位置时，回合数加1，进行下一步

                    makeGrid(currentMatrix);                                //更新boardGrid,使棋子在显示上发生移动
                    result = proCon.checkResult(currentMatrix);                    //检查游戏是否结束

                    if (result == false)                                    //如果结束
                    {
                        //makeGrid(currentMatrix);                        
                        if (count % 2 == 1) msgShow(6, count);                     //红方赢
                        else msgShow(7, count);                                          //黑方赢
                        currentMatrix = proMod.setGround();                 //初始化当前的矩阵，即实际的棋盘按钮矩阵
                        path = proMod.setRoad();                            //初始化棋子的路径
                        makeGrid(currentMatrix);             //游戏结束自动退出程序
                    }
                }

                firstClicked = false;            //标记当前棋子已被选中  
            }

            else          //选中了棋子开始选择移动时
            {
                chozenX = btnRow;
                chozenY = btnCol;

                if (currentMatrix[chozenX, chozenY].side == Base.Player.blank)      //选中到了空位置
                    msgShow(3, count);
                else if (count % 2 == 0)                //红方时
                {
                    if (currentMatrix[chozenX, chozenY].side == Base.Player.black)              //选中到了黑方
                        msgShow(4, count);
                    else
                    {
                        path = proCon.showRoad(chozenX, chozenY, currentMatrix);           //显示当前棋子可行路径
                        firstClicked = true;                                    //初始化标记，使之回复到未选中棋子状态
                        makeGrid(currentMatrix);                              //更新boardGrid
                        path = proMod.setRoad();                           //初始化路径            
                    }
                }
                else if (count % 2 == 1)                        //黑方时
                {
                    if (currentMatrix[chozenX, chozenY].side == Base.Player.red)            //选中到了红方
                        msgShow(5, count);
                    else
                    {                                                               //作用同上
                        path = proCon.showRoad(chozenX, chozenY, currentMatrix);
                        firstClicked = true;
                        makeGrid(currentMatrix);
                        path = proMod.setRoad();
                    }
                }
            }
        }

        public void msgShow(int i, int count)               //一个MessageBox.Show的集合，事先写好，方便调用
        {
            switch (i)
            {
                case 1:
                    MessageBox.Show("Please try again!");
                    break;
                case 2:
                    MessageBox.Show("You cannot move to there!");
                    break;
                case 3:
                    MessageBox.Show("There is no piece!");
                    break;
                case 4:
                    MessageBox.Show("It is not your turn, it is red's turn now!");
                    break;
                case 5:
                    MessageBox.Show("It is not your turn, it is black's turn now!");
                    break;
                case 6:
                    MessageBox.Show("Red Win!!\n" + "You have used " + ((int)count / 2 + 1) + " rounds to be finished.");
                    break;
                case 7:
                    MessageBox.Show("Black Win!!\n" + "You have used " + ((int)count / 2 + 1) + " rounds to be finished.");
                    break;
            }
        }
    }
}
