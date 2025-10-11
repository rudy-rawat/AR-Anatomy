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

```

#  Setup & Installation Guide for AR Anatomy

## Prerequisites

Before setting up the project, make sure you have the following installed:

* **Unity 2021.3 LTS or newer**
* **Vuforia Engine SDK** (via Unity Package Manager)
* **TextMeshPro** (comes pre-installed with Unity)
* A **webcam or smartphone camera**
* **NCERT Biology textbook** (Class 11th or 12th) or printed target images

---

##  Step-by-Step Setup

### 1. Clone the Repository

```bash
git clone (https://github.com/rudy-rawat/AR-Anatomy.git)
```

### 2. Open the Project in Unity

* Launch **Unity Hub**
* Click **Add Project** → Select the cloned project folder
* Open it in Unity Editor

### 3. Install Required Packages

In **Unity**, go to:

```
Window → Package Manager
```

Ensure the following packages are installed:

* **Vuforia Engine (AR)**

  * If not installed, click **Add package from git URL...** and paste:

    ```
    https://library.vuforia.com/Unity/package
    ```
* **TextMeshPro** (included by default)
* **Universal Render Pipeline (optional)** – For improved 3D rendering visuals

### 4. Enable Vuforia in Project Settings

1. Go to **Edit → Project Settings → XR Plug-in Management**
2. Under **AR**, enable **Vuforia Engine**

### 5. Configure the Vuforia License Key

1. Visit [https://developer.vuforia.com](https://developer.vuforia.com)
2. Log in and create a new project.
3. Copy your **Vuforia License Key**.
4. In Unity, open:

   ```
   Assets → Resources → VuforiaConfiguration.asset
   ```
5. Paste your key into the **App License Key** field.

### 6. Add Target Images for Image Recognition

1. In your Vuforia Developer account, go to **Target Manager**.
2. Upload the organ images (from NCERT textbook).
3. Download the **.unitypackage** database file.
4. Import it into your Unity project (**Assets → Import Package → Custom Package...**).

### 7. Configure Organ Data in Registry

In Unity Editor:

* Open the **OrganRegistry** component.
* Add each organ entry with:

  * **Organ Name** (e.g., Heart, Brain)
  * **Basic Prefab** (outer model)
  * **Detailed Prefab** (cross-section)
  * **Organ Info** (text description)
  * **Quiz Questions** (assign question data)

### 8. Configure Scene Hierarchy

Ensure your scene contains:

```
Main Camera
Directional Light
ARCamera (with VuforiaBehaviour)
ImageTargets (with OrganTarget.cs attached)
UI Canvas (buttons, info panel, quiz panel)
OrganRegistry (singleton script)
```

### 9. Build Settings

1. Go to **File → Build Settings**
2. Choose your platform (**Android** or **iOS**)
3. Click **Switch Platform**
4. Then click **Build and Run**

---

##  Running the App

1. Open the app on your device.
2. Scan an organ image from your NCERT textbook.
3. The 3D model will appear over the book page.
4. Use on-screen controls to:

   * **Toggle** between basic/detailed view.
   * **Show/Hide Labels**.
   * **Open Info Panel**.
   * **Start Quiz**.
   * **Refresh Model.**
5. Use **pinch gestures** to zoom and **drag** to rotate the model.

---

##  Troubleshooting

* **Model not appearing:** Check if your Vuforia license key and image target database are correctly set.
* **UI buttons not responding:** Ensure all button references are linked to `OrganToggleUI.cs` in the inspector.
* **Quiz not starting:** Verify that quiz questions are linked under the correct organ in `OrganRegistry`.

---

##  Developer Notes

* To add new organs, simply create new prefabs and register them in `OrganRegistry.cs`.
* The project is modular — all systems (labels, info, quiz) can be extended independently.
* Recommended build platform: **Android 10+**.

---

###  Enjoy exploring human anatomy in Augmented Reality with *AR Anatomy!*

