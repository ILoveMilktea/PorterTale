using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightUIController : MonoSingleton<FightUIController>
{
    // 각 캐릭터 UI (이름, Hp)
    private PlayerCharacterUI playerCharacterUI;
    private Dictionary<GameObject, CharacterUI> enemyCharacterUIs;
    // Joystick들
    private JoystickAttack joystickAttack;
    private JoystickMove joystickMove;
    private JoystickSkill joystickSkill;
    private bool activeSelf_JoystickSkill;
    // 상황에 따른 Window
    public ResultWindow resultWindow;
    public DeadWindow deadWindow;
    // pause
    public GameObject pauseImage;
    public Button pauseButton;
    // topbar
    public GameObject topBar;
    public Text playTimer;
    public Text parts;
    // Grouping
    public Transform characterUIGroup;
    private UIGroup fightGroup = new UIGroup();
    private UIGroup pauseGroup = new UIGroup();
    private UIGroup statusGroup = new UIGroup();

    protected override void Init()
    {
        enemyCharacterUIs = new Dictionary<GameObject, CharacterUI>();
        joystickAttack = FindObjectOfType<JoystickAttack>();
        joystickMove = FindObjectOfType<JoystickMove>();
        joystickSkill = FindObjectOfType<JoystickSkill>();

        resultWindow = FindObjectOfType<ResultWindow>();
        pauseButton.onClick.AddListener(OnClickPauseButton);
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            resultWindow.OpenResultWindow();
        }
    }

    public void SetStageUI()
    {
        SetUIToAllCharacters();
        UIGrouping();
        deadWindow.fightFog.SetFloat("_Slide", 0.2f);
    }

    // pause
    public void OnClickPauseButton()
    {
        FightState curState = FightSceneController.Instance.GetCurrentFightState();
        if (curState == FightState.Pause)
        {
            joystickSkill.gameObject.SetActive(activeSelf_JoystickSkill);
            FightSceneController.Instance.ChangeFightState(FightState.Fight);
        }
        else if (curState == FightState.Fight)
        {
            activeSelf_JoystickSkill = joystickSkill.gameObject.activeSelf;
            joystickSkill.gameObject.SetActive(false);
            FightSceneController.Instance.ChangeFightState(FightState.Pause);
        }
    }

    public void SetTimerText(float time)
    {
        playTimer.text = time.ToString("00:00");
    }
    public void SetParts(int value)
    {
        parts.text = value.ToString();
    }

    private void SetUIToAllCharacters()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        //GameObject playerCharacterUIprefab = Instantiate(Resources.Load("Prefab/UI/PlayerCharacterUI"), characterUIGroup) as GameObject;
        GameObject playerCharacterUIprefab = UIPoolManager.Instance.Get("PlayerCharacterUI");
        playerCharacterUI = playerCharacterUIprefab.GetComponent<PlayerCharacterUI>();

        FightStatus fightStatus = FightSceneController.Instance.GetFightStatus();

        CharacterStatus playerStatus = fightStatus.playerStatus;
        playerCharacterUI.SetStatus(playerStatus);
        playerCharacterUI.SetTarget(player);
        playerCharacterUI.gameObject.SetActive(true);
        //playerCharacterUI.ResizeUI();

        GameObject[] enemies = fightStatus.enemies.Keys.ToArray();
        foreach (var enemy in enemies)
        {
            CharacterStatus enemyStatus = FightSceneController.Instance.GetPlayerStatus();
            if (enemyStatus.name == DataManager.Instance.GetPlayInfo.CurDungeon)
            {
                //GameObject griffonUIprefab = Instantiate(Resources.Load("Prefab/UI/GriffonUI"), characterUIGroup) as GameObject;
                GameObject griffonUIprefab = UIPoolManager.Instance.Get("GriffonUI");
                enemyCharacterUIs.Add(enemy, griffonUIprefab.GetComponent<GriffonUI>());
                enemyCharacterUIs[enemy].SetStatus(fightStatus.enemies[enemy]);
                enemyCharacterUIs[enemy].SetTarget(enemy);
                enemyCharacterUIs[enemy].ResizeUI();
            }
            else
            {
                //GameObject EnemyCharacterUIprefab = Instantiate(Resources.Load("Prefab/UI/EnemyCharacterUI"), characterUIGroup) as GameObject;
                GameObject EnemyCharacterUIprefab = UIPoolManager.Instance.Get("EnemyCharacterUI");
                enemyCharacterUIs.Add(enemy, EnemyCharacterUIprefab.GetComponent<CharacterUI>());
                
                enemyCharacterUIs[enemy].SetStatus(fightStatus.enemies[enemy]);
                enemyCharacterUIs[enemy].SetTarget(enemy);
            }
            enemyCharacterUIs[enemy].gameObject.SetActive(true);
        }
    }
    // 일시 정지할때 표시 안할 ui grouping
    private void UIGrouping()
    {
        // 전투 중 active한 UI들
        fightGroup.SetMember(joystickAttack.gameObject);
        fightGroup.SetMember(joystickMove.gameObject);
        //fightGroup.SetMember(joystickSkill.gameObject);

        // 일시정지 중 active한 UI들
        pauseGroup.SetMember(pauseImage.gameObject);

        // status UI들
        statusGroup.SetMember(topBar.gameObject);
        statusGroup.SetMember(pauseButton.gameObject);

        Action<FightState,Action> action = FightSceneController.Instance.SetStateChangeCallback;
        action(FightState.Pause, fightGroup.InactiveAllMembers);
        action(FightState.Pause, pauseGroup.ActiveAllMembers);

        action(FightState.Fight, fightGroup.ActiveAllMembers);
        action(FightState.Fight, pauseGroup.InactiveAllMembers);

        action(FightState.Dead, fightGroup.InactiveAllMembers);
        action(FightState.Dead, statusGroup.InactiveAllMembers);
    }

    public void SkillButtonOn(WeaponType type, string key)
    {
        joystickSkill.gameObject.SetActive(true);
        string spriteName = WeaponSkillTable.Instance.GetTuple(type.ToString(), key).m_spriteName;
        //joystickSkill.SkillOn(Resources.Load<Sprite>("Image/Skill/" + spriteName));
        joystickSkill.SkillOn(SpritePoolManager.Instance.Get(spriteName));
    }

    public void SkillButtonOff()
    {
        joystickSkill.gameObject.SetActive(false);
    }

    public void SwapWeaponImage(WeaponType weapon)
    {
        joystickAttack.WeaponImageSwap(weapon);
    }

    public void LockAttackJoystick()
    {
        joystickAttack.LockJoystick();
    }
    public void UnLockAttackJoystick()
    {
        joystickAttack.UnLockJoystick();
    }
    public void DownPlayerHpSlider(int damage)
    {
        playerCharacterUI.HpDown(damage);
    }
    public void DownEnemyHpSlider(GameObject Enemy, int damage)
    {
        if (enemyCharacterUIs.ContainsKey(Enemy))
        {
            enemyCharacterUIs[Enemy].HpDown(damage);
        }
    }
    public void SetUIPlayerDead()
    {
        OffAllCharacterUI();
        SkillButtonOff();

        OnDeadWindow();
    }
    public void SetUIEnemyDead(GameObject enemy)
    {
        UIPoolManager.Instance.Free(enemyCharacterUIs[enemy].gameObject);
        enemyCharacterUIs.Remove(enemy);
    }
    public void OffAllCharacterUI() // 죽었을 때
    {
        GameObject[] enemies = FightSceneController.Instance.GetFightStatus().enemies.Keys.ToArray();
        
        foreach (var enemy in enemies)
        {
            if (enemyCharacterUIs.ContainsKey(enemy))
            {
                UIPoolManager.Instance.Free(enemyCharacterUIs[enemy].gameObject);
                //enemyCharacterUIs[enemy].gameObject.SetActive(false);
            }
        }
        UIPoolManager.Instance.Free(playerCharacterUI.gameObject);
        playerCharacterUI.gameObject.SetActive(false);
    }
    public void OnDeadWindow()
    {
        deadWindow.fightFog.SetFloat("_Slide", 0.2f);
        deadWindow.gameObject.SetActive(true);
        StartCoroutine(deadWindow.DeadHandler());
    }

    public void ShowResult()
    {
        //5스테이지 클리어
        resultWindow.OpenResultWindow();
    }

}
