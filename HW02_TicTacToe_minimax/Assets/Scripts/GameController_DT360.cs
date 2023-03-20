using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController_DT360 : MonoBehaviour
{

    public int whosTurn;
    public int turnCount;
    public int[] markedSpaces;
    public Button[] tictactoeSpaces;

    public GameObject[] turnIcons;
    public Sprite[] playIcons;
    public Text WinnerText;
    public GameObject[] winningLines;
    public GameObject WinnerPannel;
    public int xPlayersScore;
    public int oPlayersScore;
    public Text xPlaterScoreText;
    public Text oPlayerScoreText;
    public Button xPlayerButton;
    public Button oPlayerButton;

    // NEW:
    public enum Strategy
    {
        None,
        FirstAvailable,
        Random,
        MiniMax
    }
    public Strategy ai_strategy;
    const int EMPTY_SPACE = -100;

    // TODO: 
    // Change ai_strategy to Strategy.MiniMax on the line 92 in Start() 
    /***************************************/
    /*****  IMPLEMENT YOUR CODE HERE  ******/
    /***************************************/
    int pickMiniMax()
    {
        int bestScore = int.MinValue;
        int pick = -1;

        for (int i = 0; i < 9; i++)
        {
            if (markedSpaces[i] == EMPTY_SPACE)
            {
                markedSpaces[i] = whosTurn + 1;
                int score = MiniMax(false, whosTurn);
                markedSpaces[i] = EMPTY_SPACE;

                if (score > bestScore)
                {
                    bestScore = score;
                    pick = i;
                }
            }
        }

        return pick;
    }
    int MiniMax(bool isMaximizing, int currentPlayer)
    {
        int result = CheckWinning(currentPlayer);
        if (result != 0)
        {
            return result * (turnCount + 1);
        }
        else if (turnCount == 8)
        {
            return 0;
        }

        int bestScore = isMaximizing ? int.MinValue : int.MaxValue;

        for (int i = 0; i < 9; i++)
        {
            if (markedSpaces[i] == EMPTY_SPACE)
            {
                markedSpaces[i] = currentPlayer + 1;

                int score = MiniMax(!isMaximizing, 1 - currentPlayer);
                markedSpaces[i] = EMPTY_SPACE;

                if (isMaximizing)
                {
                    bestScore = Mathf.Max(bestScore, score);
                }
                else
                {
                    bestScore = Mathf.Min(bestScore, score);
                }
            }
        }

        return bestScore;
    }

    int CheckWinning(int currentPlayer)
    {
        int[,] winningLines = new int[8, 3] {
        {0, 1, 2},
        {3, 4, 5},
        {6, 7, 8},
        {0, 3, 6},
        {1, 4, 7},
        {2, 5, 8},
        {0, 4, 8},
        {2, 4, 6}
    };

        for (int i = 0; i < 8; i++)
        {
            int a = winningLines[i, 0];
            int b = winningLines[i, 1];
            int c = winningLines[i, 2];

            if (markedSpaces[a] == currentPlayer + 1 && markedSpaces[b] == currentPlayer + 1 && markedSpaces[c] == currentPlayer + 1)
            {
                return currentPlayer == whosTurn ? 1 : -1;
            }
        }

        return 0;
    }


    /***************************************/
    /***** END OF YOUR IMPLEMENTATION ******/
    /***************************************/
    // TODO: END


    // NEW:
    // Simple strategy to pick the first available spot.
    int pickFirstAvailableSpot()
    {
        for(int i=0; i<9; i++)
        {
            if (markedSpaces[i] == EMPTY_SPACE)
            {
                return i;
            }
        }

        return -1;
    }

    // NEW:
    // Simple strategy to randomly pick the available spot.
    int pickRandomSpot()
    {
        int spot = -1;
        var emptySpots = new List<int>();

        // create a list of empty spots
        for (int i=0; i<9; i++)
        {
            if (markedSpaces[i] == EMPTY_SPACE)
            {
                emptySpots.Add(i);
            }
        }

        // randomly pick an empty spot
        int index = UnityEngine.Random.Range(0, emptySpots.Count);
        spot = emptySpots[index];
        return spot;
    }

    // Start is called before the first frame update
    void Start()
    {
        ai_strategy = Strategy.MiniMax;
        GameSetup();
    }

    void GameSetup() 
    {
        turnCount = 0;
        SetPlayerTurn(0);

        for (int i = 0; i < tictactoeSpaces.Length; i++) 
        {
            markedSpaces[i] = EMPTY_SPACE;

            tictactoeSpaces[i].interactable = true;
            tictactoeSpaces[i].GetComponent<Image>().sprite = null;
        }
    }

    public void TicTacToeButton(int whichnumber) 
    {
        xPlayerButton.interactable = false;
        oPlayerButton.interactable = false;
        tictactoeSpaces[whichnumber].image.sprite = playIcons[whosTurn];
        tictactoeSpaces[whichnumber].interactable = false;

        markedSpaces[whichnumber] = whosTurn + 1;   // assign X = 1 & O = 2 for calculation of the winner in Winnercheck()
        turnCount++;

        if (turnCount >= 4) {
            bool iswinner =  Winnercheck();

            if (turnCount == 9 && iswinner==false) {
                winnerDisplay(-1, whosTurn);  // tie
            }
        }
    
        SetPlayerTurn(whosTurn == 0 ? 1 : 0);
   
        // NEW
        // Player 1 picks the spot according to its strategy
        if (turnCount < 9 && whosTurn == 1)
        {
            int spot = -1;
            switch (ai_strategy)
            {
                case Strategy.None:             // manual play
                    break;

                case Strategy.FirstAvailable:   // pick first available spot
                    spot = pickFirstAvailableSpot();
                    TicTacToeButton(spot);
                    break;

                case Strategy.Random:           // pick random spot
                    spot = pickRandomSpot();
                    TicTacToeButton(spot);
                    break;

                case Strategy.MiniMax:          // compute from MiniMax algorithm
                    spot = pickMiniMax();
                    TicTacToeButton(spot);
                    break;
            }
        }
    }

    bool Winnercheck() 
    {
        int s1 = markedSpaces[0] + markedSpaces[1] + markedSpaces[2];
        int s2 = markedSpaces[3] + markedSpaces[4] + markedSpaces[5];
        int s3 = markedSpaces[6] + markedSpaces[7] + markedSpaces[8];
        int s4 = markedSpaces[0] + markedSpaces[3] + markedSpaces[6];
        int s5 = markedSpaces[1] + markedSpaces[4] + markedSpaces[7];
        int s6 = markedSpaces[2] + markedSpaces[5] + markedSpaces[8];
        int s7 = markedSpaces[0] + markedSpaces[4] + markedSpaces[8];
        int s8 = markedSpaces[2] + markedSpaces[4] + markedSpaces[6];
        var Solutions = new int[] { s1, s2, s3, s4, s5, s6, s7, s8 };
        for (int i = 0; i < Solutions.Length; i++)
        {
            if (Solutions[i] == 3 *(whosTurn + 1))
            {
                winnerDisplay(i, whosTurn);
                return true;
            }
            
        }
        return false;
    }


    void winnerDisplay(int index, int who) 
    {
        if (index < 0)
        {
            WinnerText.text = "It's a draw!!";
            WinnerText.fontSize = 185;
        }
        else
        {
            if (who == 0)
            {
                WinnerText.text = "Player X Wins!";
                WinnerText.fontSize = 150;
                xPlayersScore++;
                xPlaterScoreText.text = xPlayersScore.ToString();
            }
            else
            {
                WinnerText.text = "Player O Wins!";
                WinnerText.fontSize = 150;
                oPlayersScore++;
                oPlayerScoreText.text = oPlayersScore.ToString();
            }

            winningLines[index].SetActive(true);
        }

        WinnerPannel.SetActive(true);
    }

    public void ReStart() 
    {
        GameSetup();

        xPlayerButton.interactable = true;
        oPlayerButton.interactable = true;
        WinnerPannel.SetActive(false);
        for (int i = 0; i < winningLines.Length; i++)
        {
            winningLines[i].SetActive(false);
        }
    }

    public void SetPlayerTurn(int player)
    {
        whosTurn = player;

        switch (player)
        {
            case 0:
                turnIcons[0].SetActive(true);
                turnIcons[1].SetActive(false);
                break;
            case 1:
                turnIcons[0].SetActive(false);
                turnIcons[1].SetActive(true);
                break;
        }
    }
}
