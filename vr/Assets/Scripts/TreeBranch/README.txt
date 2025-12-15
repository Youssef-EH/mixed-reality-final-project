VR BRANCH PULL SYSTEM - SETUP INSTRUCTIONS
==========================================

This folder contains a clean implementation for VR branch pulling.

FILES IN THIS FOLDER:
--------------------
1. VRBranchPull.cs - Main script handling the branch pulling interaction

SETUP STEPS:
-----------

STEP 1: Remove Old Components from Branch01
--------------------------------------------
Select the Branch01 GameObject (/Tree01B/Branch01) and remove these old components:
- BranchPullInteraction
- BranchDebugger  
- BranchJointSetup

Keep these components:
- Transform
- Rigidbody
- XRGrabInteractable
- CapsuleCollider

STEP 2: Add New Component
--------------------------
1. Select Branch01 GameObject
2. Click "Add Component" in the Inspector
3. Search for "VRBranchPull" and add it

STEP 3: Configure Branch01 Components
--------------------------------------
Make sure your components are configured as follows:

RIGIDBODY:
- Mass: 0.5 (or any lightweight value)
- Drag: 0
- Angular Drag: 0.05
- Use Gravity: OFF (will be enabled after detaching)
- Is Kinematic: OFF
- Collision Detection: Continuous

XRGRABINTERACTABLE:
- Interaction Manager: XR Interaction Manager (auto-assigned)
- Interaction Layer Mask: Default
- Colliders: Should include the CapsuleCollider
- Movement Type: Velocity Tracking (will be set by script)

CAPSULECOLLIDER:
- Make sure it covers the branch properly
- Is Trigger: OFF

STEP 4: Configure VRBranchPull Settings
----------------------------------------
In the VRBranchPull component:

PULL SETTINGS:
- Pull Distance Threshold: 0.15 (how far to pull before it counts)
- Pull Time Required: 1.5 (seconds to hold the pull)
- Max Stretch Distance: 0.3 (maximum stretch before breaking)

PHYSICS:
- Joint Spring Strength: 500 (how strong the branch "snaps back")
- Joint Damping: 50 (smoothness of the spring)
- Detach Force: 5 (impulse force when branch breaks)

AUDIO (Optional):
- Snap Sound: Add a sound for when grabbing starts
- Detach Sound: Add a sound for when branch breaks off

DEBUG:
- Enable Debug Logs: ON (to see what's happening in Console)
- Show Gizmos: ON (to see visual indicators in Scene view)

STEP 5: Verify Tree01B Setup
-----------------------------
Select Tree01B GameObject and verify:

RIGIDBODY:
- Mass: 10 or higher (should be heavier than branch)
- Use Gravity: ON or OFF (your choice)
- Is Kinematic: Can be ON if tree shouldn't move

Make sure Branch01 is a CHILD of Tree01B in the hierarchy!

STEP 6: Test in Play Mode
--------------------------
1. Enter Play Mode (or build to VR headset)
2. Grab the branch with your VR controller (grip button)
3. Pull the branch away from the tree
4. Hold it stretched for the required time (default 1.5 seconds)
5. The branch should break off!

TROUBLESHOOTING:
---------------

Problem: Branch doesn't grab
Solution: Check that XRGrabInteractable is enabled and collider is not a trigger

Problem: Branch detaches immediately
Solution: Increase "Pull Distance Threshold" or "Pull Time Required"

Problem: Branch never detaches
Solution: Decrease "Pull Distance Threshold" or "Pull Time Required"
         Check Console logs to see pull progress

Problem: Branch is too stretchy
Solution: Increase "Joint Spring Strength" value

Problem: Branch is too stiff
Solution: Decrease "Joint Spring Strength" value

Problem: Joint breaks on grab
Solution: Make sure "Break Force" is set to Infinity on the ConfigurableJoint

CUSTOMIZATION TIPS:
------------------
- Adjust Pull Distance Threshold based on your branch size
- Adjust Pull Time Required to make it easier (lower) or harder (higher)
- Add particle effects when branch detaches
- Add different audio clips for wood cracking sounds
- Try different Joint Spring/Damping values for different "feel"

HOW IT WORKS:
------------
1. On Start, a ConfigurableJoint is created connecting the branch to the tree
2. When grabbed, the script tracks the distance from the attachment point
3. If pulled beyond the threshold for the required time, the branch detaches
4. The joint is destroyed and the branch becomes a free physics object
5. Gravity is enabled and a small force is applied for realistic motion
