# 俯视角游戏（Top-down games）

Cinemachine 相机（CinemachineCamera）的设计模拟了人类摄影师操作真实相机的方式。因此，这类相机对上下轴（up/down axis）较为敏感，且总会尽量避免在相机构图中引入侧滚（roll）效果。受这种敏感性影响，Cinemachine 相机会避免长时间直接朝上或朝下拍摄。短时间内的上下取景可能没问题，但如果“看向目标（Look At target）”长时间处于正上方或正下方，相机未必能始终呈现出理想效果。

若你正在制作相机需直接朝下拍摄的俯视角游戏，最佳实践是为相机重新定义“上方向（up direction）”。具体操作方式为：在 [Cinemachine 控制器（Cinemachine Brain）](CinemachineBrain.md) 中，将“世界上方向覆盖（World Up Override）”设置为一个游戏对象（GameObject）——该对象的局部上方向（local up）需与你期望的 Cinemachine 相机常规上方向一致。此“上方向”定义将适用于所有受该 Cinemachine 控制器控制的 Cinemachine 相机。