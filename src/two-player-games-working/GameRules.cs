﻿using System.Collections.Generic;

namespace two_player_games_working
{
    public abstract class GameRules<TState> where TState: IState
    {
        public abstract List<TState> Expand(TState state);
        public abstract float? DetermineWinner(TState state);
        public abstract string FirstPlayerName { get; }
        public abstract string SecondPlayerName { get; }
    }
}
