using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightSceneController : MonoSingleton<FightSceneController>
{
    private PlayTimer playTimer;

    private FightStatus fightStatus;

    private FightStateObserver fightStateObserver;
    private FightScheduler fightScheduler;
    private Player player;
    private List<GameObject> curUsingBullets;


    protected override void Init()
    {
        playTimer = FindObjectOfType<PlayTimer>();

        fightStateObserver = FindObjectOfType<FightStateObserver>();
        fightScheduler = FindObjectOfType<FightScheduler>();
        player = FindObjectOfType<Player>();
        curUsingBullets = new List<GameObject>();
    }

    void Start()
    {
        StartFightScene();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha9))
        {
            GameObject[] enemies = fightStatus.enemies.Keys.ToArray();
            foreach(var enemy in enemies)
            {
                EnemyDead(enemy); 
            }
        }
    }

    public void StartFightScene()
    {

        fightStatus = new FightStatus();

        SetTimer();
        SetPlayerOnStage();
        SetEnemyOnStage();
        FightUIController.Instance.SetStageUI();

        fightScheduler.StageStart();
    }
    private void SetTimer()
    {
        playTimer.StartTimer();

        SetStateChangeCallback(FightState.Standby, playTimer.StandbyTimer);
        SetStateChangeCallback(FightState.Fight, playTimer.ReleaseTimer);
        SetStateChangeCallback(FightState.Pause, playTimer.FreezeTimer);
        SetStateChangeCallback(FightState.Dead, playTimer.StandbyTimer);
        SetStateChangeCallback(FightState.Clear_Stage, playTimer.ReleaseTimer);
        SetStateChangeCallback(FightState.Clear_Dungeon, playTimer.StandbyTimer);
        SetStateChangeCallback(FightState.End, playTimer.StandbyTimer);
    }
    // player status 등록
    private void SetPlayerOnStage()
    {
        PlayerStatusInfo statusFromData = DataManager.Instance.GetPlayerStatus;
        CharacterStatus status = new CharacterStatus("Player", statusFromData.MaxHp + statusFromData.BuffHp, statusFromData.RemainHp, statusFromData.Atk + statusFromData.BuffAtk);

        fightStatus.SetPlayerStatus(status);
        fightStatus.AddParts(DataManager.Instance.GetPlayInfo.Parts);
        FightUIController.Instance.SetParts(fightStatus.gainParts);
    }
    // stage에 해당하는 몹 생성, 위치지정
    private void SetEnemyOnStage()
    {
        int stageNumber = DataManager.Instance.GetPlayInfo.Stage;
        int enemyCount = 0;

        Dictionary<int, StageEnemyInfo> stageTable = StageEnemyTable.Instance.GetTable("Stage" + stageNumber.ToString());
        foreach(var stageEnemyInfo in stageTable.Values)
        {
            EnemyStatusInfo enemyInfo = Tables.Instance.EnemyStatus.GetTuple(stageEnemyInfo.m_serialNumber);

            GameObject enemy = ObjectPoolManager.Instance.Get(enemyInfo.m_name);
            enemy.GetComponent<EnemyAttack>().SetProjectilePrefabName(enemyInfo.m_name);

            EnemyCharacterStatus status = new EnemyCharacterStatus(enemyInfo.m_name, enemyInfo.m_hp, enemyInfo.m_hp, enemyInfo.m_atk, enemyInfo.m_dropParts);

            fightStatus.AddEnemyInstance(enemy, status);
            enemy.transform.position = new Vector3(stageEnemyInfo.m_posX, 0, stageEnemyInfo.m_posY);
            enemyCount++;

            enemy.SetActive(true);
        }

        fightStatus.SetRemainEnemy(enemyCount);
    }

    // 전투 중 일시정지시에 처리할 콜백함수들 등록
    public void SetStateChangeCallback(FightState fightState, Action func)
    {
        fightStateObserver.SetStateChangeCallback(fightState, func);
    }
    public void RemoveStateChangeCallback(FightState fightState, Action func)
    {
        fightStateObserver.RemoveStateChangeCallback(fightState, func);
    }

    public FightState GetCurrentFightState()
    {
        return fightStateObserver.curFightState;
    }
    public void ChangeFightState(FightState state)
    {
        fightStateObserver.SetCurrentFightState(state);
    }
    public CharacterStatus GetPlayerStatus()
    {
        return fightStatus.playerStatus;
    }
    public CharacterStatus GetEnemyStatus(GameObject enemy)
    {
        return fightStatus.enemies[enemy];
    }
    public FightStatus GetFightStatus()
    {
        return fightStatus;
    }

    // player control
    public int GetPlayerATK()
    {
        return fightStatus.playerStatus.atk;
    }
    // 플레이어 이동
    public void MovePlayer(Vector3 dir, float amount)
    {
        if (fightStateObserver.curFightState == FightState.Fight ||
            fightStateObserver.curFightState == FightState.Clear_Stage)
        {
            player.Move(dir, amount);
        }
    }
    // 플레이어 멈춤
    public void StopPlayer()
    {
        if (fightStateObserver.curFightState == FightState.Fight ||
            fightStateObserver.curFightState == FightState.Clear_Stage)
        {
            player.StopMove();
        }
    }
    // 플레이어 공격
    public void PlayerAttack(Vector3 dir)
    {
        if (fightStateObserver.curFightState == FightState.Fight ||
            fightStateObserver.curFightState == FightState.Clear_Stage)
        {
            player.Attack(dir);
        }
    }
    public void PlayerSkillReady(Vector3 dir)
    {
        if (fightStateObserver.curFightState == FightState.Fight ||
            fightStateObserver.curFightState == FightState.Clear_Stage)
        {
            player.SkillReady(dir);
        }
    }
    // 플레이어 스킬
    public void PlayerSkill(Vector3 dir)
    {
        if (fightStateObserver.curFightState == FightState.Fight ||
            fightStateObserver.curFightState == FightState.Clear_Stage)
        {
            player.SkillAttack(dir);
        }
    }
    // 플레이어 공격 대기
    public void PlayerStandby()
    {
        if (fightStateObserver.curFightState == FightState.Fight ||
            fightStateObserver.curFightState == FightState.Clear_Stage)
        {
            player.Standby();
        }
    }
    // 무기 변경
    public void SwapWeapon()
    {
        WeaponType weapon = player.SwapWeapon();
        FightUIController.Instance.SwapWeaponImage(weapon);
    }
    public void LockMoveRotate()
    {
        player.isAttacking = true;
    }
    public void UnLockMoveRotate()
    {
        player.isAttacking = false;
    }

    // ---- hp
    public void DamageToCharacter(GameObject character, int damage)
    {
        if(character == player.gameObject&& GetCurrentFightState() != FightState.Dead)
        {
            FightUIController.Instance.DownPlayerHpSlider(damage);
            bool isCharacterDead = fightStatus.playerStatus.DamageToCharacter(damage);
            if(isCharacterDead)
            {
                PlayerDead();
            }
        }
        else if(fightStatus.enemies.ContainsKey(character))
        {
            FightUIController.Instance.DownEnemyHpSlider(character, damage);
            bool isCharacterDead = fightStatus.enemies[character].DamageToCharacter(damage);
            if (isCharacterDead)
            {
                EnemyDead(character);
            }
        }
    }

    public void DamageToCharacter(GameObject source, GameObject target, float gunDamage)
    {
        if (target == player.gameObject && GetCurrentFightState() != FightState.Dead)
        {
            int damage = fightStatus.enemies[source].atk;

            FightUIController.Instance.DownPlayerHpSlider(damage);
            bool isCharacterDead = fightStatus.playerStatus.DamageToCharacter(damage);
            if (isCharacterDead)
            {
                PlayerDead();
            }
        }
        else if (source == player.gameObject && target.activeSelf == true)
        {
            int damage = (int)gunDamage;

            FightUIController.Instance.DownEnemyHpSlider(target, damage);
            bool isCharacterDead = fightStatus.enemies[target].DamageToCharacter(damage);
            if (isCharacterDead)
            {
                EnemyDead(target);
            }
        }
    }

    public void PlayerDead()
    {
        ChangeFightState(FightState.Dead);
        OffAllBullets();

        // playtime 데이터처리
        playTimer.StopTimer();
        if (DataManager.Instance.GetPlayInfo.Stage != 1)
        {
            // 1층에서 뒤지면 저장안함
            DataManager.Instance.SetPlayTime(playTimer.GetPlaytime());
            DataManager.Instance.SetStage(DataManager.Instance.GetPlayInfo.Stage - 1);
            DataManager.Instance.Save();
        }

        FightUIController.Instance.SetUIPlayerDead();

        player.PlayerDead();
    }
    public void EnemyDead(GameObject enemy)
    {
        // gain parts
        int dropParts = fightStatus.enemies[enemy].dropParts;
        fightStatus.AddParts(dropParts);
        fightStatus.RemoveEnemyInstance(enemy);

        FightUIController.Instance.SetParts(fightStatus.gainParts);
        FightUIController.Instance.SetUIEnemyDead(enemy);

        ParticleManager.Instance.OnParticle(fightStatus.enemies[enemy].name + "Death", 2.0f, enemy.transform.position);

        ObjectPoolManager.Instance.Free(enemy);
    }

    // ---- hp


    //----- bullet
    public void AddBulletToList(GameObject bullet)
    {
        curUsingBullets.Add(bullet);
    }
    public void RemoveBulletFromList(GameObject bullet)
    {
        if(curUsingBullets.Contains(bullet))
        {
            curUsingBullets.Remove(bullet);
        }
    }
    public void OffAllBullets()
    {
        foreach(var bullet in curUsingBullets)
        {
            ObjectPoolManager.Instance.Free(bullet);
        }

        curUsingBullets.Clear();
    }
    //----- bullet

    public string GetEnemyName(GameObject enemy)
    {
        if(fightStatus.enemies.ContainsKey(enemy))
        {
            return fightStatus.enemies[enemy].name;
        }
        else
        {
            return "default";
        }
    }


    public int RemainEnemyNumber()
    {
        return fightStatus.remainEnemy;
    }
    public void Retreat()
    {
        ChangeFightState(FightState.End);
        
        OffAllBullets();
        player.StopMove();

        playTimer.StopTimer();
        DataManager.Instance.SetPlayTime(playTimer.GetPlaytime());

        DataManager.Instance.SetParts(fightStatus.gainParts);
        DataManager.Instance.SetRemainHp(fightStatus.playerStatus.remainHp);
        DataManager.Instance.SetStage(DataManager.Instance.GetPlayInfo.Stage - 1);

        DataManager.Instance.Save();

        GameManager.Instance.LoadNextScene(Constants.FightSceneName, Constants.UpgradeSceneName);
    }

    public void GoNext()
    {
        ChangeFightState(FightState.End);

        OffAllBullets();
        player.StopMove();

        playTimer.StopTimer();
        DataManager.Instance.SetPlayTime(playTimer.GetPlaytime());

        DataManager.Instance.SetParts(fightStatus.gainParts);
        DataManager.Instance.SetRemainHp(fightStatus.playerStatus.remainHp);
        DataManager.Instance.SetAlreadyAct(false);


        DataManager.Instance.Save();

        GameManager.Instance.LoadNextScene(Constants.FightSceneName, Constants.UpgradeSceneName);
    }

    public void EscapeDungeon()
    {
        //나중에 돈 추가해줘
        DataManager.Instance.ResetDungeonData();
        DataManager.Instance.Save();

        GameManager.Instance.LoadNextScene(Constants.FightSceneName, Constants.WorldMapSceneName);
    }
}


