# BloodMusicManager 使用说明

## 1. 系统概述

BloodMusicManager是一个功能全面的Unity音乐管理系统，专为2D平台游戏设计，提供背景音乐控制、音效管理、音量控制和淡入淡出效果等核心功能。

### 主要特性：
- 单例模式设计，全局访问
- 背景音乐播放、暂停、停止、切换
- 音效对象池管理
- 音量控制（主音量、音乐音量、音效音量）
- 平滑淡入淡出效果
- 事件驱动架构
- 音频剪辑预加载
- 与Unity AudioMixer集成

## 2. 快速开始

### 2.1 安装与设置

1. **创建管理器对象**：
   - 在场景中创建一个空物体，命名为"BloodMusicManager"
   - 将`BloodMusicManager.cs`脚本添加到该物体上

2. **配置组件**：
   - 为`backgroundMusicSource`字段分配一个AudioSource组件（用于背景音乐）
   - 音效源将自动创建，无需手动配置

3. **（可选）配置音频混合器**：
   - 创建AudioMixer并分配给`audioMixer`字段
   - 在AudioMixer中创建三个参数：
     - MasterVolume (0-1)
     - MusicVolume (0-1) 
     - SFXVolume (0-1)

### 2.2 基础使用

```csharp
// 播放背景音乐（带淡入效果）
BloodMusicManager.Instance.PlayBackgroundMusic(backgroundMusicClip, true);

// 播放音效
BloodMusicManager.Instance.PlaySoundEffect(soundEffectClip);

// 设置音量
BloodMusicManager.Instance.SetMasterVolume(0.8f);
BloodMusicManager.Instance.SetMusicVolume(0.6f);
BloodMusicManager.Instance.SetSFXVolume(0.9f);
```

## 3. 核心功能

### 3.1 背景音乐控制

#### 播放背景音乐
```csharp
// 参数1：音乐剪辑
// 参数2：是否淡入（可选，默认false）
BloodMusicManager.Instance.PlayBackgroundMusic(musicClip, true);
```

#### 停止背景音乐
```csharp
// 参数1：是否淡出（可选，默认false）
BloodMusicManager.Instance.StopBackgroundMusic(true);
```

#### 暂停/恢复
```csharp
// 暂停
BloodMusicManager.Instance.PauseBackgroundMusic();

// 恢复
BloodMusicManager.Instance.ResumeBackgroundMusic();
```

#### 切换背景音乐
```csharp
// 参数1：新音乐剪辑
// 参数2：淡入淡出持续时间（可选，默认1秒）
BloodMusicManager.Instance.SwitchBackgroundMusic(newMusicClip, 2f);
```

### 3.2 音效管理

#### 播放音效
```csharp
// 参数1：音效剪辑
// 参数2：音量（可选，默认1f）
// 参数3：音调（可选，默认1f）
BloodMusicManager.Instance.PlaySoundEffect(soundClip, 0.8f, 1.2f);
```

#### 通过名称播放
```csharp
// 需先预加载音效剪辑
BloodMusicManager.Instance.PreloadAudioClips(jumpClip, attackClip, hitClip);

// 通过名称播放
BloodMusicManager.Instance.PlaySoundEffect("jumpClip", 0.9f);
```

### 3.3 音量控制

#### 设置音量
```csharp
// 设置主音量（0-1范围）
BloodMusicManager.Instance.SetMasterVolume(0.8f);

// 设置音乐音量
BloodMusicManager.Instance.SetMusicVolume(0.6f);

// 设置音效音量
BloodMusicManager.Instance.SetSFXVolume(0.9f);
```

#### 音量转换说明
- 内部将线性音量(0-1)转换为对数音量(dB)用于AudioMixer
- 0对应-80dB（静音），1对应0dB（最大音量）

### 3.4 淡入淡出效果

#### 自定义淡入淡出
```csharp
// 自定义淡入淡出持续时间
float customFadeDuration = 3f;
BloodMusicManager.Instance.SwitchBackgroundMusic(newMusicClip, customFadeDuration);
```

#### 手动控制淡入淡出
```csharp
// 示例：实现自定义淡入淡出逻辑
IEnumerator CustomFadeSequence(AudioClip newClip)
{
    // 淡出当前音乐
    BloodMusicManager.Instance.StopBackgroundMusic(true);
    yield return new WaitForSeconds(BloodMusicManager.Instance.fadeDuration);
    
    // 延迟一下
    yield return new WaitForSeconds(1f);
    
    // 淡入新音乐
    BloodMusicManager.Instance.PlayBackgroundMusic(newClip, true);
}
```

## 4. 事件系统

BloodMusicManager提供事件机制，可用于监听音乐状态变化：

