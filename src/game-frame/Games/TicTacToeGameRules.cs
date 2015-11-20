﻿using System.Collections.Generic;
using System.Linq;

namespace GameFrame.Games
{
    public class TicTacToeState : IState
    {
        public const int BoardLength = 9;

        public uint Board { get; set; }
        public int ActivePlayer { get; set; }
        public int LastMove { get; set; }

        public ulong GetStateHash()
        {
            return (ulong)Board;
        }

        public ushort GetHistoryHash()
        {
            return (ushort)LastMove;
        }

        public float GetHeuristicValue()
        {
            return 0.01f;
        }

        public void PreRun() { }
        public void PostRun() { }

        public static TicTacToeState Empty => new TicTacToeState()
        {
            Board = 0U,
            ActivePlayer = 1,
            LastMove = -1,
        };

        public TicTacToeState ApplyMove(int move)
        {
            var successor = new TicTacToeState
            {
                Board = Board,
                ActivePlayer = -ActivePlayer,
                LastMove = move,
            };
            successor.Board |= (ActivePlayer == 1 ? 1u : 2u) << (move * 2);
            return successor;
        }

        public override string ToString()
        {
            var args = GetBoardFormatArgs().Select(m => m == 0 ? " " : (m == 1 ? "W" : "B")).ToArray();
            return string.Format("{0}|{1}|{2}\n-+-+-\n{3}|{4}|{5}\n-+-+-\n{6}|{7}|{8}", args);
        }

        private int[] GetBoardFormatArgs()
        {
            var board = new int[BoardLength];
            for (int i = 0; i < BoardLength; i++)
            {
                board[i] = (int)((Board >> (i * 2)) & 3UL);
            }
            return board;
        }

        public string LastMoveDescription()
        {
            return LastMove.ToString();
        }
    }

    public class TicTacToeGameRules : GameRules<TicTacToeState>
    {
        public override string Name { get; } = "tictactoe";
        public override string[] Roles { get; } = { "X", "O" };

        public override List<TicTacToeState> Expand(TicTacToeState state)
        {
            var successors = new List<TicTacToeState>();
            for (int i = 0; i < TicTacToeState.BoardLength; i++)
            {
                if (((state.Board >> (i * 2)) & 3UL) != 0UL) continue;
                var successor = state.ApplyMove(i);
                successors.Add(successor);
            }
            return successors;
        }

        public override float? DetermineWinner(TicTacToeState state)
        {
            if (state.LastMove == -1) return null;
            var lastPlayerMoves = state.Board >> (state.ActivePlayer == 1 ? 1 : 0);
            foreach (var winner in winners[state.LastMove])
            {
                if ((lastPlayerMoves & winner) == winner)
                {
                    return -1f;
                }
            }
            uint player1Moves = state.Board & 0x15555U;
            uint player2Moves = (state.Board >> 1) & 0x15555U;
            uint empties = (player1Moves | player2Moves) ^ 0x15555U;
            if (empties == 0U) return 0f;
            return null;
        }

        public override int? GetWinningPlayerNumber(TicTacToeState state)
        {
            var winner = DetermineWinner(state);
            if (winner == null) return null;
            if (winner == 0f) return 0;
            return state.ActivePlayer * -1;
        }

        static readonly uint[][] winners = new uint[][]
        {
            new [] { 0x00015U, 0x01041U, 0x10101U },
            new [] { 0x00015U, 0x04104U },
            new [] { 0x00015U, 0x10410U, 0x01110U },
            new [] { 0x01041U, 0x00540U },
            new [] { 0x10101U, 0x04104U, 0x01110U, 0x00540U },
            new [] { 0x10410U, 0x00540U },
            new [] { 0x01041U, 0x01110U, 0x15000U },
            new [] { 0x04104U, 0x15000U },
            new [] { 0x10101U, 0x10410U, 0x15000U },
        };
    }
}
