using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    public ItemSO itemSO;
    public int quantity;

    public virtual void UpdateUI() { }
    public virtual void Select() { }
    public virtual void Deselect() { }
}
