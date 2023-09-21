using System;
using System.Data;
using System.Drawing;
using System.Net.Http.Headers;
using System.Numerics;
using System.Reflection;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static Random rnd = new Random();
        static string[,] TTTBoard = initializeBoard();
        static List<string> Pool = new List<string>();
        static List<string> playermoves = new List<string>();
        static List<string> Logs = new List<string>();

        static int ConsecDraws = 0;
        static int Round = 0;
        static string Player = "";
        static int HumanWins = 0;
        static int ComputerWins = 0;
        static string MovedFirst = "";

        static void Main(string[] args)
        {
            gameLoop();
        }

        static void gameLoop()
        {
            display(TTTBoard);
            string Player = firstMove();
            string moveTracker = Player;

            string Winner = "";
            

            while (!isWinner(out Winner) || ConsecDraws >= 5)
            {
                playermoves = playerMove(Player);
                updateBoard(playermoves[0], playermoves[1], Player);
                Player = changePlayerTurn(Player);
                if (winDetection(Player, out Winner))
                {
                    if (Winner == "X")
                    {
                        HumanWins++;
                        Winner = "Human";
                    }
                    else
                    {
                        ComputerWins++;
                        Winner = "Computer";
                    }
                    ConsecDraws = 0;
                    roundEnd("win", Winner);

                    // Ensures the game alternates who goes first
                    Player = changePlayerTurn(moveTracker);
                    moveTracker = Player;

                }
                // if board is populated or draw
                if (isPopulated(TTTBoard))
                {
                    ConsecDraws++;
                    roundEnd("draw", "");
                }
            }
            Console.WriteLine("The Winner of the best of 5 is the {0}!", Winner);
            Console.ReadKey();
        }

        static string firstMove()
        {
            if (rnd.Next(1, 10) % 2 == 0)
                Player = "Human";
            else
                Player = "Computer";

            return Player;
        }

        static void OutputHistory()
        {
            string FileName = "Logs_Round" + Round++ + ".txt";
            using (StreamWriter sw = new StreamWriter(FileName, false))
                for (int i = 0; i < Logs.Count; i++)
                    sw.WriteLine(Logs[i]);

        }

        static bool isWinner(out string Winner)
        {
            if (HumanWins == 3 || ComputerWins == 3)
            {
                if (HumanWins == 3)
                    Winner = "Human";
                else
                    Winner = "Computer";
                return true;
            }

            else
            {
                Winner = "";
                return false;
            }

        }


        static void roundEnd(string result, string winner_)
        {
            if (result == "win")
                Console.WriteLine("The winner of this round is {0}!", winner_);
            else
                Console.WriteLine("Round draw!");
            Console.ReadKey();

            clearBoard(TTTBoard);
            display(TTTBoard);
            OutputHistory();
            Logs.Clear();
            Player = changePlayerTurn(MovedFirst);
        }

        static bool continuePlaying()
        {
            Console.WriteLine("Continue Playing? \nType YES to continue playing");
            string temp = Console.ReadLine();
            if (temp.ToUpper() == "YES")
            {
                Player = changePlayerTurn(Player);
                clearBoard(TTTBoard);
                display(TTTBoard);
                return true;
            }
            else
                return false;
        }

        static bool winDetection(string Player, out string winner)
        {
            winner = "";
            // check row
            for (int col = 0; col < TTTBoard.GetLength(0); col++)
                if ((TTTBoard[2, col] == TTTBoard[1, col]) && (TTTBoard[1, col] == TTTBoard[0, col]))
                {
                    if (TTTBoard[2, col] == null)
                        continue;
                    else
                    {
                        winner = TTTBoard[2, col];
                        return true;
                    }

                }
            // check col
            for (int row = 0; row < TTTBoard.GetLength(1); row++)
                if ((TTTBoard[row, 2] == TTTBoard[row, 1]) && (TTTBoard[row, 1] == TTTBoard[row, 0]))
                {
                    if (TTTBoard[row, 2] == null)
                        continue;
                    else
                    {
                        winner = TTTBoard[row, 2];
                        return true;
                    }
                }

            // check / diagonal
            if ((TTTBoard[2, 0] == TTTBoard[1, 1]) && (TTTBoard[1, 1] == TTTBoard[0, 2]))
            {
                if (TTTBoard[2, 0] != null)
                {
                    winner = TTTBoard[2, 0];
                    return true;
                }
            }

            // check \ diagonal
            if ((TTTBoard[0, 0] == TTTBoard[1, 1]) && (TTTBoard[1, 1] == TTTBoard[2, 2]))
            {
                if (TTTBoard[0, 0] != null)
                {
                    winner = TTTBoard[0, 0];
                    return true;
                }
            }

            return false;
        }
        static string AIprayingformydownfall()
        {
            Pool.Clear();
            Pool = validCoordinates(TTTBoard);
            string coord = "";
            int attempts = 0;


            while (attempts < Pool.Count)
            {
                // col
                for (int col = 0; col < TTTBoard.GetLength(1); col++)
                {
                    //Console.WriteLine("Checking col {0}", col);
                    if ((TTTBoard[col, 2] == TTTBoard[col, 1]))
                        if (TTTBoard[col, 2] != null)
                            coord = col + "0";
                    if (Pool.Contains(coord))
                        return coord;

                    if (TTTBoard[col, 1] == TTTBoard[col, 0])
                        if (TTTBoard[col, 1] != null)
                            coord = col + "2";

                    if (Pool.Contains(coord))
                        return coord;

                    if (TTTBoard[col, 2] == TTTBoard[col, 0])
                        if (TTTBoard[col, 2] != null)
                            coord = col + "1";

                    if (Pool.Contains(coord))
                        return coord;
                }

                    // row
                for (int col = 0; col < TTTBoard.GetLength(0); col++)
                {

                    //Console.WriteLine("Checking row {0}", col);
                    if ((TTTBoard[2, col] == TTTBoard[1, col]))
                        if (TTTBoard[2, col] != null)
                            coord = "0" + col;
                    if (Pool.Contains(coord))
                        return coord;

                    if (TTTBoard[1, col] == TTTBoard[0, col])
                        if (TTTBoard[1, col] != null)
                            coord = "2" + col;

                    if (Pool.Contains(coord))
                        return coord;

                    if (TTTBoard[2, col] == TTTBoard[0, col])
                        if (TTTBoard[2, col] != null)
                            coord = "1" + col;

                    if (Pool.Contains(coord))
                        return coord;
                }


                // check / diagonal
                //Console.WriteLine("Checking / Diagonal");
                if (TTTBoard[2, 0] == TTTBoard[1, 1])
                    if (TTTBoard[2, 0] != null)
                        coord = "02";

                if (Pool.Contains(coord))
                    return coord;

                if (TTTBoard[1, 1] == TTTBoard[0, 2])
                    if (TTTBoard[1, 1] != null)
                        coord = "20";

                if (Pool.Contains(coord))
                    return coord;

                if ((TTTBoard[0, 2]) == TTTBoard[2, 0])
                    if (TTTBoard[0, 2] != null)
                        coord = "11";

                if (Pool.Contains(coord))
                    return coord;

                // check \ diagonal
                //Console.WriteLine("Checking other Diagonal");
                if (TTTBoard[0, 0] == TTTBoard[1, 1])
                    if (TTTBoard[0, 0] != null)
                        coord = "22";

                if (Pool.Contains(coord))
                    return coord;

                if (TTTBoard[1, 1] == TTTBoard[2, 2])
                    if (TTTBoard[1, 1] != null)
                        coord = "00";

                if (Pool.Contains(coord))
                    return coord;

                if ((TTTBoard[0, 0]) == TTTBoard[2, 2])
                    if (TTTBoard[0, 0] != null)
                        coord = "11";

                if (Pool.Contains(coord))
                    return coord;

                attempts++;

            }
           
            return "11";

        }


        static List<string> validCoordinates(string[,] board)
        {
            Pool.Clear();
            // lists all coordinates and puts it in a pool
            for (int row = 0; row < board.GetLength(0); row++)
                for (int column = 0; column < board.GetLength(1); column++)
                    if (board[row, column] == null)
                        Pool.Add(row.ToString() + column.ToString());

            return Pool;
        }

        static bool isPopulated(string[,] board)
        {
            for (int row = 0; row < board.GetLength(0); row++)
                for (int column = 0; column < board.GetLength(1); column++)
                    if (board[row, column] == null)
                        return false;

            return true;
        }

        static List<string> playerMove(string Player)
        {
            string xcoor = "";
            string ycoor = "";

            // contains all valid moves
            Pool = validCoordinates(TTTBoard);

            if (Player == "Human")
            {
                bool valid = false;
                do
                {
                    Console.Write("Enter X coordinate of your move: \t");
                    xcoor = Console.ReadLine();
                    Console.Write("Enter Y coordinate of your move: \t");
                    ycoor = Console.ReadLine();

                    if (Pool.Contains(xcoor + ycoor))
                    {
                        valid = true;
                        Logs.Add("X: " + xcoor + "," + ycoor);
                    }
                        
                    else
                        display(TTTBoard);


                } while (!valid);

            }

            if (Player == "Computer")
            {

                string coords = AIprayingformydownfall();
                if (Pool.Contains(coords))
                {
                    Console.WriteLine("The coordinates: " + coords[0] + "," + coords[1] + " was calculated by the computer");
                    xcoor = coords[0].ToString();
                    ycoor = coords[1].ToString();
                    Logs.Add("O: " + xcoor + "," + ycoor);
                }
                // No smart play so just pick a random valid coordinate from the pool
                else
                {
                    Console.WriteLine("AI function was not called");
                    int computerMove = rnd.Next(0, Pool.Count);
                    xcoor = Pool[computerMove][0].ToString();
                    ycoor = Pool[computerMove][1].ToString();
                    Logs.Add("O: " + xcoor + "," + ycoor);
                }

            }



            return new List<string> { xcoor, ycoor };
        }

        static string[,] initializeBoard()
        {
            return new string[3, 3];
        }

        static void clearBoard(string[,] Board)
        {
            TTTBoard = new string[3, 3];
        }

        static string changePlayerTurn(string Player)
        {
            if (Player == "Computer")
                return "Human";
            else
                return "Computer";
        }

        static void updateBoard(string xcoor, string ycoor, string player)
        {
            if (TTTBoard[int.Parse(xcoor), int.Parse(ycoor)] == null)
            {
                if (player == "Human")
                    TTTBoard[int.Parse(xcoor), int.Parse(ycoor)] = "X";
                else if (player == "Computer")
                    TTTBoard[int.Parse(xcoor), int.Parse(ycoor)] = "O";
            }
            display(TTTBoard);
        }

        static void display(string[,] board)
        {
            Console.Clear();
            Console.WriteLine("Player Human:X \t Player Computer:O");
            Console.WriteLine("Human Wins:{0} \t Computer Wins:{1}", HumanWins, ComputerWins);
            for (int row = 0; row < board.GetLength(0); row++)
            {
                Console.WriteLine("\n -------------------------------------------------");
                for (int column = 0; column < board.GetLength(1); column++)
                {
                    Console.Write("| \t" + board[column, row] + "\t |");
                }
                Console.WriteLine("\n -------------------------------------------------");
            }

        }
    }
}
