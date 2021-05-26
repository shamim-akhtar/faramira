using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Patterns;
using UnityEngine.SceneManagement;

namespace Trivia
{
    public class QuizManager : MonoBehaviour
    {
        public Sprite mTickSprite;
        public Sprite mCrossSprite;

        public Image[] mResultsImage;

        public class Question
        {
            public Question(string question, string[] options, int indexToCorrectAnswer)
            {
                mQuestion = question;
                for (int i = 0; i < options.Length; ++i)
                {
                    mOptions.Add(options[i]);
                }
                mCorrectIndex = indexToCorrectAnswer;
            }

            public string QuestionStr
            {
                get { return mQuestion; }
            }

            public List<string> OptionsStr
            {
                get { return mOptions; }
            }

            public int CorrectAnswerIndex
            {
                get { return mCorrectIndex; }
            }

            string mQuestion;
            List<string> mOptions = new List<string>();
            int mCorrectIndex;
        }

        //public Button mBtnNext;
        //public Button mBtnExitToMainMenu;
        //public Button mExitYes;
        //public Button mExitNo;

        public Button[] mBtnOptions;
        public Text mTextQuestion;
        public Text[] mTextOptions;
        public Text mTextTimer;
        public Text mExitQuizText;

        public AudioSource mAudioSource;
        public AudioClip mAudioClipNoResponse;
        public AudioClip mAudioClipResponse;

        public GameMenuHandler mGameMenuHandler;

        public int mTimeLimit = 10;
        float mTimerStartTime = 0.0f;
        bool mResponse = false;
        int mResponseIndex = -1;

        private Question mCurrentQuestion;

        FiniteStateMachine mFsm = new FiniteStateMachine();

        Question GetNextQuestion()
        {
            string[] options = { "Option A", "Option B", "Option C", "Option D" };
            Question q = new Question("This is a test question. Select B for correct andwer", options, 1);
            return q;
        }

        // Start is called before the first frame update
        void Start()
        {
            mFsm.Add(
                new GameState(
                    GameState.StateID.NEW_QUESTION,
                    OnEnterNewQuestion)
                );
            mFsm.Add(
                new GameState(
                    GameState.StateID.NO_RESPONSE,
                    OnEnterNoResponse)
                );
            mFsm.Add(
                new GameState(
                    GameState.StateID.RESPONSE,
                    OnEnterResponse)
                );
            mFsm.Add(
                new GameState(
                    GameState.StateID.TIMER,
                    OnEnterTimer,
                    null,
                    OnUpdateTimer)
                );
            mFsm.Add(
                new GameState(
                    GameState.StateID.SHOW_RESULTS,
                    OnEnterShowResults)
                );
            mFsm.Add(
                new GameState(
                    GameState.StateID.SHOW_FEEDBACK,
                    OnEnterShowfeedback)
                );
            mFsm.Add(
                new GameState(
                    GameState.StateID.EXIT,
                    OnEnterExit)
                );
            mFsm.SetCurrentState((int)GameState.StateID.NEW_QUESTION);

            mGameMenuHandler.onClickNextGame += OnNextQuestion;
        }

        // Update is called once per frame
        void Update()
        {
            mFsm.Update();
        }

        public void OnSelectOption()
        {
            string buttonName = EventSystem.current.currentSelectedGameObject.name;
            char lastCharacter = buttonName[buttonName.Length - 1];
            if ( lastCharacter == 'A')
            {
                mResponseIndex = 0;
            }
            else if (lastCharacter == 'B')
            {
                mResponseIndex = 1;
            }
            else if (lastCharacter == 'C')
            {
                mResponseIndex = 2;
            }
            else if (lastCharacter == 'D')
            {
                mResponseIndex = 3;
            }

            for(int i = 0; i < mBtnOptions.Length; ++i)
            {
                mBtnOptions[i].interactable = false;
            }

            mResponse = true;
        }

