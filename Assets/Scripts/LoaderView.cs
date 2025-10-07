using System.Collections;
// using DG.Tweening;
using UnityEngine;

public class LoaderView : MonoBehaviour
{
    private Coroutine _loadingCoroutine;

    private bool _canRotate;

    private float _rotationSpeed = 100f;


    private void Update()
    {
        RotateObject(_rotationSpeed);
    }

    private void RotateObject(float rotationSpeed)
    {
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}
