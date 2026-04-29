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
            SceneManager.LoadScene(GameManager.Instance.previousScene);
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
        yield return new WaitForSeconds(2f);
        SaveManager.Instance.SaveGame();
    }
}
