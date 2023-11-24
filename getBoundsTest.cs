using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getBoundsTest : MonoBehaviour
{

    [SerializeField]
    [Tooltip("生成するObject")]
    private GameObject item;

    [SerializeField]
    [Tooltip("Boundsの上面に生成するObject")]
    private GameObject boundsTop;

    [SerializeField]
    [Tooltip("Boundsの底面に生成するObject")]
    private GameObject boundsBottom;
    [SerializeField]
    [Tooltip("生成する数")]
    private int itemCount = 1;

    [SerializeField]
    [Tooltip("生成する範囲A(左下)")]
    private Transform rangeA;

    [SerializeField]
    [Tooltip("生成する範囲B(右上)")]
    private Transform rangeB;

    // アイテムのインスタンス
    List<GameObject> items = new List<GameObject>();
    List<GameObject> boundsTops = new List<GameObject>();
    List<GameObject> boundsBottoms = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            // アイテムを全削除
            foreach (var i in items)
            {
                Destroy(i);
            }
            items.Clear();
            // アイテムを全削除
            foreach (var i in boundsTops)
            {
                Destroy(i);
            }
            boundsTops.Clear();
            foreach (var i in boundsBottoms)
            {
                Destroy(i);
            }
            boundsBottoms.Clear();

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

                        // objectのバウンディングボックス取得
                        Bounds objectBounds = items[i].GetComponent<Renderer>().bounds;
                        Debug.Log($"objectBounds: {objectBounds}");
                        Vector3 topRight = objectBounds.max;
                        Vector3 bottomLeft = objectBounds.min;
                        boundsBottoms.Add(Instantiate(boundsBottom, bottomLeft, Quaternion.identity));
                        boundsBottoms.Add(Instantiate(boundsBottom, new Vector3(topRight.x, topRight.y, bottomLeft.z), Quaternion.identity));
                        boundsTops.Add(Instantiate(boundsTop, topRight, Quaternion.identity));
                        Debug.Log($"BoundsMin: {objectBounds.min}\nBoundsMax: {objectBounds.max}");

                        break;
                    }
                }

            }

        }
    }
}
