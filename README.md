# 🫀 AR Anatomy – Educational Augmented Reality Application

![Status](https://img.shields.io/badge/status-prototype-yellow.svg)
![Engine](https://img.shields.io/badge/engine-Unity-black.svg)
![AR SDK](https://img.shields.io/badge/AR-Vuforia-green.svg)
![Platform](https://img.shields.io/badge/platform-Android-lightgrey.svg)
![Domain](https://img.shields.io/badge/domain-Educational%20AR-blue.svg)
![Architecture](https://img.shields.io/badge/architecture-modular%20systems-informational.svg)

**AR Anatomy** is an **educational Augmented Reality application** that enables students to scan **NCERT human anatomy textbook images** and interact with **realistic 3D organ models** in real time.

The project focuses on:
- Visual learning through AR
- Interactive 3D anatomy exploration
- Modular, scalable Unity architecture
- Curriculum-aligned educational content

---

## 📸 Application Preview

![Home Screen](docs/images/home_screen.png)

---

## 📱 Project Overview

| Property | Details |
|--------|---------|
| **Engine** | Unity |
| **AR SDK** | Vuforia |
| **Platform** | Android |
| **Domain** | Educational / AR |
| **Interaction** | Touch (Pinch, Rotate, UI-based) |
| **Content Type** | NCERT-aligned Human Anatomy |

---

## 🧭 Application Flow

1. User launches the app
2. Selects learning mode
3. Scans NCERT textbook image
4. AR organ model spawns
5. User interacts with:
   - 3D models
   - Labels
   - Quiz
   - Description panels

---

# 📚 Core Features

## 1️⃣ AR Textbook Image Recognition

- Uses **Vuforia Image Targets**
- Detects NCERT textbook organ diagrams
- Spawns corresponding organ model in AR space
- Handles tracking loss and recovery gracefully

---

## 2️⃣ Basic Organ Model (Outer Anatomy)

![Basic Model](docs/images/basic_model.jpeg)

### Features
- Displays external structure of the organ
- Touch-based interaction:
  - Pinch-to-zoom
  - Single-finger rotation (Y-axis)
- Optimized to reduce AR jitter

### Stability Tools
- **Refresh Button**
  - Clears active models
  - Resets AR state
  - Helps recover from tracking instability

---

## 3️⃣ Detailed Organ Model (Cross-Section View)

![Detailed Model](docs/images/detailed_model_labels.jpeg)

### Features
- Cross-sectional anatomical model
- Highlights internal structures
- Toggle between basic ↔ detailed view

### Dynamic Label System
- Labels are **not hardcoded**
- Each organ prefab contains:
  - Empty GameObjects as label anchors
  - Label names defined via Inspector
- Labels:
  - Spawn dynamically
  - Always face the camera
  - Improve readability in AR

---

## 4️⃣ Organ Registry Architecture

Centralized system that manages all organ data.

### Stored Information
- Organ Name
- Description
- Basic Model Prefab
- Detailed Model Prefab

### Benefits
- Easy to add new organs
- Clean separation of data & logic
- Scalable for future expansion

---

## 5️⃣ Interactive Quiz System

![Quiz Result](docs/images/quiz_result.jpeg)

### Quiz Logic
- Organ-specific questions
- Each quiz attempt:
  - 5 random questions
  - Selected from ~10-question database
- Prevents repetition within a single attempt

### Evaluation
- Instant feedback
- Displays:
  - Score
  - Percentage
  - Grade (A+, A, etc.)

---

## 6️⃣ Organ Description Panel

![Organ Description](docs/images/organ_description.jpeg)

### Information Displayed
- Organ name
- Functional description
- Location in the human body

### Design Goals
- Minimal UI obstruction
- Maintains AR immersion
- Supports conceptual understanding

---

# 🧠 Interaction System

## Touch Controls
| Gesture | Action |
|------|-------|
| Pinch | Zoom In / Out |
| Single Finger Drag | Rotate Model |
| UI Button | Toggle / Refresh / Info |

---

## UI Controls
- Toggle Model Button
- Refresh AR Button
- Quiz Button
- Description Button
- Home Navigation

---

# 🛠 Technical Stack

| Component | Technology |
|--------|------------|
| Engine | Unity |
| Language | C# |
| AR SDK | Vuforia |
| UI | Unity UI + TextMeshPro |
| Platform | Android |

---

# 🧪 Design Principles

- Modular architecture
- Data-driven organ management
- Separation of AR, UI, and logic layers
- Performance-conscious AR rendering

---

# 🚀 Future Enhancements

- Add more organs (Brain, Lungs, Kidney, Liver)
- Voice-based explanations
- Markerless AR support
- Student progress tracking
- Multi-language support

---

# 🎓 Educational Use Case

- Designed for school-level anatomy learning
- Strong alignment with NCERT curriculum
- Ideal for:
  - Visual learners
  - AR-assisted classrooms
  - Self-paced exploration

---

## ⭐ Support the Project
If you found this project useful or interesting, consider giving it a ⭐ on GitHub.
