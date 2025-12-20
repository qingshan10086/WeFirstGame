using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasImageDistanceController : MonoBehaviour
{
    [Header("距离检测设置")]
    public disdanceCheck distanceCheckScript; // 现有的距离检测脚本
    
    public float maxDistance = 10f;   // 最大距离，超过此距离图片完全透明
    public float switchDistance = 3f; // 切换图片的距离阈值
    
    [Header("图片设置")]
    public Image image1;              // 第一张图片
    public Image image2;              // 第二张图片
    
    private float currentDistance;
    private bool isImage1Active = true;
    
    void Start()
    {
        // 设置初始图片状态
        if (image1 != null && image2 != null)
        {
            SwitchToImage1();
        }
    }
    
    void Update()
    {
        // 获取当前距离
        if (!GetCurrentDistance())
        {
            return;
        }
        
        // 更新图片透明度
        UpdateImageTransparency();
        
        // 检查是否需要切换图片
        CheckImageSwitch();
    }
    
    private bool GetCurrentDistance()
    {
        if (distanceCheckScript == null)
        {
            Debug.LogWarning("Distance check script not assigned!");
            return false;
        }
        
        // 使用现有的距离检测脚本获取距离
        currentDistance = Mathf.Abs(distanceCheckScript.getDistance());
        return true;
    }
    
    private void UpdateImageTransparency()
    {
        // 计算透明度 (距离越近透明度越高)
        float transparency = 1f - Mathf.Clamp01(currentDistance / maxDistance);
        
        // 更新当前激活图片的透明度
        Image activeImage = isImage1Active ? image1 : image2;
        if (activeImage != null)
        {
            activeImage.color = new Color(activeImage.color.r, activeImage.color.g, activeImage.color.b, transparency);
        }
    }
    
    private void CheckImageSwitch()
    {
        // 当距离小于切换阈值时
        if (currentDistance <= switchDistance)
        {
            // 如果当前显示的是第一张图片，切换到第二张
            if (isImage1Active)
            {
                SwitchToImage2();
            }
        }
        else
        {
            // 当距离大于切换阈值时
            // 如果当前显示的是第二张图片，切换回第一张
            if (!isImage1Active)
            {
                SwitchToImage1();
            }
        }
    }
    
    private void SwitchToImage1()
    {
        if (image1 != null)
        {
            image1.gameObject.SetActive(true);
        }
        if (image2 != null)
        {
            image2.gameObject.SetActive(false);
        }
        isImage1Active = true;
    }
    
    private void SwitchToImage2()
    {
        if (image1 != null)
        {
            image1.gameObject.SetActive(false);
        }
        if (image2 != null)
        {
            image2.gameObject.SetActive(true);
        }
        isImage1Active = false;
    }
    
    // 获取当前距离的公共方法
    public float getCurrentDistance()
    {
        return currentDistance;
    }
}