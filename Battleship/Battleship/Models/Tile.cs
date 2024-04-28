using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Models
{
    public enum State
    {
        Hit = 1,
        Water = 2,
        Ship = 4,
        Miss = 8
    }
    public class Tile : INotifyPropertyChanged
    {
        public Tile(Coordinate coordinate)
        {
            Coordinate = coordinate;
        }

        public Tile()
        {
        }

        private Coordinate coordinate;

        public Coordinate Coordinate
        {
            get { return coordinate; }
            set { coordinate = value; }
        }

        private State tileState;

        public State TileState
        {
            get { return tileState; }
            set { tileState = value; FieldChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void FieldChanged([CallerMemberName] string field = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(field));
        }
    }
}
