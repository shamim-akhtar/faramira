using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns;

public class GameState : Patterns.State
{
    public enum StateID
    {
        FADEIN,
        WAIT,
        RANDOMIZE,
        PLAYING,
        WIN,
        COMPARE,
        NEXT_PUZZLE_IMAGE,
        SHOW_AD,
        SHOW_HINT,
        CANCEL,
        SHOW_REWARD_AD,
        ASTAR_SOLUTION,
    };
    protected PuzzleBoard _puzzle;
    protected FiniteStateMachine m_fsm;
    public GameState(Patterns.FiniteStateMachine fsm, int id, PuzzleBoard puzzle) : base()
    {
        m_fsm = fsm;
        ID = id;
        _puzzle = puzzle;
    }
    public override void Enter() { }
    public override void Exit() { }
    public override void Update() { }
    public override void FixedUpdate() { }
}

public class GameState_FADEIN : GameState
{
    private float m_duration = 2.0f;
    private float m_deltatIme = 0.0f;
    public GameState_FADEIN(Patterns.FiniteStateMachine fsm, int id, PuzzleBoard puzzle) : base(fsm, id, puzzle)
    {
    }
    public override void Enter()
    {
        //AudioManager.Instance.PlayFadeIn(0, 2, 0.0f, 0.8f, true);
        m_deltatIme = 0.0f;
        Debug.Log("GameState_FADEIN");
    }

    public override void Exit()
    {
        _puzzle.FillerGameObject.SetActive(false);
    }

    public override void Update()
    {
        m_deltatIme += Time.deltaTime;
        if(m_deltatIme <= m_duration)
        {
            float v = m_deltatIme / m_duration;
            Color c = _puzzle.Filler.color;
            c.a = 1.0f - v;
            _puzzle.Filler.color = c;
        }
        else
        {
            m_fsm.SetCurrentState((int)StateID.WAIT);
        }
    }
}

public class GameState_WAIT : GameState
{
    public GameState_WAIT(Patterns.FiniteStateMachine fsm, int id, PuzzleBoard puzzle) : base(fsm, id, puzzle)
    {
    }
    public override void Enter()
    {
        _puzzle.buttonStart.gameObject.SetActive(true);//allow fading.
        _puzzle.yourSolutionStates.Clear();
        _puzzle.textAStarScore.text = "";
        _puzzle.textYourScore.text = "";
        _puzzle.yourScore = 0;
        _puzzle.m_astarSolutionIndex = 0;
        _puzzle.m_astarSolution.Clear();

        _puzzle.textMessage.text = "Click below to randomize or shuffle your tiles.";

        Debug.Log("GameState_WAIT");
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        // if we are showing the game exit menu then do nothing.
        if (_puzzle.mGameMenuHandler.mShowingExitPopup) return;
        if (_puzzle.buttonStart.Pressed)
        {
            m_fsm.SetCurrentState((int)StateID.RANDOMIZE);
            _puzzle.audioSource.PlayOneShot(_puzzle.audioClickTile);
            _puzzle.buttonStart.Pressed = false;
        }
    }
}

public class GameState_WIN : GameState
{
    public GameState_WIN(Patterns.FiniteStateMachine fsm, int id, PuzzleBoard puzzle) : base(fsm, id, puzzle)
    {
    }
    public override void Enter()
    {
        _puzzle.audioSource.PlayOneShot(_puzzle.audioWin);
        _puzzle.buttonHint.gameObject.SetActive(false);
        _puzzle.buttonCancel.gameObject.SetActive(false);

        _puzzle.buttonNext.gameObject.SetActive(true);
        _puzzle.buttonRetry.gameObject.SetActive(true);
        if (_puzzle.m_astarSolved)
        {
            _puzzle.buttonCompare.gameObject.SetActive(true);
            _puzzle.image_message.gameObject.SetActive(true);
        }

        _puzzle._puzzleLayout.SetState(new Puzzle.State(_puzzle.PuzzleRowsOrCols));

        _puzzle.buttonRetry.Pressed = false;

        if (_puzzle.yourTotalScore == 0)
        {
            _puzzle.yourTotalScore = _puzzle.yourScore;
        }
        else
        {
            _puzzle.yourTotalScore += _puzzle.yourScore;
            //_puzzle.yourTotalScore = _puzzle.yourTotalScore / 2;
        }
        _puzzle.totalScore.text = _puzzle.yourTotalScore.ToString();

        Debug.Log("GameState_WIN");
    }

    public override void Exit()
    {
        _puzzle.buttonCompare.gameObject.SetActive(false);

        _puzzle.buttonNext.gameObject.SetActive(false);
        _puzzle.buttonRetry.gameObject.SetActive(false);
        _puzzle.image_message.gameObject.SetActive(false);

        //_puzzle.textMessage.text = "Congratulations. You have solved the puzzle.";
    }

