# E-Commerce 3D Product Filter & Preview System
**Unity Developer · Round 2 Technical Assignment — KiksAR (Simulation / AR / VR)**  
**Developer**: Goutamraj Sahu  
**Unity Version**: Unity 6 (`6000.3.16f1`)  
**Render Pipeline**: Universal Render Pipeline (URP)  
**Target Platform**: Android (API 29+) & Standalone PC / Unity Editor  

---

## 📖 Project Overview
This project is an e-commerce-style 3D product catalogue and interactive preview application built in Unity. It demonstrates simulation-grade patterns commonly employed in AR/VR applications:
1. **Dynamic Runtime Data Loading**: Loads product data asynchronously from a local JSON file in `StreamingAssets` without hardcoding or network dependency.
2. **High-Performance UI Grid**: Handles catalogues up to 1,000+ items smoothly at 60+ FPS on mobile using a **Virtual / Recycling Scroll Grid** and **Object Pooling**.
3. **Runtime Asset Streaming & In-Memory Caching**: Dynamically downloads and caches thumbnail images via `UnityWebRequest` using a custom `Dictionary<string, Texture2D>` cache.
4. **Hierarchical Filter System**: Multi-category chips, dynamic subcategory adaptation, smooth panel animations, and empty-state handling.
5. **Interactive 3D Model Viewer**: Dedicated studio stage with realistic lighting, representative 3D models, and **raw gesture touch controls written from scratch** (delta rotation, pinch-to-scale, and double-tap smooth lerp reset).

---

## 🏗️ Architecture & Code Organization

The codebase follows strict separation of concerns (MVC / decoupled service pattern):

```
Assets/Project/
├── Models/              # 3D assets (Watch, Clothes/Hoodie, Jewellery/Necklace)
├── Prefabs/             # UI and Model prefabs (ProductCard, FilterCategory, 3D Models)
├── Scripts/
│   ├── Classes.cs       # Data models: ProductData, ProductDatabase (JSON serializable)
│   ├── ProductsManager.cs # Core singleton: loads JSON, manages dataset & texture cache
│   ├── ProductCardPool.cs # GameObject pool for ProductCard recycling
│   ├── 3D/
│   │   └── Model3DViewHandler.cs # 3D stage manager & model lifecycle controller
│   └── UI/
│       ├── ProductVirtualGrid.cs  # Virtualizing / recycling scroll grid view
│       ├── ProductCard.cs         # Card view component with lazy-loading bindings
│       ├── ProductDetails.cs      # Product detail panel UI controller
│       ├── FilterBoxController.cs # Filter panel animations & multi-selection logic
│       ├── FilterCategory.cs      # Filter chip component
│       └── Model3DInteraction.cs  # Raw touch & mouse gesture mathematics
├── Skybox/              # Studio lighting cubemap
└── StreamingAssets/     # Runtime JSON & unbundled image files
    ├── Thumbnails/      # Provided icon assets loaded strictly at runtime
    ├── products.json    # Default sample JSON (24 items)
    └── products2.json   # High-volume stress-test JSON (1,000 items)
```

---

## ⚡ Key Technical Implementations

### 1. High-Performance Catalogue (1,000 Items Support)
* **Virtual / Recycling Scroll Grid (`ProductVirtualGrid.cs`)**:
  * Instead of instantiating 1,000 GameObjects, the grid instantiates only the small number of cards visible in the viewport plus buffer rows (~12–16 items).
  * As the user scrolls, cards that exit one side are repositioned to the other and re-bound with new data.
* **Object Pooling (`ProductCardPool.cs`)**:
  * Pre-warms cards during initialization and stores them under a designated hierarchy storage GameObject.
  * Zero memory allocations (`GC.Alloc`) during active scrolling.
* **Lazy Texture Loading & Cache (`ProductsManager.cs`)**:
  * Thumbnails are never pre-bundled as Unity Sprites.
  * Downloaded lazily using `UnityWebRequestTexture` only when a card becomes visible.
  * Stored in an in-memory `Dictionary<string, Texture2D>` so each image is fetched at most once per session.

### 2. Multi-Level UI Filter System
* **Overlay Panel Animation**: Animated slide/fade using DOTween / CanvasGroup.
* **Dynamic Subcategory Sync**:
  * Filter categories are displayed as toggleable chips (`Watches`, `Clothes`, `Jewellery`).
  * Subcategories (`Male`, `Female`, `Kids — Boy`, `Kids — Girl`) appear and update dynamically based on active categories.
* **Empty State Handling**:
  * If a filter combination yields zero products, a friendly empty-state message with a quick reset action is presented.

### 3. 3D Model Viewer & Raw Gesture Mathematics (`Model3DInteraction.cs`)
Implemented entirely from scratch with raw input APIs without third-party gesture assets:
* **Single-Finger Drag (Mouse Left-Click Drag)**:
  * Calculates screen-space delta $\Delta x$ and $\Delta y$.
  * Applies proportional angular velocity around the world Y and X axes.
* **Two-Finger Pinch (Mouse Scroll Wheel)**:
  * Measures distance delta between Touch 0 and Touch 1: $\Delta d = \|p_0 - p_1\|_{\text{current}} - \|p_0 - p_1\|_{\text{previous}}$.
  * Scales model uniformly with safe min/max clamping ($0.5\times$ to $2.5\times$).
* **Double-Tap (Mouse Double-Click)**:
  * Tracks tap intervals ($< 0.3\text{s}$).
  * Initiates a smooth `Lerp` / `Slerp` coroutine that smoothly animates orientation and scale back to defaults.
* **Public `ResetModel()`**:
  * Callable by UI button and external controllers.

---

## 🚀 Setup & Running Instructions

### In Unity Editor
1. Clone the repository:
   ```bash
   git clone https://github.com/GoutamrajSahu/KiksAR_Assignment.git
   ```
2. Open the project using **Unity 6 (6000.3.16f1)**.
3. Open the main scene located at `Assets/Scenes/SampleScene.unity`.
4. Press **Play**.
   * Left-click + drag: Rotate 3D model.
   * Mouse scroll: Zoom / scale 3D model.
   * Double-click: Smoothly reset 3D model.

### Testing 1,000 Items Performance
1. In `Assets/Project/Scripts/ProductsManager.cs`, the default loaded file is `products.json`.
2. To test with 1,000 items, change `jsonFileName` in the Inspector of `[ProductsManager]` to `products2.json` (or rename `products2.json` to `products.json`).
3. Enter Play mode and observe smooth scrolling and instant recycling.

### Building the Android APK
1. Go to **File > Build Settings**.
2. Switch platform to **Android**.
3. In **Player Settings**:
   * Minimum API Level: **API level 29** (Android 10.0).
   * Scripting Backend: **IL2CPP**.
   * Target Architectures: **ARM64** (and ARMv7).
4. Click **Build** to produce the standalone `.apk`.

---

## 📜 Credits & Third-Party Assets
All third-party 3D models, textures, and plugins are documented in **[`CREDITS.txt`](./CREDITS.txt)** in the repository root.
