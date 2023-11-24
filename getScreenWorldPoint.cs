using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class getScreenWorldPoint : MonoBehaviour
{

    [SerializeField]
    [Tooltip("生成するObject")]
    private GameObject item;

    private Camera mainCamera;
    private string saveTxt;
    [SerializeField]
    [Tooltip("保存先ディレクトリ名")]
    private string screenshotPath = "ScreenShots";

    [SerializeField]
    [Tooltip("右上と左下にObjectを生成するか否か(デフォルト値false)")]
    private bool PlaceObjectEnabled = false;

    // アイテムのインスタンス
    List<GameObject> items = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found.");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 左クリック
        if (Input.GetMouseButtonDown(0))
        {

            float depth = mainCamera.transform.position.y;
            Vector3 bottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, depth));
            Vector3 topRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, depth));

            Vector3 screenBottomLeft = mainCamera.WorldToScreenPoint(bottomLeft);
            Vector3 screenTopRight = mainCamera.WorldToScreenPoint(topRight);
            // Debug.Log($"screenBottomLeft: {screenBottomLeft}\nscreenTopRight: {screenTopRight}");
            // Debug.Log($"bottomLeft: {bottomLeft}\ntopRight: {topRight}");

            if (PlaceObjectEnabled)
            {
                // アイテムを全削除
                foreach (var i in items)
                {
                    Destroy(i);
                }
                items.Clear();

                items.Add(Instantiate(item, bottomLeft, Quaternion.identity));
                items.Add(Instantiate(item, topRight, Quaternion.identity));
            }

            saveTxt = $"{topRight.x - bottomLeft.x} {topRight.z - bottomLeft.z} screenWidth{Screen.width} screenHeight{Screen.height} depth{depth}";
            File.WriteAllText($"{screenshotPath}/WorldPoint.txt", saveTxt);
        }
    }
}
