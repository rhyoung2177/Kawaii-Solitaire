using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{   
    public static GameManager Instance { get; private set; }
    public Canvas canvas;
    public List<Space> spaceList;
    public Transform dragLayer; // 카드 이동 시 Space 뎁스보다 위쪽에 위치하게끔 하기 위함

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
