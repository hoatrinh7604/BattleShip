using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemController : MonoBehaviour
{
    [SerializeField] int id;
    [SerializeField] Button button;
    [SerializeField] GameObject BG;
    [SerializeField] TextMeshProUGUI number;
    [SerializeField] Image avatar;

    [SerializeField] Sprite[] sprites;
    [SerializeField] bool isTicked;
    [SerializeField] GameObject image;
    private bool isUser;

    public bool isKilled { set; get; }

    private int row;
    private int col;
    private bool noticing;

    // Start is called before the first frame update
    void Start()
    {
        isTicked = false;
        button.onClick.AddListener(() => ChoosingItem());
    }

    public void SetID(int value, bool isUser)
    {
        this.isUser = isUser;
        id = value;
        SetNumber(id);
        avatar.sprite = sprites[Random.Range(0, sprites.Length)];
    }

    public void SetNumber(int value)
    {
        number.text = value.ToString();
    }

    public void ChoosingItem()
    {
        if (isKilled)
            return;
        
        GameController.Instance.UserChooseItem(row, col, isUser);
        image.SetActive(true);
    }

    public void ShowImage()
    {
        image.SetActive(true);
    }

    public void UnTicked()
    {
        isTicked = false;
        button.GetComponent<Image>().color = new Color(0, 0, 0, 0);
    }

    public void UpdatePos(int rowValue, int colValue)
    {
        row = rowValue;
        col = colValue;
    }

    public void Hide(bool isHide)
    {
        noticing = false;
        isKilled = isHide;
        BG.SetActive(!isHide);
        button.interactable = false;
        button.image.enabled = true;
    }

    public void ChangeSibling(int index)
    {
        transform.SetSiblingIndex(index);
    }

    public void Noticing()
    {
        noticing = true;
        StartCoroutine(Notice());
    }

    IEnumerator Notice()
    {
        float alpha = 0.5f;
        var baseColor = avatar.color;
        while (noticing)
        {
            yield return new WaitForSeconds(0.1f);
            avatar.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            alpha += 0.05f;
            if(alpha>1f)
            {
                alpha = 0.5f;
            }
        }
    }
}
