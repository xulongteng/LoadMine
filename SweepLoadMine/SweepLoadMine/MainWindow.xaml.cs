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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;
using System.Windows.Threading;
using System.Threading;

namespace SweepLoadMine
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }              
        /// <summary>
        /// 地图图片数组
        /// </summary>
        private Border[,] backborder = new Border[10,10];
        DispatcherTimer timer = new DispatcherTimer();      
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LabCont.Foreground = new LinearGradientBrush(Colors.Orange, Colors.Purple, 0);
            CreatLoadMine();
            Suiji();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        int timerNum;
        private void Timer_Tick(object sender, EventArgs e)
        {
            timerNum += 1;
            TimeNum.Content = timerNum;
        }

        //开始游戏
        private void BegGame_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Bground.Visibility = Visibility.Hidden;
            GameMain.Visibility = Visibility.Visible;
        }
        //退出游戏
        private void SopGame_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Environment.Exit(0);

        }
        Random r = new Random();             
        LoadMind loadMind;
        
       
        /// <summary>
        /// 绘制带有地雷格子
        /// </summary>        
        private void CreatLoadMine()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    loadMind = new LoadMind();

                    loadMind.X = i;
                    loadMind.Y = j;

                    backborder[i, j] = new Border();
                    backborder[i, j].MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;
                    backborder[i, j].MouseRightButtonDown += MainWindow_MouseRightButtonDown;
                    backborder[i, j].Background = Brushes.White;
                    backborder[i, j].BorderThickness = new Thickness(1);
                    backborder[i, j].BorderBrush = Brushes.Black;

                    backborder[i, j].Tag = loadMind;
                    backborder[i, j].Width = 60;
                    backborder[i, j].Height = 50;
                    Canvas.SetLeft(backborder[i, j], i * backborder[i, j].Width);
                    Canvas.SetTop(backborder[i, j], j * backborder[i, j].Height);
                    Map.Children.Add(backborder[i, j]);

                    Label label = new Label();
                    label.Width = backborder[i, j].Width;
                    label.Height = backborder[i, j].Height;
                    label.Content = "";
                    backborder[i, j].Child = label;
                }
            }          
        }
        /// <summary>
        /// 随机雷的位置
        /// </summary>
        void Suiji()
        {
            for (int i = 0; i < 10; i++)
            {
                int col = r.Next(0, 10);
                int row = r.Next(0, 10);
                if (((LoadMind)backborder[col, row].Tag).Ismind == false)
                {
                    ((LoadMind)backborder[col, row].Tag).Ismind = true;
                }
                else
                {
                    i--;
                }

            }
        }


        //右键扫雷
        int flagNum =10;
        private void MainWindow_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border borders = (Border)sender;
         
            if (borders.Background==Brushes.White||(((Label)borders.Child).Content.ToString())=="")
            {
                if (((LoadMind)borders.Tag).Isflag == false)
                {
                    ImageBrush flag = new ImageBrush();                   
                     flag.ImageSource = new BitmapImage(new Uri(@"..\..\img\Map\flag.png", UriKind.Relative));
                    borders.Background = flag;
                    ((LoadMind)borders.Tag).Isflag = true;                   
                    if (flagNum > 0)
                    {
                        flagNum -= 1;
                        MindNum.Content = flagNum;
                    }
                    else
                    {
                        return;
                    }
                    MindNum.Content= flagNum;
                }
                else if (((LoadMind)borders.Tag).Isflag == true)
                {
                    borders.Background = Brushes.White;
                    ((LoadMind)borders.Tag).Isflag = false;
                    if (flagNum>=10)
                    {
                        flagNum = 10;
                    }
                    else
                    {
                        flagNum += 1;
                        MindNum.Content = flagNum;
                    }                   
                }            
            }
            else
            {
                return;
            }       
            //当所有的旗子都在地雷上面  游戏胜利
            if (((LoadMind)borders.Tag).Ismind==true&& Convert.ToInt32(MindNum.Content) == 0)
            {
                MessageBox.Show("恭喜你！游戏胜利！");
                Map.Background = Brushes.White;
                timer.Stop();
                return;
            }                                                                                                             
        }
        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border border = (Border)sender;
            if (((LoadMind)border.Tag).Isflag==true)
            {
                return;
            }
            JudgeMind(border);                     
        }
        private void JudgeMind(Border border)
        {           
            LoadMind lm = ((LoadMind)border.Tag);           
            //如果是1就是雷
            if (lm.Ismind == true)
            {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (((LoadMind)backborder[i, j].Tag).Ismind == true)
                        {
                            ImageBrush image = new ImageBrush();                           
                            image.ImageSource = new BitmapImage(new Uri(@"..\..\img\Map\mind.jpg", UriKind.Relative));
                            backborder[i, j].Background = image;
                        }
                    }
                }
                timer.Stop();
                MessageBox.Show("Game Over！很遗憾，再接再厉！");               
                Map.Children.Clear();
                Map.Background = Brushes.White;
                return;                                                                                                    
            }          
            else 
            {
                border.Background = Brushes.Gray;              
                
                if (lm.Y - 1 >= 0 && ((LoadMind)backborder[lm.X, lm.Y - 1].Tag).Ismind == true)
                {
                    //有雷 计数
                    //return;
                    lm.Count++;                 
                }
                //
                if (lm.X - 1 >= 0 && lm.Y - 1 >= 0 && ((LoadMind)backborder[lm.X - 1, lm.Y - 1].Tag).Ismind == true)
                {
                    //有雷 计数
                    //return;
                    lm.Count++;
                   
                }
                if (lm.X - 1 >= 0 && ((LoadMind)backborder[lm.X - 1, lm.Y].Tag).Ismind == true)
                {
                    //有雷 计数
                    //return;
                    lm.Count++;
                    
                }
                if (lm.X - 1 >= 0 && lm.Y + 1 < 10 && ((LoadMind)backborder[lm.X - 1, lm.Y + 1].Tag).Ismind == true)
                {
                    //有雷 计数
                    //return;
                    lm.Count++;
                    
                }
                if (lm.Y + 1 < 10 && ((LoadMind)backborder[lm.X, lm.Y + 1].Tag).Ismind == true)
                {
                    //有雷 计数
                    //return;
                    lm.Count++;
                    
                }
                if (lm.X + 1 < 10 && lm.Y + 1 < 10 && ((LoadMind)backborder[lm.X + 1, lm.Y + 1].Tag).Ismind == true)
                {
                    //有雷 计数
                    //return;
                    lm.Count++;
                   
                }
                if (lm.X + 1 < 10 && ((LoadMind)backborder[lm.X + 1, lm.Y].Tag).Ismind == true)
                {
                    //有雷 计数
                    //return;
                    lm.Count++;
                   
                }
                if (lm.X + 1 < 10 && lm.Y - 1 >= 0 && ((LoadMind)backborder[lm.X + 1, lm.Y - 1].Tag).Ismind == true)
                {
                    //有雷 计数
                    //return;
                    lm.Count++;
                    
                }
                //左边
                if (lm.Y>0&&((LoadMind)backborder[lm.X, lm.Y - 1].Tag).Ismind == false&& backborder[lm.X, lm.Y - 1].Background==Brushes.White)
                {
                   
                    if (lm.Count == 0)
                    {
                        JudgeMind(backborder[lm.X, lm.Y - 1]);
                    }
                }
                        
                //左上
                if (lm.X>0&&lm.Y>0&&((LoadMind)backborder[lm.X - 1, lm.Y - 1].Tag).Ismind == false && backborder[lm.X-1, lm.Y - 1].Background == Brushes.White)
                {
                    if (lm.Count == 0)
                    {
                        JudgeMind(backborder[lm.X - 1, lm.Y - 1]);
                    }
                   
                }
              
                //上
                if (lm.X>0&&((LoadMind)backborder[lm.X - 1, lm.Y].Tag).Ismind == false && backborder[lm.X-1, lm.Y ].Background == Brushes.White)
                {
                    if (lm.Count == 0)
                    {
                        JudgeMind(backborder[lm.X - 1, lm.Y]);
                    }
                    
                }
               
                //右上
                if (lm.X>0&&lm.Y<9&&((LoadMind)backborder[lm.X - 1, lm.Y + 1].Tag).Ismind == false && backborder[lm.X-1, lm.Y +1].Background == Brushes.White)
                {
                    if (lm.Count == 0)
                    {
                        JudgeMind(backborder[lm.X - 1, lm.Y + 1]);
                    }
                  
                }
               
                //右边
                if (lm.Y<9&&((LoadMind)backborder[lm.X, lm.Y + 1].Tag).Ismind == false && backborder[lm.X, lm.Y + 1].Background == Brushes.White)
                {
                    if (lm.Count == 0)
                    {
                        JudgeMind(backborder[lm.X, lm.Y + 1]);
                    }
                   
                }
               
                //右下
                if (lm.X<9&&lm.Y<9&&((LoadMind)backborder[lm.X + 1, lm.Y + 1].Tag).Ismind == false && backborder[lm.X+1, lm.Y + 1].Background == Brushes.White)
                {
                    if (lm.Count == 0)
                    {
                        JudgeMind(backborder[lm.X + 1, lm.Y + 1]);
                    }
                    
                }
               
                //下
                if (lm.X<9&&((LoadMind)backborder[lm.X + 1, lm.Y].Tag).Ismind == false && backborder[lm.X+1, lm.Y].Background == Brushes.White)
                {
                    if (lm.Count==0)
                    {
                        JudgeMind(backborder[lm.X + 1, lm.Y]);
                    }
                   
                }
               
                //左边下
                if (lm.X<9&&lm.Y>0&&((LoadMind)backborder[lm.X + 1, lm.Y - 1].Tag).Ismind == false && backborder[lm.X+1, lm.Y - 1].Background == Brushes.White)
                {
                    if (lm.Count==0)
                    {
                        JudgeMind(backborder[lm.X + 1, lm.Y - 1]);
                    }                    
                }                            
            }
            //总共的雷数量显示
            if (lm.Count>0)
            {            
                if (lm.Iskey==false)
                {
                    Label lb= (Label)border.Child;
                    lb.Content = lm.Count;
                    lb.HorizontalContentAlignment = HorizontalAlignment.Center;
                    lb.VerticalContentAlignment = VerticalAlignment.Center;
                    lb.FontSize = 28;
                    if (lb.Content.ToString()=="1")
                    {
                        lb.Foreground = Brushes.Red;
                    }
                    else if (lb.Content.ToString() == "2")
                    {
                        lb.Foreground = Brushes.Blue;
                    }
                    else if (lb.Content.ToString() == "3")
                    {
                        lb.Foreground = Brushes.Green;
                    }
                    else
                    {
                        lb.Foreground = Brushes.Purple;
                    }
                    lm.Iskey = true;
                }
                else if(lm.Iskey==true)
                {
                    return;
                }               
            }          
        }    
        private void Again_Click(object sender, RoutedEventArgs e)
        {
            Map.Children.Clear();
            TimeNum.Content = 0;        
            flagNum = 10;
            MindNum.Content = 10;
            timerNum = 0;
            timer.Start();                  
            CreatLoadMine();
            Suiji();
        }
        /// <summary>
        /// 简单模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Simple_Click(object sender, RoutedEventArgs e)
        {
            Map.Children.Clear();
            CreatLoadMine();
            for (int i = 0; i < 5; i++)
            {
                int col = r.Next(0, 10);
                int row = r.Next(0, 10);
                if (((LoadMind)backborder[col, row].Tag).Ismind == false)
                {
                    ((LoadMind)backborder[col, row].Tag).Ismind = true;
                }
                else
                {
                    i--;
                }

            }
                  
            flagNum = 5;
            MindNum.Content = 5;
            timerNum = 0;
            TimeNum.Content = 0;
            timer.Start();
        }
        /// <summary>
        /// 一般模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Common_Click(object sender, RoutedEventArgs e)
        {
            Map.Children.Clear();
            CreatLoadMine();
            Suiji();           
            flagNum = 10;
            MindNum.Content = 10;
            timerNum = 0;
            TimeNum.Content = 0;
            timer.Start();
        }
        /// <summary>
        /// 困难模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Difficulty_Click(object sender, RoutedEventArgs e)
        {
            Map.Children.Clear();
            CreatLoadMine();
            for (int i = 0; i < 20; i++)
            {
                int col = r.Next(0, 10);
                int row = r.Next(0, 10);
                if (((LoadMind)backborder[col, row].Tag).Ismind == false)
                {
                    ((LoadMind)backborder[col, row].Tag).Ismind = true;
                }
                else
                {
                    i--;
                }

            }           
            flagNum = 20;
            MindNum.Content = 20;
            timerNum = 0;
            TimeNum.Content = 0;
            timer.Start();
        }
    }
}