//using System;
//using System.Linq;
//using System.Reflection;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class FightSceneController : MonoSingleton<FightSceneController>
//{
//    private FightStatus fightStatus;
//    private PlayTimer playTimer;

//    private FightStateObserver fightStateObserver;
//    private FightScheduler fightScheduler;
//    private Player player;
//    private List<GameObject> curUsingBullets;

//    // UI 관련
//    private PlayerCharacterUI playerCharacterUI;
//    private Dictionary<GameObject, CharacterUI> enemyCharacterUIs;
//    private JoystickAttack joystickAttack;
//    private JoystickMove joystickMove;
//    private JoystickSkill joystickSkill;
//    private bool activeSelf_JoystickSkill;

//    public ResultWindow resultWindow;
//    public DeadWindow deadWindow;

//    public Transform characterUIGroup;
//    public GameObject standbyImage;
//    public GameObject pauseImage;
//    public Button pauseButton;
//    public Text parts;

//    private UIGroup fightGroup = new UIGroup();
//    private UIGroup pauseGroup = new UIGroup();
//    private UIGroup statusGroup = new UIGroup();

//    protected override void Init()
//    {
//        playTimer = FindObjectOfType<PlayTimer>();

//        fightStateObserver = FindObjectOfType<FightStateObserver>();
//        fightScheduler = FindObjectOfType<FightScheduler>();
//        player = FindObjectOfType<Player>();
//        curUsingBullets = new List<GameObject>();

