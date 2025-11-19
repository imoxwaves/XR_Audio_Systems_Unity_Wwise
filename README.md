# XR_Audio_Systems_Unity_Wwise: Technical Sound Design Demo 

PROJECT CONTEXT & BASE FRAMEWORK

This repository contains the C# scripting and Wwise integration required to implement a robust audio system for a VR environment.

The project is built upon the **Hurricane VR Framework** for basic character movement and interactions. The primary focus of this repository is the **extension, adaptation, and optimization** of this base code to integrate advanced Wwise audio features, rather than core game logic development. All custom audio features are designed to be modular and decoupled from the base framework’s core mechanics.

This repository is a procedural audio systems built for an Extended Reality (XR) environment, showcasing C# logic and Wwise integration.

 CORE TECHNICAL SYSTEMS

Dynamic Occlusion/Obstruction (C# Raycasting):Implemented a robust C# system using dual Raycasting to differentiate between Obstructed and Occluded states. Optimized with CPU throttling to ensure stability in VR, the system sends a 3-state RTPC (0, 50, 100) to Wwise for dynamic LPF filtering.
RTPC Proximity & Corruption: Uses C# scripting to calculate distance, feeding an RTPC to modulate the sound source's **Pitch and Distortion**, creating a smooth, tension-building corruption effect as the player moves away.

WWISE & ARCHITECTURE IMPLEMENTATION

Spatialization & Environment Management: Utilizes Unity Triggers to control Wwise **Auxiliary Sends**, enabling seamless transition between distinct acoustic environments (Reverbs) for maximal immersion.
Animation Synchronization & Foley Sequencing:** Audio events for player interaction (door open/close, keys) are precisely timed using **Animation Notifications**, ensuring high-quality *foley* synchronization managed by Wwise "Sequence Containers*".
Performance & Memory Budgeting: Project architecture is optimized using segmented **SoundBanks** and event randomization to maintain a low memory footprint and stable CPU usage, a necessity for the target VR platform.



-Procedural Audio Corruption System (RTPC Proximity)
Script Focus:
[TvRTPC_Proximity.cs](https://github.com/imoxwaves/XR_Audio_Systems_Unity_Wwise/blob/645ae1d34b09b2586e91482888c49e44831ceeae/Assets/Custom_Scripts/TvRTPC_Proximity.cs)

This C# script dynamically calculates the distance between the VR player's head (listener) and a critical sound source. It implements CPU Throttling (updateFrequency = 0.2f) to optimize performance and sends a normalized value (0-100) to a Wwise RTPC. This system drives modular effects, allowing sound corruption (Pitch, Distortion) to scale smoothly with distance, proving an understanding of both Wwise API calls and VR performance consciousness.



-Dynamic Occlusion/Obstruction System (C# Raycasting)
Script Focus:
[AkObstruction_Raycast.cs](https://github.com/imoxwaves/XR_Audio_Systems_Unity_Wwise/blob/645ae1d34b09b2586e91482888c49e44831ceeae/Assets/Custom_Scripts/Ak_Obstruction_Raycast.cs)

This C# script implements a high-performance Dual-Raycast system to simulate realistic sound filtering in 3D space. It calculates three distinct states (Clear, Obstructed [50], Occluded [100]) and sends the result to the Wwise RTPC. The system features essential VR optimization via CPU Throttling and utilizes Unity's LayerMasks for precise obstruction detection, demonstrating expertise in physics-based audio engineering.

-Spatialization and Environment Management
System Focus: Dynamic acoustic environment management using Wwise Auxiliary Busses and engine logic. Unity Triggers dynamically route sound emitters to specific Aux Busses. This architecture ensures seamless, CPU-efficient transitions between distinct acoustic spaces, proving control over spatialization and advanced Wwise routing practices.

-Procedural Footstep Cadence System
Script Focus:
[VRPlayerFootsteps.cs](https://github.com/imoxwaves/XR_Audio_Systems_Unity_Wwise/blob/645ae1d34b09b2586e91482888c49e44831ceeae/Assets/VRPlayerFootsteps.cs)

This C# script implements a Timing-Based Footstep System, utilizing the Unity CharacterController for real-time velocity calculation. It dynamically adjusts the footstep cadence (interval) via Mathf.Lerp based on player speed, providing smooth transitions between walking and running sounds. Furthermore, it sends the player's velocity to a Wwise Global RTPC, proving integration expertise and an understanding of speed-dependent audio effects.

TECHNICAL APPROACH: EFFICIENCY AND TSD LOGIC

This project emphasizes a performance-first approach, prioritizing robust C# logic and Wwise architecture over simple event posting.

Use of Productivity Tools

Initial adaptation of the base Hurricane VR C# classes for Wwise event posting utilized AI assistance to enhance development efficiency and speed up the initial integration phase.

all advanced audio logic and core TSD systems are original code written by the developer. This includes:
-The Dual-Raycast Occlusion system (`AkObstruction_Raycast.cs`).
-The speed/cadence logic (`VRPlayerFootsteps.cs`) utilizing `Mathf.Lerp`.
-All CPU throttling and performance optimization logic.

The goal was to demonstrate proficiency in extending existing codebases and creating high-value TSD features using modern development workflows.


Project Links
[https//:www.oscarperla.com]

