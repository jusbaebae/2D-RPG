using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBox : MonoBehaviour
{
    public List<LootItem> bonusTable;
    public GameObject bonusPrefab;
    [SerializeField] private GameObject OpenBox;
    [SerializeField] private GameObject CloseBox;
    [SerializeField] private GameObject PressIcon;

    private bool canOpen;
    private bool isOpened;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isOpened)
        {
            canOpen = true;
            PressIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isOpened)
        {
            canOpen = false;
            PressIcon.SetActive(false);
        }
    }

    void Update()
    {
        if (canOpen && !isOpened && Input.GetKeyDown(KeyCode.Space))
        {
            Open();
        }
    }
    void Open()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Treasure);
        GetComponent<Collider2D>().enabled = false;

        isOpened = true;
        OpenBox.SetActive(true);
        CloseBox.SetActive(false);
        PressIcon.SetActive(false);
        SpawnItems();
    }

    public void SpawnItems()
    {
        foreach (var bonus in bonusTable)
        {
            if (Random.value <= bonus.dropChance)
            {
                int amount = Random.Range(bonus.quantityRange.x, bonus.quantityRange.y + 1);
                Vector3 offset = Random.insideUnitCircle * 1f;
                GameObject obj = Instantiate(bonusPrefab, OpenBox.transform.position + offset, Quaternion.identity);
                obj.GetComponent<Loot>().Initialize(bonus.itemSO, amount, true);
                obj.GetComponent<ItemPop>().Pop();
            }
        }
    }
}
