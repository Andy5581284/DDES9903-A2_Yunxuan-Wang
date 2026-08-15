using UnityEngine;
using System.Collections;

public class CountdownLookLeft : MonoBehaviour
{
    public Transform playerCamera;

    public float countdownTime = 3f;
    public float turnDuration = 0.8f; // 转向持续时间
    public float turnAngle = -90f;    // 左转90度

    void Start()
    {
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(countdownTime);

        yield return StartCoroutine(SmoothLookLeft());
    }

    IEnumerator SmoothLookLeft()
    {
        Quaternion startRotation = playerCamera.rotation;

        Quaternion targetRotation =
            startRotation * Quaternion.Euler(0f, turnAngle, 0f);

        float elapsed = 0f;

        while (elapsed < turnDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / turnDuration;

            // SmoothStep让开始和结束都更柔和
            t = Mathf.SmoothStep(0f, 1f, t);

            playerCamera.rotation =
                Quaternion.Slerp(startRotation, targetRotation, t);

            yield return null;
        }

        playerCamera.rotation = targetRotation;
    }
}