    public override void Update()
    {
        // if we are showing the game exit menu then do nothing.
        if (_puzzle.mGameMenuHandler.mShowingExitPopup) return;
        if (_puzzle.buttonCompare.Pressed)
        {
            if (_puzzle.m_astarSolved)
            {
                _puzzle.buttonCompare.Pressed = false;
                m_fsm.SetCurrentState((int)StateID.COMPARE);
                //_puzzle.mBottomMenu.SetActive(false);
                _puzzle.mGameMenuHandler.mBottomMenu.SetActive(false);
            }
        }
        if (_puzzle.buttonRetry.Pressed)
        {
            _puzzle.buttonRetry.Pressed = false;
            m_fsm.SetCurrentState((int)StateID.RANDOMIZE);
        }
        if (_puzzle.buttonNext.Pressed)
        {
            _puzzle.buttonNext.Pressed = false;
            m_fsm.SetCurrentState((int)StateID.SHOW_AD);
        }
    }
}

public class GameState_SHOW_AD : GameState
{
    public GameState_SHOW_AD(Patterns.FiniteStateMachine fsm, int id, PuzzleBoard puzzle) : base(fsm, id, puzzle)
    {
    }
    public override void Enter()
    {
#if FARAMIRA_USE_ADS
#if UNITY_IPHONE
        _puzzle.ShowSolutionAd();
#endif

#if UNITY_ANDROID
        _puzzle.ShowSolutionAd();
#endif

#if UNITY_WEBGL
#endif
#endif
        Debug.Log("GameState_SHOW_AD");
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
#if FARAMIRA_USE_ADS

#if UNITY_IPHONE
        if (_puzzle.adState == PuzzleBoard.AdRunningState.SUCCESS)
        {
            _puzzle.adState = PuzzleBoard.AdRunningState.NO_AD_STARTED;
            //m_fsm.SetCurrentState((int)StateID.ASTAR_SOLUTION);
            m_fsm.SetCurrentState((int)StateID.NEXT_PUZZLE_IMAGE);
        }
        if (_puzzle.adState == PuzzleBoard.AdRunningState.FAILED)
        {
            _puzzle.adState = PuzzleBoard.AdRunningState.NO_AD_STARTED;
            //m_fsm.SetCurrentState((int)StateID.PLAYING);
            m_fsm.SetCurrentState((int)StateID.NEXT_PUZZLE_IMAGE);
        }
#endif

#if UNITY_ANDROID
        if (_puzzle.adState == PuzzleBoard.AdRunningState.SUCCESS)
        {
            _puzzle.adState = PuzzleBoard.AdRunningState.NO_AD_STARTED;
            //m_fsm.SetCurrentState((int)StateID.ASTAR_SOLUTION);
            m_fsm.SetCurrentState((int)StateID.NEXT_PUZZLE_IMAGE);
        }
        if (_puzzle.adState == PuzzleBoard.AdRunningState.FAILED)
        {
            _puzzle.adState = PuzzleBoard.AdRunningState.NO_AD_STARTED;
            //m_fsm.SetCurrentState((int)StateID.PLAYING);
            m_fsm.SetCurrentState((int)StateID.NEXT_PUZZLE_IMAGE);
        }
#endif
#if UNITY_WEBGL
            m_fsm.SetCurrentState((int)StateID.NEXT_PUZZLE_IMAGE);
#endif

#if UNITY_STANDALONE
        m_fsm.SetCurrentState((int)StateID.NEXT_PUZZLE_IMAGE);
#endif
#else
        m_fsm.SetCurrentState((int)StateID.NEXT_PUZZLE_IMAGE);
#endif
    }
}

public class GameState_NEXT_PUZZLE_IMAGE : GameState
{
    public GameState_NEXT_PUZZLE_IMAGE(Patterns.FiniteStateMachine fsm, int id, PuzzleBoard puzzle) : base(fsm, id, puzzle)
    {
    }
    public override void Enter()
    {
        Debug.Log("GameState_NEXT_PUZZLE_IMAGE");
        _puzzle.LoadPuzzleImage();
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        m_fsm.SetCurrentState((int)StateID.WAIT);
    }
}

public class GameState_SHOW_HINT : GameState
{
    private Puzzle.State _oldState;
    public GameState_SHOW_HINT(Patterns.FiniteStateMachine fsm, int id, PuzzleBoard puzzle) : base(fsm, id, puzzle)
    {
    }
    public override void Enter()
    {
        _oldState = _puzzle._state;
        Puzzle.State state = new Puzzle.State(_puzzle.PuzzleRowsOrCols);
        _puzzle._puzzleLayout.SetState(state);
        //AudioManager.Instance.PlayFadeIn(3, 0.5f, 0.0f, 0.8f, true);
        _puzzle.audioSource.PlayOneShot(_puzzle.audioHint);
        Debug.Log("GameState_SHOW_HINT");
    }

    public override void Exit()
    {
        //AudioManager.Instance.StopFadeOut(3, 0.5f, 0.0f, 0.8f);
        _puzzle._puzzleLayout.SetState(_oldState);
    }

    public override void Update()
    {
        // if we are showing the game exit menu then do nothing.
        if (_puzzle.mGameMenuHandler.mShowingExitPopup) return;
        if (_puzzle.buttonHint.Pressed)
        {
        }
        else
        {
            m_fsm.SetCurrentState((int)StateID.PLAYING);
        }
    }
}

