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
            // animator.GetBoneTransform(HumanBodyBones.),
            // animator.GetBoneTransform(HumanBodyBones.),
            // animator.GetBoneTransform(HumanBodyBones.Hips)
        };

        // Debug.Log($"{targetBones[0]}");

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
                float xRot = Random.Range(150, 210); // 前後の回転
                float yRot = Random.Range(180, 190); // 外側と内側への回転
                float zRot = 0; // 左太ももはZ軸周りにはほとんど回転しません

                RightUpLeg = Quaternion.Euler((360 - LeftUpLegX), yRot, zRot);

                bone.localRotation = RightUpLeg;
            }
            else if (bone.name.Contains("LeftArm"))
            {
                LeftUpArmX = Random.Range(50, 65); // 正面から見たときの開閉
                float yRot = Random.Range(-5, 5); // 腕のひねり
                if (LeftUpLegX > 180)
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
    }

    // ボーンごとに適切な回転範囲を定義
    Quaternion GetRandomRotationForBone(Transform bone)
    {
        // 例: すべてのボーンに対して同じ範囲を使用
        // 実際にはボーンの種類に応じて範囲を調整することが望ましい
        float minAngle = -90f;
        float maxAngle = 90f;

        // float xRot = Random.Range(minAngle, maxAngle);
        // float yRot = Random.Range(minAngle, maxAngle);
        // float zRot = Random.Range(minAngle, maxAngle);

        // 左ふともも
        float xRot = Random.Range(150, 210); // 前後の回転
        float yRot = Random.Range(180, 190); // 外側と内側への回転
        float zRot = 0; // 左太ももはZ軸周りにはほとんど回転しません

        return Quaternion.Euler(xRot, yRot, zRot);
    }
}
