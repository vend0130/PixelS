using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class I
{
    public static GM gm;
    public static GameUI gui;
    public static AudioManager audioManager;
    public static CameraScript cameraScript;
    public static AdMob adMob;

    public static int currentLevel = 0;
    public static int progress = 0;
    public static bool audio = true;
    public static bool endGame = false;
    public static bool jiggle = true;
    public static int levelHelp = -1;
    public static string path = "/GameData.dat";

    public static string instagramWebLink = "http://instagram.com/uniquetouch75/";
    public static string instagramAppLink = "instagram://user?username=uniquetouch75";

    public static void Save()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = File.Create(Application.persistentDataPath + I.path);

        binaryFormatter.Serialize(fileStream, new GameData
        {
            currentLevel = I.currentLevel,
            progress = I.progress,
            audio = I.audio,
            endGame = I.endGame,
            jiggle = I.jiggle,
            levelHelp = I.levelHelp
        });

        fileStream.Close();
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + I.path))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = File.Open(Application.persistentDataPath + I.path, FileMode.OpenOrCreate);
            GameData gameData = (GameData)binaryFormatter.Deserialize(fileStream);

            fileStream.Close();

            I.currentLevel = gameData.currentLevel;
            I.progress = gameData.progress;
            I.audio = gameData.audio;
            I.endGame = gameData.endGame;
            I.jiggle = gameData.jiggle;
            I.levelHelp = gameData.levelHelp;
        }
    }

    public static void ClearSave()
    {
        using (Stream stream = File.Open(Application.persistentDataPath + I.path, FileMode.Create))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            GameData gameData = new GameData();
            binaryFormatter.Serialize(stream, gameData);
        }

        I.currentLevel = 0;
        I.progress = 0;
        I.audio = true;
        I.endGame = false;
        I.jiggle = true;
        I.levelHelp = -75;
    }
}

[System.Serializable]
public class GameData
{
    public int currentLevel = 0;
    public int progress = 0;
    public bool audio = true;
    public bool endGame = false;
    public bool jiggle = true;
    public int levelHelp = -75;
}
