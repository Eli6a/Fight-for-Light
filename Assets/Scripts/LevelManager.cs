using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public static int mainMenu = 0;
    public static int introduction = 1;
    public static int goodEnding = 2;
    public static int badEnding = 3;
    public static int levelIndexMin = 4;
    public static int levelIndexMax = 6;
    public static int[] levels = new int[3] {4, 5, 6};
 //   public static int bossLevel = 7;

    public static int currentLevel = mainMenu;
    public static int enemyLevel = 0;

    public static float playerAttack = 10;
    public static float playerHp = 30;

    public static bool angry;
    public static float enemyAttack = 5;
    public static float enemyHp = 15;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
    //        RandomizeLevel();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

  /*  void RandomizeLevel()
    {
        int random = Convert.ToInt32(Random.Range(levelIndexMin, levelIndexMax));
        for (int i = levelIndexMin; i <= levelIndexMax; i++)
        {
            if (random == i)
            {
                levels[0] = i;
                break;
            }
        }

        while (levels[1] == -1)
        {
            random = Convert.ToInt32(Random.Range(levelIndexMin, levelIndexMax));
            for (int i = levelIndexMin; i <= levelIndexMax; i++)
            {
                if (random == i && random != levels[0])
                {
                    levels[1] = i;
                    break;
                }
            }
        }
        
    }
*/
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenu);
        currentLevel = mainMenu;
    }
    public void LoadMainMenu(AudioManager audioManager)
    {
        LoadMainMenu();
        if (!audioManager.audioSource.isPlaying)
        audioManager.Play();
    }

    public void LoadNextLevel()
    {
        if (currentLevel == mainMenu)
        {
            SetEnemyAttackHP();
            currentLevel = introduction;
            SceneManager.LoadScene(currentLevel);
            angry = false;
        }
        else if (currentLevel == introduction)
        {
            currentLevel = levels[0];
            SceneManager.LoadScene(currentLevel);  
        }
        else if (currentLevel == levels[0])
        {
            enemyLevel++;
            SetEnemyAttackHP();
            currentLevel = levels[1];
            SceneManager.LoadScene(currentLevel);
        }
        else if (currentLevel == levels[1])
        {
            enemyLevel++;
            SetEnemyAttackHP();
            currentLevel = levels[2];
            SceneManager.LoadScene(currentLevel);
        }
        else if (currentLevel == levels[2])
        {
            if (angry)
            {
                SceneManager.LoadScene(badEnding);
                currentLevel = badEnding;
            }
            else
            {
                SceneManager.LoadScene(goodEnding);
                currentLevel = goodEnding;
            }
            
        }

    }
    public void TryAgain()
    {
        SceneManager.LoadScene(currentLevel);
    }

    public static void SetPlayerAttackHP(float attack, float hp)
    {
        playerAttack = attack;
        playerHp = hp;
    }

    static void SetEnemyAttackHP()
    {
        if (enemyLevel != 0)
        {
            enemyAttack += enemyAttack/4;
            enemyHp += enemyHp/4;
        }
    }

    public void ChangeAudioAkeboshi(AudioManager audioManager)
    {
        audioManager.GetComponent<AudioSource>().clip = audioManager.akeboshiClip;
        audioManager.GetComponent<AudioSource>().loop = true;
        LoadMainMenu(audioManager);
    }

}
