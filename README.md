#  AR Anatomy  
### “Bringing Textbook Biology to Life through Augmented Reality”

---

##  Overview

**AR Anatomy** is an interactive Augmented Reality (AR) application designed to make learning human anatomy more engaging and immersive.  
By scanning organ images from **Class 11th and 12th NCERT biology textbooks**, students can view **3D organ models** directly over their textbook pages in real-world space.  

The app allows users to:
- Switch between **basic** and **detailed** organ models.  
- View **floating labels** for key organ parts.  
- Access **information panels** with detailed descriptions.  
- Take **interactive quizzes** for each organ.  

Built using **Unity** and **Vuforia SDK**, AR Anatomy transforms static textbook learning into a hands-on, 3D exploration experience.

---

##  Features

###  1. Real-Time Image Recognition  
- Uses **Vuforia SDK** to detect organ images from NCERT textbooks.  
- Spawns corresponding 3D organ models accurately anchored in the real world.

###  2. Model Toggle System  
- Toggle between **basic model** (outer view) and **detailed model** (cross-section).  
- Smooth fade-in/out animation for model transitions.

###  3. Dynamic Label System  
- Auto-detects anatomical parts in the model.  
- Displays floating 3D labels that face the camera dynamically.  
- Toggle labels on/off to declutter the view.

###  4. Info Panel  
- Displays brief information about the selected organ from **OrganRegistry**.

###  5. Quiz Module  
- Each organ has its own quiz with **5 randomized questions**.  
- Provides instant feedback, score, and grade evaluation.

###  6. Refresh & Contextual UI  
- **Refresh button** clears all models and reinitializes recognition.  
- UI buttons appear only when an organ is detected — keeping the AR view clean.

###  7. Gesture Controls  
- Supports rotation and scaling of 3D models using **pinch and drag** gestures.

---

##  System Architecture

```plaintext
AR Anatomy
│
├── Vuforia SDK                 # Handles image tracking and detection
│
├── Scripts/
│   ├── OrganRegistry.cs        # Central registry for all organ data
│   ├── OrganTarget.cs          # Spawning and AR tracking logic
│   ├── OrganToggleUI.cs        # UI control system for toggles and panels
│   ├── OrganLabelManager.cs    # Manages dynamic label creation and updates
│   ├── OrganInfoUI.cs          # Organ info display panel
│   ├── QuizUI.cs               # Handles quiz logic and UI flow
│   ├── OrganVariant.cs         # Organ data structure (models + info + quiz)
│   ├── LabelUI.cs, LabelPoint.cs  # Label definitions and world-space rendering
│   └── QuizQuestion.cs         # Quiz question model
│
└── Assets/
    ├── Prefabs/                # Basic and detailed organ prefabs
    ├── Images/                 # Vuforia target images (NCERT diagrams)
    ├── UI/                     # Panels, buttons, and canvas
    └── Materials/              # Line renderers, label visuals