        public void OnNextQuestion()
        {
            mFsm.SetCurrentState((int)GameState.StateID.NEW_QUESTION);
        }

        #region QUIZ FSM

        IEnumerator Coroutine_ShowQuestion()
        {
            for(int i = 0; i < mBtnOptions.Length; ++i)
            {
                mBtnOptions[i].gameObject.SetActive(false);
                mResultsImage[i].gameObject.SetActive(false);
            }
            mTextQuestion.text = "";

            mTextQuestion.text = mCurrentQuestion.QuestionStr;
            yield return new WaitForSeconds(1.0f);
            for (int i = 0; i < mTextOptions.Length; ++i)
            {
                mBtnOptions[i].gameObject.SetActive(true);
                //mBtnOptions[i].enabled = true;
                mBtnOptions[i].interactable = true;
                mTextOptions[i].text = mCurrentQuestion.OptionsStr[i];
            }
            mFsm.SetCurrentState((int)GameState.StateID.TIMER);
        }
        void OnEnterNewQuestion()
        {
            // set question parameters.
            mCurrentQuestion = GetNextQuestion();
            mResponse = false;
            mResponseIndex = -1;

            mGameMenuHandler.SetActiveBtnHome(false);
            mGameMenuHandler.SetActiveBtnNext(false);

            StartCoroutine(Coroutine_ShowQuestion());
        }

        void OnEnterTimer()
        {
            mTimerStartTime = Time.time;
        }
        void OnUpdateTimer()
        {
            float dt = Time.time - mTimerStartTime;
            mTextTimer.text = ((int)(mTimeLimit - dt + 1)).ToString();

            if (Time.time - mTimerStartTime > mTimeLimit)
            {
                mFsm.SetCurrentState((int)GameState.StateID.NO_RESPONSE);
            }

            if(mResponse)
            {
                mFsm.SetCurrentState((int)GameState.StateID.RESPONSE);
            }
        }

        IEnumerator Coroutine_NoResponse()
        {
            mFsm.SetCurrentState((int)GameState.StateID.SHOW_RESULTS);
            yield return StartCoroutine(AmbientSound.Coroutine_PlayShot(mAudioSource, mAudioClipNoResponse));
        }
        void OnEnterNoResponse()
        {
            StartCoroutine(Coroutine_NoResponse());
        }

        IEnumerator Coroutine_Response()
        {
            if (mResponseIndex == mCurrentQuestion.CorrectAnswerIndex)
            {
                mFsm.SetCurrentState((int)GameState.StateID.SHOW_RESULTS);
                yield return StartCoroutine(AmbientSound.Coroutine_PlayShot(mAudioSource, mAudioClipResponse, 0.50f));
            }
            else
            {
                mFsm.SetCurrentState((int)GameState.StateID.SHOW_RESULTS);
                yield return StartCoroutine(AmbientSound.Coroutine_PlayShot(mAudioSource, mAudioClipNoResponse, 0.75f));
            }
        }
        void OnEnterResponse()
        {
            StartCoroutine(Coroutine_Response());
        }

        IEnumerator Coroutine_ShowResults()
        {
            for (int i = 0; i < 4; ++i)
            {
                if (i == mCurrentQuestion.CorrectAnswerIndex)
                {
                    mResultsImage[i].sprite = mTickSprite;
                }
                else
                {
                    mResultsImage[i].sprite = mCrossSprite;
                }
                mBtnOptions[i].interactable = false;
                mResultsImage[i].gameObject.SetActive(true);

                yield return new WaitForSeconds(0.1f);
            }

            mFsm.SetCurrentState((int)GameState.StateID.SHOW_FEEDBACK);
        }
        void OnEnterShowResults()
        {
            StartCoroutine(Coroutine_ShowResults());
        }

        void OnEnterShowfeedback()
        {
            mGameMenuHandler.SetActiveBtnHome(true);
            mGameMenuHandler.SetActiveBtnNext(true);
        }

        void OnEnterExit()
        {
        }
        #endregion
    }
}