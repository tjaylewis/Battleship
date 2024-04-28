using Battleship.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Battleship
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Board playerboard = new Board();
        private Board aiboard = new Board();
        private bool placing = true;
        private int ship = 0;
        private Random rand = new Random();
        private bool cheating = false;
        private Tile AILastTarget;
        private List<Tile> AIPastTargets = new List<Tile>();
        private bool gameOver = false;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void OnNewGame(object sender, TappedRoutedEventArgs trea)
        {
            setup();
        }

        private void setup()
        {
            cheating = false;
            AILastTarget = null;
            AIPastTargets.Clear();
            gameOver = false;
            placing = true;
            ship = 0;
            playerboard.initBoard();
            playerboard.initShips();
            aiboard.initBoard();
            aiboard.initShips();
            initButtons();
            initGridCoordinates();
            PlaceAiShips();
        }

        private void initGridCoordinates()
        {
            for (int i = 1; i < 11; i++)
            {
                TextBlock tb = new TextBlock();
                tb.SetValue(TextBlock.TextProperty, i.ToString());
                Grid.SetColumn(tb, 0);
                Grid.SetRow(tb, i);
                PlayerBoardGrid.Children.Add(tb);
                tb = new TextBlock();
                tb.SetValue(TextBlock.TextProperty, i.ToString());
                Grid.SetColumn(tb, 0);
                Grid.SetRow(tb, i);
                AIBoardGrid.Children.Add(tb);
                tb = new TextBlock();
                tb.SetValue(TextBlock.TextProperty, ((char)(64 + i)).ToString());
                Grid.SetColumn(tb, i);
                Grid.SetRow(tb, 0);
                PlayerBoardGrid.Children.Add(tb);
                tb = new TextBlock();
                tb.SetValue(TextBlock.TextProperty, ((char)(64 + i)).ToString());
                Grid.SetColumn(tb, i);
                Grid.SetRow(tb, 0);
                AIBoardGrid.Children.Add(tb);
            }
        }

        private void initButtons()
        {
            AIBoardGrid.Children.Clear();
            PlayerBoardGrid.Children.Clear();
            Converters.TileToBrushConverter converter = new Converters.TileToBrushConverter();
            foreach (Tile t in playerboard.Tiles)
            {
                Button b = new Button();
                b.Margin = new Thickness(1);
                b.Padding = new Thickness(1);
                PlayerBoardGrid.HorizontalAlignment = HorizontalAlignment.Center;
                PlayerBoardGrid.VerticalAlignment = VerticalAlignment.Center;
                b.Width = 100;
                b.Height = 100;
                

                Binding bi = new Binding();
                bi.Path = new PropertyPath("TileState");
                bi.Mode = BindingMode.OneWay;
                bi.Source = t;
                bi.Converter = converter;

                SolidColorBrush scb = new SolidColorBrush(Colors.Blue);
                b.Background = scb;
                b.SetBinding(BackgroundProperty, bi);

                Grid.SetColumn(b, t.Coordinate.Col);
                Grid.SetRow(b, t.Coordinate.Row);
                b.Tapped += OnBoardButtonTapped;
                b.RightTapped += OnBoardButtonRightTapped;
                PlayerBoardGrid.Children.Add(b);
            }
            foreach (Tile t in aiboard.Tiles)
            {
                Button b = new Button();
                b.Margin = new Thickness(1);
                b.Padding = new Thickness(1);
                AIBoardGrid.HorizontalAlignment = HorizontalAlignment.Center;
                AIBoardGrid.VerticalAlignment = VerticalAlignment.Center;
                b.Width = 100;
                b.Height = 100;

                Binding bi = new Binding();
                bi.Path = new PropertyPath("TileState");
                bi.Mode = BindingMode.OneWay;
                bi.Source = t;
                bi.Converter = converter;

                SolidColorBrush scb = new SolidColorBrush(Colors.Blue);
                b.Background = scb;
                b.SetBinding(BackgroundProperty, bi);

                Grid.SetColumn(b, t.Coordinate.Col);
                Grid.SetRow(b, t.Coordinate.Row);
                b.Tapped += OnBoardButtonTapped;
                b.RightTapped += OnBoardButtonRightTapped;
                AIBoardGrid.Children.Add(b);
            }

        }

        private void OnBoardButtonRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (!gameOver)
            {
                if (placing)
                {
                    PlacePlayerShip(sender, e);
                }
            }
        }

        private void OnBoardButtonTapped(object sender, TappedRoutedEventArgs e)
        {
            if (!gameOver)
            {
                if (placing)
                {
                    PlacePlayerShip(sender, e);
                }
                else
                {
                    int col = Grid.GetColumn(sender as Button);
                    int row = Grid.GetRow(sender as Button);
                    Tile tile = aiboard.Tiles.Where(t => t.Coordinate.Col == col && t.Coordinate.Row == row).FirstOrDefault();
                    if (tile.TileState == State.Ship || tile.TileState == (State.Ship & State.Water))
                    {
                        tile.TileState = State.Hit;
                        aiboard.TotalHealth--;
                    }
                    else if (tile.TileState == State.Water)
                    {
                        tile.TileState = State.Miss;
                    }
                    if (aiboard.TotalHealth == 0)
                    {
                        GameOver();
                    }
                    else
                    {
                        AIAttack();
                    }
                }
            }
        }

        private void GameOver()
        {
            gameOver = true;
            WinnerWinnerChickenDinner();
        }

        private async void WinnerWinnerChickenDinner()
        {

            MessageDialog md = new MessageDialog("");
            if (aiboard.TotalHealth == 0)
            {
                md = new MessageDialog("The Player has beaten the AI and sunk all of their ships! Would you like to play again?");
            }
            else
            {
                md = new MessageDialog("The AI has bested you and sunk all of your ships! Would you like to play again?");

            }
            md.Commands.Add(new UICommand("YES", new UICommandInvokedHandler(this.CommandInvokedHandler)));
            md.Commands.Add(new UICommand("NO", new UICommandInvokedHandler(this.CommandInvokedHandler)));
            md.DefaultCommandIndex = 0;
            md.CancelCommandIndex = 1;
            _ = await md.ShowAsync();
        }


        private void CommandInvokedHandler(IUICommand command)
        {
            switch (command.Label.ToLower())
            {
                case "yes":
                    setup();
                    break;
                case "no":
                    Application.Current.Exit();
                    break;
                default:
                    break;

            }
        }

        private void AIAttack()
        {
            if (AILastTarget is null)
            {
                bool found = false;
                Tile tile = new Tile();
                while (!found)
                {
                    int col = rand.Next(1, 10);
                    int row = rand.Next(1, 10);
                    tile = playerboard.Tiles.Where(t => t.Coordinate.Col == col && t.Coordinate.Row == row).FirstOrDefault();
                    if (!AIPastTargets.Contains(tile))
                    {
                        found = true;
                    }
                }

                if (tile.TileState == State.Ship)
                {
                    tile.TileState = State.Hit;
                    playerboard.TotalHealth--;
                    AILastTarget = tile;
                    AIPastTargets.Add(tile);
                    if (playerboard.TotalHealth == 0)
                    {
                        GameOver();
                    }
                }
                else if (tile.TileState == State.Water)
                {
                    AILastTarget = tile;
                    AIPastTargets.Add(tile);
                }
            }
            else
            {
                int col = AILastTarget.Coordinate.Col;
                int row = AILastTarget.Coordinate.Row;
                List<Tile> adjTiles = playerboard.GetAdjacentTiles(aiboard.Tiles.Where(t => t.Coordinate.Col == col && t.Coordinate.Row == row).FirstOrDefault());
                Tile tile = new Tile();
                foreach (Tile t in adjTiles)
                {
                    if (!AIPastTargets.Contains(t))
                    {
                        tile = t;
                        break;
                    }
                }

                if(tile is null)
                {
                    bool found = false;
                    tile = new Tile();
                    while (!found)
                    {
                        col = rand.Next(1, 10);
                        row = rand.Next(1, 10);
                        tile = playerboard.Tiles.Where(t => t.Coordinate.Col == col && t.Coordinate.Row == row).FirstOrDefault();
                        if (!AIPastTargets.Contains(tile))
                        {
                            found = true;
                        }
                    }
                }

                if (tile.TileState == State.Ship)
                {
                    tile.TileState = State.Hit;
                    playerboard.TotalHealth--;
                    AILastTarget = tile;
                    AIPastTargets.Add(tile);
                    if (playerboard.TotalHealth == 0)
                    {
                        GameOver();
                    }
                }
                else if (tile.TileState == State.Water)
                {
                    AILastTarget = tile;
                    AIPastTargets.Add(tile);
                }
            }
        }

        private void PlacePlayerShip(object sender, RoutedEventArgs rea)
        {
            int col = Grid.GetColumn(sender as Button);
            int row = Grid.GetRow(sender as Button);
            bool valid = true;
            if (rea is TappedRoutedEventArgs)
            {
                if (playerboard.Tiles.Where(t => t.Coordinate.Col == col && t.Coordinate.Row == row).FirstOrDefault().TileState != State.Ship)
                {
                    List<Tile> tiles = new List<Tile>();

                    for (int i = row; i < playerboard.Ships[ship].Health + row; i++)
                    {
                        Tile tile = playerboard.Tiles.Where(t => t.Coordinate.Col == col && t.Coordinate.Row == i - (playerboard.Ships[ship].Health - 1)).FirstOrDefault();
                        tiles.Add(tile);
                    }

                    foreach (Tile t in tiles)
                    {
                        if (t == null || t.TileState != State.Water)
                        {
                            valid = false;
                        }
                    }
                    if (valid)
                    {
                        foreach (Tile t in tiles)
                        {
                            t.TileState = State.Ship;
                        }
                        ship++;
                    }
                }
                if (ship == 5)
                {
                    placing = false;
                }
            }
            else if(rea is RightTappedRoutedEventArgs)
            {
                if (playerboard.Tiles.Where(t => t.Coordinate.Col == col && t.Coordinate.Row == row).FirstOrDefault().TileState != State.Ship)
                {
                    List<Tile> tiles = new List<Tile>();

                    for (int i = col; i < playerboard.Ships[ship].Health + col; i++)
                    {
                        Tile tile = playerboard.Tiles.Where(t => t.Coordinate.Col == i && t.Coordinate.Row == row).FirstOrDefault();
                        tiles.Add(tile);
                    }

                    foreach (Tile t in tiles)
                    {
                        if (t == null || t.TileState != State.Water)
                        {
                            valid = false;
                        }
                    }
                    if (valid)
                    {
                        foreach (Tile t in tiles)
                        {
                            t.TileState = State.Ship;
                        }
                        ship++;
                    }
                }
                if (ship == 5)
                {
                    placing = false;
                }
            }
        }

        private void PlaceAiShips()
        {
            int aiship = 0;
            while(aiship != 5)
            { 
                int col = rand.Next(1, 10);
                int row = rand.Next(1, 10);
                int randNum = rand.Next();
                bool valid = true;
                if (randNum % 2 == 0)
                {
                    if (aiboard.Tiles.Where(t => t.Coordinate.Col == col && t.Coordinate.Row == row).FirstOrDefault().TileState != State.Ship)
                    {
                        List<Tile> tiles = new List<Tile>();

                        for (int i = row; i < aiboard.Ships[aiship].Health + row; i++)
                        {
                            Tile tile = aiboard.Tiles.Where(t => t.Coordinate.Col == col && t.Coordinate.Row == i - (aiboard.Ships[aiship].Health - 1)).FirstOrDefault();
                            tiles.Add(tile);
                        }

                        foreach (Tile t in tiles)
                        {
                            if (t == null || t.TileState != State.Water)
                            {
                                valid = false;
                            }
                        }
                        if (valid)
                        {
                            foreach (Tile t in tiles)
                            {
                                t.TileState = State.Ship & State.Water;
                            }
                            aiship++;
                        }
                    }
                }
                else
                {
                    if (aiboard.Tiles.Where(t => t.Coordinate.Col == col && t.Coordinate.Row == row).FirstOrDefault().TileState != State.Ship)
                    {
                        List<Tile> tiles = new List<Tile>();

                        for (int i = col; i < aiboard.Ships[aiship].Health + col; i++)
                        {
                            Tile tile = aiboard.Tiles.Where(t => t.Coordinate.Col == i && t.Coordinate.Row == row).FirstOrDefault();
                            tiles.Add(tile);
                        }

                        foreach (Tile t in tiles)
                        {
                            if (t == null || t.TileState != State.Water)
                            {
                                valid = false;
                            }
                        }
                        if (valid)
                        {
                            foreach (Tile t in tiles)
                            {
                                t.TileState = State.Ship & State.Water;
                            }
                            aiship++;
                        }
                    }
                }
            }
        }


        private void OnLoadGame(object sender, TappedRoutedEventArgs trea)
        {

        }

        private void OnCheat(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            if (cheating)
            {
                aiboard.Tiles.Where(t => t.TileState == State.Ship).ToList().ForEach(t => t.TileState = State.Ship & State.Water);
                cheating = false;
            }
            else
            {
                aiboard.Tiles.Where(t => t.TileState == (State.Ship & State.Water)).ToList().ForEach(t => t.TileState = State.Ship);
                cheating = true;
            }
        }
    }
}
