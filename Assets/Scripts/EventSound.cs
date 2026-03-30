using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSound : MonoBehaviour
{
    private void WalkSound() //걷는사운드 (애니메이션 이벤트로 삽입)
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Walk);
    }
}
