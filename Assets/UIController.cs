using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI endGameTitle;
    [SerializeField] TextMeshProUGUI currentNumber;
    [SerializeField] TextMeshProUGUI remainNumber;
    [SerializeField] GameObject gameOver;
    [SerializeField] GameObject confirm;
    [SerializeField] Slider slider;

    [SerializeField] TextMeshProUGUI score;
    [SerializeField] TextMeshProUGUI highScore;

    [SerializeField] TextMeshProUGUI turn;

    [SerializeField] TextMeshProUGUI[] countShip;
    [SerializeField] TextMeshProUGUI[] countShipBot;


    // Start is called before the first frame update
    void Start()
    {
        gameOver.SetActive(false);
        confirm.SetActive(false);
    }

    public void UpdateTurn(bool isUser)
    {
        if(isUser)
        {
            turn.text = "Your turn!";
        }
        else
        {
            turn.text = "Enemy turn!";
        }
    }

    public void UpdateCurrentNumber(int value)
    {
        currentNumber.text = value.ToString();
    }

    public void UpdateRemainNumber(int value)
    {
        remainNumber.text = value.ToString();
    }

    public void GameOver(bool isWin)
    {
        gameOver.SetActive(true);

        endGameTitle.text = "Game over!!!";
        if (isWin) endGameTitle.text = "You win!!!";
    }

    public void UpdateCountShip(int ship1, int ship2, int ship3, int ship4)
    {
        countShip[0].text = ship1.ToString();
        countShip[1].text = ship2.ToString();
        countShip[2].text = ship3.ToString();
        countShip[3].text = ship4.ToString();
    }

    public void UpdateCountShipBot(int ship1, int ship2, int ship3, int ship4)
    {
        countShipBot[0].text = ship1.ToString();
        countShipBot[1].text = ship2.ToString();
        countShipBot[2].text = ship3.ToString();
        countShipBot[3].text = ship4.ToString();
    }

    public void UpdateScore(int value)
    {
        score.text = FloatToTime(value);
    }

    public void UpdateHighScore(int value)
    {
        highScore.text = FloatToTime(value);
    }

    private string FloatToTime(int value)
    {
        int min = value / 60;
        return min + "m" + ((value - min * 60)) + "s";
    }

    public void ShowConfirm(bool isShow)
    {
        if(isShow)
        {
            Time.timeScale = 0;
            confirm.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            confirm.SetActive(false);
        }
    }

    public void SetSlider(float value)
    {
        slider.maxValue = value;
        slider.value = value;
    }

    public void UpdateSliderValue(float value)
    {
        slider.value = value;
    }
}
