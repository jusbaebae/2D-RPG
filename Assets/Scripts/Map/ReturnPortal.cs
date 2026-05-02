using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnPortal : MonoBehaviour
{
    private bool canUse = false;
    public GameObject pressIcon;

    void Update()
    {
        if (canUse && Input.GetKeyDown(KeyCode.Space))
        {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.Click);
            DungeonManager.Instance.uianim.Show(DungeonManager.Instance.ReturnUI);
            StartCoroutine(Save());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pressIcon.SetActive(true);
            canUse = true;
        }
            
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pressIcon.SetActive(false);
            canUse = false;
        }
    }

    IEnumerator Save()
    {
        yield return new WaitForSeconds(2f); //방이동시간
        SaveManager.Instance.SaveGame();
    }
}
