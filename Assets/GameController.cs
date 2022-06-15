using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this.gameObject)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public int firstChoosingRowIndex;
    public int firstChoosingColIndex;

    [SerializeField] ContentController contentController;
    [SerializeField] ContentController contentControllerBot;

    [SerializeField] int[] list = {1,1,2,2,1,2};
    [SerializeField] int[] listControl = {1,1,2,2,1,2};
    [SerializeField] List<int> listId = new List<int>();

    private int row = 6;
    private int col = 6;
    [SerializeField] int[] rowLevel = {8, 10, 12};
    [SerializeField] int[] colLevel = {6, 8, 10 };

    [SerializeField] float[] timeLevel = { 200, 250, 300 };
    [SerializeField] float time = 200;
    [SerializeField] float timeOfGame = 200;

    public int remainingNumber;
    public int remainingNumberBot;
    private int currentNumber;

    private int shuffeRemaining;

    private float timeNotice = 0;

    private int score;
    private float highScore;

    [SerializeField] int[] posOfShip;
    [SerializeField] int[] posOfUserChoose;
    private List<int> shipList1 = new List<int>();
    private List<int> shipList2 = new List<int>();
    private List<int> shipList3 = new List<int>();
    private List<int> shipList4 = new List<int>();

    [SerializeField] int[] posOfShip_2;
    [SerializeField] int[] posOfUserChoose_2;
    private List<int> shipList1_2 = new List<int>();
    private List<int> shipList2_2 = new List<int>();
    private List<int> shipList3_2 = new List<int>();
    private List<int> shipList4_2 = new List<int>();

    private int numberOfTry;


    // Start is called before the first frame update
    void Start()
    {
        ListMaker();
        StartGame();
    }

    public void Notice()
    {
        for(int i = 0; i < list.Length; i++)
        {
            int rowT = i / col;
            int colT = i - (rowT * col);
            if(list[i] == currentNumber)
            {
                contentController.Notice(rowT, colT);
            }
        }
    }

    public void UpdateRemainNumber()
    {
        GetComponent<UIController>().UpdateRemainNumber(remainingNumber);
    }

    public void StartGame()
    {
        if (PlayerPrefs.GetInt("level") < 1)
            PlayerPrefs.SetInt("level", 1);

        score = 0;
        GetComponent<UIController>().UpdateScore(score);
        highScore = PlayerPrefs.GetInt("score");
        GetComponent<UIController>().UpdateHighScore((int)highScore);

        row = rowLevel[PlayerPrefs.GetInt("level")-1];
        col = colLevel[PlayerPrefs.GetInt("level")-1];
        timeOfGame = timeLevel[PlayerPrefs.GetInt("level") - 1];

        posOfShip = new int[row * col];
        posOfShip_2 = new int[row * col];
        posOfUserChoose = new int[row * col];
        posOfUserChoose_2 = new int[row * col];
        contentController.SpawItems(col, row, currentNumber);
        contentControllerBot.SpawItems(col, row, currentNumber);
        SpawShip();

        remainingNumber = shipList1.Count + shipList2.Count*2 + shipList3.Count*3 + shipList4.Count*4;
        remainingNumberBot = remainingNumber;
       // numberOfTry = remainingNumber + Random.Range(5, 8);
        //UpdateRemainingShuffe();
       // UpdateRemainNumber();

        //ShowAllShips();
        //ShowAllShips2();
        GetComponent<UIController>().UpdateCountShip(shipList1.Count, shipList2.Count, shipList3.Count, shipList4.Count);
        GetComponent<UIController>().UpdateCountShipBot(shipList1_2.Count, shipList2_2.Count, shipList3_2.Count, shipList4_2.Count);
        time = timeOfGame;
        //SetSliderValue(time);
        currentNumber = Random.Range(0,10);
        StartNewTurn();
        Time.timeScale = 1;
    }

    public void StartNewTurn()
    {
        timeNotice = 0;
        time = timeOfGame;
        currentTurn = 0;
        GetComponent<UIController>().UpdateTurn(true);
        //currentNumber = Random.Range(0,100);
        //GetComponent<UIController>().UpdateCurrentNumber(currentNumber);
        //contentController.UpdateItems(row, col, currentNumber);
    }

    IEnumerator HandleTurn()
    {
        yield return new WaitForSeconds(0.5f);
        currentTurn++;
        GetComponent<UIController>().UpdateTurn(currentTurn % 2 == 0);

        if(currentTurn % 2 == 0)
        {
            // User turn
        }
        else
        {
            // Enemy turn
            Bot();
        }
    }

    private int foundIndex = -1;
    private int foundShip = 0;
    public void Bot()
    {
        List<int> list = new List<int>();
        for(int i = 0; i < posOfUserChoose_2.Length; i++)
        {
            if (posOfUserChoose_2[i] == 0)
            {
                list.Add(i);
            }
        }

        int row, col, random = -1;
        if(foundIndex != -1)
        {
            random = BotFoundIndex(foundIndex);
        }

        if (random != -1 && posOfUserChoose_2[random] == 0)
        {
            row = random / this.col;
            col = random - row * this.col;
            foundShip--;
        }
        else
        {
            int randomIndex = Random.Range(0, list.Count);
            int valueOfIndex = list[randomIndex];

            row = valueOfIndex / this.col;
            col = valueOfIndex - row * this.col;
        }

        BotChooseItem(row, col);
        StartCoroutine(HandleTurn());
    }

    private int BotFoundIndex(int indexCheck)
    {
        int currentCol = indexCheck % this.row;
        int result = -1;
        if(currentCol==0)
        {
            //checkright
            result = CheckRight(indexCheck);
        }
        else if(currentCol == this.col - 1)
        {
            //checkleft
            result = CheckLeft(indexCheck);
        }
        else
        {
            //checkrandom
            int random = Random.Range(0, 2);
            if(random == 1)
            {
                result = CheckLeft(indexCheck);
                if(result == -1) 
                    result = CheckRight(indexCheck);
            }
            else
            {
                result = CheckRight(indexCheck);
                if (result == -1)
                    result = CheckLeft(indexCheck);
            }
        }

        return result;
    }

    private int CheckLeft(int index)
    {
        for (int i = index; i >= 0; i--)
        {
            if (posOfUserChoose_2[i] == 0)
            {
                return i;
            }
        }

        return -1;
    }

    private int CheckRight(int index)
    {
        for (int i = index; i < posOfUserChoose_2.Length; i++)
        {
            if (posOfUserChoose_2[i] == 0)
            {
                return i;
            }
        }

        return -1;
    }

    public void UpdateScore()
    {
        GetComponent<UIController>().UpdateScore((int)time);
    }

    private int currentTurn = 0;


    public void BotChooseItem(int row, int col)
    {
        contentControllerBot.ShowImage(row, col);

        int check = row * this.col + col;
        posOfUserChoose_2[check] = -1;
        if (posOfShip_2[check] != 0)
        {
            contentControllerBot.ShowFire(row, col);
        }
        if(foundShip <= 0)
            foundIndex = -1;

        if (posOfShip_2[check] == 1)
        {
            posOfUserChoose_2[check] = 1;
            contentControllerBot.ShowShip(row, col, 0);
            remainingNumberBot--;
            //UpdateRemainNumber();
            shipList1_2.RemoveAt(0);
            UpdateRemainingShip();
            foundShip = 0;
            foundIndex = -1;
        }
        else if (posOfShip_2[check] == 2)
        {
            foundIndex = check;
            posOfUserChoose_2[check] = 2;
            int index = BotCheckShip2(check);
            if (index != -1)
            {
                int rowT = index / this.col;
                int colT = index - rowT * this.col;
                contentControllerBot.ShowShip(rowT, colT, 1);
                remainingNumberBot -= 2;
                //UpdateRemainNumber();

                shipList2_2.RemoveAt(0);
                UpdateRemainingShip();
                foundShip = 0;
                foundIndex = -1;
            }
            if (foundShip == 0) foundShip = 2;
        }
        else if (posOfShip_2[check] == 3)
        {
            foundIndex = check;
            posOfUserChoose_2[check] = 3;
            int index = BotCheckShip3(check);
            if (index != -1)
            {
                int rowT = index / this.col;
                int colT = index - rowT * this.col;
                contentControllerBot.ShowShip(rowT, colT, 2);
                remainingNumberBot -= 3;
                //UpdateRemainNumber();

                shipList3_2.RemoveAt(0);
                UpdateRemainingShip();
                foundShip= 0;
                foundIndex = -1;
            }
            if (foundShip == 0) foundShip = 3;
        }
        else if (posOfShip_2[check] == 4)
        {
            foundIndex = check;
            posOfUserChoose_2[check] = 4;
            int index = BotCheckShip4(check);
            if (index != -1)
            {
                int rowT = index / this.col;
                int colT = index - rowT * this.col;
                contentControllerBot.ShowShip(rowT, colT, 3);
                remainingNumberBot -= 4;
                //UpdateRemainNumber();

                shipList4_2.RemoveAt(0);
                UpdateRemainingShip();
                foundShip = 0;
                foundIndex = -1;
            }
            if (foundShip == 0) foundShip = 4;
        }

        if (remainingNumberBot <= 0)
        {
            GameOver(false);
        }
    }

    public void UserChooseItem(int row, int col, bool isUser)
    {
        if (currentTurn % 2 == 1) return;
        if (!isUser) return;
        //numberOfTry--;
        //UpdateRemainingShuffe();
        int check = row * this.col + col;
        if (posOfShip[check] != 0)
        {
            //numberOfTry++;
            //UpdateRemainingShuffe();
            contentController.ShowFire(row, col);
        }

        if (posOfShip[check] == 1)
        {
            posOfUserChoose[check] = 1;
            contentController.ShowShip(row, col, 0);
            remainingNumber--;
            //UpdateRemainNumber();

            shipList1.RemoveAt(0);
            UpdateRemainingShip();
        }
        else if (posOfShip[check] == 2)
        {
            posOfUserChoose[check] = 2;
            int index = UserCheckShip2(check);
            if (index != -1)
            {
                int rowT = index / this.col;
                int colT = index - rowT*this.col;
                contentController.ShowShip(rowT, colT, 1);
                remainingNumber-=2;
                //UpdateRemainNumber();

                shipList2.RemoveAt(0);
                UpdateRemainingShip();
            }
        }
        else if (posOfShip[check] == 3)
        {
            posOfUserChoose[check] = 3;
            int index = UserCheckShip3(check);
            if (index != -1)
            {
                int rowT = index / this.col;
                int colT = index - rowT * this.col;
                contentController.ShowShip(rowT, colT, 2);
                remainingNumber-=3;
                //UpdateRemainNumber();

                shipList3.RemoveAt(0);
                UpdateRemainingShip();
            }
        }
        else if (posOfShip[check] == 4)
        {
            posOfUserChoose[check] = 4;
            int index = UserCheckShip4(check);
            if (index != -1)
            {
                int rowT = index / this.col;
                int colT = index - rowT * this.col;
                contentController.ShowShip(rowT, colT, 3);
                remainingNumber-=4;
                //UpdateRemainNumber();

                shipList4.RemoveAt(0);
                UpdateRemainingShip();
            }
        }

        StartCoroutine(HandleTurn());

        if (remainingNumber <= 0)
        {
            GameOver(true);
        }
    }

    private int UserCheckShip2(int index)
    {
        int row = index/this.col;
        int col = index - row*this.col;
        if(col==0)
        {
            if (posOfUserChoose[index] == 2 && posOfUserChoose[index+1] == 2)
                return (index + 1);
        }
        else if(col== this.col-1)
        {
            if (posOfUserChoose[index] == 2 && posOfUserChoose[index - 1] == 2)
                return index;
        }
        else
        {
            if (posOfUserChoose[index] == 2 && posOfUserChoose[index + 1] == 2)
                return index + 1;
            else if (posOfUserChoose[index] == 2 && posOfUserChoose[index - 1] == 2)
                return index;
        }

        return -1;
    }

    private int UserCheckShip3(int index)
    {
        int row = index / this.col;
        int col = index - row * this.col;

        int current = 0;
        int result = -1;
        bool checkResult = false;
        for(int i = 0; i< this.col; i++)
        {
            int check = row * this.col + i;
            if (posOfUserChoose[check] == 3)
            {
                current++;
                if (current == 3) result = check;
                if(check == index)
                {
                    checkResult = true;
                }
            }
        }

        if(checkResult && current == 3)
            return result;

        return -1;
    }

    private int UserCheckShip4(int index)
    {
        int row = index / this.col;
        int col = index - row * this.col;

        int current = 0;
        bool checkResult = false;
        int result = -1;
        for (int i = 0; i < this.col; i++)
        {
            int check = row * this.col + i;
            if (posOfUserChoose[check] == 4)
            {
                current++;
                if(current == 4) result = check;
                if (check == index)
                {
                    checkResult = true;
                }
            }
        }

        if (checkResult && current == 4)
            return result;

        return -1;
    }

    private int BotCheckShip2(int index)
    {
        int row = index / this.col;
        int col = index - row * this.col;
        if (col == 0)
        {
            if (posOfUserChoose_2[index] == 2 && posOfUserChoose_2[index + 1] == 2)
                return (index + 1);
        }
        else if (col == this.col - 1)
        {
            if (posOfUserChoose_2[index] == 2 && posOfUserChoose_2[index - 1] == 2)
                return index;
        }
        else
        {
            if (posOfUserChoose_2[index] == 2 && posOfUserChoose_2[index + 1] == 2)
                return index + 1;
            else if (posOfUserChoose_2[index] == 2 && posOfUserChoose_2[index - 1] == 2)
                return index;
        }

        return -1;
    }

    private int BotCheckShip3(int index)
    {
        int row = index / this.col;
        int col = index - row * this.col;

        int current = 0;
        int result = -1;
        bool checkResult = false;
        for (int i = 0; i < this.col; i++)
        {
            int check = row * this.col + i;
            if (posOfUserChoose_2[check] == 3)
            {
                current++;
                if (current == 3) result = check;
                if (check == index)
                {
                    checkResult = true;
                }
            }
        }

        if (checkResult && current == 3)
            return result;

        return -1;
    }

    private int BotCheckShip4(int index)
    {
        int row = index / this.col;
        int col = index - row * this.col;

        int current = 0;
        bool checkResult = false;
        int result = -1;
        for (int i = 0; i < this.col; i++)
        {
            int check = row * this.col + i;
            if (posOfUserChoose_2[check] == 4)
            {
                current++;
                if (current == 4) result = check;
                if (check == index)
                {
                    checkResult = true;
                }
            }
        }

        if (checkResult && current == 4)
            return result;

        return -1;
    }

    public void ListMaker()
    {
        int[] temp = new int[row * col];

        for (int i = 0; i < row; i++)
        {
            for(int j = 0; j< col; j++)
            {
                int index = i * col + j;
                temp[index] = 0;
            }
        }

        list = temp;
        listControl = temp;
    }

    public void UpdateRemainingShip()
    {
        GetComponent<UIController>().UpdateCountShip(shipList1.Count, shipList2.Count, shipList3.Count, shipList4.Count);
        GetComponent<UIController>().UpdateCountShipBot(shipList1_2.Count, shipList2_2.Count, shipList3_2.Count, shipList4_2.Count);
    }

    public void SpawShip()
    {
        // Spaw ship_4
        int num4 = Random.Range(0, 3);
        for(int i = 0; i< num4; i++)
        {
            int check = SpawShip4(posOfShip);
            if (check != -1)
                shipList4.Add(check);

            check = SpawShip4(posOfShip_2);
            if (check != -1)
                shipList4_2.Add(check);
        }

        // Spaw ship_3
        int num3 = Random.Range(1, 3);
        for (int i = 0; i < num3; i++)
        {
            int check = SpawShip3(posOfShip);
            if (check != -1)
                shipList3.Add(check);

            check = SpawShip3(posOfShip_2);
            if (check != -1)
                shipList3_2.Add(check);
        }

        // Spaw ship_2
        int num2 = Random.Range(2, 4);
        for (int i = 0; i < num2; i++)
        {
            int check = SpawShip2(posOfShip);
            if (check != -1)
                shipList2.Add(check);

            check = SpawShip2(posOfShip_2);
            if (check != -1)
                shipList2_2.Add(check);
        }

        // Spaw ship_1
        int num1 = Random.Range(2, 5);
        for (int i = 0; i < num1; i++)
        {
            int check = SpawShip1(posOfShip);
            if (check != -1)
                shipList1.Add(check);

            check = SpawShip1(posOfShip_2);
            if (check != -1)
                shipList1_2.Add(check);
        }
    }

    #region checkship
    public int SpawShip4(int[] posOfShip)
    {
        List<int> list = new List<int>();
        for(int i = 0; i < row; i++)
        {
            for(int j = 0; j< col; j++)
            {
                if(CheckShip4(i,j, posOfShip))
                {
                    list.Add(i * col + j);
                }
            }
        }
        if (list.Count <= 0) return -1;
        int result = list[Random.Range(0, list.Count)];

        posOfShip[result] = 4;
        posOfShip[result + 1] = 4;
        posOfShip[result + 2] = 4;
        posOfShip[result + 3] = 4;

        return result + 3;
    }

    public int SpawShip3(int[] posOfShip)
    {
        List<int> list = new List<int>();
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (CheckShip3(i, j, posOfShip))
                {
                    list.Add(i * col + j);
                }
            }
        }
        if (list.Count <= 0) return -1;
        int result = list[Random.Range(0, list.Count)];

        posOfShip[result] = 3;
        posOfShip[result + 1] = 3;
        posOfShip[result + 2] = 3;

        return result + 2;
    }

    public int SpawShip2(int[] posOfShip)
    {
        List<int> list = new List<int>();
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (CheckShip2(i, j, posOfShip))
                {
                    list.Add(i * col + j);
                }
            }
        }
        if (list.Count <= 0) return -1;
        int result = list[Random.Range(0, list.Count)];

        posOfShip[result] = 2;
        posOfShip[result + 1] = 2;

        return result + 1;
    }

    public int SpawShip1(int[] posOfShip)
    {
        List<int> list = new List<int>();
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (CheckShip1(i, j, posOfShip))
                {
                    list.Add(i * col + j);
                }
            }
        }
        if (list.Count <= 0) return -1;
        int result = list[Random.Range(0, list.Count)];

        posOfShip[result] = 1;

        return result;
    }

    private bool CheckShip4(int rowC, int colC, int[] posOfShip)
    {
        if (colC > col - 4) return false;
        int check = rowC * col + colC;
        if (posOfShip[check] == 0 && posOfShip[check + 1] == 0 && posOfShip[check + 2] == 0 && posOfShip[check + 3] == 0)
        {
            return true;
        }

        return false;
    }

    private bool CheckShip3(int rowC, int colC, int[] posOfShip)
    {
        if (colC > col - 3) return false;
        int check = rowC * col + colC;
        if (posOfShip[check] == 0 && posOfShip[check + 1] == 0 && posOfShip[check + 2] == 0)
        {
            return true;
        }

        return false;
    }

    private bool CheckShip2(int rowC, int colC, int[] posOfShip)
    {
        if (colC > col - 2) return false;
        int check = rowC * col + colC;
        if (posOfShip[check] == 0 && posOfShip[check + 1] == 0)
        {
            return true;
        }

        return false;
    }

    private bool CheckShip1(int rowC, int colC, int[] posOfShip)
    {
        int check = rowC * col + colC;
        if (posOfShip[check] == 0)
        {
            return true;
        }

        return false;
    }
    #endregion

    public void ShowAllShips()
    {
        // Show ship 1
        for(int i = 0; i < shipList1.Count; i++)
        {
            int rowT = shipList1[i]/col;
            int colT = shipList1[i] - rowT*col;
            contentController.ShowShip(rowT, colT, 0);
        }

        // Show ship 2
        for (int i = 0; i < shipList2.Count; i++)
        {
            int rowT = shipList2[i] / col;
            int colT = shipList2[i] - rowT * col;
            contentController.ShowShip(rowT, colT, 1);
        }

        // Show ship 3
        for (int i = 0; i < shipList3.Count; i++)
        {
            int rowT = shipList3[i] / col;
            int colT = shipList3[i] - rowT * col;
            contentController.ShowShip(rowT, colT, 2);
        }

        // Show ship 1
        for (int i = 0; i < shipList4.Count; i++)
        {
            int rowT = shipList4[i] / col;
            int colT = shipList4[i] - rowT * col;
            contentController.ShowShip(rowT, colT, 3);
        }
    }

    public void ShowAllShips2()
    {
        // Show ship 1
        for (int i = 0; i < shipList1_2.Count; i++)
        {
            int rowT = shipList1_2[i] / col;
            int colT = shipList1_2[i] - rowT * col;
            contentControllerBot.ShowShip(rowT, colT, 0);
        }

        // Show ship 2
        for (int i = 0; i < shipList2_2.Count; i++)
        {
            int rowT = shipList2_2[i] / col;
            int colT = shipList2_2[i] - rowT * col;
            contentControllerBot.ShowShip(rowT, colT, 1);
        }

        // Show ship 3
        for (int i = 0; i < shipList3_2.Count; i++)
        {
            int rowT = shipList3_2[i] / col;
            int colT = shipList3_2[i] - rowT * col;
            contentControllerBot.ShowShip(rowT, colT, 2);
        }

        // Show ship 1
        for (int i = 0; i < shipList4_2.Count; i++)
        {
            int rowT = shipList4_2[i] / col;
            int colT = shipList4_2[i] - rowT * col;
            contentControllerBot.ShowShip(rowT, colT, 3);
        }
    }

    public void GameOver(bool isWin)
    {
        GetComponent<UIController>().GameOver(isWin);
    }

    public void UpdateRemainingShuffe()
    {
        GetComponent<UIController>().UpdateCurrentNumber(numberOfTry);
    }

    public void UpdateSliderValue(float value)
    {
        GetComponent<UIController>().UpdateSliderValue(value);
    }

    public void SetSliderValue(float value)
    {
        GetComponent<UIController>().SetSlider(value);
    }
}
