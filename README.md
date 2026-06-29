# Wildlife VR Rescue

**Wildlife VR Rescue** is a fully immersive VR conservation game where players become part of a fictional Southeast Asian emergency wildlife rescue team. The player pilots a human-scale Wildlife Rescue Exo-Mech to rescue endangered animals, restore damaged habitats, and protect a rainforest reserve affected by illegal logging, fire, dry soil, and habitat destruction.

The project is developed for **MMD 6246 Immersive Media Design 4**, focusing on VR interaction design, environmental storytelling, animal education, and realistic rainforest world-building for HMD devices. The target users are kids and teenagers aged **12–15 years old**. The course brief requires a VR application for HMD, using OpenXR / XR Interaction Toolkit, with immersive interactions, habitat research, animal integration, and a final Unity EXE or Quest APK build. :contentReference[oaicite:0]{index=0}

## Game Concept

In the game, the rainforest reserve is under emergency status. Several endangered animals have been displaced or placed into temporary care because their habitats are no longer safe. The player begins at the **Emergency HQ / Rescue Nursery**, receives a mission briefing, and uses the exo-mech system to complete a rescue-and-restore mission.

The main demo focuses on one complete mission loop in the **Tapir Zone**, while the Tiger, Elephant, and Lar Gibbon zones are shown as locked future missions through nursery enclosures and the mission board. This keeps the project achievable while still showing the full game world. :contentReference[oaicite:1]{index=1}

## Core Gameplay Loop

The gameplay follows this conservation mission flow:

1. Receive emergency briefing  
2. Enter the Wildlife Rescue Exo-Mech  
3. Scan damaged habitat  
4. Rescue and guide the tapir to the nursery  
5. Feed the tapir and unlock educational facts  
6. Collect clean river water  
7. Plant and activate Eco-Seed Capsules  
8. Extinguish small fire threats  
9. Release the tapir back into the restored habitat  

This loop teaches players that endangered animals cannot survive through care alone. They also need safe habitats, food sources, clean water, connected forests, and protection from human-caused threats. :contentReference[oaicite:2]{index=2}

## Demo Structure

The demo is designed with **5 gameplay sections across 3 main map areas**:

| Section | Area | Purpose |
|---|---|---|
| 1 | Emergency HQ / Rescue Nursery | Briefing, mission board, animal enclosures, teleport system |
| 2 | Tapir Zone | Scan habitat, find tapir, rescue animal |
| 3 | Nursery Care | Feed tapir and unlock fact card |
| 4 | River Source + Restoration | Collect water, plant Eco-Seed, restore habitat |
| 5 | Fire Response + Release | Extinguish fire and release tapir |

The three main map areas are:

- **Emergency HQ / Rescue Nursery**
- **Tapir Zone**
- **River Source**

## VR Interaction Features

The project uses simple and practical VR interactions:

- Teleport movement between mission areas
- Hand interaction for grabbing, feeding, pressing UI, and clearing obstacles
- HUD Habitat Scanner to detect animals, damaged soil, blocked paths, fire risks, and restoration points
- Water Blaster mode to collect water, activate Eco-Seeds, and extinguish fire
- Eco-Seed Launcher mode to plant restoration capsules
- Wrist HUD showing Forest Health, Animal Safety, and Threat Level
- Mission board and spatial UI for zone selection and progress tracking

## Exo-Mech System

The player does not control a full vehicle simulation. Instead, the exo-mech is represented through VR hands, a modular right-arm tool system, wrist UI, scan highlights, and cockpit-style HUD feedback.

The right hand has two main states:

- **Hand Mode** — grab objects, feed animals, press UI, and clear small obstacles
- **Tool Mode** — switch between Water Blaster and Eco-Seed Launcher

The scanner, inventory, and rescue guidance are integrated into the HUD to keep the gameplay readable and achievable in Unity VR.

## Educational Goal

Wildlife VR Rescue is designed to help young players understand the relationship between animals and their environment. Instead of only reading facts, players learn through action: rescuing animals, restoring habitats, responding to threats, and seeing how the forest changes after their choices.

## Built With

- Unity
- OpenXR
- XR Interaction Toolkit
- VR / HMD interaction system
- 3D rainforest environment assets
- Spatial UI and wrist HUD design

## Project Status

This repository contains the development files for the VR prototype, including the Unity scene, interaction systems, level blockout, UI/UX elements, and demo gameplay structure for the Wildlife VR Rescue project.
