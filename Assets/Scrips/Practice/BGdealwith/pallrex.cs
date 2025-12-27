
using UnityEngine;

public class pallrex : MonoBehaviour
{
    private GameObject mainCamera;
    private GameObject player;
    public float xparallaxEffect = 0.1f;
    public float yparallaxEffect = 0.1f;
    private Vector3 lastCameraPosition;
    private Vector3 lastTransformPosition;
    public bool sign=false;
    
    // 延迟相关变量
    private bool signWasActiveLastFrame = false;
    private float signActivationTime = 0f;
    private float delayBeforeEffect = 2f; // 2秒延迟
    private Vector3 beginPosition;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
      
        lastCameraPosition = mainCamera.transform.position;
        if (mainCamera == null)
        {
            Debug.LogError("未找到MainCamera标签的游戏对象");
        }
        
        // 初始化状态
        signWasActiveLastFrame = sign;
        beginPosition=transform.position;
        if (sign)
        {
            signActivationTime = Time.time;
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        // 检查sign状态变化
        if (sign && !signWasActiveLastFrame)
        {
            // sign变为true，记录激活时间
            signActivationTime = Time.time;
        }
        
        float xoffset = (lastCameraPosition.x - mainCamera.transform.position.x) * xparallaxEffect;
        float yoffset = (lastCameraPosition.y - mainCamera.transform.position.y) * yparallaxEffect;
        lastCameraPosition = mainCamera.transform.position;
        
        // 应用视差效果（如果sign为true且已经过了延迟时间）
        if (sign && (Time.time - signActivationTime) >= delayBeforeEffect)
        {
            transform.position = transform.position - new Vector3(xoffset, yoffset, 0);
        }
         if(!sign)
        {
            transform.position=beginPosition;
        }
        // 更新上一帧的状态
        signWasActiveLastFrame = sign;
    }
}