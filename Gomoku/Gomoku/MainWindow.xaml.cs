using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Gomoku.GameLogic;

namespace Gomoku
{
    public partial class MainWindow : Window
    {
        const int nRows = 10, nCols = 10;
        double baseHeight = 65.0, baseWidth = 65.0;
        GameLogic gameLogic;
        Computer computer;
        Button[,] buttonBoard;
        public MainWindow()
        {
            InitializeComponent();
            InitializeButton();
            Console.WriteLine("heelo");
        }

        void InitializeButton()
        {
            gameLogic = new GameLogic(nRows, nCols, 2);
            computer = new Computer(gameLogic, 1);
            buttonBoard = new Button[nRows, nCols];
            double baseHeight = fieldGrid.Height / nRows;
            double baseWidth = fieldGrid.Width / nRows;
            Console.WriteLine(baseHeight.ToString() + " "+baseWidth.ToString());
            GridLengthConverter gridLengthConverter = new GridLengthConverter();
            ThicknessConverter thicknessConverter = new ThicknessConverter();

            for (int i = 0; i < nRows; i++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = (GridLength)gridLengthConverter.ConvertFrom("*");
                fieldGrid.RowDefinitions.Add(rowDefinition);
            }

            for (int i = 0; i < nCols; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = (GridLength)gridLengthConverter.ConvertFrom("*");
                fieldGrid.ColumnDefinitions.Add(columnDefinition);
            }

            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    Button newButton = new Button();
                    newButton.Name = "Button" + i.ToString() + "_" + j.ToString();

                    newButton.Click += NextMove;
                    newButton.Style = this.FindResource("FieldButton") as Style;

                    Grid.SetRow(newButton, i);
                    Grid.SetColumn(newButton, j);

                    fieldGrid.Children.Add(newButton);
                    buttonBoard[i, j] = newButton;
                }
            }
        }
        public void NextMove(object sender, RoutedEventArgs e)
        {   
            // wtf...
            Button sendButton = (Button)sender;
            int r = Grid.GetRow(sendButton);
            int c = Grid.GetColumn(sendButton);
            Ellipse ellipse = new Ellipse();
            ellipse.Stroke = Brushes.Black;
            double width = baseWidth * (c+1);
            ellipse.Width = 100;
            double height = baseHeight * (r+1);
            ellipse.Height = 100;
            ellipse.Margin = new Thickness(10, 10, 0, 0);
            double left = width - baseWidth;
            double top = height - baseHeight;
            double zero = 0.0;
            //Console.WriteLine(left.);
            //ellipse.Margin = new Thickness(left, top, zero, zero);
            Console.WriteLine(ellipse.Width);
            Console.WriteLine(ellipse.Height);
            Console.WriteLine(ellipse.Margin);
            
            fieldGrid.Children.Add(ellipse);
            Console.WriteLine(ellipse);

            Console.WriteLine(fieldGrid.Children);
            Console.WriteLine(buttonBoard[r, c].Parent); 
            if (!gameLogic.IsEmpty(r, c))
                return;
            Console.Write(r.ToString());
            Console.Write(" ");
            Console.Write(c.ToString());
            Console.Write("\n");




            //< Ellipse HorizontalAlignment = "Left" Height = "70" Margin = "250,309,0,0" Stroke = "Black" VerticalAlignment = "Top" Width = "70" />
        }

        private void NewGame(object sender, RoutedEventArgs e)
        {

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