//        enemyCharacterUIs = new Dictionary<GameObject, CharacterUI>();
//        joystickAttack = FindObjectOfType<JoystickAttack>();
//        joystickMove = FindObjectOfType<JoystickMove>();
//        joystickSkill = FindObjectOfType<JoystickSkill>();

//        resultWindow = FindObjectOfType<ResultWindow>();
//        pauseButton.onClick.AddListener(OnClickPauseButton);
//    }

//    void Start()
//    {
//        StartFightScene();
//    }

//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Alpha9))
//        {
//            GameObject[] enemies = fightStatus.enemies.Keys.ToArray();
//            foreach (var enemy in enemies)
//            {
//                EnemyDead(enemy);
//            }
//        }
//    }

//    public void StartFightScene()
//    {
//        SetTimer();

//        fightStatus = new FightStatus();

//        SetPlayerOnStage();
//        SetEnemyOnStage();
//        SetUIToAllCharacters();
//        UIGrouping();

//        fightScheduler.StageStart();
//        deadWindow.fightFog.SetFloat("_Slide", 0.2f);
//    }
//    private void SetTimer()
//    {
//        playTimer.StartTimer();

//        SetStateChangeCallback(FightState.Standby, playTimer.StandbyTimer);
//        SetStateChangeCallback(FightState.Fight, playTimer.ReleaseTimer);
//        SetStateChangeCallback(FightState.Pause, playTimer.FreezeTimer);
//        SetStateChangeCallback(FightState.Dead, playTimer.StandbyTimer);
//        SetStateChangeCallback(FightState.Clear_Stage, playTimer.ReleaseTimer);
//        SetStateChangeCallback(FightState.Clear_Dungeon, playTimer.StandbyTimer);
//        SetStateChangeCallback(FightState.End, playTimer.StandbyTimer);
//    }

