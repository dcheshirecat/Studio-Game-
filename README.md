# Endless, Beloved

> *"You wake slowly. Not all at once -- in pieces."*

A branching narrative visual novel framework for Android with tarot mechanics, romance routes, and (in the Heal version) therapeutic skill systems.

## Two Apps, One Engine

| App | Bundle ID | Theme | Therapeutic Skills |
|-----|-----------|-------|-------------------|
| **Endless, Beloved: Heal** | `com.endlessbeloved.heal` | Warm sanctuary | CBT/DBT skills unlocked through story |
| **Endless, Beloved** | `com.endlessbeloved.dark` | Dark fantasy | N/A |

---

## Building the APK

### 1. Open in Unity
- Unity 2022.3 LTS or newer
- Import/open the project
- Wait for package resolution to complete

### 2. First Time Setup
In Unity Editor, run:
```
Endless Beloved > First Time Setup
```
This creates folders, placeholder assets, and generates all scenes.

### 3. Build
```
Endless Beloved > Build Heal APK    # Therapeutic version
Endless Beloved > Build Dark APK    # Fantasy version
```

Output: `build/Android/EndlessBelovedHeal.apk` or `EndlessBelovedDark.apk`

---

## Project Structure

```
Assets/
├── SharedEngine/
│   └── Runtime/
│       ├── Core/           # GameState, SaveSystem, SceneFlow, Audio
│       ├── Dialogue/       # DialogueRunner, Parser, JSON format
│       ├── UI/             # All UI components (AutoWire by name)
│       ├── Characters/     # CharacterData, CharacterDatabase
│       ├── Tarot/          # TarotDeck, CardData
│       ├── Altar/          # AltarManager, SpellData
│       ├── Therapeutic/    # SkillSystem, SkillData (Heal only)
│       ├── Avatar/         # AvatarCustomization
│       └── Minigames/      # CardMemoryGame, MinigameBase
├── _Project/
│   ├── Editor/             # SceneGenerator, IconGenerator, BuildScript
│   ├── Scenes/            # Game scenes (auto-generated)
│   ├── Data/               # ScriptableObject data
│   └── generate_placeholders.py  # SVG asset generator
└── Resources/
    ├── Story/              # Chapter JSON files
    ├── Backgrounds/        # Background images (1080x1920)
    ├── Characters/          # Character portraits
    ├── Audio/Music/        # BGM tracks
    ├── Audio/SFX/           # Sound effects
    └── Tarot/               # Card images
```

---

## Story Content Format

Story chapters use JSON with node-based dialogue:

```json
{
    "prologue_01": {
        "type": "line",
        "speaker": "narrator",
        "text": "You wake slowly. Not all at once.",
        "background": "sanctuary_garden",
        "next": "prologue_02"
    },
    "prologue_02": {
        "type": "choice",
        "text": "What do you do?",
        "choices": [
            {
                "text": "Look around",
                "next": "look_around",
                "affinity": {"oracle": 5}
            },
            {
                "text": "Stay still",
                "next": "stay_still"
            }
        ]
    },
    "chapter_end": {
        "type": "end",
        "next_scene": "AltarHome",
        "effects": [
            {"type": "set_flag", "target": "prologue_done", "value": "true"},
            {"type": "change_affinity", "target": "oracle", "value": "2"}
        ]
    }
}
```

### Node Types
- `line` - Display dialogue
- `choice` - Present choices (with optional affinity changes)
- `end` - End chapter, load next scene
- `card_draw` - Trigger tarot draw
- `minigame` - Trigger minigame
- `branch` - Conditional routing

---

## Therapeutic Skills (Heal version only)

Skills unlock through story flags and display as achievements:

| CBT Skills | DBT Skills |
|------------|------------|
| Cognitive Restructuring | Mindfulness |
| Behavioral Activation | Distress Tolerance |
| Exposure Therapy | Emotion Regulation |
| Problem Solving | Interpersonal Effectiveness |

Each skill includes:
- In-game description (how character learned it)
- Real-life description (how player can use it)
- Quick tip for immediate use

---

## Character Routes

| Archetype | Tagline | Status |
|-----------|---------|--------|
| The Oracle | Foresight & fate | Available |
| The Angel | Grace & ruin | Available |
| The Keeper | Memory & loss | Available |
| The Wanderer | Freedom & longing | Unlocks over time |
| The Apprentice | Power & becoming | Unlocks over time |
| The Weaver | Threads & endings | Unlocks over time |

---

## Creating Content

### Add a Character
1. Right-click `Assets/_Project/Data/Characters`
2. Create > Endless Beloved > Character Data
3. Fill in name variants, bio, personality traits
4. Add to CharacterDatabase asset

### Add Tarot Cards
1. Right-click `Assets/_Project/Data/Cards`
2. Create > Endless Beloved > Card Data
3. Fill in card name, upright/reversed meanings
4. Add to TarotDeck ScriptableObject

### Add Story Chapter
1. Create JSON in `Assets/Resources/Story/`
2. Naming: `{route}_ch{n}.json` (e.g., `oracle_ch1.json`)
3. Follow dialogue format above

### Generate Placeholder Art
```bash
cd Assets/_Project
python generate_placeholders.py
```
Creates SVG placeholders for backgrounds, characters, cards, and UI.

---

## Content Warning

Both apps contain:
- Adult romance themes
- Mental health themes (Heal version)
- References to trauma and recovery

Intended for ages 18+. If in crisis, contact a mental health professional.
