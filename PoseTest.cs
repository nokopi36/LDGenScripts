using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseTest : MonoBehaviour
{
    [SerializeField]
    [Tooltip("ポーズを変えたいモデル")]
    private GameObject model;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Z
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SetRandomPose(model);
        }
    }

    // ランダムなポーズを設定
    public void SetRandomPose(GameObject character)
    {
        SetMaterialsToModels(character);
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
            return;
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
        }
    }

    public void SetMaterialsToModels(GameObject parentObject)
    {
        // 肌の色
        Transform bodyTransform = parentObject.transform.Find("Body");
        if (bodyTransform != null)
        {
            Renderer bodyRenderer = bodyTransform.GetComponent<Renderer>();
            if (bodyRenderer != null)
            {
                Color32 color1 = new Color32(255, 245, 240, 255);
                Color32 color2 = new Color32(250, 190, 150, 255);
                float lerpFactor = Random.Range(0f, 1f);
                Color32 randomColor32 = Color32.Lerp(color1, color2, lerpFactor);
                Color randomColor = randomColor32;
                Material newMaterial = new Material(Shader.Find("Standard"));
                newMaterial.color = randomColor;

                bodyRenderer.material = newMaterial;
            }
        }

        // 髪の毛
        Transform hairTransform = parentObject.transform.Find("Hair");
        if (hairTransform != null)
        {
            Renderer hairRenderer = hairTransform.GetComponent<Renderer>();
            if (hairRenderer != null)
            {
                Color32 color1 = new Color32(0, 0, 0, 255);
                Color32 color2 = new Color32(116, 80, 48, 255);
                float lerpFactor = Random.Range(0f, 1f);
                Color32 randomColor32 = Color32.Lerp(color1, color2, lerpFactor);
                Color randomColor = randomColor32;
                Material newMaterial = new Material(Shader.Find("Standard"));
                newMaterial.color = randomColor;

                hairRenderer.material = newMaterial;
            }
        }

        // 髭
        Transform beardTransform = parentObject.transform.Find("Beard");
        if (beardTransform != null)
        {
            Renderer beardRenderer = beardTransform.GetComponent<Renderer>();
            if (beardRenderer != null)
            {
                Color32 color1 = new Color32(0, 0, 0, 255);
                Color32 color2 = new Color32(116, 80, 48, 255);
                float lerpFactor = Random.Range(0f, 1f);
                Color32 randomColor32 = Color32.Lerp(color1, color2, lerpFactor);
                Color randomColor = randomColor32;
                Material newMaterial = new Material(Shader.Find("Standard"));
                newMaterial.color = randomColor;

                beardRenderer.material = newMaterial;
            }
        }

        // 上半身の服
        Transform sweaterTransform = parentObject.transform.Find("Tops");
        if (sweaterTransform != null)
        {
            Renderer sweaterRenderer = sweaterTransform.GetComponent<Renderer>();
            if (sweaterRenderer != null)
            {
                // Color32 color1 = new Color32(255, 245, 240, 255);
                // Color32 color2 = new Color32(250, 190, 150, 255);
                // float lerpFactor = Random.Range(0f, 1f);
                // Color32 randomColor32 = Color32.Lerp(color1, color2, lerpFactor);
                // Color randomColor = randomColor32;
                Color randomColor = new Color(Random.value, Random.value, Random.value);
                Material newMaterial = new Material(Shader.Find("Standard"));
                newMaterial.color = randomColor;

                sweaterRenderer.material = newMaterial;
            }
        }

        // 下半身の服
        Transform pantsTransform = parentObject.transform.Find("Pants");
        if (pantsTransform != null)
        {
            Renderer pantsRenderer = pantsTransform.GetComponent<Renderer>();
            if (pantsRenderer != null)
            {
                // Color32 color1 = new Color32(255, 245, 240, 255);
                // Color32 color2 = new Color32(250, 190, 150, 255);
                // float lerpFactor = Random.Range(0f, 1f);
                // Color32 randomColor32 = Color32.Lerp(color1, color2, lerpFactor);
                // Color randomColor = randomColor32;
                Color randomColor = new Color(Random.value, Random.value, Random.value);
                Material newMaterial = new Material(Shader.Find("Standard"));
                newMaterial.color = randomColor;

                pantsRenderer.material = newMaterial;
            }
        }

        // 靴
        Transform shoesTransform = parentObject.transform.Find("Shoes");
        if (shoesTransform != null)
        {
            Renderer shoesRenderer = shoesTransform.GetComponent<Renderer>();
            if (shoesRenderer != null)
            {
                // Color32 color1 = new Color32(200, 200, 200, 255);
                // Color32 color2 = new Color32(0, 0, 0, 255);
                // float lerpFactor = Random.Range(0f, 1f);
                // Color32 randomColor32 = Color32.Lerp(color1, color2, lerpFactor);
                // Color randomColor = randomColor32;
                Color randomColor = new Color(Random.value, Random.value, Random.value);
                Material newMaterial = new Material(Shader.Find("Standard"));
                newMaterial.color = randomColor;

                shoesRenderer.material = newMaterial;
            }
        }
    }

}
