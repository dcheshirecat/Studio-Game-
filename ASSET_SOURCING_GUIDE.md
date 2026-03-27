# Asset Sourcing Guide -- Endless, Beloved

This guide covers where to find free and paid assets for both versions of the game.

## Current Placeholder Assets

105 SVG placeholder assets have been generated in `Assets/_Project/Art/`:
- 10 background scenes (5 dark fantasy + 5 therapeutic)
- 48 character portraits (6 characters x 8 expressions)
- 24 tarot card faces (22 Major Arcana + 2 card backs)
- 21 UI elements (dialogue panels, buttons, choice buttons)

These are functional placeholders -- colored silhouettes and gradients that let you test the game flow. Replace them with real art when ready.

---

## Free Asset Sources

### Character Portraits / Visual Novel Art

**Best options for VN-style character art:**

1. **Sutemo's Character Maker** (itch.io)
   - URL: itch.io/search?q=sutemo+character
   - Free anime/VN-style character generators
   - Multiple expressions, layered PSD files

2. **Mannequin VN Sprite Generator** (itch.io)
   - URL: mannequin.itch.io
   - Free base sprites, customizable expressions
   - Commercial use OK

3. **VN Character Maker by croc** (itch.io)
   - URL: croc-maker.itch.io
   - Layered character sprites with expression swaps

4. **OpenGameArt.org** -- search "visual novel" or "portrait"
   - URL: opengameart.org
   - CC0 and CC-BY licensed art
   - Mixed quality but good variety

5. **Stable Diffusion / DALL-E / Midjourney** (AI generation)
   - Best for consistent character art across expressions
   - Use ControlNet for expression consistency
   - Prompt template: "[character description], visual novel portrait, dark fantasy, half body, [expression], high quality anime style"

### Backgrounds

1. **Noraneko Games** (itch.io)
   - URL: noranekogames.itch.io
   - Free VN backgrounds, multiple styles
   - Interior/exterior scenes

2. **Uncle Mugen** (Lemma Soft Forums)
   - URL: lemmasoft.renai.us -- search "Uncle Mugen"
   - Huge library of free VN backgrounds
   - CC-BY license

3. **Pixabay / Unsplash** (free photos)
   - Use as base, apply filters for VN look:
     - Posterize + color overlay for anime style
     - Oil paint filter in Photoshop/GIMP

4. **AI Generation** (recommended for consistency)
   - Prompt template: "[scene description], fantasy interior, atmospheric lighting, visual novel background, no characters, high detail"

### Tarot Card Art

1. **Rider-Waite Tarot** (public domain)
   - The original 1909 Rider-Waite deck is public domain
   - High-res scans available on Wikimedia Commons
   - URL: commons.wikimedia.org -- search "Rider-Waite"

2. **Open Tarot** (GitHub)
   - URL: github.com/topics/tarot
   - Several open-source tarot art projects

3. **Custom AI Generation** (recommended)
   - Generate a consistent set using a style prompt
   - Prompt: "[card name] tarot card, dark gothic art, ornate frame, mystical, high detail, vertical card format"

### UI / Interface

1. **Kenney.nl**
   - URL: kenney.nl/assets
   - Massive free UI asset library
   - CC0 license, commercial use OK
   - Game icons, buttons, panels, sliders

2. **Game-Icons.net**
   - URL: game-icons.net
   - 4000+ free game icons (CC BY 3.0)
   - Spells, potions, cards, symbols

3. **Fantasy UI by Wenrexa** (itch.io)
   - URL: wenrexa.itch.io
   - Fantasy-themed UI kits, some free

### Audio / Music

1. **Freesound.org**
   - URL: freesound.org
   - Massive library of CC-licensed sound effects
   - Search: "card flip", "magic", "ambient dark", "nature"

2. **Kevin MacLeod (Incompetech)**
   - URL: incompetech.com
   - Hundreds of free music tracks (CC BY)
   - Dark, ambient, fantasy genres available

3. **OpenGameArt.org -- Audio**
   - URL: opengameart.org -- filter by Audio
   - CC0 and CC-BY music and SFX