public class GameState_CANCEL : GameState
{
    private Puzzle.State _oldState;
    public GameState_CANCEL(Patterns.FiniteStateMachine fsm, int id, PuzzleBoard puzzle) : base(fsm, id, puzzle)
    {
    }
    public override void Enter()
    {
        _puzzle.audioSource.PlayOneShot(_puzzle.audioCancel);
        Debug.Log("GameState_SHOW_CANCEL");

        // ideally should fade out.
        _puzzle.buttonCancel.gameObject.SetActive(false);
        _puzzle.buttonHint.gameObject.SetActive(false);
        _puzzle.buttonStart.gameObject.SetActive(false);
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        // if we are showing the game exit menu then do nothing.
        if (_puzzle.mGameMenuHandler.mShowingExitPopup) return;
        m_fsm.SetCurrentState((int)StateID.SHOW_REWARD_AD);
    }
}

public class GameState_SHOW_REWARD_AD : GameState
{
    public GameState_SHOW_REWARD_AD(Patterns.FiniteStateMachine fsm, int id, PuzzleBoard puzzle) : base(fsm, id, puzzle)
    {
    }
    public override void Enter()
    {
#if FARAMIRA_USE_ADS

#if (UNITY_ANDROID) || (UNITY_IOS)
        _puzzle.ShowSolutionAd();
#endif
#endif
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
#if FARAMIRA_USE_ADS

#if (UNITY_ANDROID) || (UNITY_IOS)
        if(_puzzle.adState == PuzzleBoard.AdRunningState.SUCCESS)
        {
            _puzzle.adState = PuzzleBoard.AdRunningState.NO_AD_STARTED;
            m_fsm.SetCurrentState((int)StateID.ASTAR_SOLUTION);
        }
        if(_puzzle.adState == PuzzleBoard.AdRunningState.FAILED)
        {
            _puzzle.adState = PuzzleBoard.AdRunningState.NO_AD_STARTED;
            m_fsm.SetCurrentState((int)StateID.PLAYING);
        }
#else
        m_fsm.SetCurrentState((int)StateID.ASTAR_SOLUTION);
#endif
#else
        m_fsm.SetCurrentState((int)StateID.ASTAR_SOLUTION);
#endif
    }
}

public class GameState_ASTAR_SOLUTION : GameState
{
    private Puzzle.State _oldState;
    private int m_index = 0;

    public GameState_ASTAR_SOLUTION(Patterns.FiniteStateMachine fsm, int id, PuzzleBoard puzzle) : base(fsm, id, puzzle)
    {
    }
    public override void Enter()
    {
        Debug.Log("GameState_ASTAR_SOLUTION");

        // ideally should fade out.
        _puzzle.buttonCancel.gameObject.SetActive(false);
        _puzzle.buttonHint.gameObject.SetActive(false);
        _puzzle.buttonStart.gameObject.SetActive(false);

        _puzzle.buttonNextAStar.gameObject.SetActive(true);
        _puzzle.buttonPrevAStar.gameObject.SetActive(true);
        _puzzle.buttonBack.gameObject.SetActive(true);

        _oldState = _puzzle._state;

        // set the initial state to the puzzle layout.
        if (_puzzle.m_astarSolved)
        {
            _puzzle._puzzleLayout.SetState(_puzzle.m_astarSolution[_puzzle.m_astarSolutionIndex].State);
        }
        m_index = 0;
    }

    public override void Exit()
    {
        _puzzle._state = _oldState;
        _puzzle._puzzleLayout.SetState(_oldState);

        _puzzle.buttonCancel.gameObject.SetActive(true);
        _puzzle.buttonHint.gameObject.SetActive(true);
        
        _puzzle.buttonNextAStar.gameObject.SetActive(false);
        _puzzle.buttonPrevAStar.gameObject.SetActive(false);
        _puzzle.buttonBack.gameObject.SetActive(false);
    }

    public override void Update()
    {
        // if we are showing the game exit menu then do nothing.
        if (_puzzle.mGameMenuHandler.mShowingExitPopup) return;
        if (_puzzle.buttonBack.Pressed)
        {
            _puzzle.buttonBack.Pressed = false;
            m_fsm.SetCurrentState((int)StateID.PLAYING);
        }
        if (_puzzle.buttonNextAStar.Pressed)
        {
            _puzzle.buttonNextAStar.Pressed = false;
            if (_puzzle.m_astarSolved)
            {
                m_index++;
                if (m_index >= 0 && m_index < _puzzle.astarSolutionStates.Count)
                {
                    _puzzle._puzzleLayout.SetState(_puzzle.astarSolutionStates[m_index]);
                }
            }
        }
        if (_puzzle.buttonPrevAStar.Pressed)
        {
            _puzzle.buttonPrevAStar.Pressed = false;
            if (_puzzle.m_astarSolved)
            {
                m_index--;
                if (m_index >= 0 && m_index < _puzzle.astarSolutionStates.Count)
                {
                    _puzzle._puzzleLayout.SetState(_puzzle.astarSolutionStates[m_index]);
                }
            }
        }
    }
}