//    // player status 등록
//    private void SetPlayerOnStage()
//    {
//        PlayerStatusInfo statusFromData = DataManager.Instance.GetPlayerStatus;
//        CharacterStatus status = new CharacterStatus("Player", statusFromData.MaxHp + statusFromData.BuffHp, statusFromData.RemainHp, statusFromData.Atk + statusFromData.BuffAtk);

//        fightStatus.SetPlayerStatus(status);
//        fightStatus.AddParts(DataManager.Instance.GetPlayInfo.Parts);
//        parts.text = fightStatus.gainParts.ToString();
//    }
//    // stage에 해당하는 몹 생성, 위치지정
//    private void SetEnemyOnStage()
//    {
//        int stageNumber = DataManager.Instance.GetPlayInfo.Stage;
//        int enemyCount = 0;

//        Dictionary<int, StageEnemyInfo> stageTable = StageEnemyTable.Instance.GetTable("Stage" + stageNumber.ToString());
//        foreach (var stageEnemyInfo in stageTable.Values)
//        {
//            EnemyStatusInfo enemyInfo = Tables.Instance.EnemyStatus.GetTuple(stageEnemyInfo.m_serialNumber);

//            GameObject enemy = ObjectPoolManager.Instance.Get(enemyInfo.m_name);
//            enemy.GetComponent<EnemyAttack>().SetProjectilePrefabName(enemyInfo.m_name);

