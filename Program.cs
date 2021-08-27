using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace TicTacNo
{
    class Program
    {

        class Game
        {
            public Piece[] Board = new Piece[9];
            private static readonly int[,] Win =
            {
                { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 },
                { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 },
                { 0, 4, 8 }, { 2, 4, 6 },
            };

            public enum Piece { _ = 0, X = 1, O = 2 };

            public Piece Turn = Piece.X;
            int Choice = 0;
            int RandChoice = 0;
            public int Steps = 0;
            public int ComputerEgo = 0;

            public Piece Computer;
            public Piece Player;

            public Game()
            {
                Turn = Piece.X;
                Player = Piece.X;
            }

            public void ResetBoard()
            {
                Turn = Piece.X;
                SetPlayer(Piece.X);
                Board = new Piece[9];
            }

            public void Display()
            {
                Console.WriteLine("\n┌─────┬─────┬─────┐");
                Console.WriteLine("│     │     │     │");
                Console.WriteLine("│  {0}  │  {1}  │  {2}  │", Board[0], Board[1], Board[2]);
                Console.WriteLine("│     │     │     │");
                Console.WriteLine("├─────┼─────┼─────┤");
                Console.WriteLine("│     │     │     │");
                Console.WriteLine("│  {0}  │  {1}  │  {2}  │", Board[3], Board[4], Board[5]);
                Console.WriteLine("│     │     │     │");
                Console.WriteLine("├─────┼─────┼─────┤");
                Console.WriteLine("│     │     │     │");
                Console.WriteLine("│  {0}  │  {1}  │  {2}  │", Board[6], Board[7], Board[8]);
                Console.WriteLine("│     │     │     │");
                Console.WriteLine("└─────┴─────┴─────┘");
            }

            public void SetPlayer(Piece Player)
            {
                this.Player = Player;
                this.Computer = SwitchPiece(Player);
            }

            public void Play(int Move)
            {
                if (Turn == Player)
                {
                    Board = SetBoard(Board, Turn, Move);
                    Turn = SwitchPiece(Turn);
                }
                else if (Turn == Computer)
                {
                    Minimax(Copy(Board), false, Steps);
                    Board = SetBoard(Board, Turn, Choice);
                    Turn = SwitchPiece(Turn);
                }
                Display();

                //
                //Console.WriteLine(ComputerEgo.ToString());
                //Console.WriteLine(Steps.ToString());
                //
            }

            public void Complacency(int computerEgo)
            {
                if (Steps == 9)
                {
                    return;
                }
                else if (Steps == 0)
                {
                    if (computerEgo == -1) Steps++;
                }
                else if (Steps == 8)
                {
                    if (computerEgo == 1) Steps--;
                }
                else
                {
                    Steps -= computerEgo;
                }
                ComputerEgo = 0;
            }

            float Minimax(Piece[] InputBoard, bool isMinimising, int steps)
            {   
                Piece[] BoardCopy = Copy(InputBoard);

                if (CheckWin(BoardCopy, Player)) return 10f;
                if (CheckWin(BoardCopy, SwitchPiece(Player))) return -10f;
                if (CheckGameOver(BoardCopy))
                {
                    return 0;
                }

                List<float> scoreList = new List<float>();
                List<float> moveList = new List<float>();

                for (int i = 0; i < 9; i++)
                {
                    if (BoardCopy[i] == Piece._)
                    {
                        BoardCopy[i] = isMinimising ? Player : SwitchPiece(Player);
                        if (steps < 1)
                        {
                            Random rnd = new Random();
                            RandChoice = rnd.Next(0, 9);
                            return -20;
                        }
                        float score = Minimax(BoardCopy, !isMinimising, steps - 1);
                        BoardCopy[i] = Piece._;
                        scoreList.Add(score);
                        moveList.Add(i);
                    }
                }

                var dict = moveList.Zip(scoreList, (k, v) => new { k, v })
                                   .ToDictionary(x => x.k, x => x.v);

                if (isMinimising)
                {
                    var highestMove = dict.MaxBy(x => x.Value).FirstOrDefault().Key;
                    Choice = scoreList.Max() > -20 ? (int)highestMove : RandChoice;
                    //Console.WriteLine($"MA-{Choice}");
                    return scoreList.Max();
                }

                var lowestMove = dict.MinBy(x => x.Value).FirstOrDefault().Key;
                Choice = (int)lowestMove;
                //Console.WriteLine($"MI-{Choice}");
                return scoreList.Min();
            }

            public static bool CheckWin(Piece[] Board, Piece Player)
            {
                for (int i = 0; i < 8; i++)
                {
                    if
                    (
                        Board[Win[i, 0]] == Player &&
                        Board[Win[i, 1]] == Player &&
                        Board[Win[i, 2]] == Player
                    )
                        return true;
                }
                return false;
            }

            public static bool CheckGameOver(Piece[] Board)
            {
                foreach (Piece p in Board)
                {
                    if (p == Piece._) return false;
                }
                return true;
            }

            static Piece SwitchPiece(Piece Piece)
            {
                if (Piece == Piece.X) return Piece.O;
                else return Piece.X;
            }

            static Piece[] Copy(Piece[] Board)
            {
                Piece[] Copy = new Piece[9];
                for (int i = 0; i < 9; i++)
                {
                    Copy[i] = Board[i];
                }
                return Copy;
            }

            static Piece[] SetBoard(Piece[] Board, Piece Move, int Pos)
            {
                Piece[] newBoard = Copy(Board);
                newBoard[Pos] = Move;
                return newBoard;
            }

        }

        static Random rnd = new Random();
        static void Main(string[] args)
        {
            bool playing = true;
            Game game = new Game();

            Console.Write("\nPick a starting difficulty (1/2/3): ");
            string difficulty = Console.ReadLine();

            switch (difficulty)
            {
                case "1":
                    {
                        game.Steps = 3;
                        Console.Write("\nDifficulty Level 1");
                        break;
                    }
                case "2":
                    {
                        game.Steps = 5;
                        Console.Write("\nDifficulty Level 2");
                        break;
                    }
                case "3":
                    {
                        game.Steps = 9;
                        Console.Write("\nDifficulty Level 3");
                        break;
                    }
                default:
                    {
                        game.Steps = rnd.Next(0, 8);
                        Console.Write("\nRandom Difficulty Level chosen");
                        break;
                    }
            }

            while (playing)
            {
                game.ResetBoard();
                
                bool gameOver = false;
                string winner = "";
                
                game.Display();

                while (!gameOver)
                {
                    bool inputCorrect;
                    int input = 0;

                    do
                    {
                        Console.Write($"\n{game.Player}'s turn. Enter a number from 1-9: ");
                        try
                        {
                            input = Convert.ToInt32(Console.ReadLine());
                        }
                        catch
                        {
                            Console.WriteLine("\nEnter a number");
                            inputCorrect = false;
                            continue;
                        }

                        if (input > 0 && input < 10)
                        {
                            if (game.Board[input - 1] == Game.Piece._)
                            {
                                inputCorrect = true;
                            }
                            else
                            {
                                Console.WriteLine("\nThis space is already taken!");
                                inputCorrect = false;
                                continue;
                            }
                        }
                        else
                        {
                            Console.WriteLine("\nEnter only a number from 1-9");
                            inputCorrect = false;
                            continue;
                        }
                    }
                    while (!inputCorrect);

                    //Player
                    game.Play(input - 1);

                    if (Game.CheckWin(game.Board, game.Player))
                    {
                        winner = "Player";
                        break;
                    }
                    if (Game.CheckWin(game.Board, game.Computer))
                    {
                        winner = "Computer";
                        break;
                    }
                    if (Game.CheckGameOver(game.Board))
                    {
                        break;
                    }

                    //Computer
                    Console.WriteLine($"\n{game.Computer}'s turn.");
                    game.Play(0);

                    if (Game.CheckWin(game.Board, game.Player))
                    {
                        winner = "Player";
                        break;
                    }
                    if (Game.CheckWin(game.Board, game.Computer))
                    {
                        winner = "Computer";
                        break;
                    }
                    if (Game.CheckGameOver(game.Board))
                    {
                        break;
                    }
                }

                if (winner == "Player")
                {
                    Console.WriteLine("The winner is Player!");
                    
                    int egoDecrease = Convert.ToBoolean(rnd.Next(0, (int)Math.Round(1.0 + ((double)game.Steps / 2)))) ? 0 : -1;
                    game.ComputerEgo += egoDecrease;
                }
                else if (winner == "Computer")
                {
                    Console.WriteLine("The winner is Computer!");
                    int numb = (int)Math.Round(1.0 + ((double)game.Steps / 3));
                    int rand = rnd.Next(0, numb);
                    int egoIncrease = Convert.ToBoolean(rand) ? 1 : 0;
                    game.ComputerEgo += egoIncrease;
                }
                else
                {
                    Console.WriteLine("Draw!");
                }

                game.Complacency(game.ComputerEgo);

                Console.Write("\nPlay again? (Y/N): ");
                string playingInput = Console.ReadLine();

                if (playingInput.ToUpper() == "N")
                {
                    playing = false;
                }
            }
        }
    }
}
