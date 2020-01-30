
using System;
using System.Data;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace TicTacToe
{
    class Program
    {
        // State Enumeration - x, o, blank
        public enum State
        {
            Undecided,
            X,
            O
        };


        // Position Class -  with row and column properties - Read only values of rows and columns
        public class Position
        {
            public int Row { get; }
            public int Column { get; }

            public Position(int row, int column)
            {
                Row = row;
                Column = column;
            }
        }


        // Board Class - 3x3 array of state values - 
        public class Board
        {
            private State[,] state;
            public State NextTurn { get; private set; }

            public Board()
            {
                state = new State[3, 3];
                NextTurn = State.X;
            }

            public State GetState(Position position)
            {
                return state[position.Row, position.Column];
            }

            public bool SetState(Position position, State newState)
            {
                if (newState != NextTurn) return false;
                if (state[position.Row, position.Column] != State.Undecided) return false;

                state[position.Row, position.Column] = newState;
                SwitchNextTurn();
                return true;
            }

            private void SwitchNextTurn()
            {
                if (NextTurn == State.X) NextTurn = State.O;
                else NextTurn = State.X;
            }
        }

        // Winchecker Class - Analyzes board and determines if someone won
        public class WinChecker
        {
            public State Check(Board board)
            {
                if (CheckForWin(board, State.X)) return State.X;
                if (CheckForWin(board, State.O)) return State.O;
                return State.Undecided;
            }

            private bool CheckForWin(Board board, State player)
            {
                for (int row = 0; row < 3; row++)
                    if (AreAll(board, new Position[] {
                        new Position(row, 0),
                        new Position(row, 1),
                        new Position(row, 2) }, player))
                        return true;

                for (int column = 0; column < 3; column++)
                    if (AreAll(board, new Position[] {
                        new Position(0, column),
                        new Position(1, column),
                        new Position(2, column) }, player))
                        return true;

                if (AreAll(board, new Position[] {
                    new Position(0, 0),
                    new Position(1, 1),
                    new Position(2, 2) }, player))
                    return true;

                if (AreAll(board, new Position[] {
                    new Position(2, 0),
                    new Position(1, 1),
                    new Position(0, 2) }, player))
                    return true;

                return false;
            }

            private bool AreAll(Board board, Position[] positions, State state)
            {
                foreach (Position position in positions)
                    if (board.GetState(position) != state) return false;

                return true;
            }



            public bool IsDraw(Board board)
            {
                for (int row = 0; row < 3; row++)
                for (int column = 0; column < 3; column++)
                    if (board.GetState(new Position(row, column)) == State.Undecided) return false;

                return true;
            }
        }


        // Renderer Class - drawing the game's current state
        public class Renderer
        {
            public void Render(Board board)
            {
                char[,] symbols = new char[3, 3];
                for (int row = 0; row < 3; row++)
                for (int column = 0; column < 3; column++)
                    symbols[row, column] = SymbolFor(board.GetState(new Position(row, column)));

                Console.WriteLine($" {symbols[0, 0]} | {symbols[0, 1]} | {symbols[0, 2]} ");
                Console.WriteLine("---+---+---");
                Console.WriteLine($" {symbols[1, 0]} | {symbols[1, 1]} | {symbols[1, 2]} ");
                Console.WriteLine("---+---+---");
                Console.WriteLine($" {symbols[2, 0]} | {symbols[2, 1]} | {symbols[2, 2]} ");
            }

            private char SymbolFor(State state)
            {
                switch (state)
                {
                    case State.O: return 'O';
                    case State.X: return 'X';
                    default: return ' ';
                }
            }

            public void RenderResults(State winner)
            {
                switch (winner)
                {
                    case State.O:
                    case State.X:
                        Console.WriteLine(SymbolFor(winner) + " Wins!");
                        break;
                    case State.Undecided:
                        Console.WriteLine("Draw!");
                        break;
                }
            }
        }


        // Player Class - Input from player into a board position 
        public class Player
        {
            public Position GetPosition(Board board)
            {
                int position = Convert.ToInt32(Console.ReadLine());
                Position desiredCoordinate = PositionForNumber(position);
                return desiredCoordinate;
            }

            private Position PositionForNumber(int position)
            {
                switch (position)
                {
                    case 1: return new Position(2, 0); // Bottom Left
                    case 2: return new Position(2, 1); // Bottom Middle 
                    case 3: return new Position(2, 2); // Bottom Right
                    case 4: return new Position(1, 0); // Middle Left
                    case 5: return new Position(1, 1); // Middle Middle
                    case 6: return new Position(1, 2); // Middle Right
                    case 7: return new Position(0, 0); // Top Left
                    case 8: return new Position(0, 1); // Top Middle
                    case 9: return new Position(0, 2); // Top Right
                    default: return null;
                }
            }
        }


        // Driver or Main Class
        static void Main(string[] args)
        {
            Board board = new Board();
            WinChecker winChecker = new WinChecker();
            Renderer renderer = new Renderer();
            Player player1 = new Player();
            Player player2 = new Player();

            while (!winChecker.IsDraw(board) && winChecker.Check(board) == State.Undecided)
            {
                renderer.Render(board);

                Position nextMove;
                if (board.NextTurn == State.X)
                    nextMove = player1.GetPosition(board);
                else
                    nextMove = player2.GetPosition(board);

                if (!board.SetState(nextMove, board.NextTurn))
                    Console.WriteLine("That is not a legal move.");
            }

            renderer.Render(board);
            renderer.RenderResults(winChecker.Check(board));

            Console.ReadKey();
        }
    }
}

