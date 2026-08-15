using UnityEngine;

public class CubeRunAway : MonoBehaviour
{
    [Header("玩家")]
    public Transform player;

    [Header("逃跑设置")]
    public float detectDistance = 5f;   // 玩家进入这个距离后开始逃跑
    public float moveSpeed = 3f;        // 逃跑速度

    void Update()
    {
        if (player == null)
            return;

        // 计算玩家和 Cube 的距离
        float distance = Vector3.Distance(transform.position, player.position);

        // 玩家进入检测范围
        if (distance <= detectDistance)
        {
            // 计算远离玩家的方向
            Vector3 runDirection = transform.position - player.position;

            // 如果游戏是在地面上移动，不让 Cube 往上下跑
            runDirection.y = 0f;

            runDirection.Normalize();

            // Cube 逃跑
            transform.position += runDirection * moveSpeed * Time.deltaTime;
        }
    }
}