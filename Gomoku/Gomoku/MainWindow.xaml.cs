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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Gomoku.GameLogic;
using static System.Net.WebRequestMethods;

namespace Gomoku
{
    public partial class MainWindow : Window
    {
        int compLV = 1;
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
            computer = new Computer(gameLogic, compLV);
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

        private void DrawEllipse(int r, int c)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Stroke = Brushes.Red;
            ellipse.StrokeThickness = 5;
            ellipse.Width = baseWidth - 5;
            ellipse.Height = baseHeight - 5;
            Grid.SetRow(ellipse, r);
            Grid.SetColumn(ellipse, c);
            fieldGrid.Children.Add(ellipse);
        }

        private void DrawCross(int r, int c)
        {
            Line line = new Line();
            line.Stroke = Brushes.Blue;
            line.StrokeThickness = 5;
            line.X1 = 0;
            line.Y1 = 0;
            line.X2 = baseWidth;
            line.Y2 = baseHeight;
            Grid.SetRow(line, r);
            Grid.SetColumn(line, c);
            Line line2 = new Line();
            line2.Stroke = Brushes.Blue;
            line2.StrokeThickness = 5;
            line2.X1 = 0;
            line2.Y1 = baseHeight;
            line2.X2 = baseWidth;
            line2.Y2 = 0;
            Grid.SetRow(line2, r);
            Grid.SetColumn(line2, c);
            fieldGrid.Children.Add(line);
            fieldGrid.Children.Add(line2);
        }

        void DrawEndMessage(int result = 0)
        {
            blurGrid.IsEnabled = false;
            if (result == 0)
            {
                EndMessage.Text = "Tie!";
                EndMessage.Foreground = Brushes.White;
            }
            else if (result == 1)
            {
                EndMessage.Text = "You win!";
                EndMessage.Foreground = Brushes.Lime;
            }
            else if (result == 2) {
                EndMessage.Text = "You lose!";
                EndMessage.Foreground = Brushes.Red;
            }
            
            Storyboard sb = (Storyboard)TryFindResource("Storyboard_TextBlock");
            sb.Begin();
            blurGrid.IsEnabled = true;
        }

        public void NextMove(object sender, RoutedEventArgs e)
        {
            Button sendButton = (Button)sender;
            int r = Grid.GetRow(sendButton);
            int c = Grid.GetColumn(sendButton);
            
            if (!gameLogic.IsEmpty(r, c))
                return;

            Console.Write(r.ToString());
            Console.Write(" ");
            Console.Write(c.ToString());
            Console.Write("\n");

            gameLogic.NextMove(r, c);
            DrawCross(r, c);
            if (gameLogic.CheckWin(r, c))
            {
                DrawEndMessage(1);
                fieldGrid.IsEnabled = false;
                return;
            } else if (gameLogic.isEnd())
            {
                DrawEndMessage(0);
                fieldGrid.IsEnabled = false;
                return;
            }

            Move move = computer.NextMove();
            gameLogic.NextMove(move.row, move.column);
            DrawEllipse(move.row, move.column);
            if (gameLogic.CheckWin(move.row, move.column))
            {
                DrawEndMessage(2);
                fieldGrid.IsEnabled = false;
                return;
            } else if (gameLogic.isEnd())
            {
                DrawEndMessage(0);
                fieldGrid.IsEnabled = false;
                return;
            }

        }

        private void NewGame(object sender, RoutedEventArgs e)
        {
            var ellipses = fieldGrid.Children.OfType<Ellipse>().ToList();
            foreach (var ellipse in ellipses)
            {
                fieldGrid.Children.Remove(ellipse);
            }
            var lines = fieldGrid.Children.OfType<Line>().ToList();
            foreach (var line in lines)
            {
                fieldGrid.Children.Remove(line);
            }

            gameLogic = new GameLogic(nRows, nCols, 2);
            computer = new Computer(gameLogic, compLV);
            fieldGrid.IsEnabled = true;

        }

        private void RadioButton1_Checked(object sender, RoutedEventArgs e)
        {
            compLV = 1;
            computer.ChangeLV(1);
        }

        private void RadioButton2_Checked(object sender, RoutedEventArgs e)
        {
            compLV = 2;
            computer.ChangeLV(2);
        }

        private void RadioButton3_Checked(object sender, RoutedEventArgs e)
        {
            compLV = 3;
            computer.ChangeLV(3);
        }

    }
}
