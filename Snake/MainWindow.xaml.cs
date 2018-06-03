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
using System.Windows.Threading;

namespace Snake
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int PADDING = 2;
        private const int N_CIRCLES = 30;
        private double radius;
        private Point headPosition = new Point(15, 15);
        private int longSnake = 4;
        private List<Ellipse> snake = new List<Ellipse>();
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private enum DirMovement
        {
            UP,
            LEFT,
            DOWN,
            RIGHT
        }
        private DirMovement currentDir = DirMovement.RIGHT;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            radius = (canvas.ActualHeight - PADDING * N_CIRCLES) / N_CIRCLES;
            for (int i = 0; i <= longSnake; i++)
            {
                var snakePart = new Ellipse()
                {
                    Height = radius,
                    Width = radius,
                    Fill = Brushes.LightGreen,
                };

                if (i == 0)
                {
                    snakePart.Fill = Brushes.ForestGreen;
                    //snakePart.Tag = "head";
                }
                var currentCol = Column2CorX((int)headPosition.Y - i);
                var currentRow = Row2CorY((int)headPosition.X);

                Canvas.SetLeft(snakePart, currentCol);
                Canvas.SetBottom(snakePart, currentRow);
                canvas.Children.Add(snakePart);
                snake.Add(snakePart);
            }

            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);



        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            var tail = snake.Last<Ellipse>();
            snake.Remove(tail);
            canvas.Children.Remove(tail);
            snake.First<Ellipse>().Fill = Brushes.LightGreen;

            switch (currentDir)
            {
                case DirMovement.UP:
                    headPosition.X -= 1;
                    break;
                case DirMovement.DOWN:
                    headPosition.X += 1;
                    break;
                case DirMovement.LEFT:
                    headPosition.Y -= 1;
                    break;
                case DirMovement.RIGHT:
                    headPosition.Y += 1;
                    break;
                default:
                    break;
            }

            var snakePart = new Ellipse()
            {
                Height = radius,
                Width = radius,
                Fill = Brushes.ForestGreen,
            };
            var currentCol = Column2CorX((int)headPosition.Y);
            var currentRow = Row2CorY((int)headPosition.X);
            Canvas.SetLeft(snakePart, currentCol);
            Canvas.SetBottom(snakePart, currentRow);
            canvas.Children.Add(snakePart);
            snake.Insert(0, snakePart);
        }

        private int CorX2Column(double xCor)
        {
            return Convert.ToInt32((xCor - PADDING) / (PADDING + radius));
        }

        private int CorY2Row(double yCor)
        {
            return Convert.ToInt32((N_CIRCLES - ((canvas.ActualHeight - yCor) / (radius + PADDING))));
        }

        private double Column2CorX(int col)
        {
            return PADDING + col * (PADDING + radius);
        }

        private double Row2CorY(int row)
        {
            return (N_CIRCLES - row) * (radius + PADDING);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Start();
            canvas.Focus();
        }

        private void Canvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                switch (currentDir)
                {
                    case DirMovement.UP:
                        currentDir = DirMovement.LEFT;
                        break;
                    case DirMovement.LEFT:
                        currentDir = DirMovement.DOWN;
                        break;
                    case DirMovement.DOWN:
                        currentDir = DirMovement.RIGHT;
                        break;
                    case DirMovement.RIGHT:
                        currentDir = DirMovement.UP;
                        break;
                    default:
                        break;
                }

            }
            else if (e.Key == Key.Right)
            {
                switch (currentDir)
                {
                    case DirMovement.UP:
                        currentDir = DirMovement.RIGHT;
                        break;
                    case DirMovement.LEFT:
                        currentDir = DirMovement.UP;
                        break;
                    case DirMovement.DOWN:
                        currentDir = DirMovement.LEFT;
                        break;
                    case DirMovement.RIGHT:
                        currentDir = DirMovement.DOWN;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
