using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScreenShotCapture : MonoBehaviour
{
    [SerializeField]
    [Tooltip("撮影する画像の横")]
    private int width = 1352;
    [SerializeField]
    [Tooltip("撮影する画像の縦")]
    private int height = 1013;
    [SerializeField]
    [Tooltip("保存先ディレクトリ名")]
    private string screenshotPath = "ScreenShots";

    // [SerializeField]
    // private string saveTxt = "aaa";
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            // スクリーンショットを撮影
            RenderTexture rt = new RenderTexture(width, height, 24);
            Camera camera = GetComponent<Camera>();
            camera.targetTexture = rt;

            Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
            camera.Render();
            RenderTexture.active = rt;
            screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            camera.targetTexture = null;
            RenderTexture.active = null;
            rt.Release();
            // Destroy(rt);

            // スクリーンショットを保存
            byte[] bytes = screenshot.EncodeToPNG();
            string screenshotFileName = $"{screenshotPath}/Screenshot_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
            Directory.CreateDirectory(screenshotPath);
            File.WriteAllBytes(screenshotFileName, bytes);

            // File.WriteAllText($"{screenshotPath}/Screenshot_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt", saveTxt);
        }
    }
}