//            EnemyCharacterStatus status = new EnemyCharacterStatus(enemyInfo.m_name, enemyInfo.m_hp, enemyInfo.m_hp, enemyInfo.m_atk, enemyInfo.m_dropParts);

//            fightStatus.AddEnemyInstance(enemy, status);
//            enemy.transform.position = new Vector3(stageEnemyInfo.m_posX, 0, stageEnemyInfo.m_posY);
//            enemyCount++;

//            enemy.SetActive(true);
//        }

//        fightStatus.SetRemainEnemy(enemyCount);
//    }
//    // 모든 character(player, enemy) hp바 붙이기
//    private void SetUIToAllCharacters()
//    {
//        GameObject player = GameObject.FindGameObjectWithTag("Player");
//        GameObject playerCharacterUIprefab = Instantiate(Resources.Load("Prefab/UI/PlayerCharacterUI"), characterUIGroup) as GameObject;
//        playerCharacterUI = playerCharacterUIprefab.GetComponent<PlayerCharacterUI>();

//        playerCharacterUI.SetName(fightStatus.playerStatus.name);
//        playerCharacterUI.SetMaxHp(fightStatus.playerStatus.maxHp);
//        playerCharacterUI.SetRemainHp(fightStatus.playerStatus.remainHp);
//        playerCharacterUI.SetTarget(player);
//        //playerCharacterUI.ResizeUI();

//        GameObject[] enemies = fightStatus.enemies.Keys.ToArray();
//        foreach (var enemy in enemies)
//        {
//            if (fightStatus.enemies[enemy].name == "Griffon")
//            {
//                GameObject griffonUIprefab = Instantiate(Resources.Load("Prefab/UI/GriffonUI"), characterUIGroup) as GameObject;
//                enemyCharacterUIs.Add(enemy, griffonUIprefab.GetComponent<GriffonUI>());
//                enemyCharacterUIs[enemy].SetName(fightStatus.enemies[enemy].name);
//                enemyCharacterUIs[enemy].SetMaxHp(fightStatus.enemies[enemy].maxHp);
//                enemyCharacterUIs[enemy].SetTarget(enemy);
//                enemyCharacterUIs[enemy].ResizeUI();
//            }
//            else
//            {
//                GameObject EnemyCharacterUIprefab = Instantiate(Resources.Load("Prefab/UI/EnemyCharacterUI"), characterUIGroup) as GameObject;
//                enemyCharacterUIs.Add(enemy, EnemyCharacterUIprefab.GetComponent<CharacterUI>());

//                enemyCharacterUIs[enemy].SetName(fightStatus.enemies[enemy].name);
//                enemyCharacterUIs[enemy].SetMaxHp(fightStatus.enemies[enemy].maxHp);
//                enemyCharacterUIs[enemy].SetTarget(enemy);
//            }
//        }
//    }
//    // 일시 정지할때 표시 안할 ui grouping
//    private void UIGrouping()
//    {
//        // 전투 중 active한 UI들
//        fightGroup.SetMember(joystickAttack.gameObject);
//        fightGroup.SetMember(joystickMove.gameObject);
//        //fightGroup.SetMember(joystickSkill.gameObject);

//        // 일시정지 중 active한 UI들
//        pauseGroup.SetMember(pauseImage.gameObject);

