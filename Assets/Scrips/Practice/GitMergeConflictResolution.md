# Git 合并冲突解决方案

## 问题描述
项目设置文件发生了Git合并冲突，具体在以下文件：
1. `ProjectSettings/Physics2DSettings.asset` - 碰撞矩阵冲突
2. `ProjectSettings/TagManager.asset` - 层设置冲突

## 解决方法

### 1. 解决 Physics2DSettings.asset 冲突

**冲突内容：**
```
<<<<<<< HEAD
  m_LayerCollisionMatrix: bfffffffffffffffffffffffffffffffffffffffffffffff7effffff3ffeffff7fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff
=======
  m_LayerCollisionMatrix: bfffffffffffffffffffffffffffffffffffffffffffffff7efeffff3ffeffff3fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff
>>>>>>> 76dd36b86fc7aca599e3d9f821357cea3dbaa606
```

**解决步骤：**
1. 打开 `ProjectSettings/Physics2DSettings.asset` 文件
2. 删除 Git 冲突标记（<<<<<<< HEAD, =======, >>>>>>> commit-hash）
3. 选择保留其中一个版本的碰撞矩阵值
   - 建议保留 HEAD 版本（上面的版本），因为它与现有项目兼容性更好

### 2. 解决 TagManager.asset 冲突

**冲突内容：**
```
<<<<<<< HEAD
  - EnemyTrigger
  - trap
=======
  - "障碍物"
  - 
>>>>>>> 76dd36b86fc7aca599e3d9f821357cea3dbaa606
```

**解决步骤：**
1. 打开 `ProjectSettings/TagManager.asset` 文件
2. 删除 Git 冲突标记
3. 合并层设置，保留所有需要的层：
```yaml
  - EnemyTrigger
  - trap
  - "障碍物"
  - 
```

### 3. 完成合并

1. 保存修改后的文件
2. 在 Git 中标记冲突已解决：
```bash
git add ProjectSettings/Physics2DSettings.asset ProjectSettings/TagManager.asset
git commit -m "Resolve merge conflicts in project settings"
```

## 注意事项

1. **备份重要文件**：在修改项目设置文件前，建议先备份这些文件
2. **测试游戏功能**：解决冲突后，务必测试游戏的物理碰撞和层交互功能
3. **提交前检查**：确保所有 Git 冲突标记都已删除

## 替代方法

如果您不熟悉手动编辑 YAML 文件，可以：

1. 使用 Unity 编辑器打开项目
2. 手动重新配置物理设置和层设置
3. 然后在 Git 中提交更改

## 后续建议

为避免未来出现类似冲突：
1. 定期合并主分支到您的开发分支
2. 避免多人同时修改相同的项目设置
3. 对项目设置的修改进行详细的提交说明

如果您需要进一步的帮助，请随时咨询！