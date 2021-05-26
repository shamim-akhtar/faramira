using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_RANDOMIZE : GameState
{
    private int _index = 0;
    private int _depth;
    public GameState_RANDOMIZE(Patterns.FiniteStateMachine fsm, int id, PuzzleBoard puzzle) : base(fsm, id, puzzle)
    {
    }
    public override void Enter()
    {
        _puzzle._state = new Puzzle.State(_puzzle.PuzzleRowsOrCols);
        _puzzle._goal = new Puzzle.State(_puzzle.PuzzleRowsOrCols);
        _puzzle._puzzleLayout.SetState(_puzzle._state);
        _depth = Random.Range(100, 150);
        _index = 0;
        Debug.Log("GameState_RANDOMIZE - Depth " + _depth);
        _puzzle.audioSource.PlayOneShot(_puzzle.audioRandomize);
        _puzzle.buttonRetry.gameObject.SetActive(false);

        //_puzzle.mBottomMenu.SetActive(false);
    }

    public override void Exit()
    {
        // randomization is complete. 
        // we will now use astar to do a search in the background.
        _puzzle.m_astarSolved = false;

        _puzzle.StartAStarSearchInBackground(new Puzzle.State(_puzzle._state));
        _puzzle.yourSolutionStates.Clear();
        _puzzle.yourSolutionStates.Add(new Puzzle.State(_puzzle._state));
        _puzzle.textAStarScore.text = "";
        _puzzle.textYourScore.text = "";
        _puzzle.yourScore = 0;
    }

    public override void Update()
    {
        // if we are showing the game exit menu then do nothing.
        if (_puzzle.mGameMenuHandler.mShowingExitPopup) return;
        if (_index < _depth)
        {
            int zero = _puzzle._state.FindEmptyTileIndex();
            int[] neighbours = Puzzle.Neighbours.Instance.GetNeighbors(zero);

            // get a random neignbour.
            int i = Random.Range(0, neighbours.Length);
            _puzzle.SwapTiles(neighbours[i], _puzzle._state, false);
            _index++;
        }
        else
        {
            // go to play state.
            m_fsm.SetCurrentState((int)StateID.PLAYING);
        }
    }
}