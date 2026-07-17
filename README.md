# Wildlife VR Rescue

**Rescue wildlife. Restore the rainforest. Protect the forest.**

Wildlife VR Rescue is a student-built virtual reality conservation experience developed in Unity for Meta Quest. Players enter a damaged Southeast Asian rainforest reserve, rescue a displaced Malayan tapir, restore damaged vegetation, control forest fires, and help return the animal to a safer habitat.

The project was created for **MMD 6246 – Immersive Media Design 4** and is designed for players aged **12–15**.

> **Academic project notice:** This is an educational prototype inspired by a WWF endangered-species design brief. It is not an official WWF product or commercial release.

---

## Project Status

The repository contains the final playable academic prototype. The original exo-mech-arm concept was simplified during production because the custom arm rig was not reliable in Unity VR. The current version uses standard VR hands while retaining the rescue tools, mission systems, environmental interactions, and conservation theme.

The completed prototype focuses on one full **Malayan tapir rescue-and-restoration mission**. The Indochinese tiger, Sumatran elephant, and lar gibbon are represented as supporting habitat or nursery content and as potential future mission expansions.

---

## Core Gameplay Loop

1. Begin at the Emergency HQ / Rescue Nursery.
2. Review the mission information and enter the Tapir Rescue Zone.
3. Deploy the Rescue Drone to locate the tapir and reveal the safe route.
4. Remove fallen trunks and clear the animal’s path.
5. Use the Water Cannon to extinguish nearby forest fires.
6. Guide the rescued tapir toward the Rescue Carrier.
7. Use the Eco-Seed Launcher to restore damaged planting zones.
8. Complete the restoration objective and continue to the release sequence.

---

## Main Features

- Fully immersive first-person VR gameplay
- Meta Quest controller support
- Smooth movement, snap rotation, jumping, grabbing, and teleportation
- Mission board, tutorial prompts, instruction panels, and scene transitions
- Rescue Drone scanning, tracking, and animal-guidance sequence
- Physics-based trunk interaction with fade/removal feedback
- Water Cannon with particle collision, looping sound, and fire-extinguishing logic
- Eco-Seed Launcher with a bio-restoration beam and tree-restoration objectives
- Malayan tapir animation, navigation, rescue, and release behaviour
- Environmental audio, background music, tool sound effects, and random animal calls
- Multiple connected scenes for HQ, rescue, restoration, and release gameplay
- Southeast Asian rainforest environments featuring four endangered species

---

## Rescue Equipment

### Water Cannon

The Water Cannon produces a continuous stream of purified water used to extinguish forest fires and protect recovering habitats. Its internal system automatically regenerates water, so the player does not need to return to a refill source during the mission.

**Use:** Aim at a fire and hold the right Trigger until the flames are extinguished.

### Eco-Seed Launcher

The Eco-Seed Launcher projects a bio-restoration beam containing microscopic native seed material, nutrients, and beneficial soil organisms. When aimed at a valid restoration target, it repairs the damaged soil and rapidly grows new vegetation.

**Use:** Aim at a highlighted planting target and hold the right Trigger until restoration is complete.

### Rescue Drone

The Rescue Drone scans the surrounding area, tracks the Malayan tapir, travels toward its location, and helps guide it along a safe path to the Rescue Carrier.

**Use:** Deploy the drone, follow it to the tapir, free the animal, and allow the drone to guide it toward safety.

---

## Meta Quest Controls

### Right Controller

| Input | Action |
|---|---|
| **B** | Open or close the equipment inventory |
| **A** | Jump |
| **Grip** | Hold an equipped tool or grab an object |
| **Trigger** | Use the equipped tool |
| **Right Thumbstick** | Rotate the player view |

### Left Controller

| Input | Action |
|---|---|
| **X** | Activate teleportation and teleport to the selected point |
| **Y** | Cancel the current teleport selection |
| **Grip** | Grab or interact with nearby objects |
| **Left Thumbstick** | Move through the environment |

---

## Technology

- **Engine:** Unity
- **Target headset:** Meta Quest 2 / compatible Quest devices
- **XR runtime:** OpenXR
- **VR framework:** XR Interaction Toolkit
- **Input:** Unity Input System and XR controller input
- **Rendering:** Universal Render Pipeline
- **Navigation:** Unity NavMesh / AI Navigation
- **Primary language:** C#

Use the Unity editor version recorded in:

```text
ProjectSettings/ProjectVersion.txt
```

