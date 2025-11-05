# Test Plan — Ella's Farm
Author: Rishav Shrestha  
Date: 2025-11-05

## Scope
Validate camera, navigation, core crop lifecycle, inventory, UI, audio and tool action behavior implemented across:
- `Assets/Scripts/UI/MainMenu.cs`
- `Assets/Scripts/Core/CropDatabase.cs`
- `Assets/Scripts/UI/Clickable.cs`
- `Assets/Scripts/Core/Crop.cs`
- `Assets/Scripts/Core/Inventory.cs`
- `Assets/Scripts/Core/FarmTile.cs`
- `Assets/Scripts/Core/UI/UIController.cs`
- `Assets/Scripts/AudioManager.cs`

## Assumptions & Test Environment
- Unity Editor and a Windows build available.
- Default `CropDatabase` entries and audio assets present.
- Input devices: mouse and keyboard 
---
### TC-001 — Test Case: Farm grid generation on scene load
- Priority: High
Preconditions:
- Scene contains the farm manager/spawner (e.g., `FarmManager`, `FarmTile` prefab or equivalent).
- Default farm grid configuration available (size, spacing, origin) in project settings or scene component.
- `CropDatabase` and required tile prefabs/sprites are present.
- On scene load (New Game or Load), the farm grid is instantiated according to configuration:
  - Correct number of `FarmTile` instances (rows × columns) are created.
  - Each tile is positioned using the configured grid spacing and origin; no overlapping tiles.
  - Tiles are parented under the farm root GameObject (keeps hierarchy tidy).
  - Each tile has required components active (e.g., `FarmTile` script, collider, renderer).
  - No console errors or missing-reference warnings during generation.

---

### TC-002 — Farm centered on screen on load
- Priority: High
Preconditions: Game scene contains farm layout; launch from `MainMenu` (New Game or Load).
Expected:
- The visible viewport is centered on the farm spawn/defined farm center.
- Player spawn and primary farm tiles are visible (not off-screen).
- UI overlays do not occlude essential farm area.
Notes: Verify no console errors; capture screenshot for pass/fail.

---

### TC-003 — Does the action correlate with selected tool
- Severity: High
- Priority: P1
Preconditions: Multiple tools available (e.g., hoe, watering can, scythe) and selectable in UI/toolbelt.
Expected:
- Selected tool determines the action performed (watering only with watering can, till with hoe, etc.).
- Wrong-tool actions are prevented or produce expected feedback (e.g., "cannot use" tooltip).
- Tool selection visuals and cursors update correctly.
Notes: Test switching tools rapidly and confirm no action bleed-over.

---

### TC-004 — Growth progression to get to next stage
- Severity: High
- Priority: P1
Preconditions: Crop definitions include growth stages and watering rules.
Expected:
- Crop advances stages only when growth conditions (time + water) are satisfied.
- If watering is required and not performed, growth stalls (or follows documented behavior).
- Visual stage, internal stage counters, and any UI indicators update correctly.
Notes: Test immediate and delayed watering, over-watering (if applicable), and log growth timestamps.

---

### TC-005 — Does harvesting crops increase crop amount in inventory
- Severity: High
- Priority: P1
Preconditions: Crop is at harvestable stage.
Expected:
- Harvested item(s) added to inventory with correct quantity.
- Stack limits respected; overflow behavior is handled (drop to ground, blocked, or new slot).
- Harvest sound plays and crop removed or set to post-harvest state.
Notes: Test when inventory is near-full to confirm overflow behavior.

---

### TC-006 — Does the crop die if left without harvesting for a while
- Severity: Medium
- Priority: P2
Preconditions: Crop has a harvest window or decay behavior defined.
Expected:
- Crop moves to a withered/dead state or decays per design after the allowed period.
- Inventory is not incorrectly credited unless decay rules specify salvage yields.
- Visual and state transitions reflect the death/decay.
Notes: Validate configurable decay times from `CropDatabase` or data files.

---

### TC-007 — Does the zoom and panning work as expected
- Severity: Medium
- Priority: P2
Preconditions: Camera supports zoom and pan controls (mouse wheel, pinch, keyboard).
Expected:
- Zoom clamps to min/max and interpolates smoothly.
- Panning works at all zoom levels, respects world boundaries, and does not reveal invalid areas.
- No camera jitter, and performance remains acceptable.
Notes: Verify zoom persistence across scene loads if expected.

---

### TC-008 — Can you navigate around the farm at all widths and heights
- Severity: Medium
- Priority: P2
Preconditions: Game running with default camera controls; test machine supports window resizing.
Expected:
- Camera and player navigation work correctly at all tested sizes.
- No UI elements block navigation; interactive elements remain reachable.
- No camera clipping, jitter, or input dead zones introduced by size changes.
Notes: Log any responsive layout issues; test minimum supported resolution.

---

### TC-007 — UI interaction: does the UI behave correctly
- Severity: Medium
- Priority: P2
Preconditions: `UIController` active and interactive elements present.
Expected:
- Buttons and clickable elements respond once per activation; no double-triggering.
- Tooltips appear/disappear as expected, focus is managed, modal dialogs block input beneath them.
- UI scales and remains usable across tested resolutions.
Notes: Test controller/gamepad input if supported.

---


### TC-010 — Does VolumeControl from main menu work
- Severity: Low
- Priority: P3
Preconditions: Audio enabled; main menu accessible.
Expected:
- Volume slider changes immediately affect in-game audio.
- Mute/restore behaves correctly.
- Volume settings persist across scene loads and saved settings (if design requires).
Notes: Verify correct clips are audible and no audio exceptions occur.

---