```csharp
private void OnEnable()
{
    // 注册事件
    BloodMusicManager.OnBackgroundMusicStarted += OnMusicStarted;
    BloodMusicManager.OnBackgroundMusicStopped += OnMusicStopped;
    BloodMusicManager.OnBackgroundMusicPaused += OnMusicPaused;
    BloodMusicManager.OnVolumeChanged += OnVolumeChanged;
    BloodMusicManager.OnSoundEffectPlayed += OnSoundEffectPlayed;
}

private void OnDisable()
{
    // 取消注册
    BloodMusicManager.OnBackgroundMusicStarted -= OnMusicStarted;
    BloodMusicManager.OnBackgroundMusicStopped -= OnMusicStopped;
    BloodMusicManager.OnBackgroundMusicPaused -= OnMusicPaused;
    BloodMusicManager.OnVolumeChanged -= OnVolumeChanged;
    BloodMusicManager.OnSoundEffectPlayed -= OnSoundEffectPlayed;
}

// 事件处理方法
private void OnMusicStarted()
{
    Debug.Log("背景音乐开始播放");
}

private void OnVolumeChanged()
{
    Debug.Log("音量已改变");
}
```

## 5. 音频剪辑管理

### 5.1 预加载音频剪辑

```csharp
// 预加载多个音频剪辑
BloodMusicManager.Instance.PreloadAudioClips(
    level1Music,
    level2Music,
    jumpSound,
    attackSound,
    hitSound
);
```

### 5.2 注册/获取音频剪辑

```csharp
// 注册单个音频剪辑
BloodMusicManager.Instance.RegisterAudioClip("customSound", customSoundClip);

// 获取音频剪辑
AudioClip clip = BloodMusicManager.Instance.GetAudioClip("customSound");
if (clip != null)
{
    BloodMusicManager.Instance.PlaySoundEffect(clip);
}
```

## 6. 最佳实践

### 6.1 性能优化

1. **预加载常用音频**：在游戏开始或场景加载时预加载常用音频
2. **合理设置音效池大小**：根据游戏需求调整`maxSoundEffectSources`
3. **避免频繁切换音乐**：频繁切换会增加CPU负担，尽量减少不必要的切换
4. **使用适当的音量**：避免将音量设置过高，影响游戏体验

### 6.2 与游戏系统集成

1. **场景切换**：
   ```csharp
   // 场景切换时切换音乐
   public void LoadNextScene(int sceneIndex)
   {
       BloodMusicManager.Instance.SwitchBackgroundMusic(nextSceneMusic, 1.5f);
       SceneManager.LoadScene(sceneIndex);
   }
   ```

2. **Boss战**：
   ```csharp
   // Boss战开始
   public void OnBossBattleStart()
   {
       BloodMusicManager.Instance.SwitchBackgroundMusic(bossBattleMusic, 2f);
   }

   // Boss战结束
   public void OnBossBattleEnd()
   {
       BloodMusicManager.Instance.SwitchBackgroundMusic(normalMusic, 2f);
   }
   ```

3. **玩家动作**：
   ```csharp
   // 玩家跳跃时播放音效
   public void OnJump()
   {
       BloodMusicManager.Instance.PlaySoundEffect(jumpSound, 0.8f, 1.1f);
   }
   ```

### 6.3 调试与测试

- 使用`Play Test Sound`上下文菜单测试系统功能
- 使用`Reset Volumes`上下文菜单重置音量设置
- 利用示例脚本`BloodMusicManagerExample.cs`学习系统使用

## 7. 常见问题

### 7.1 没有声音
- 检查AudioSource组件是否已启用
- 检查音量设置是否正确
- 确保音频剪辑已正确导入

### 7.2 音效播放不出来
- 检查音效池大小是否足够
- 确保音频剪辑已预加载或正确传递

### 7.3 淡入淡出效果不明显
- 增加`fadeDuration`参数值
- 检查音频剪辑的音量设置

## 8. 版本历史

### v1.0.0 (2025-12-24)
- 初始版本
- 实现核心功能：背景音乐控制、音效管理、音量控制
- 添加淡入淡出效果
- 实现事件系统
- 支持音频剪辑预加载

## 9. 扩展建议

1. **添加音乐播放列表**：支持按顺序或随机播放多个音乐剪辑
2. **实现音乐书签**：支持在特定时间点播放音乐
3. **添加环境音效**：支持3D空间音效
4. **实现音频可视化**：添加音频波形显示
5. **支持多语言音效**：根据游戏语言切换音效

---

**注意**：使用前请确保已在Unity中导入所有必要的音频剪辑，并设置正确的音频格式（建议使用MP3或OGG格式）。