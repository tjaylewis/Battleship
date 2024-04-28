using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Models
{
    public enum Orientation
    {
        Up,
        Down,
        Left,
        Right
    }

    public class Ship : INotifyPropertyChanged
    {
        public Ship(string name)
        {
            switch(name)
            {
                case "Battleship":
                    Name = name;
                    Health = 4;
                    break;
                case "Carrier":
                    Name = name;
                    Health = 5;
                    break;
                case "Cruiser":
                    Name = name;
                    Health = 3;
                    break;
                case "Destroyer":
                    Name = name;
                    Health = 2;
                    break;
                case "Submarine":
                    Name = name;
                    Health = 3;
                    break;
            }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; FieldChanged(); }
        }

        private int health;

        public int Health
        {
            get { return health; }
            set { health = value; FieldChanged(); }
        }

        private Orientation shipOrientation;

        public Orientation ShipOrientation
        {
            get { return shipOrientation; }
            set { shipOrientation = value; FieldChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void FieldChanged([CallerMemberName] string field = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(field));
        }
    }
}
