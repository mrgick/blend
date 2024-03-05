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
        private static Random random = new Random();
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
            ellipse.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#E36687"));
            ellipse.StrokeThickness = 7;
            ellipse.Width = baseWidth - 5;
            ellipse.Height = baseHeight - 5;
            Grid.SetRow(ellipse, r);
            Grid.SetColumn(ellipse, c);
            fieldGrid.Children.Add(ellipse);
            int rand = random.Next(1, 5);
            if (rand == 1)
            {
                DoubleAnimation anim = new DoubleAnimation();
                if (random.Next(0,2) == 0)
                {
                    anim.From = random.Next(0, Convert.ToInt32(baseWidth) - 10);
                    anim.To = baseWidth - 5;
                } else
                {
                    anim.From = random.Next(Convert.ToInt32(baseWidth), Convert.ToInt32(baseWidth)+10);
                    anim.To = baseWidth - 5;
                }
                anim.Duration = TimeSpan.FromSeconds(0.3);
                ellipse.BeginAnimation(Ellipse.WidthProperty, anim);
                ellipse.BeginAnimation(Ellipse.HeightProperty, anim);
            } else if (rand == 2)
            {
                ColorAnimation anim = new ColorAnimation();
                String[] colors = { "#0034E6", "#E6A800", "#00E61E", "#663341", "#66C4E3" };
                anim.From = (Color)ColorConverter.ConvertFromString(colors[random.Next(0, colors.Length)]);
                anim.To = (Color)ColorConverter.ConvertFromString("#E36687");
                anim.Duration = TimeSpan.FromSeconds(0.4);
                ellipse.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, anim);
            } else if (rand == 3)
            {
                DoubleAnimation anim = new DoubleAnimation();
                if (random.Next(0, 2) == 0)
                {
                    anim.From = random.Next(0, 1);
                    anim.To = 7;
                }
                else
                {
                    anim.From = random.Next(10, 25);
                    anim.To = 7;
                }
                anim.Duration = TimeSpan.FromSeconds(0.3);
                ellipse.BeginAnimation(Ellipse.StrokeThicknessProperty, anim);
            } else if (rand == 4) 
            {
                ThicknessAnimation anim = new ThicknessAnimation();
                int margin = -random.Next(Convert.ToInt32(baseWidth / 2) + 1, Convert.ToInt32(baseWidth)+1);
                int rnd2 = random.Next(0, 4);
                if (rnd2 == 0)
                {
                    anim.From = new Thickness(margin, 0, 0, 0);
                } else if (rnd2 == 1)
                {
                    anim.From = new Thickness(0, margin, 0, 0);
                }
                else if (rnd2 == 2)
                {
                    anim.From = new Thickness(0, 0, margin, 0);
                }
                else if (rnd2 == 3)
                {
                    anim.From = new Thickness(0, 0, 0, margin);
                }
                anim.To = new Thickness(0, 0, 0, 0);
                anim.Duration = TimeSpan.FromSeconds(0.3);
                ellipse.BeginAnimation(Ellipse.MarginProperty, anim);
            } else if (rand == 5)
            {
                DoubleAnimation anim = new DoubleAnimation();
                anim.From = random.Next(0, 100);
                anim.To = 360;
                anim.Duration = TimeSpan.FromSeconds(0.3);
                RotateTransform rotateTransform1 = new RotateTransform();
                ellipse.RenderTransform = rotateTransform1;
                rotateTransform1.BeginAnimation(RotateTransform.AngleProperty, anim);
            }
        }

        private void DrawCross(int r, int c)
        {
            Line line1 = new Line();
            line1.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#66C4E3"));
            line1.StrokeThickness = 7;
            line1.X1 = 5;
            line1.Y1 = 5;
            line1.X2 = baseWidth - 5;
            line1.Y2 = baseHeight - 5;
            Grid.SetRow(line1, r);
            Grid.SetColumn(line1, c);
            Line line2 = new Line();
            line2.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#66C4E3"));
            line2.StrokeThickness = 7;
            line2.X1 = 5;
            line2.Y1 = baseHeight - 5;
            line2.X2 = baseWidth - 5;
            line2.Y2 = 5;
            Grid.SetRow(line2, r);
            Grid.SetColumn(line2, c);
            fieldGrid.Children.Add(line1);
            fieldGrid.Children.Add(line2);

            int rand = random.Next(1, 6);
            if (rand == 1)
            {
                DoubleAnimation anim = new DoubleAnimation();
                if (random.Next(0, 2) == 0)
                {
                    anim.From = random.Next(0, Convert.ToInt32(baseWidth) - 10);
                    anim.To = baseWidth - 5;
                }
                else
                {
                    anim.From = random.Next(Convert.ToInt32(baseWidth), Convert.ToInt32(baseWidth) + 10);
                    anim.To = baseWidth - 5;
                }
                anim.Duration = TimeSpan.FromSeconds(0.3);
                line1.BeginAnimation(Line.WidthProperty, anim);
                line1.BeginAnimation(Line.HeightProperty, anim);
                line2.BeginAnimation(Line.WidthProperty, anim);
                line2.BeginAnimation(Line.HeightProperty, anim);
            } else if (rand == 2)
            {
                ColorAnimation anim = new ColorAnimation();
                String[] colors = { "#99F72F", "#DB2FF7", "#F7952F", "#9755A2", "#E36687" };
                anim.From = (Color)ColorConverter.ConvertFromString(colors[random.Next(0, colors.Length)]);
                anim.To = (Color)ColorConverter.ConvertFromString("#66C4E3");
                anim.Duration = TimeSpan.FromSeconds(0.4);
                line1.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, anim);
                line2.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, anim);
            } else if (rand == 3)
            {
                DoubleAnimation anim = new DoubleAnimation();
                if (random.Next(0, 2) == 0)
                {
                    anim.From = random.Next(0, 1);
                    anim.To = 7;
                }
                else
                {
                    anim.From = random.Next(10, 30);
                    anim.To = 7;
                }
                anim.Duration = TimeSpan.FromSeconds(0.3);
                line1.BeginAnimation(Ellipse.StrokeThicknessProperty, anim);
                line2.BeginAnimation(Ellipse.StrokeThicknessProperty, anim);
            } else if (rand == 4)
            {
                ThicknessAnimation anim = new ThicknessAnimation();
                int margin = -random.Next(Convert.ToInt32(baseWidth/2) + 1, Convert.ToInt32(baseWidth) + 1);
                int rnd2 = random.Next(0, 4);
                if (rnd2 == 0)
                {
                    anim.From = new Thickness(margin, 0, 0, 0);
                }
                else if (rnd2 == 1)
                {
                    anim.From = new Thickness(0, margin, 0, 0);
                }
                else if (rnd2 == 2)
                {
                    anim.From = new Thickness(-margin, 0, 0, 0);
                }
                else if (rnd2 == 3)
                {
                    anim.From = new Thickness(0, -margin, 0, 0);
                }
                anim.To = new Thickness(0, 0, 0, 0);
                anim.Duration = TimeSpan.FromSeconds(0.3);
                line1.BeginAnimation(Ellipse.MarginProperty, anim);
                line2.BeginAnimation(Ellipse.MarginProperty, anim);
            } else if (rand == 5)
            {
                DoubleAnimation anim = new DoubleAnimation();
                anim.From = random.Next(0, 100);
                anim.To = 360;
                anim.Duration = TimeSpan.FromSeconds(0.3);
                RotateTransform rotateTransform1 = new RotateTransform();
                line1.RenderTransform = rotateTransform1;
                rotateTransform1.BeginAnimation(RotateTransform.AngleProperty, anim);
                RotateTransform rotateTransform2 = new RotateTransform();
                line2.RenderTransform = rotateTransform2;
                rotateTransform2.BeginAnimation(RotateTransform.AngleProperty, anim);
            }
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
                EndMessage.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#00E676"));
            }
            else if (result == 2) {
                EndMessage.Text = "You lose!";
                EndMessage.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#E61B00"));
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
            foreach (var line1 in lines)
            {
                fieldGrid.Children.Remove(line1);
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