//        // status UI들
//        statusGroup.SetMember(parts.gameObject);
//        statusGroup.SetMember(playTimer.gameObject);
//        statusGroup.SetMember(pauseButton.gameObject);


//        SetStateChangeCallback(FightState.Pause, fightGroup.InactiveAllMembers);
//        SetStateChangeCallback(FightState.Pause, pauseGroup.ActiveAllMembers);

//        SetStateChangeCallback(FightState.Fight, fightGroup.ActiveAllMembers);
//        SetStateChangeCallback(FightState.Fight, pauseGroup.InactiveAllMembers);

//        SetStateChangeCallback(FightState.Dead, fightGroup.InactiveAllMembers);
//        SetStateChangeCallback(FightState.Dead, statusGroup.InactiveAllMembers);
//    }

//    // pause
//    public void OnClickPauseButton()
//    {
//        if (fightStateObserver.curFightState == FightState.Pause)
//        {
//            joystickSkill.gameObject.SetActive(activeSelf_JoystickSkill);
//            fightStateObserver.SetCurrentFightState(FightState.Fight);
//        }
//        else if (fightStateObserver.curFightState == FightState.Fight)
//        {
//            activeSelf_JoystickSkill = joystickSkill.gameObject.activeSelf;
//            joystickSkill.gameObject.SetActive(false);
//            fightStateObserver.SetCurrentFightState(FightState.Pause);
//        }
//    }

//    public void ChangeFightState(FightState state)
//    {
//        fightStateObserver.SetCurrentFightState(state);
//    }

//    // 전투 중 일시정지시에 처리할 콜백함수들 등록
//    public void SetStateChangeCallback(FightState fightState, Action func)
//    {
//        fightStateObserver.SetStateChangeCallback(fightState, func);
//    }
//    public void RemoveStateChangeCallback(FightState fightState, Action func)
//    {
//        fightStateObserver.RemoveStateChangeCallback(fightState, func);
//    }


//    // player control
//    public int GetPlayerATK()
//    {
//        return fightStatus.playerStatus.atk;
//    }
//    // 플레이어 이동
//    public void MovePlayer(Vector3 dir, float amount)
//    {
//        if (fightStateObserver.curFightState == FightState.Fight ||
//            fightStateObserver.curFightState == FightState.Clear_Stage)
//        {
//            player.Move(dir, amount);
//        }
//    }
//    // 플레이어 멈춤
//    public void StopPlayer()
//    {
//        if (fightStateObserver.curFightState == FightState.Fight ||
//            fightStateObserver.curFightState == FightState.Clear_Stage)
//        {
//            player.StopMove();
//        }
//    }
//    // 플레이어 공격
//    public void PlayerAttack(Vector3 dir)
//    {
//        if (fightStateObserver.curFightState == FightState.Fight ||
//            fightStateObserver.curFightState == FightState.Clear_Stage)
//        {
//            player.Attack(dir);
//        }
//    }
//    public void PlayerSkillReady(Vector3 dir)
//    {
//        if (fightStateObserver.curFightState == FightState.Fight ||
//            fightStateObserver.curFightState == FightState.Clear_Stage)
//        {
//            player.SkillReady(dir);
//        }
//    }
//    // 플레이어 스킬
//    public void PlayerSkill(Vector3 dir)
//    {
//        if (fightStateObserver.curFightState == FightState.Fight ||
//            fightStateObserver.curFightState == FightState.Clear_Stage)
//        {
//            player.SkillAttack(dir);
//        }
//    }
//    public void SkillButtonOn(WeaponType type, string key)
//    {
//        joystickSkill.gameObject.SetActive(true);
//        string spriteName = WeaponSkillTable.Instance.GetTuple(type.ToString(), key).m_spriteName;
//        joystickSkill.SkillOn(Resources.Load<Sprite>("Image/Skill/" + spriteName));
//    }

