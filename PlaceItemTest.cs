using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlaceItemTest : MonoBehaviour
{
    [SerializeField]
    [Tooltip("生成するObject")]
    private GameObject item;

    [SerializeField]
    [Tooltip("生成する数")]
    private int itemCount = 30;

    [SerializeField]
    [Tooltip("生成する範囲A(左下)")]
    private Transform rangeA;

    [SerializeField]
    [Tooltip("生成する範囲B(右上)")]
    private Transform rangeB;

    [SerializeField]
    [Tooltip("撮影する画像の横")]
    private int width = 1352;
    [SerializeField]
    [Tooltip("撮影する画像の縦")]
    private int height = 1013;

    [SerializeField]
    [Tooltip("保存先ディレクトリ名")]
    private string screenshotPath = "ScreenShots";

    [SerializeField]
    [Tooltip("座標を保存するか否か(デバッグ用)")]
    private bool saveTxtEnabled = true;

    // アイテムのインスタンス
    List<GameObject> items = new List<GameObject>();

    private string saveTxt;

    private Camera mainCamera;

    private Bounds objectBounds;

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
            // ScreenShotCapture();
            // アイテムを全削除
            foreach (var i in items)
            {
                Destroy(i);
            }
            items.Clear();
            saveTxt = "";

            float depth = mainCamera.transform.position.y;
            Vector3 bottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, depth));
            Vector3 topRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, depth));
            rangeA.position = bottomLeft;
            rangeB.position = topRight;

            // ボックスサイズの半分
            Vector3 halfExtents = new Vector3(0.5f, 0.5f, 0.5f);

            // アイテムを作る
            for (int i = 0; i < itemCount; i++)
            {
                // 10回試す
                for (int n = 0; n < 10; n++)
                {
                    // ランダムの位置
                    float x = Random.Range(rangeA.position.x, rangeB.position.x);
                    float y = Random.Range(rangeA.position.y, rangeB.position.y);
                    float z = Random.Range(rangeA.position.z, rangeB.position.z);
                    Vector3 pos = new Vector3(x, y, z);

                    // ボックスとアイテムが重ならないとき
                    if (!Physics.CheckBox(pos, halfExtents, Quaternion.identity, 1 << 12))
                    {
                        // アイテムをインスタンス化
                        items.Add(Instantiate(item, pos, Quaternion.identity));

                        // カメラのメインカメラを使用してワールド座標からスクリーン座標に変換
                        Camera mainCamera = Camera.main;
                        Vector3 screenPosition = mainCamera.WorldToScreenPoint(pos);
                        saveTxt += $"{screenPosition}\n";
                        // Debug.Log($"screenPosition:{screenPosition}");

                        // objectのバウンディングボックス取得
                        objectBounds = items[i].GetComponent<Renderer>().bounds;
                        AnnotateObject(objectBounds, mainCamera);
                        CalculateBoundingBox(items[i], mainCamera);
                        // Debug.Log($"objectBounds: {objectBounds}");
                        // Debug.Log($"BoundsMin: {objectBounds.min}\nBoundsMax: {objectBounds.max}");

                        Vector3 objectBoundsSize = objectBounds.size;
                        Vector3 objectCenter = objectBounds.center;
                        Vector3 screenObjectCenter = mainCamera.WorldToScreenPoint(objectCenter);
                        Vector3 screenBoundsMin = mainCamera.WorldToScreenPoint(objectBounds.min);
                        Vector3 screenBoundsMax = mainCamera.WorldToScreenPoint(objectBounds.max);

                        Debug.Log($"screenBoundsMin: {screenBoundsMin}\nscreenBoundsMax: {screenBoundsMax}");
                        // Debug.Log($"objectCenter: {objectCenter}");
                        // Debug.Log($"screenObjectCenter: {screenObjectCenter}");
                        // Debug.Log($"objectBoundsSize: {objectBoundsSize}");

                        string result = NormalizePosition(screenObjectCenter, screenBoundsMin, screenBoundsMax);
                        Debug.Log($"result: {result}");

                        break;
                    }
                }

            }

            // ScreenShotCapture();

            if (saveTxtEnabled)
            {
                File.WriteAllText($"{screenshotPath}/Screenshot_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt", saveTxt);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(objectBounds.center, objectBounds.size);
    }

    // 配置した人の場所を0から1に正規化
    public string NormalizePosition(Vector3 objectCenter, Vector3 boundsMin, Vector3 boundsMax)
    {

        float bgWidth = Screen.width;
        float bgHeight = Screen.height;
        // Debug.Log($"bgWidth: {bgWidth}, bgHeight: {bgHeight}");

        float objectWidth = boundsMax.x - boundsMin.x;
        float objectHeight = (bgHeight - boundsMin.y) - (bgHeight - boundsMax.y);
        // Debug.Log($"objectWidth: {objectWidth}, objectHeight: {objectHeight}");

        float normalizedX = objectCenter.x / bgWidth;
        float normalizedY = (bgHeight - objectCenter.y) / bgHeight;
        float normalizedWidth = objectWidth / bgWidth;
        float normalizedHeight = objectHeight / bgHeight;

        string result = $"0 {normalizedX} {normalizedY} {normalizedWidth} {normalizedHeight}\n";

        return result;
    }

    public void ScreenShotCapture()
    {
        // スクリーンショットを撮影
        RenderTexture rt = new RenderTexture(width, height, 24);
        rt.Create();
        Camera camera = GetComponent<Camera>();
        camera.targetTexture = rt;

        camera.Render();
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        // camera.Render();
        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        camera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        // スクリーンショットを保存
        byte[] bytes = screenshot.EncodeToPNG();
        string screenshotFileName = $"{screenshotPath}/Screenshot_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
        Directory.CreateDirectory(screenshotPath);
        File.WriteAllBytes(screenshotFileName, bytes);
    }

    private void AnnotateObject(Bounds bounds, Camera perspectiveCamera)
    {
        // スクリーン空間でのバウンディングボックスのコーナーを取得
        Vector3[] screenCorners = new Vector3[8];
        int i = 0;
        foreach (Vector3 corner in GetBoundingBoxCorners(bounds))
        {
            screenCorners[i++] = perspectiveCamera.WorldToScreenPoint(corner);
        }

        // スクリーン空間での四角形を作るために必要な点を計算
        Vector2 min = screenCorners[0];
        Vector2 max = screenCorners[0];
        foreach (Vector3 corner in screenCorners)
        {
            min = Vector2.Min(min, (Vector2)corner);
            max = Vector2.Max(max, (Vector2)corner);
        }

        // WidthとHeightを計算
        float width = max.x - min.x;
        float height = max.y - min.y;


        // Debug.Log($"minviewport: {min.x}, {min.y}, 0");

        // YOLOアノテーション形式への変換は、ビューポート座標を使用する必要があるため、
        // ここでスクリーン座標をビューポート座標に変換します。
        Vector2 minViewport = perspectiveCamera.ScreenToViewportPoint(new Vector3(min.x, Screen.height - min.y, 0));
        Vector2 maxViewport = perspectiveCamera.ScreenToViewportPoint(new Vector3(max.x, Screen.height - max.y, 0));

        // YOLO形式のバウンディングボックス（中心点とサイズ）
        Vector2 center = (minViewport + maxViewport) / 2;
        Vector2 size = maxViewport - minViewport;

        // YOLOアノテーション形式の出力
        // Debug.Log($"Center: {center.x} {1 - center.y}, Size: {size.x} {1 - size.y}");
        Debug.Log($"0 {center.x} {center.y} {size.x} {size.y}");
    }

    // バウンディングボックスの8つのコーナーを計算するメソッド
    private Vector3[] GetBoundingBoxCorners(Bounds bounds)
    {
        return new Vector3[] {
            bounds.min,
            new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.min.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.min.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.max.y, bounds.min.z),
            bounds.max
        };
    }

    private void CalculateBoundingBox(GameObject obj, Camera camera)
    {
        Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        // スクリーン上での最小・最大の座標を初期化
        Vector3 minScreen = new Vector3(float.MaxValue, float.MaxValue, 0);
        Vector3 maxScreen = new Vector3(float.MinValue, float.MinValue, 0);

        foreach (Vector3 vertex in vertices)
        {
            // ワールド座標に変換
            Vector3 worldPoint = obj.transform.TransformPoint(vertex);

            // スクリーン座標に変換
            Vector3 screenPoint = camera.WorldToScreenPoint(worldPoint);

            // 最小・最大のスクリーン座標を更新
            minScreen = Vector3.Min(minScreen, screenPoint);
            maxScreen = Vector3.Max(maxScreen, screenPoint);
        }

        // バウンディングボックスの座標を正規化
        Rect boundingBox = new Rect(
            minScreen.x / Screen.width,
            (Screen.height - maxScreen.y) / Screen.height, // Y座標を反転
            (maxScreen.x - minScreen.x) / Screen.width,
            (maxScreen.y - minScreen.y) / Screen.height
        );
        float xCenter = boundingBox.x + boundingBox.width / 2;
        float yCenter = boundingBox.y + boundingBox.height / 2;
        string text = $"0 {xCenter} {yCenter} {boundingBox.width} {boundingBox.height}";

        Debug.Log($"Bounding Box: {boundingBox}, text: {text}");
    }

}
