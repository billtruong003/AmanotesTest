using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    [System.Serializable]
    public class SettingConfig
    {
        [Range(0f, 1f)] public float volumeMaster = 1f;
        [Range(0f, 1f)] public float volumeVFX = 1f;
        public KeyCode keyBinding = KeyCode.Mouse0; // Gán phím mặc định
        public AudioClip bgm = null;
        public Sprite bgImage = null;
    }

    public SettingConfig settings = new SettingConfig();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Đảm bảo GameDataManager không bị hủy khi chuyển scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Các hàm khác của GameDataManager (nếu có)
}