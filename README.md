# Endless, Beloved: Heal

> *"You wake slowly. Not all at once -- in pieces. The tightness in your chest loosens. Just a little. But enough."*

A therapeutic 18+ romance visual novel for Android. Six healing routes. CBT/DBT skill system. Built for survivors.

## What Makes This Different

The protagonist lives with C-PTSD. This is woven into the story naturally, not clinically. As you play, you unlock real CBT and DBT skills through story moments and choices. Each skill is logged like an achievement with:

- **In-game description**: how your character learned it
- **Real-life description**: how you, the player, can use it
- **Quick tip**: a short, actionable summary

The skill log is always accessible -- a real, usable mental health reference you build over time.

## Project Structure

Same shared engine as the main Endless, Beloved game, with therapeutic-specific additions:

```
Assets/
  SharedEngine/                    -- Shared codebase with dark fantasy version
    Runtime/
      Therapeutic/
        SkillData.cs               -- ScriptableObject for CBT/DBT skills
        SkillSystem.cs             -- Skill unlock manager
        SkillLogUI.cs              -- Achievement-style skill log UI
  _Project/
    Content/
      Story/Oracle/oracle_ch1.json -- Therapeutic Oracle route
    Scripts/
      GameBootstrap.cs             -- Entry point
    Art/                           -- Warm/nature themed visuals
    Audio/                         -- Healing ambient music
```

## Therapeutic Skills

Skills are organized by therapy type:

### CBT (Cognitive Behavioral Therapy)
- Cognitive Restructuring
- Behavioral Activation
- Exposure Therapy
- Problem Solving

### DBT (Dialectical Behavior Therapy)
- Mindfulness
- Distress Tolerance
- Emotion Regulation
- Interpersonal Effectiveness

## Character Routes

| Archetype | Tagline | Status |
|-----------|---------|--------|
| The Oracle | Foresight & fate | Available (Ch1 complete) |
| The Angel | Grace & ruin | Available (placeholder) |
| The Keeper | Memory & loss | Available (placeholder) |
| The Wanderer | Freedom & longing | Unlocks over time |
| The Apprentice | Power & becoming | Unlocks over time |
| The Weaver | Threads & endings | Unlocks over time |

## Getting Started

Same setup as the main game -- see the shared README for Unity setup instructions.

## Content Warning

This game contains explicit adult content, themes of trauma and recovery, and depictions of C-PTSD symptoms. While the therapeutic content is designed to be helpful, this game is not a substitute for professional mental health support. If you are in crisis, please contact a mental health professional or crisis line.
