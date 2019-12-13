using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayModeUIController
{
    // UI onoff
    void HidePlayUI();
    void DisplayPlayUI();
    // pause
    void PauseGame();
    void ReleaseGame();
    // hp
    void DamageToCharacter(GameObject character, int value);
    void HealCharacter(GameObject character, int value);
    void OffCharacterUI(GameObject character); // 죽었을 때

}
