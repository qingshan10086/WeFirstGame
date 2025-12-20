using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background1 : MonoBehaviour
{
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    public Vector2 parallaxFactor;
    private float textureUnitSizeX;
    private float textureUnitSizeY;
    public float verticalOffset = 6f; // 添加垂直偏移量

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
        textureUnitSizeY = texture.height / sprite.pixelsPerUnit;
        
        // 初始位置上抬6个单位
        transform.position = new Vector3(transform.position.x, transform.position.y + verticalOffset, transform.position.z);
    }

    private void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxFactor.x, deltaMovement.y * parallaxFactor.y, 0);
        lastCameraPosition = cameraTransform.position;
        if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
        {
            float offsetPositionX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
            transform.position = new Vector3(cameraTransform.position.x + offsetPositionX, transform.position.y, transform.position.z);
        }
        if (Mathf.Abs(cameraTransform.position.y - transform.position.y) >= textureUnitSizeY)
        {
            float offsetPositionY = (cameraTransform.position.y - transform.position.y) % textureUnitSizeY;
            // 在Y轴位置计算中加入垂直偏移量
            transform.position = new Vector3(transform.position.x, cameraTransform.position.y + offsetPositionY + verticalOffset, transform.position.z);
        }
    }
}
