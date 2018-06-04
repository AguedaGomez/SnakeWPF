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
        private const int PADDING = 0;
        private const int N_CIRCLES = 32;
        private const int TRY = 20;
        private double radius;
        private Point headPosition = new Point(15, 15);
        private int longSnake = 4;
        private int score = 0;
        private List<GameObject> snake = new List<GameObject>();
        private Dictionary<Point, GameObject> occupedCellds = new Dictionary<Point, GameObject>();
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private enum DirMovement
        {
            UP,
            LEFT,
            DOWN,
            RIGHT
        }
        private DirMovement currentDir = DirMovement.RIGHT;
        private int speed = 500;
        private int foodAvaiable = 0;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            radius = (canvas.ActualHeight - PADDING * N_CIRCLES) / N_CIRCLES;

            Label gameOverText = new Label()
            {
                Content = "PRESS START",
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Width = 350,
                Height = 100,
                FontSize = 50,

            };
            Canvas.SetLeft(gameOverText, (canvas.Width - gameOverText.Width) / 2);
            Canvas.SetBottom(gameOverText, (canvas.Height - gameOverText.Height) / 2);
            canvas.Children.Clear();
            canvas.Children.Add(gameOverText);

            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, speed);

        }

        private void CreateSnake()
        {
            var color = Brushes.LightGreen;
            for (int i = 0; i <= longSnake; i++)
            {
                if (i == 0)
                    color = Brushes.ForestGreen;
                else
                    color = Brushes.LightGreen;

                var col = (int)headPosition.Y - i;
                var row = (int)headPosition.X;

                var snakeGO = CreateGameObject("snake", color, col, row);

                snake.Add(snakeGO);
                occupedCellds.Add(new Point(row, col), snakeGO);
            }
        }

        private void UpdateScore()
        {
            score += 10;
            puntos.Text = "score: " + score;
        }

        private GameObject CreateGameObject(string tag, Brush color, int col, int row)
        {
            var ellipse = new Ellipse()
            {
                Height = radius,
                Width = radius,
                Tag = tag,
                Fill = color,
            };

            var currentCol = Column2CorX(col);
            var currentRow = Row2CorY(row);

            Canvas.SetLeft(ellipse, currentCol);
            Canvas.SetBottom(ellipse, currentRow);
            canvas.Children.Add(ellipse);

            GameObject GO = new GameObject(ellipse, new Point(row, col));
            return GO;
            /*if (tag == "snakeBody")
                snake.Add(GO);
            else if (tag == "snakeHead")
                snake.Insert(0, GO);
            if (!CheckCollision())
                occupedCellds.Add(new Point(row, col), GO);*/

        }

        private void GameOver()
        {
            dispatcherTimer.Stop();
            Label gameOverText = new Label()
            {
                Content = "GAME OVER",
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Width = 350,
                Height = 100,
                FontSize = 50,

            };
            Canvas.SetLeft(gameOverText, (canvas.Width - gameOverText.Width) / 2);
            Canvas.SetBottom(gameOverText, (canvas.Height - gameOverText.Height) / 2);
            canvas.Children.Clear();
            canvas.Children.Add(gameOverText);
            Start.IsEnabled = true;
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            var tail = snake.Last();
            occupedCellds.Remove(tail.cells);
            snake.Remove(tail);
            canvas.Children.Remove(tail.ellipse);
            snake.First().ellipse.Fill = Brushes.LightGreen;

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

            var col = (int)headPosition.Y;
            var row = (int)headPosition.X;

            var snakeHead = CreateGameObject("snake", Brushes.ForestGreen, col, row);

            snake.Insert(0, snakeHead);
            if (!CheckCollision())
                occupedCellds.Add(new Point(row, col), snakeHead);
            if (foodAvaiable < 1)
                CreateFood();
        }

        private void CreateFood()
        {
            Random rand = new Random();
            for (int i=0; i<TRY; i++)
            {
                var row = rand.Next(0, N_CIRCLES);
                var col = rand.Next(0, N_CIRCLES);
                var foodPosition = new Point(row, col);
                if (!occupedCellds.ContainsKey(foodPosition))
                {
                    var food = CreateGameObject("food", Brushes.Red, col, row);
                    occupedCellds.Add(foodPosition, food);
                    foodAvaiable = 1;
                    break;
                }
        
            }

        }

        private bool CheckCollision()
        {
            if (occupedCellds.ContainsKey(headPosition))
            {
                switch (occupedCellds[headPosition].ellipse.Tag)
                {
                    case "snake":
                        GameOver();
                        break;
                    case "food":
                        canvas.Children.Remove(occupedCellds[headPosition].ellipse);
                        occupedCellds.Remove(headPosition);
                        foodAvaiable = 0;
                        CreateFood();
                        if (speed >= 50)
                            speed -= 50;
                        var col = (int)snake.Last().cells.Y;
                        var row = (int)snake.Last().cells.X;
                        var snakeGO = CreateGameObject("snake", Brushes.LightGreen, col, row);
                        snake.Add(snakeGO);
                        UpdateScore();
                        break;
                    default:
                        break;
                }
                return true;
            }
            else
            {
                if (headPosition.X > N_CIRCLES | headPosition.Y >= N_CIRCLES | headPosition.X <= 0 | headPosition.Y < 0)
                    GameOver();
            }
            return false;
        }

        private int CorX2Column(double xCor)
        {
            var res = (xCor - PADDING) / (PADDING + radius);
            return (int)Math.Round(res);
        }

        private int CorY2Row(double yCor)
        {
            var res = (N_CIRCLES - ((canvas.ActualHeight - yCor) / (radius + PADDING)));
            return (int)Math.Round(res);
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
            ClearState();
            CreateSnake();
            dispatcherTimer.Start();
            canvas.Focus();
            CreateFood();
            Start.IsEnabled = false;
        }

        private void ClearState()
        {
            snake.Clear();
            canvas.Children.Clear();
            occupedCellds.Clear();
            headPosition = new Point(15, 15);
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