Opening the project with a different Unity version may trigger package, shader, input, or rendering issues.

---

## Getting Started

### Requirements

- Unity Hub
- The Unity editor version specified by the project
- Android Build Support for Quest APK builds
- A Meta Quest headset with Developer Mode enabled
- A compatible USB cable or wireless development connection
- Git LFS, if the repository stores large binary assets through LFS

### Clone the Repository

```bash
git clone <repository-url>
cd <repository-folder>
```

When Git LFS is used:

```bash
git lfs install
git lfs pull
```

### Open in Unity

1. Open Unity Hub.
2. Select **Add project from disk**.
3. Choose the cloned project folder.
4. Open it using the version listed in `ProjectSettings/ProjectVersion.txt`.
5. Wait for Unity to restore packages and import all assets.
6. Open the HQ / Rescue Nursery start scene or use the scenes already arranged in the project’s Build Profiles.

### Test the Project

For accurate controller input, interaction, audio, and headset tracking, test using a connected Meta Quest headset. Editor-only testing may not reproduce every XR interaction correctly.

---

## Building for Meta Quest

1. Install **Android Build Support**, including the SDK, NDK, and OpenJDK, through Unity Hub.
2. Open **File → Build Profiles**.
3. Select or create an **Android** build profile.
4. Confirm that the project scenes are included in the correct gameplay order.
5. Confirm that OpenXR and the Quest-compatible interaction profile are enabled in XR Plug-in Management.
6. Connect the Quest headset and confirm that USB debugging is allowed.
7. Select **Build and Run** to install the APK directly, or **Build** to export an APK file.

The project is intended for an Android-based Quest build. Build settings may need to be rechecked after opening the project on a new computer.

---

## Important Gameplay Systems

The prototype includes mission and interaction systems such as:

- `VRSceneTeleporter` for controller-confirmed scene transitions
- Water Cannon particle activation and fire collision detection
- Fire target and fire sound management
- Grab-triggered trunk fading and removal
- Rescue Drone travel, scan effects, and timed behaviour
- Tapir animation-state and NavMesh movement logic
- Tree restoration targets and mission completion counting
- Mission-complete UI, delayed scene loading, and release progression
- Random animal sound playback and scene-specific background music

Script names and object names may vary slightly between branches or later revisions.

---

## Repository Structure

A standard Unity project structure is used:

```text
Assets/           Game scenes, scripts, prefabs, models, materials, audio, and UI
Packages/         Unity package dependencies
ProjectSettings/  Unity editor, input, graphics, XR, and build settings
```

Generated folders such as `Library`, `Temp`, `Logs`, `obj`, and local build folders should not be committed to Git.

For large `.fbx`, `.blend`, audio, video, APK, or Unity package files, use Git LFS or attach final builds through GitHub Releases instead of committing oversized files directly.

---

## Known Limitations

- The custom exo-mech arms shown in early concept material were removed from the final implementation and replaced with standard VR hands.
- The Malayan tapir mission is the most complete playable mission in the current prototype.
- Other endangered species and habitats have limited interactions compared with the Tapir Rescue Zone.
- Performance depends on scene complexity, vegetation density, lighting, particle effects, and the target Quest device.
- Some third-party models and environment assets may require their original licence or attribution files to remain with the project.

---

## Team

**Group 2**

- **Geuan Jun Wei** — Unity game logic, terrain and scene integration, UI systems, mission scripting, Tapir Rescue and restoration progression
- **Derrick Gow** — VR mechanics, locomotion, interaction systems, rescue tools, and Meta Quest integration
- **Lau Ming Kang** — 3D environments and props, rigging and animation support, editing and compositing
- **Muhammad Amir Ashwar** — Original concept, art direction, tool design, asset sourcing and optimisation, UI design, and presentation development

---

## Credits and Asset Usage

This project may include third-party models, textures, audio, and Unity assets obtained from sources such as Sketchfab or the Unity Asset Store. All third-party content remains subject to its original creator’s licence.

Before redistributing or publishing the project, verify that:

- Required creator credits are included.
- Non-redistributable source assets are removed.
- Asset-store packages are not shared outside the licence terms.
- WWF names, logos, and branding are not presented as an official partnership or endorsement.

---

## Educational Goal

Wildlife VR Rescue uses hands-on virtual reality interactions to demonstrate that endangered animals depend on more than direct rescue. Their survival also requires safe habitats, connected forests, reliable food and water sources, and protection from human-caused threats such as illegal logging and forest fires.