4. **Pixabay Music**
   - URL: pixabay.com/music
   - Free commercial use, no attribution needed
   - Search: "dark ambient", "fantasy", "healing", "meditation"

5. **Zapsplat**
   - URL: zapsplat.com
   - Free SFX with account (attribution required)
   - Card sounds, UI sounds, ambient

### Fonts

1. **Google Fonts**
   - URL: fonts.google.com
   - Recommended for dark fantasy: Cinzel, Cormorant Garamond, Playfair Display
   - Recommended for therapeutic: Lora, Source Serif Pro, Nunito

2. **Font Squirrel**
   - URL: fontsquirrel.com
   - Commercial-use-free fonts

---

## Paid Asset Sources (Higher Quality)

### Character Art
- **Unity Asset Store**: search "Visual Novel Character" ($10-50 per set)
- **Fiverr**: commission custom VN characters ($50-200 per character)
- **Skeb**: Japanese artist commissions ($30-100 per piece)

### Backgrounds
- **Unity Asset Store**: "2D Backgrounds Fantasy" ($5-30)
- **GraphicRiver**: fantasy backgrounds ($5-15)

### Complete VN Asset Packs
- **Tyranobuilder Asset Store** (some packs work in Unity)
- **Lemma Soft Forums marketplace**

### Music
- **Epidemic Sound** ($15/month, unlimited commercial use)
- **Artlist** ($16/month)

---

## AI Generation Workflow (Recommended)

For the most consistent, high-quality results, use AI image generation:

### Setup
1. Install Stable Diffusion locally (free) or use Midjourney ($10/month)
2. Pick a consistent style seed/model for each app

### Dark Fantasy Prompts
```
Character: "[name], dark fantasy visual novel portrait, half body, [expression], gothic aesthetic, candlelight, dark purple tones, high quality anime art style, ornate clothing"

Background: "[location], dark fantasy tower interior, atmospheric, candles, purple lighting, gothic architecture, visual novel background style, no characters"

Card: "[card name] tarot card, dark gothic art nouveau, ornate gold frame, mystical symbols, vertical card format, detailed illustration"
```

### Therapeutic/Healing Prompts
```
Character: "[name], gentle fantasy visual novel portrait, half body, [expression], warm lighting, nature aesthetic, green and gold tones, comforting, high quality anime art style"

Background: "[location], cozy sanctuary interior, warm lighting, plants, herbs, natural wood, healing space, visual novel background style, no characters"

Card: "[card name] tarot card, nature art nouveau, gentle watercolor style, floral frame, soft colors, vertical card format"
```

### Expression Consistency
Use img2img or ControlNet to generate expression variants from a base neutral portrait. This keeps the character recognizable across all 8 expressions.

---

## Asset Checklist

### Per Character (x6 = 48 total portraits needed)
- [ ] neutral expression
- [ ] happy expression
- [ ] sad expression
- [ ] angry expression
- [ ] surprised expression
- [ ] thoughtful expression
- [ ] loving expression
- [ ] fearful expression

### Backgrounds (minimum for Phase 1)
- [ ] Tower base (dark) / Garden (heal)
- [ ] Tower stairs (dark) / Entrance (heal)
- [ ] Oracle chamber (dark) / Oracle sanctuary (heal)
- [ ] Oracle chamber evening (dark) / Sanctuary evening (heal)
- [ ] Tower stairs night (dark) / Sanctuary night (heal)
- [ ] Altar room (both)

### Tarot Cards (22 Major Arcana)
- [ ] Card back design (dark + heal variants)
- [ ] The Fool through The World (22 cards)

### UI
- [ ] Dialogue panel
- [ ] Choice button
- [ ] Menu buttons (8 types)
- [ ] Age gate buttons
- [ ] Save slot panel
- [ ] Journal tabs

### Audio
- [ ] 3-5 music tracks (ambient, themes, evening)
- [ ] Card flip SFX
- [ ] Card reveal SFX
- [ ] Card match SFX
- [ ] Choice select SFX
- [ ] Save confirm SFX
- [ ] Skill unlock SFX (heal version)
- [ ] Card shuffle SFX
