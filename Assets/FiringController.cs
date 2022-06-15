using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FiringController : MonoBehaviour
{

    [SerializeField] GameObject firing;
    [SerializeField] GameObject[] ships;

    public void ShowShip(int index)
    {
        HideAll();
        ships[(int)index].SetActive(true);
    }

    public void ShowFiring()
    {
        //HideAll();
        firing.SetActive(true);
    }

    public void HideAll()
    {
        firing.SetActive(false);
        for (int i = 0; i < ships.Length; i++)
        {
            ships[i].SetActive(false);
        }
    }
}
