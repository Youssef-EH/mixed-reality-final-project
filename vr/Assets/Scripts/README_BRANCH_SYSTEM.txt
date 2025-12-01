=====================================
INTERACTIVE BRANCH PULLING SYSTEM
=====================================

VERSION: 1.1 (Fixed for XR Toolkit 3.2.1 & Unity 6)

OVERVIEW:
---------
This system allows VR users to grab and pull branches from trees using XR Interaction Toolkit.

RECENT FIXES (v1.1):
--------------------
✓ Fixed XR Interaction Toolkit 3.x API compatibility
✓ Updated event argument names (SelectEnterEventArgs)
✓ Verified Unity 6 Rigidbody API (linearDamping)
✓ All scripts now compile without errors

FILES CREATED:
--------------
Core Scripts:
1. SimpleBranch.cs - Easy-to-use pull detection (RECOMMENDED)
2. BranchPullInteraction.cs - Advanced force-based pulling
3. TreeBranchManager.cs - Manages multiple branches
4. BranchPullVisualizer.cs - Visual debugging feedback
5. QuickBranchSetup.cs - Auto-setup helper

Documentation:
6. BRANCH_PULLING_SETUP_GUIDE.txt - Detailed setup instructions
7. /Prefabs/BRANCH_PREFAB_GUIDE.txt - Prefab creation guide
8. XR_TOOLKIT_3_API_NOTES.txt - API compatibility notes
9. README_BRANCH_SYSTEM.txt - This file

=====================================
QUICK START (5 MINUTES)
=====================================

STEP 1: Create a Branch Object
-------------------------------
1. In Hierarchy, find your Tree01B GameObject
2. Right-click Tree01B → Create Empty
3. Name it "Branch_01"
4. Position it where you want a branch

STEP 2: Add Visual (Optional but recommended)
----------------------------------------------
1. Select Branch_01
2. Right-click → 3D Object → Cylinder
3. Scale the cylinder: X=0.05, Y=0.3, Z=0.05
4. Add a material if desired

STEP 3: Auto-Setup the Branch
------------------------------
1. Select Branch_01
2. Add Component → Quick Branch Setup
3. Check "Auto Setup Components"
4. Set "Tree Parent" to Tree01B (or leave blank - auto-detected)
5. Adjust "Pull Speed" if needed (lower = easier to pull)

STEP 4: Test in VR
------------------
1. Enter Play Mode
2. Put on your VR headset
3. Grab the branch with controller
4. Pull away from the tree - it should detach!


=====================================
HOW IT WORKS
=====================================

The system uses three key components:

1. XR Grab Interactable
   - Makes the branch grabbable in VR
   - Handles controller interaction

2. Rigidbody
   - Starts kinematic (attached to tree)
   - Becomes dynamic when pulled off

3. SimpleBranch Script
   - Monitors pull speed and direction
   - Detaches when threshold is reached
   - Adds physics impulse on detach


=====================================
CUSTOMIZATION
=====================================

Adjust Pull Difficulty:
-----------------------
In SimpleBranch component:
- Pull Strength Required: 
  * 15-20 = Very Easy
  * 25-35 = Medium (default: 30)
  * 40-60 = Hard
  * 60+ = Very Hard

- Detach Delay:
  * 0.1s = Instant
  * 0.3s = Default
  * 0.5s+ = Requires sustained pulling

Branch Physics:
---------------
In Rigidbody component:
- Mass: Weight of branch (0.2-1.0)
- Drag After Detach: Air resistance (0.5 default)

Visual Feedback:
----------------
Add BranchPullVisualizer component:
- Branch turns yellow when grabbed
- Turns red when pulling hard enough
- Shows pull direction in Scene view


=====================================
ADVANCED USAGE
=====================================

Multiple Branches:
------------------
Repeat steps for each branch, or use TreeBranchManager:
1. Create branch prefab (see BRANCH_PREFAB_GUIDE.txt)
2. Add TreeBranchManager to tree
3. Create spawn point empty objects
4. Assign prefab and spawn points

Custom Branch Models:
---------------------
Instead of Cylinder, use your own branch mesh:
1. Import 3D branch model
2. Place as child of Branch GameObject
3. Follow same setup steps

Force-Based Pulling:
--------------------
Use BranchPullInteraction.cs instead:
- More realistic force calculation
- Supports audio feedback
- Joint-based attachment


=====================================
TROUBLESHOOTING
=====================================

"Can't grab the branch"
→ Check XR Interaction Layers match
→ Ensure Collider is present and enabled
→ Verify XR Grab Interactable is enabled

"Branch detaches immediately when grabbed"
→ Increase Pull Strength Required
→ Increase Detach Delay

"Branch never detaches no matter how hard I pull"
→ Lower Pull Strength Required to 15-20
→ Check SimpleBranch script is enabled
→ Verify Rigidbody is set to kinematic initially

"Branch flies off into space"
→ Check "Use Gravity" is enabled on Rigidbody
→ Increase mass value
→ Check Physics settings in Project Settings

"Branch falls through the ground"
→ Ensure ground has a Collider
→ Check collision layers are set correctly


=====================================
NEXT STEPS
=====================================

Enhancements:
□ Add particle effects (leaves falling)
□ Add sound effects (wood snapping)
□ Add haptic feedback on detach
□ Count collected branches
□ Respawn branches after time
□ Different branch types/sizes
□ Branch breaking into smaller pieces

Integration:
□ Connect to scoring system
□ Add to inventory
□ Use branches as tools/weapons
□ Crafting system with branches


=====================================
TECHNICAL NOTES
=====================================

Unity Version: 6000.0
XR Interaction Toolkit: 3.2.1
Render Pipeline: URP 17.0.3

The system is designed to work with:
- Unity XR Interaction Toolkit
- Both hand controllers and ray interactors
- Kinematic and dynamic rigidbody physics
- Standard Unity collision system

Performance Considerations:
- Each branch adds 1 Rigidbody + 1 Collider
- Keep total branch count under 50 for VR
- Use LOD groups if branches have complex meshes
- Pool detached branches to reduce GC


=====================================
SUPPORT & RESOURCES
=====================================

Unity XR Interaction Toolkit Docs:
https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/

For additional help:
- Check BRANCH_PULLING_SETUP_GUIDE.txt for detailed setup
- Check BRANCH_PREFAB_GUIDE.txt for prefab creation
- Review script comments for parameter details

Happy tree pulling! 🌲
