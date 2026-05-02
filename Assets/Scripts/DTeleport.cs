using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DTeleport : MonoBehaviour
{
    public string sceneToLoad;

    public UIAnim uianim;
    public GameObject Dungeoninfo;
    public string DungeonName;
    public string RecommentLV;
    public string Monsters;
    [TextArea]public string desc;
    public TextMeshProUGUI Toptext;
    public TextMeshProUGUI leveltext;
    public TextMeshProUGUI monstertext;
    public TextMeshProUGUI desctext;

    private bool openui;

    private void Start()
    {
        Toptext.text = DungeonName;
        leveltext.text = "주요 레벨 : " + RecommentLV;
        monstertext.text = "주요 몬스터 : " + Monsters;
        desctext.text = desc;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!openui && collision.gameObject.tag == "Player")
        {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.Click);
            openui = true;
            uianim.Show(Dungeoninfo);
            PlayerMovement.Instance.isinteract = true;
            UiManager.Instance.isInteract = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player") openui = false;
    }

    public void Onvisit()
    {
        PlayerMovement.Instance.isinteract = false;
        UiManager.Instance.isInteract = false;
        SceneTransition.Instance.StartTransition(sceneToLoad);
    }

    public void Onclose()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Cancel);
        uianim.Hide(Dungeoninfo);
        PlayerMovement.Instance.isinteract = false;
        UiManager.Instance.isInteract = false;
    }
}