//    public void SkillButtonOff()
//    {
//        joystickSkill.gameObject.SetActive(false);
//    }
//    // 플레이어 공격 대기
//    public void PlayerStandby()
//    {
//        if (fightStateObserver.curFightState == FightState.Fight ||
//            fightStateObserver.curFightState == FightState.Clear_Stage)
//        {
//            player.Standby();
//        }
//    }
//    // 무기 변경
//    public void SwapWeapon()
//    {
//        WeaponType weapon = player.SwapWeapon();
//        joystickAttack.WeaponImageSwap(weapon);
//    }
//    public void LockMoveRotate()
//    {
//        player.isAttacking = true;
//    }
//    public void UnLockMoveRotate()
//    {
//        player.isAttacking = false;
//    }

//    public void LockNormalAttack()
//    {
//        joystickAttack.LockJoystick();
//    }
//    public void UnLockNormalAttack()
//    {
//        joystickAttack.UnLockJoystick();
//    }

//    // ---- hp
//    public void DamageToCharacter(GameObject character, int damage)
//    {
//        if (character == player.gameObject && GetCurrentFightState() != FightState.Dead)
//        {
//            playerCharacterUI.HpDown(damage);
//            bool isCharacterDead = fightStatus.playerStatus.DamageToCharacter(damage);
//            if (isCharacterDead)
//            {
//                PlayerDead();
//            }
//        }
//        else if (fightStatus.enemies.ContainsKey(character))
//        {
//            if (enemyCharacterUIs.ContainsKey(character))
//            {
//                enemyCharacterUIs[character].HpDown(damage);
//            }
//            bool isCharacterDead = fightStatus.enemies[character].DamageToCharacter(damage);
//            if (isCharacterDead)
//            {
//                EnemyDead(character);
//            }
//        }
//    }

//    public void DamageToCharacter(GameObject source, GameObject target, float gunDamage)
//    {
//        if (target == player.gameObject && GetCurrentFightState() != FightState.Dead)
//        {
//            int damage = fightStatus.enemies[source].atk;

//            playerCharacterUI.HpDown(damage);
//            bool isCharacterDead = fightStatus.playerStatus.DamageToCharacter(damage);
//            if (isCharacterDead)
//            {
//                PlayerDead();
//            }
//        }
//        else if (source == player.gameObject && target.activeSelf == true)
//        {
//            int damage = (int)gunDamage;
//            if (enemyCharacterUIs.ContainsKey(target))
//            {
//                enemyCharacterUIs[target].HpDown(damage);
//            }
//            bool isCharacterDead = fightStatus.enemies[target].DamageToCharacter(damage);
//            if (isCharacterDead)
//            {
//                EnemyDead(target);
//            }
//        }
//    }
//    public void HealToCharacter(GameObject character, int value)
//    {
//        if (character.tag == "Player")
//        {
//            playerCharacterUI.HpUp(value);
//        }
//        else if (character.tag == "Enemy")
//        {
//            if (enemyCharacterUIs.ContainsKey(character))
//            {
//                enemyCharacterUIs[character].HpUp(value);
//            }
//        }
//    }

//    public void PlayerDead()
//    {
//        ChangeFightState(FightState.Dead);
//        OffAllCharacterUI();
//        OffAllBullets();
//        SkillButtonOff();

//        player.PlayerDead();

//        //데이터처리
//        playTimer.StopTimer();
//        if (DataManager.Instance.GetPlayInfo.Stage != 1)
//        {
//            // 1층에서 뒤지면 저장안함
//            DataManager.Instance.SetPlayTime(playTimer.GetPlaytime());
//            DataManager.Instance.SetStage(DataManager.Instance.GetPlayInfo.Stage - 1);
//            DataManager.Instance.Save();
//        }

//        deadWindow.fightFog.SetFloat("_Slide", 0.2f);
//        deadWindow.gameObject.SetActive(true);
//        StartCoroutine(deadWindow.DeadHandler(OffAllEnemy));
//    }
//    public void EnemyDead(GameObject enemy)
//    {
//        // gain parts
//        int dropParts = fightStatus.enemies[enemy].dropParts;
//        fightStatus.AddParts(dropParts);
//        parts.text = fightStatus.gainParts.ToString();
//        ParticleManager.Instance.OnParticle(fightStatus.enemies[enemy].name + "Death", 2.0f, enemy.transform.position);

