# XR_Audio_Systems_Unity_Wwise: Technical Sound Design Demo

This repository demonstrates high-performance procedural audio systems built for an Extended Reality (XR) environment, showcasing C# logic and Wwise integration.

 CORE TECHNICAL SYSTEMS

Dynamic Occlusion/Obstruction (C# Raycasting):Implemented a robust C# system using dual Raycasting to differentiate between Obstructed and Occluded states. Optimized with CPU throttling to ensure stability in VR, the system sends a 3-state RTPC (0, 50, 100) to Wwise for dynamic LPF filtering.
RTPC Proximity & Corruption: Uses C# scripting to calculate distance, feeding an RTPC to modulate the sound source's **Pitch and Distortion**, creating a smooth, tension-building corruption effect as the player moves away.

 WWISE & ARCHITECTURE IMPLEMENTATION

Spatialization & Environment Management: Utilizes Unity Triggers to control Wwise **Auxiliary Sends**, enabling seamless transition between distinct acoustic environments (Reverbs) for maximal immersion.
Animation Synchronization & Foley Sequencing:** Audio events for player interaction (door open/close, keys) are precisely timed using **Animation Notifications**, ensuring high-quality *foley* synchronization managed by Wwise "Sequence Containers*".
Performance & Memory Budgeting: Project architecture is optimized using segmented **SoundBanks** and event randomization to maintain a low memory footprint and stable CPU usage, a necessity for the target VR platform.

Project Links
[Link to your Video Reel]
[Link to your LinkedIn Profile]
