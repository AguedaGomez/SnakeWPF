using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace Snake
{
    class GameObject
    {
        public Ellipse ellipse;
        public Point cells;

        public GameObject (Ellipse ellipse, Point cells)
        {
            this.ellipse = ellipse;
            this.cells = cells;
        }
    }
}
