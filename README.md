# Wheel of Fortune

A Unity demo built for the Vertigo Games developer case. Spin to collect rewards, reach a safe zone to claim them, or risk losing the current run to a bomb.

## Running the project

1. Clone the repository and open the project through Unity Hub with **Unity 2021.3.45f2**.
2. Let Unity finish importing assets and restoring packages.
3. Open `Assets/_Project/Scenes/MainScene.unity` and press Play.

DOTween and the supplied UI assets are included in the repository. TextMeshPro and the Unity Test Framework are declared in `Packages/manifest.json`.

For an Android build, install Android Build Support, including the SDK, NDK and OpenJDK, for this Unity version. In **File > Build Settings**, switch to Android and build the enabled `MainScene`. The project uses a landscape layout.

APK downloads are hosted on [GitHub Releases](https://github.com/AriffK34/Vertigo-Games-Demo/releases).

## Playing

- **Spin** selects a reward and turns the wheel to its slice. Normal wheels contain exactly one bomb.
- Every **5th zone** is a bomb-free silver wheel. Every **30th zone** takes priority as a bomb-free gold wheel with special rewards.
- **Leave** is available while idle in a safe or super zone. **Claim** transfers every collected reward to the player's inventory and starts a new run.
- On a bomb, **Give Up** discards the run's rewards and restarts at zone 1. **Revive** spends the configured currency to keep those rewards and retry the current zone. The button is disabled when funds are insufficient.
- **Inventory** shows claimed items and currencies. The panel beside the wheel shows rewards belonging to the current run.

Claimed items survive subsequent runs within the same session. There is no save-to-disk system; closing the application clears them.

## Structure and design

Project-specific assets and code are under `Assets/_Project`:

```text
Scripts/Bootstrap       Scene setup and Unity random source
Scripts/Core            Game flow, state machine, economy, zone logic and messaging
Scripts/Data            ScriptableObject definitions and display data
Scripts/Presentation    Controllers, views, animations and slot pooling
Data                    Item definitions, wheel themes and configuration assets
Prefabs                 Reusable wheel, inventory and zone slots
Tests                   EditMode tests
```

`GameBootstrap` creates the services and injects their dependencies into the scene controllers. Controllers use small command, reader and completion interfaces. A typed, scene-local event bus carries state changes and presentation requests.

`GameSession` coordinates the run through an explicit state machine. Economy, zone selection and reward calculation have separate classes, allowing gameplay rules to be tested independently of the UI.

Items and currencies share `ItemData` definitions and an `ItemCollection`. Views display their icons and quantities from data. Inventory and claim views reuse pooled slots, and UI animations use DOTween.

## Editing content

Configuration assets are in `Assets/_Project/Data/Configs`. New definitions can be created through **Create > Wheel** in the Project window.

**Items and rewards.** Create an `ItemData` asset, assign its icon and growth profile, then use it in a wheel's slice list. Each slice defines its base amount and whether it is a bomb. A new currency uses the same item definition; it can be selected in `EconomyConfig` for revive payments. To show it in a dedicated balance bar, add an Inspector binding to `CurrencyBarView`.

**Reward progression.** Early, middle and late wheel tables change the available items as zones advance. An item's `RewardGrowthConfig` separately controls quantity growth:

```text
amount = baseAmount * (1 + growthPerZone * (zone - 1))
```

The result is rounded to the nearest integer, with midpoint values rounded away from zero.

**Zone rules.** A `ZoneRuleConfig` defines an inclusive zone range, interval, wheel, bomb requirement, leave permission and entry effect. A last zone of `0` means no upper limit. Intervals match multiples of their value within that range. `ZoneProgressionConfig` selects the **first matching rule**, so specific rules must precede general ones. The supplied list puts super zones first, safe-zone rules next, and normal-zone rules last.

Use **Validate Wheel** from a zone rule asset's Inspector context menu after editing its wheel. It checks for exactly one bomb when required and zero otherwise, along with slice references and reward amounts. These checks also run at startup.

**Wheel presentation.** `WheelTheme` holds the wheel and indicator sprites. The supplied artwork uses eight slices. When changing the slice count, update both the wheel config and the visual slice instances under the wheel base, and use matching artwork.

**Animation and runtime settings.** `UIAnimationConfig` controls popup, zone-bar, reward-flight, punch and bomb-particle settings. `WheelAnimationConfig` controls spinning, landing feedback, the indicator and wheel reset. `RuntimeConfig` sets the target frame rate.

## Tests

Open **Window > General > Test Runner**, select **EditMode**, then choose **Run All**.

The EditMode tests cover item collections, reward claims, revive payments, zone selection and priority, wheel validation, reward growth and rounding, event subscriptions, state transitions and command guards.
