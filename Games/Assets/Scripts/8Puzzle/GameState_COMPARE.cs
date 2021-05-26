using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_COMPARE : GameState
{
    private int solIndex = 0;
    public GameState_COMPARE(Patterns.FiniteStateMachine fsm, int id, PuzzleBoard puzzle) : base(fsm, id, puzzle)
    {
    }
    public override void Enter()
    {
        _puzzle.buttonHint.gameObject.SetActive(false);
        _puzzle.buttonCancel.gameObject.SetActive(false);
        _puzzle.buttonNext.gameObject.SetActive(false);
        _puzzle.buttonRetry.gameObject.SetActive(false);
        _puzzle.buttonCompare.gameObject.SetActive(false);

        _puzzle.buttonBack.gameObject.SetActive(true);
        _puzzle.buttonNextSol.gameObject.SetActive(true);
        _puzzle.buttonPrevSol.gameObject.SetActive(true);

        _puzzle._puzzleLayout.SetState(_puzzle.yourSolutionStates[0]);
        _puzzle._puzzleLayout2.SetState(_puzzle.astarSolutionStates[0]);


        _puzzle.image_sol_you.SetActive(true);
        _puzzle.image_sol_astar.SetActive(true);
        _puzzle._puzzleSet2.SetActive(true);

        Camera.main.orthographicSize = 1000.0f;
        Camera.main.transform.position = new Vector3(150.0f, -420.0f, -10.0f);
        solIndex = 0;

        Debug.Log("GameState_COMPARE");
    }

    public override void Exit()
    {
        _puzzle.buttonBack.gameObject.SetActive(false);
        _puzzle.buttonNextSol.gameObject.SetActive(false);
        _puzzle.buttonPrevSol.gameObject.SetActive(false);

        _puzzle.image_sol_you.SetActive(false);
        _puzzle.image_sol_astar.SetActive(false);
        _puzzle._puzzleSet2.SetActive(false);

        Camera.main.orthographicSize = 640.0f;
        Camera.main.transform.position = new Vector3(0.0f, -200.0f, -1.0f);
    }

    public override void Update()
    {
        // if we are showing the game exit menu then do nothing.
        if (_puzzle.mGameMenuHandler.mShowingExitPopup) return;

        if (_puzzle.buttonNextSol.Pressed)
        {
            _puzzle.buttonNextSol.Pressed = false;
            solIndex++;
            if (solIndex < _puzzle.yourSolutionStates.Count)
            {
                _puzzle._puzzleLayout.SetState(_puzzle.yourSolutionStates[solIndex]);
            }
            if (solIndex < _puzzle.m_astarSolution.Count)
            {
                _puzzle._puzzleLayout2.SetState(_puzzle.astarSolutionStates[solIndex]);
            }
            if (solIndex >= _puzzle.yourSolutionStates.Count)
                solIndex = _puzzle.yourSolutionStates.Count;
        }
        if (_puzzle.buttonPrevSol.Pressed)
        {
            _puzzle.buttonPrevSol.Pressed = false;
            --solIndex;
            if (solIndex >= 0 && solIndex < _puzzle.yourSolutionStates.Count)
            {
                _puzzle._puzzleLayout.SetState(_puzzle.yourSolutionStates[solIndex]);
            }
            if (solIndex >= 0 && solIndex < _puzzle.m_astarSolution.Count)
            {
                _puzzle._puzzleLayout2.SetState(_puzzle.astarSolutionStates[solIndex]);
            }
            if (solIndex < 0) solIndex = 0;
        }
        if (_puzzle.buttonBack.Pressed)
        {
            _puzzle.buttonBack.Pressed = false;
            m_fsm.SetCurrentState((int)StateID.WIN);
            _puzzle.mGameMenuHandler.SetActiveBtnHome(true);
        }
    }
}