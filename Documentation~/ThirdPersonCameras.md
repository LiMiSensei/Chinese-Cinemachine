# 创建第三人称相机（Third Person Camera）

虽然可以（且在很多情况下适合）使用[自由视角相机（FreeLook Camera）](FreeLookCameras.md)制作第三人称相机，但在某些场景下，这种方式无法提供所需的全部控制权。例如，当你希望实现肩后偏移视角，或需要精确的瞄准控制（如在射击游戏中），并且希望在切换到瞄准相机的过程中保持这种控制时，使用自由视角相机往往难以维持预期的精度。

为解决这一问题，Cinemachine 提供了[第三人称跟随（Third Person Follow）](CinemachineThirdPersonFollow.md)行为。该行为的使用模式与自由视角相机不同：具体而言，第三人称相机（ThirdPersonCamera）会牢牢绑定到跟踪目标（Tracking Target）上；若要调整相机瞄准方向，必须旋转目标本身。无论相机是否按照装备设置（rig settings）相对于目标有轻微偏移，相机的前进方向始终与目标的前进方向保持一致。

这意味着相机本身不内置瞄准控制功能，该功能必须由目标提供。通常，这个目标会是玩家的一个不可见子对象，从而将玩家的瞄准方向与玩家模型的旋转解耦。关于如何实现这一机制，可参考以下 Cinemachine 示例：

- 带瞄准模式的第三人称（ThirdPersonWithAimMode）
- 带跟拍跑的第三人称（ThirdPersonWithRoadieRun）

这些示例使用了[第三人称瞄准扩展（Third Person Aim extension）](CinemachineThirdPersonAim.md)，该扩展通过射线检测（raycasts）确定相机的瞄准对象，并确保即使相机启用了程序化噪声（procedural noise），瞄准点也能锁定在屏幕中心。