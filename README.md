# 🫀 AR Anatomy – Educational Augmented Reality Application

![Status](https://img.shields.io/badge/status-prototype-yellow.svg)
![Engine](https://img.shields.io/badge/engine-Unity-black.svg)
![AR SDK](https://img.shields.io/badge/AR-Vuforia-green.svg)
![Platform](https://img.shields.io/badge/platform-Android-lightgrey.svg)
![Architecture](https://img.shields.io/badge/architecture-modular%20systems-informational.svg)

**AR Anatomy** is an **educational Augmented Reality (AR) application** that allows users to scan **NCERT human anatomy textbook images** and visualize **interactive 3D organ models** in real time.

The project is designed with a **modular, data-driven Unity architecture**, focusing on scalability, technical clarity, and educational usability.

---

## 📸 Screenshots

<table>
  <tr>
    <td align="center"><img src="docs/images/home_screen.png" width="200" alt="Home Screen"/></td>
    <td align="center"><img src="docs/images/detailed_model_labels.jpeg" width="200" alt="AR View"/></td>
    <td align="center"><img src="docs/images/quiz_result.jpeg" width="200" alt="Quiz Result"/></td>
  </tr>
  <tr>
    <td align="center"><b>Home Screen</b></td>
    <td align="center"><b>AR Visualization</b></td>
    <td align="center"><b>Quiz System</b></td>
  </tr>
</table>

---

## 📂 Repository Structure

```text
AR-Anatomy/
│
├── README.md
├── docs/
│   └── images/
│       ├── home_screen.png
│       ├── basic_model.png
│       ├── detailed_model_labels.png
│       ├── quiz_result.png
│       └── organ_description.png
│
├── Assets/
│   ├── Scripts/
│   │   ├── OrganVariant.cs
│   │   ├── OrganRegistry.cs
│   │   ├── OrganTarget.cs
│   │   ├── OrganToggleUI.cs
│   │   ├── PinchToZoom.cs
│   │   ├── FaceCamera.cs
│   │   └── SceneChanger.cs
│   └── Prefabs/
│
└── ProjectSettings/
```

---

## 📱 Project Overview

| Property | Details |
|----------|---------|
| Engine | Unity |
| AR SDK | Vuforia |
| Platform | Android |
| Language | C# |
| Input | Touch + UI |
| Domain | Educational AR |
| Curriculum | NCERT – Human Anatomy |

---

## 🧠 High-Level System Architecture (Conceptual)

| Step | Component | Responsibility |
|------|-----------|----------------|
| 1 | NCERT Textbook Image | Physical marker scanned by camera |
| 2 | Vuforia Image Target | Detects and tracks textbook image |
| 3 | OrganTarget.cs | Handles AR lifecycle (spawn / destroy) |
| 4 | OrganRegistry.cs | Provides correct organ prefab |
| 5 | 3D Organ Prefab | Visual representation of the organ |
| 6 | PinchToZoom.cs | Touch-based interaction |
| 7 | FaceCamera.cs | Keeps labels facing the camera |
| 8 | UI Controls | Toggle, refresh, quiz, info |

---

## 🧩 Core Data Model

### Organ Definition (OrganVariant.cs)

Each organ is represented as a pure data object.

```csharp
public class OrganVariant
{
    public string organName;
    public string organDescription;
    public GameObject basicPrefab;
    public GameObject detailedPrefab;
}
```

**Purpose:**
- Separates content from logic
- Allows new organs to be added without modifying AR code
- Keeps anatomy data centralized and scalable

---

## 🗂 Central Organ Registry (OrganRegistry.cs)

The OrganRegistry acts as a singleton runtime database for all organs.

| Responsibility | Description |
|----------------|-------------|
| Data Storage | Holds all OrganVariant entries |
| Lookup | Uses Dictionary for O(1) access |
| Prefab Fetching | Returns basic or detailed prefab |
| Normalization | Converts organ names to lowercase |

**Why this design?**
- Prevents hardcoded prefab references
- Ensures a single source of truth
- Improves runtime performance

---

## 🎯 AR Image Target Handling (OrganTarget.cs)

OrganTarget is attached to each Vuforia Image Target and controls the AR lifecycle.

### Tracking States

| State | Behavior |
|-------|----------|
| Tracked | Spawn organ model |
| Extended Tracked | Keep model visible |
| Not Tracked | Destroy organ model |

### Runtime Flow

| Step | Action |
|------|--------|
| Image detected | Fetch prefab from OrganRegistry |
| Spawn | Instantiate organ as child of ImageTarget |
| Setup | Attach PinchToZoom dynamically |
| Visuals | Apply fade-in animation |
| UI | Enable toggle button if detailed model exists |
| Image lost | Destroy active organ model |

---

## 🔄 Basic ↔ Detailed Model Toggle

Handled through OrganToggleUI.cs.

| Action | Result |
|--------|--------|
| Toggle button clicked | Switch model type |
| Fade-out | Old model smoothly disappears |
| Spawn | New model instantiated |
| Fade-in | Smooth transition |

**Why fade animations?**
- Avoids harsh popping
- Improves AR stability perception
- Enhances user experience

---

## ✋ Touch Interaction System (PinchToZoom.cs)

Supported gestures:

| Gesture | Function |
|---------|----------|
| Single finger drag | Rotate model (Y-axis only) |
| Two finger pinch | Zoom in / out |

**Design Decisions:**
- Rotation restricted to Y-axis to prevent disorientation
- Scale clamped to avoid clipping or extreme zoom
- Threshold checks reduce touch jitter

---

## 🏷 Camera-Facing Labels (FaceCamera.cs)

```csharp
transform.LookAt(Camera.main.transform);
transform.Rotate(0, 180, 0);
```

**Purpose:**
- Ensures labels always face the camera
- Prevents mirrored or unreadable text
- Essential for AR readability

---

## 🧭 UI & Navigation

### UI Control System (OrganToggleUI.cs)

| Feature | Description |
|---------|-------------|
| Toggle Button | Switch basic ↔ detailed models |
| Refresh Button | Clears all active models |
| Visibility Control | Toggle button only shows when applicable |

**Refresh use cases:**
- Fix AR jitter
- Handle partial tracking loss
- Allow user realignment

### Scene Navigation (SceneChanger.cs)

Scene navigation is kept separate from AR logic to improve maintainability.

| Scene | Purpose |
|-------|---------|
| Home | App entry point |
| AR Scene | Main learning experience |
| Quiz Scene | Knowledge assessment |

---

## 🧪 Quiz & Description System

### Quiz System
- Organ-specific question pools
- 5 random questions per attempt
- Immediate scoring, percentage, and grade

### Description Panel
- Organ name, Function, and Anatomical location
- Overlay UI that preserves AR immersion

---

## 🧠 Architectural Principles

- **Single Responsibility Principle:** Each script handles one specific task
- **Data-driven design:** Content is separated from code
- **Loose coupling:** AR, UI, and Data communicate via references, not direct dependencies
- **Runtime component attachment:** Components like Zoom are added only when needed

---

## 🚀 Future Enhancements

- [ ] Markerless AR (ARCore/ARKit)
- [ ] Voice narration
- [ ] Student progress tracking
- [ ] Multi-organ sessions
- [ ] Multi-language support

---

## ⭐ Acknowledgement

Built using Unity and Vuforia for educational augmented reality learning.

If this project helped you, consider giving it a ⭐ on GitHub.
