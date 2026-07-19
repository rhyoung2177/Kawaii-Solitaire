using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    [SerializeField] private Transform popupRoot;

    private void Awake()
    {
        Instance = this;
    }

    public Popup OpenPopup()
    {
        Popup popup = Resources.Load<Popup>("Prefab/Popup");

        if (popup == null)
        {
            Debug.LogError($"Popup을 찾을 수 없습니다.");
            return null;
        }

        return Instantiate(popup, popupRoot);
    }
}