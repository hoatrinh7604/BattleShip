using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentController : MonoBehaviour
{
    [SerializeField] GameObject rowObject;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] bool isUser;
    public void SpawRow(int numberOfRow)
    {
        for(int i = 0; i<numberOfRow; i++)
        {
            GameObject row = Instantiate(rowObject, Vector3.zero, Quaternion.identity, transform);
            row.GetComponent<RowController>().indexOfRow = i;
        }
    }

    public void SpawItem(int value, int indexRow, int indexCol)
    {
        GameObject temp = transform.GetChild(indexRow).gameObject;
        GameObject item = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity, temp.transform);
        item.GetComponent<ItemController>().UpdatePos(indexRow, indexCol);
        item.GetComponent<ItemController>().SetID(value, isUser);
    }

    public void SpawItems(int numberOfCol, int numberOfRow, int currentNumber)
    {
        SpawRow(numberOfRow);

        for (int i = 0; i < numberOfRow; i++)
        {
            for(int j = 0; j < numberOfCol; j++)
            {
                SpawItem(0, i, j);
            }
        }
    }

    public void UpdateItems(int rowArr, int colArr, int currentNumber)
    {
        EnableItems();
        for (int i = 0; i < rowArr; i++)
        {
            for (int j = 0; j < colArr; j++)
            {
                int value = Random.Range(0, 100);
                if(value == currentNumber)
                {
                    GameController.Instance.UpdateRemainNumber();
                }
                UpdateValueOfItem(i, j, value);
            }
        }

        int random = Random.Range(1, 5);
        for(int i = 0; i < random; i++)
        {
            int randomIndex = Random.Range(0, rowArr * colArr);
            int row = randomIndex / colArr;
            int col = randomIndex - row * colArr;
            UpdateValueOfItem(row, col, currentNumber);
            GameController.Instance.UpdateRemainNumber();
        }
    }

    public void UpdateValueOfItem(int row, int col, int value)
    {
        GameObject rowParent = transform.GetChild(row).gameObject;
        GameObject colParent = rowParent.transform.GetChild(col).gameObject;

        colParent.GetComponent<ItemController>().SetID(value, isUser);
    }

    public void HideItem(int row, int col)
    {
        GameObject rowParent = transform.GetChild(row).gameObject;
        GameObject colParent = rowParent.transform.GetChild(col).gameObject;

        colParent.GetComponent<ItemController>().Hide(true);
    }

    public void ShowImage(int row, int col)
    {
        GameObject rowParent = transform.GetChild(row).gameObject;
        GameObject colParent = rowParent.transform.GetChild(col).gameObject;

        colParent.GetComponent<ItemController>().ShowImage();
    }

    public void ShowShip(int row, int col, int indexShip)
    {
        GameObject rowParent = transform.GetChild(row).gameObject;
        GameObject colParent = rowParent.transform.GetChild(col).gameObject;

        colParent.GetComponent<FiringController>().ShowShip(indexShip);
    }

    public void ShowFire(int row, int col)
    {
        GameObject rowParent = transform.GetChild(row).gameObject;
        GameObject colParent = rowParent.transform.GetChild(col).gameObject;

        colParent.GetComponent<FiringController>().ShowFiring();
    }

    public void EnableItems()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            for (int j = 0; j < transform.GetChild(i).transform.childCount; j++)
            {
                EnableItem(i, j);
            }
        }
    }

    public void EnableItem(int row, int col)
    {
        GameObject rowParent = transform.GetChild(row).gameObject;
        GameObject colParent = rowParent.transform.GetChild(col).gameObject;

        colParent.GetComponent<ItemController>().Hide(false);
    }

    public void Notice(int row, int col)
    {
        GameObject rowParent = transform.GetChild(row).gameObject;
        GameObject colParent = rowParent.transform.GetChild(col).gameObject;

        colParent.GetComponent<ItemController>().Noticing();
    }

    public void UnTicked(int row, int col)
    {
        GameObject rowParent = transform.GetChild(row).gameObject;
        GameObject colParent = rowParent.transform.GetChild(col).gameObject;

        colParent.GetComponent<ItemController>().UnTicked();
    }

    public void ChangeSibling(int row, int col, int newRow, int newIndex)
    {
        GameObject rowParent = transform.GetChild(row).gameObject;
        GameObject colParent = rowParent.transform.GetChild(col).gameObject;

        colParent.transform.SetParent(transform.GetChild(newRow));
        colParent.transform.SetSiblingIndex(newIndex);
        //colParent.GetComponent<ItemController>().UpdatePos(newRow, newIndex);
    }

    public void UpdatePosItems()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            for(int j = 0; j < transform.GetChild(i).transform.childCount; j++)
            {
                transform.GetChild(i).transform.GetChild(j).GetComponent<ItemController>().UpdatePos(i, j);
            }
        }
    }

    public void Reset()
    {
        for(int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject temp = transform.GetChild(i).gameObject;
            temp.transform.SetParent(null);
            Destroy(temp);
        }
    }
}
