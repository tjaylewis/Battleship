using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Models
{
    public class Board : INotifyPropertyChanged
    {
        private List<Tile> tiles;

        public List<Tile> Tiles
        {
            get { return tiles; }
            set { tiles = value; }
        }

        private List<Ship> ships;

        public List<Ship> Ships
        {
            get { return ships; }
            set { ships = value; }
        }

        private int totalHealth;

        public int TotalHealth
        {
            get { return totalHealth; }
            set { totalHealth = value; FieldChanged(); }
        }

        public Board()
        {
            Tiles = new List<Tile>();
            Ships = new List<Ship>();
            TotalHealth = 17;
        }

        public void initBoard()
        {
            Tiles.Clear();
            for (int r = 1; r < 11; r++)
            {
                for (int c = 1; c < 11; c++)
                {
                    Tile t = new Tile(new Coordinate(r, c));
                    t.TileState = State.Water;
                    Tiles.Add(t);
                }
            }
        }

        public void initShips()
        {
            Ships.Clear();
            for (int i = 0; i < 5; i++)
            {
                switch (i)
                {
                    case 0:
                        Ships.Add(new Ship("Battleship"));
                        break;
                    case 1:
                        Ships.Add(new Ship("Carrier"));
                        break;
                    case 2:
                        Ships.Add(new Ship("Cruiser"));
                        break;
                    case 3:
                        Ships.Add(new Ship("Destroyer"));
                        break;
                    case 4:
                        Ships.Add(new Ship("Submarine"));
                        break;
                    default:
                        break;
                }
            }
        }

        public List<Tile> GetAdjacentTiles(Tile origin)
        {
            List<Tile> adjTiles = new List<Tile>();
            for (int i = 0; i < Tiles.Count && adjTiles.Count < 8; i++)
            {
                if (origin.Coordinate.Col - 1 == Tiles[i].Coordinate.Col && origin.Coordinate.Row - 1 == Tiles[i].Coordinate.Row)
                {
                    adjTiles.Add(Tiles[i]);
                }
                else if (origin.Coordinate.Col + 1 == Tiles[i].Coordinate.Col && origin.Coordinate.Row + 1 == Tiles[i].Coordinate.Row)
                {
                    adjTiles.Add(Tiles[i]);
                }
                else if (origin.Coordinate.Col - 1 == Tiles[i].Coordinate.Col && origin.Coordinate.Row + 1 == Tiles[i].Coordinate.Row)
                {
                    adjTiles.Add(Tiles[i]);
                }
                else if (origin.Coordinate.Col + 1 == Tiles[i].Coordinate.Col && origin.Coordinate.Row - 1 == Tiles[i].Coordinate.Row)
                {
                    adjTiles.Add(Tiles[i]);
                }
                else if (origin.Coordinate.Col == Tiles[i].Coordinate.Col && origin.Coordinate.Row - 1 == Tiles[i].Coordinate.Row)
                {
                    adjTiles.Add(Tiles[i]);
                }
                else if (origin.Coordinate.Col == Tiles[i].Coordinate.Col && origin.Coordinate.Row + 1 == Tiles[i].Coordinate.Row)
                {
                    adjTiles.Add(Tiles[i]);
                }
                else if (origin.Coordinate.Col - 1 == Tiles[i].Coordinate.Col && origin.Coordinate.Row == Tiles[i].Coordinate.Row)
                {
                    adjTiles.Add(Tiles[i]);
                }
                else if (origin.Coordinate.Col + 1 == Tiles[i].Coordinate.Col && origin.Coordinate.Row == Tiles[i].Coordinate.Row)
                {
                    adjTiles.Add(Tiles[i]);
                }
            }
            return adjTiles;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void FieldChanged([CallerMemberName] string field = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(field));
        }
    }
}
