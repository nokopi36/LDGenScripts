using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LDGenerator : MonoBehaviour
{
    [SerializeField]
    [Tooltip("地面のObject")]
    private GameObject groundObject;

    [SerializeField]
    [Tooltip("生成するObject")]
    private GameObject detectObject;

    [SerializeField]
    [Tooltip("生成する数")]
    private int genDetectObject = 30;

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
    [Tooltip("座標を保存するか否か(デフォルト値true)")]
    private bool saveTxtEnabled = true;

    [SerializeField]
    [Tooltip("障害物を配置するかどうか(デフォルト値true)")]
    private bool obstacleEnabled = true;

    // アイテムのインスタンス
    List<GameObject> detectObjects = new List<GameObject>();
    List<GameObject> obstacleObjects = new List<GameObject>();

    private string saveTxt;

    private Camera mainCamera;

    string obstaclesPrefix = "Assets/prefabs/obstacles/";
    string obstaclesSuffix = ".prefab";
    private string[] obstaclesName = { "Tree9_2", "Tree9_3", "Tree9_4", "Tree9_5" };
    string[] obstacles;
    string groundTexturesPrefix = "Assets/prefabs/groundTextures/";
    string groundTexturesSuffix = ".mat";
    private string[] groundTexturesName = { "g1", "g2", "g3", "g4", "g5", "g6", "g7", "g8", "g9", "g10", "g11" };
    string[] groundTextures;


    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found.");
            return;
        }
        obstacles = CombineStrings(obstaclesPrefix, obstaclesSuffix, obstaclesName);
        groundTextures = CombineStrings(groundTexturesPrefix, groundTexturesSuffix, groundTexturesName);
    }

    // Update is called once per frame
    void Update()
    {
        // 左クリック
        if (Input.GetMouseButtonDown(0))
        {
            ApplyRandomMaterial();
            // ScreenShotCapture();
            // アイテムを全削除
            foreach (var i in detectObjects)
            {
                DestroyImmediate(i);
            }
            detectObjects.Clear();
            foreach (var j in obstacleObjects)
            {
                DestroyImmediate(j);
            }
            obstacleObjects.Clear();
            saveTxt = "";

            float depth = mainCamera.transform.position.y;
            Vector3 bottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, depth));
            Vector3 topRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, depth));
            rangeA.position = bottomLeft;
            rangeB.position = topRight;

            // ボックスサイズの半分
            Vector3 halfExtents = new Vector3(0.5f, 0.5f, 0.5f);

            // アイテムを作る
            for (int i = 0; i < genDetectObject; i++)
            {
                // 10回試す
                for (int n = 0; n < 10; n++)
                {
                    // ランダムの位置
                    float x = Random.Range(rangeA.position.x, rangeB.position.x);
                    float y = Random.Range(rangeA.position.y, rangeB.position.y);
                    float z = Random.Range(rangeA.position.z, rangeB.position.z);
                    Vector3 pos = new Vector3(x, y, z);
                    Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 359), 0);

                    // ボックスとアイテムが重ならないとき
                    if (!Physics.CheckBox(pos, halfExtents, rotation, 1 << 12))
                    {
                        // アイテムをインスタンス化
                        // detectObjects.Add(Instantiate(detectObject, pos, rotation));

                        // SetRandomPose(detectObjects[i]);

                        detectObjects.Add(Instantiate(SetRandomPose(detectObject), pos, rotation));

                        // カメラのメインカメラを使用してワールド座標からスクリーン座標に変換
                        // Camera mainCamera = Camera.main;
                        // Vector3 screenPosition = mainCamera.WorldToScreenPoint(pos);
                        // Debug.Log($"screenPosition:{screenPosition}");

                        // objectのバウンディングボックス取得
                        // saveTxt += $"{CalculateBoundingBox(detectObjects[i], mainCamera)}";
                        saveTxt += $"{CalculateSkinnedMeshRendererBoundingBox(detectObjects[i], mainCamera)}";

                        break;
                    }
                }

            }

            if (obstacleEnabled)
            {
                for (int i = 0; i < Random.Range(1, 10); i++)
                {
                    string randomObstacle = obstacles[Random.Range(0, obstacles.Length)];

                    Addressables.LoadAssetAsync<GameObject>(randomObstacle).Completed += (AsyncOperationHandle<GameObject> handle) =>
                    {
                        GameObject prefab = handle.Result;
                        if (handle.Status == AsyncOperationStatus.Succeeded)
                        {
                            // 10回試す
                            for (int n = 0; n < 10; n++)
                            {
                                // ランダムの位置
                                float x = Random.Range(rangeA.position.x, rangeB.position.x);
                                float y = Random.Range(rangeA.position.y, rangeB.position.y);
                                float z = Random.Range(rangeA.position.z, rangeB.position.z);
                                Vector3 pos = new Vector3(x, y, z);
                                Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 359), 0);

                                // ボックスとアイテムが重ならないとき
                                if (!Physics.CheckBox(pos, halfExtents, Quaternion.identity, 1 << 12))
                                {
                                    // アイテムをインスタンス化
                                    obstacleObjects.Add(Instantiate(prefab, pos, rotation));

                                    break;
                                }
                            }
                        }
                    };

                }
            }

            ScreenShotCapture();

            if (saveTxtEnabled)
            {
                File.WriteAllText($"{screenshotPath}/Screenshot_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt", saveTxt);
            }
        }
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
        DestroyImmediate(rt);

        // スクリーンショットを保存
        byte[] bytes = screenshot.EncodeToPNG();
        string screenshotFileName = $"{screenshotPath}/Screenshot_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
        Directory.CreateDirectory(screenshotPath);
        File.WriteAllBytes(screenshotFileName, bytes);
    }

    // MeshFilterを持っている場合
    // private string CalculateBoundingBox(GameObject obj, Camera camera)
    // {
    //     Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
    //     Vector3[] vertices = mesh.vertices;

    //     // スクリーン上での最小・最大の座標を初期化
    //     Vector3 minScreen = new Vector3(float.MaxValue, float.MaxValue, 0);
    //     Vector3 maxScreen = new Vector3(float.MinValue, float.MinValue, 0);

    //     foreach (Vector3 vertex in vertices)
    //     {
    //         // ワールド座標に変換
    //         Vector3 worldPoint = obj.transform.TransformPoint(vertex);

    //         // スクリーン座標に変換
    //         Vector3 screenPoint = camera.WorldToScreenPoint(worldPoint);

    //         // 最小・最大のスクリーン座標を更新
    //         minScreen = Vector3.Min(minScreen, screenPoint);
    //         maxScreen = Vector3.Max(maxScreen, screenPoint);
    //     }

    //     // バウンディングボックスの座標を正規化
    //     Rect boundingBox = new Rect(
    //         minScreen.x / Screen.width,
    //         (Screen.height - maxScreen.y) / Screen.height, // Y座標を反転
    //         (maxScreen.x - minScreen.x) / Screen.width,
    //         (maxScreen.y - minScreen.y) / Screen.height
    //     );
    //     float xCenter = boundingBox.x + boundingBox.width / 2;
    //     float yCenter = boundingBox.y + boundingBox.height / 2;
    //     string text = $"0 {xCenter} {yCenter} {boundingBox.width} {boundingBox.height}\n";

    //     // Debug.Log($"Bounding Box: {boundingBox}, text: {text}");

    //     return text;
    // }

    // SkinnedMeshRendererを持っている場合
    private string CalculateSkinnedMeshRendererBoundingBox(GameObject obj, Camera camera)
    {
        Vector3 minScreen = new Vector3(float.MaxValue, float.MaxValue, 0);
        Vector3 maxScreen = new Vector3(float.MinValue, float.MinValue, 0);
        string saves = "";

        // すべてのSkinnedMeshRendererコンポーネントを取得
        SkinnedMeshRenderer[] skinnedMeshRenderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
        {
            // 現在のスキンドメッシュの状態を反映したメッシュを生成
            Mesh bakedMesh = new Mesh();
            skinnedMeshRenderer.BakeMesh(bakedMesh);

            foreach (Vector3 vertex in bakedMesh.vertices)
            {
                // ワールド座標に変換
                Vector3 worldPoint = skinnedMeshRenderer.transform.TransformPoint(vertex);

                // スクリーン座標に変換
                Vector3 screenPoint = camera.WorldToScreenPoint(worldPoint);

                // 最小・最大のスクリーン座標を更新
                minScreen = Vector3.Min(minScreen, screenPoint);
                maxScreen = Vector3.Max(maxScreen, screenPoint);
            }
            // 生成した一時メッシュの破棄
            DestroyImmediate(bakedMesh);
        }

        // スクリーン座標でのバウンディングボックスを算出
        Rect boundingBox = new Rect(
            minScreen.x / Screen.width,
            (Screen.height - maxScreen.y) / Screen.height,
            (maxScreen.x - minScreen.x) / Screen.width,
            (maxScreen.y - minScreen.y) / Screen.height
        );
        float xCenter = boundingBox.x + boundingBox.width / 2;
        float yCenter = boundingBox.y + boundingBox.height / 2;
        string text = $"0 {xCenter} {yCenter} {boundingBox.width} {boundingBox.height}\n";

        return text;
    }

    // この関数を呼び出してランダムなポーズを設定
    public GameObject SetRandomPose(GameObject character)
    {
        // キャラクターの全てのボーンを取得
        // 例として、Animatorコンポーネントを使用してボーンを取得
        Animator animator = character.GetComponent<Animator>();

        Quaternion LeftUpLeg = Quaternion.Euler(180, 180, 0);
        Quaternion RightUpLeg, LeftUpArm, RightUpArm;
        float LeftUpLegX = 0;
        float LeftUpArmX = 0;
        float LeftUpArmZ = 0;
        float RightUpArmX = 0;
        float RightUpArmZ = 0;

        if (animator == null)
        {
            Debug.LogError("Animator component not found on the character.");
            return character;
        }

        // animator.GetBoneTransform(HumanBodyBones.),
        Transform[] targetBones = {
            animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg),
            animator.GetBoneTransform(HumanBodyBones.RightUpperLeg),
            animator.GetBoneTransform(HumanBodyBones.LeftUpperArm),
            animator.GetBoneTransform(HumanBodyBones.RightUpperArm)
        };

        // ボーンのTransformコンポーネントを反復処理
        foreach (Transform bone in targetBones)
        {
            if (bone.name.Contains("LeftUpLeg"))
            {
                // 左ふともも
                LeftUpLegX = Random.Range(150, 210); // 前後の回転
                float yRot = Random.Range(180, 190); // 外側と内側への回転
                float zRot = 0; // 左太ももはZ軸周りにはほとんど回転しません
                LeftUpLeg = Quaternion.Euler(LeftUpLegX, yRot, zRot);
                Debug.Log($"LeftUpLeg:{LeftUpLeg}");

                bone.localRotation = LeftUpLeg;
            }
            else if (bone.name.Contains("RightUpLeg"))
            {
                // 右ふともも
                float xRot = Random.Range(150, 210); // 前後の回転
                float yRot = Random.Range(180, 190); // 外側と内側への回転
                float zRot = 0; // 左太ももはZ軸周りにはほとんど回転しません

                RightUpLeg = Quaternion.Euler((360 - LeftUpLegX), yRot, zRot);

                bone.localRotation = RightUpLeg;
            }
            else if (bone.name.Contains("LeftArm"))
            {
                // 左上腕
                LeftUpArmX = Random.Range(50, 65); // 正面から見たときの開閉
                float yRot = Random.Range(-5, 5); // 腕のひねり
                if (LeftUpLegX > 180) // 横から見たときの前後
                {
                    LeftUpArmZ = Random.Range(0, 50);
                }
                else
                {
                    LeftUpArmZ = Random.Range(-50, 0);
                }

                LeftUpArm = Quaternion.Euler(LeftUpArmX, yRot, LeftUpArmZ);

                bone.localRotation = LeftUpArm;
            }
            else if (bone.name.Contains("RightArm"))
            {
                // 右上腕
                RightUpArmX = LeftUpArmX;
                float yRot = Random.Range(-5, 5); // 腕のひねり
                RightUpArmZ = LeftUpArmZ;

                RightUpArm = Quaternion.Euler(RightUpArmX, yRot, RightUpArmZ);

                bone.localRotation = RightUpArm;
                Debug.Log($"LeftUpArmZ:{LeftUpArmZ}, RightUpArmZ:{RightUpArmZ}");
            }
            // ボーンごとに異なる回転範囲を設定するための関数を呼び出し
            // bone.localRotation = GetRandomRotationForBone(bone);
            // bone.rotation = GetRandomRotationForBone(bone);
        }

        return character;
    }

    // ボーンごとに適切な回転範囲を定義
    Quaternion GetRandomRotationForBone(Transform bone)
    {
        // 例: すべてのボーンに対して同じ範囲を使用
        // 実際にはボーンの種類に応じて範囲を調整することが望ましい
        float minAngle = -20f;
        float maxAngle = 20f;

        float xRot = Random.Range(minAngle, maxAngle);
        float yRot = Random.Range(minAngle, maxAngle);
        float zRot = Random.Range(minAngle, maxAngle);

        return Quaternion.Euler(xRot, yRot, zRot);
    }

    public string[] CombineStrings(string prefix, string suffix, string[] items)
    {
        string[] combinedArray = new string[items.Length];

        for (int i = 0; i < items.Length; i++)
        {
            combinedArray[i] = prefix + items[i] + suffix;
        }

        return combinedArray;
    }

    public void ApplyRandomMaterial()
    {
        // Color randomColor = new Color(Random.value, Random.value, Random.value);
        // Debug.Log($"{randomColor}");
        // Material newMaterial = new Material(Shader.Find("Standard"));

        Renderer groundRenderer = groundObject.GetComponent<Renderer>();
        string randomGroundTexture = groundTextures[Random.Range(0, groundTextures.Length)];
        Addressables.LoadAssetAsync<Material>(randomGroundTexture).Completed += (AsyncOperationHandle<Material> handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Material loadedMaterial = handle.Result;
                groundRenderer.material = loadedMaterial;
            }
        };
        // newMaterial.color = randomColor;
        // groundRenderer.material = newMaterial;
    }

}