//        // enemy ui, character offf
//        fightStatus.RemoveEnemyInstance(enemy);
//        enemyCharacterUIs[enemy].gameObject.SetActive(false);
//        enemyCharacterUIs.Remove(enemy);

//        ObjectPoolManager.Instance.Free(enemy);
//    }

//    public void OffAllEnemy()
//    {
//        foreach (var enemy in fightStatus.enemies.Keys)
//        {
//            enemy.SetActive(false);
//        }
//    }
//    public void OffAllCharacterUI() // 죽었을 때
//    {
//        foreach (var enemy in fightStatus.enemies.Keys)
//        {
//            if (enemyCharacterUIs.ContainsKey(enemy))
//            {
//                enemyCharacterUIs[enemy].gameObject.SetActive(false);
//            }
//        }
//        playerCharacterUI.gameObject.SetActive(false);
//    }
//    // ---- hp


//    //----- bullet
//    public void AddBulletToList(GameObject bullet)
//    {
//        curUsingBullets.Add(bullet);
//    }
//    public void RemoveBulletFromList(GameObject bullet)
//    {
//        if (curUsingBullets.Contains(bullet))
//        {
//            curUsingBullets.Remove(bullet);
//        }
//    }
//    public void OffAllBullets()
//    {
//        foreach (var bullet in curUsingBullets)
//        {
//            ObjectPoolManager.Instance.Free(bullet);
//        }

//        curUsingBullets.Clear();
//    }
//    //----- bullet

//    public string GetEnemyName(GameObject enemy)
//    {
//        if (fightStatus.enemies.ContainsKey(enemy))
//        {
//            return fightStatus.enemies[enemy].name;
//        }
//        else
//        {
//            return "default";
//        }
//    }


//    public int RemainEnemyNumber()
//    {
//        return fightStatus.remainEnemy;
//    }
//    public FightState GetCurrentFightState()
//    {
//        return fightStateObserver.curFightState;
//    }
//    public void Retreat()
//    {
//        ChangeFightState(FightState.End);
//        OffAllBullets();
//        playTimer.StopTimer();
//        player.StopMove();

//        DataManager.Instance.SetPlayTime(playTimer.GetPlaytime());
//        DataManager.Instance.SetParts(fightStatus.gainParts);
//        DataManager.Instance.SetRemainHp(fightStatus.playerStatus.remainHp);
//        DataManager.Instance.SetStage(DataManager.Instance.GetPlayInfo.Stage - 1);

//        DataManager.Instance.Save();

//        GameManager.Instance.LoadNextScene(Constants.FightSceneName, Constants.UpgradeSceneName);
//    }

//    public void GoNext()
//    {
//        ChangeFightState(FightState.End);
//        OffAllBullets();
//        playTimer.StopTimer();
//        player.StopMove();

//        DataManager.Instance.SetPlayTime(playTimer.GetPlaytime());
//        DataManager.Instance.SetParts(fightStatus.gainParts);
//        DataManager.Instance.SetRemainHp(fightStatus.playerStatus.remainHp);
//        DataManager.Instance.SetAlreadyAct(false);


//        DataManager.Instance.Save();

//        GameManager.Instance.LoadNextScene(Constants.FightSceneName, Constants.UpgradeSceneName);
//    }

//    public void DungeonClear()
//    {
//        //5스테이지 클리어
//        resultWindow.OpenResultWindow();
//    }

//    public void EscapeDungeon()
//    {
//        //나중에 돈 추가해줘
//        DataManager.Instance.ResetDungeonData();
//        DataManager.Instance.Save();

//        GameManager.Instance.LoadNextScene(Constants.FightSceneName, Constants.WorldMapSceneName);
//    }
//}
