# 安装与升级（Installation and Upgrade）

Cinemachine 是一款免费包，可用于任何项目。

如需安装此包，请遵循 [包管理器文档](https://docs.unity3d.com/Manual/upm-ui.html) 中的说明操作。

> [!提示]
> 安装完成后，会新增一个 **游戏对象（GameObject）> Cinemachine** 菜单。通过该菜单，可根据需求添加 [预构建的 Cinemachine 相机](ui-ref-pre-built-cameras.md)。


## 安装要求（Installation Requirements）

此版本的 Cinemachine 兼容以下版本的 Unity 编辑器：
* 2022.3 LTS 及更高版本

Cinemachine 的外部依赖项极少，只需安装即可开始使用。如果您还通过 HDRP（高清渲染管线）或 URP（通用渲染管线）体积使用后期处理功能，Cinemachine 会提供适配模块——这些模块由 `ifdef` 指令保护，当检测到相关依赖项存在时，指令会自动定义。

对于时间线（Timeline）、UGUI 等其他包，Cinemachine 也提供了类似的、由 `ifdef` 指令保护的行为。


## Cinemachine 项目升级（Cinemachine Project Upgrade）

如果您的项目使用早期版本的 Cinemachine，且需要更新到最新版本，请参考下表中的链接。

> [!警告]
> 相较于 Cinemachine 2.x 及更早版本，Cinemachine 3.x 的架构包含大量不兼容变更。虽然可以将现有项目从 Cinemachine 2.x 升级到 3.x，但您应慎重考虑是否愿意投入相应的工作成本。

| 章节（Section） | 说明（Description） |
| :--- | :--- |
| [将项目从 Cinemachine 2.x 升级](CinemachineUpgradeFrom2.md) | 适用于当前使用 Cinemachine 2.x 的项目的操作指南。 |
| [从 Asset Store 版本的 Cinemachine 升级](CinemachineUpgradeFromAssetStore.md) | 适用于当前使用 Asset Store 旧版 Cinemachine（2.x 之前版本）的项目的操作指南。 |


## 其他资源（Additional Resources）
* [Cinemachine 2.x 与 3.x 之间的 API 变更](whats-new.md#major-api-changes)