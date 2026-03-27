#!/usr/bin/env python3
"""
Generates all placeholder SVG assets for Endless, Beloved.
Run: python3 generate_placeholders.py
These SVGs can be imported into Unity or converted to PNG.
"""
import os

def write_svg(path, content):
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with open(path, 'w') as f:
        f.write(content)
    print(f"  Created: {path}")

# ═══════════════════════════════════════════════════════════════════
# DARK FANTASY BACKGROUNDS
# ═══════════════════════════════════════════════════════════════════

DARK_BACKGROUNDS = {
    "tower_stairs": ("#1a0a2e", "#2d1b4e", "#0d0d0d", "TOWER STAIRS", [
        '<line x1="100" y1="1800" x2="500" y2="200" stroke="#333" stroke-width="3"/>',
        '<line x1="980" y1="1800" x2="580" y2="200" stroke="#333" stroke-width="3"/>',
        '<rect x="200" y="1600" width="680" height="40" rx="5" fill="#252540" stroke="#444" stroke-width="1"/>',
        '<rect x="250" y="1400" width="580" height="40" rx="5" fill="#252540" stroke="#444" stroke-width="1"/>',
        '<rect x="300" y="1200" width="480" height="40" rx="5" fill="#252540" stroke="#444" stroke-width="1"/>',
        '<rect x="340" y="1000" width="400" height="40" rx="5" fill="#252540" stroke="#444" stroke-width="1"/>',
        '<rect x="370" y="800" width="340" height="40" rx="5" fill="#252540" stroke="#444" stroke-width="1"/>',
        '<circle cx="540" cy="300" r="60" fill="#4a1942" opacity="0.4"/>',
    ]),
    "oracle_chamber": ("#1a0a2e", "#3d1b6e", "#1a0a2e", "ORACLE CHAMBER", [
        '<circle cx="540" cy="800" r="200" fill="#2a1050" opacity="0.5"/>',
        '<circle cx="540" cy="800" r="100" fill="#4a1080" opacity="0.3"/>',
        '<rect x="340" y="750" width="400" height="200" rx="10" fill="#1a1a2e" stroke="#6a3090" stroke-width="2"/>',
        '<circle cx="300" cy="600" r="30" fill="#ff8800" opacity="0.3"/>',
        '<circle cx="780" cy="600" r="30" fill="#ff8800" opacity="0.3"/>',
        '<circle cx="540" cy="500" r="25" fill="#ff8800" opacity="0.25"/>',
    ]),
    "oracle_chamber_evening": ("#0d0520", "#1a0a2e", "#050010", "ORACLE CHAMBER - EVENING", [
        '<circle cx="540" cy="800" r="200" fill="#1a0840" opacity="0.5"/>',
        '<rect x="340" y="750" width="400" height="200" rx="10" fill="#0d0d1e" stroke="#4a2070" stroke-width="2"/>',
        '<circle cx="300" cy="600" r="25" fill="#ff6600" opacity="0.2"/>',
        '<circle cx="780" cy="600" r="25" fill="#ff6600" opacity="0.2"/>',
    ]),
    "tower_stairs_night": ("#050010", "#0d0520", "#000005", "TOWER STAIRS - NIGHT", [
        '<line x1="100" y1="1800" x2="500" y2="200" stroke="#1a1a30" stroke-width="3"/>',
        '<line x1="980" y1="1800" x2="580" y2="200" stroke="#1a1a30" stroke-width="3"/>',
        '<circle cx="540" cy="400" r="80" fill="#1a0a3e" opacity="0.3"/>',
    ]),
    "altar_room": ("#1a0a2e", "#2d1040", "#0d0d1a", "ALTAR ROOM", [
        '<rect x="240" y="900" width="600" height="400" rx="10" fill="#1a1a2e" stroke="#6a3090" stroke-width="2"/>',
        '<circle cx="540" cy="850" r="60" fill="#ff8800" opacity="0.2"/>',
        '<rect x="440" y="950" width="200" height="20" rx="5" fill="#3a2060"/>',
        '<circle cx="380" cy="1000" r="15" fill="#8844cc" opacity="0.5"/>',
        '<circle cx="700" cy="1000" r="15" fill="#8844cc" opacity="0.5"/>',
    ]),
}

HEAL_BACKGROUNDS = {
    "sanctuary_garden": ("#1a4a2a", "#2d6b3e", "#0d2d1a", "SANCTUARY GARDEN", [
        '<circle cx="200" cy="600" r="150" fill="#2a6030" opacity="0.4"/>',
        '<circle cx="800" cy="500" r="180" fill="#2a6030" opacity="0.3"/>',
        '<circle cx="500" cy="400" r="120" fill="#3a7040" opacity="0.3"/>',
        '<rect x="0" y="1400" width="1080" height="520" fill="#1a3a20" opacity="0.6"/>',
        '<circle cx="300" cy="1500" r="40" fill="#4a8050" opacity="0.4"/>',
        '<circle cx="700" cy="1480" r="35" fill="#4a8050" opacity="0.4"/>',
    ]),
    "sanctuary_entrance": ("#2a4a3a", "#3d6b4e", "#1a3a2a", "SANCTUARY ENTRANCE", [
        '<rect x="340" y="400" width="400" height="600" rx="20" fill="#2a3a2a" stroke="#5a8060" stroke-width="3"/>',
        '<rect x="380" y="440" width="320" height="500" rx="15" fill="#3a5040" opacity="0.5"/>',
        '<circle cx="200" cy="300" r="100" fill="#3a7040" opacity="0.3"/>',
        '<circle cx="880" cy="350" r="120" fill="#3a7040" opacity="0.3"/>',
    ]),
    "oracle_sanctuary": ("#2a3a2a", "#3d5040", "#1a2a1a", "ORACLE SANCTUARY", [
        '<rect x="200" y="700" width="680" height="400" rx="15" fill="#2a3a2a" stroke="#6a9070" stroke-width="2"/>',
        '<rect x="350" y="750" width="380" height="200" rx="10" fill="#1a2a1a" stroke="#5a8060" stroke-width="1"/>',
        '<circle cx="300" cy="650" r="25" fill="#ffcc44" opacity="0.3"/>',
        '<circle cx="780" cy="650" r="25" fill="#ffcc44" opacity="0.3"/>',
    ]),
    "sanctuary_evening": ("#1a3020", "#2a4a30", "#0d1a0d", "SANCTUARY - EVENING", [
        '<circle cx="800" cy="300" r="100" fill="#ff8844" opacity="0.15"/>',
        '<rect x="0" y="1400" width="1080" height="520" fill="#0d1a0d" opacity="0.6"/>',
    ]),
    "sanctuary_night": ("#0d1a0d", "#1a2a1a", "#050d05", "SANCTUARY - NIGHT", [
        '<circle cx="540" cy="200" r="40" fill="#ddddaa" opacity="0.15"/>',
        '<circle cx="300" cy="150" r="2" fill="#fff" opacity="0.6"/>',
        '<circle cx="700" cy="100" r="2" fill="#fff" opacity="0.6"/>',
        '<circle cx="450" cy="80" r="1.5" fill="#fff" opacity="0.5"/>',
        '<circle cx="850" cy="180" r="1.5" fill="#fff" opacity="0.5"/>',
    ]),
}

def make_background(name, c1, c2, c3, label, extras):
    extra_str = "\n  ".join(extras)
    return f'''<svg xmlns="http://www.w3.org/2000/svg" width="1080" height="1920" viewBox="0 0 1080 1920">
  <defs>
    <linearGradient id="bg" x1="0" y1="0" x2="0" y2="1">
      <stop offset="0%" stop-color="{c1}"/>
      <stop offset="50%" stop-color="{c2}"/>
      <stop offset="100%" stop-color="{c3}"/>
    </linearGradient>
  </defs>
  <rect width="1080" height="1920" fill="url(#bg)"/>
  {extra_str}
  <text x="540" y="1860" text-anchor="middle" fill="#666" font-size="20" font-family="sans-serif">{label}</text>
</svg>'''

# ═══════════════════════════════════════════════════════════════════
# CHARACTER SILHOUETTES
# ═══════════════════════════════════════════════════════════════════

def make_character_portrait(name, expression, color1, color2, label):
    # Eye expression variations
    eyes = {
        "neutral": '<ellipse cx="220" cy="280" rx="15" ry="12" fill="#fff" opacity="0.8"/><ellipse cx="310" cy="280" rx="15" ry="12" fill="#fff" opacity="0.8"/>',
        "happy": '<path d="M205,285 Q220,270 235,285" fill="none" stroke="#fff" stroke-width="3" opacity="0.8"/><path d="M295,285 Q310,270 325,285" fill="none" stroke="#fff" stroke-width="3" opacity="0.8"/>',
        "sad": '<path d="M205,275 Q220,285 235,275" fill="none" stroke="#fff" stroke-width="3" opacity="0.8"/><path d="M295,275 Q310,285 325,275" fill="none" stroke="#fff" stroke-width="3" opacity="0.8"/>',
        "angry": '<line x1="200" y1="270" x2="235" y2="275" stroke="#fff" stroke-width="3" opacity="0.8"/><line x1="330" y1="270" x2="295" y2="275" stroke="#fff" stroke-width="3" opacity="0.8"/><ellipse cx="220" cy="285" rx="12" ry="10" fill="#fff" opacity="0.7"/><ellipse cx="310" cy="285" rx="12" ry="10" fill="#fff" opacity="0.7"/>',
        "surprised": '<circle cx="220" cy="280" r="18" fill="none" stroke="#fff" stroke-width="3" opacity="0.8"/><circle cx="310" cy="280" r="18" fill="none" stroke="#fff" stroke-width="3" opacity="0.8"/>',
        "thoughtful": '<ellipse cx="220" cy="282" rx="14" ry="10" fill="#fff" opacity="0.6"/><ellipse cx="310" cy="278" rx="14" ry="10" fill="#fff" opacity="0.8"/>',
        "loving": '<path d="M205,285 Q220,270 235,285" fill="none" stroke="#ffaacc" stroke-width="3" opacity="0.9"/><path d="M295,285 Q310,270 325,285" fill="none" stroke="#ffaacc" stroke-width="3" opacity="0.9"/>',
        "fearful": '<ellipse cx="220" cy="278" rx="18" ry="15" fill="#fff" opacity="0.9"/><ellipse cx="310" cy="278" rx="18" ry="15" fill="#fff" opacity="0.9"/><circle cx="220" cy="280" r="6" fill="#000"/><circle cx="310" cy="280" r="6" fill="#000"/>',
    }
    eye_svg = eyes.get(expression, eyes["neutral"])

    return f'''<svg xmlns="http://www.w3.org/2000/svg" width="540" height="960" viewBox="0 0 540 960">
  <defs>
    <linearGradient id="body" x1="0" y1="0" x2="0" y2="1">
      <stop offset="0%" stop-color="{color1}"/>
      <stop offset="100%" stop-color="{color2}"/>
    </linearGradient>
    <radialGradient id="face_glow" cx="50%" cy="35%" r="30%">
      <stop offset="0%" stop-color="{color1}" stop-opacity="0.3"/>
      <stop offset="100%" stop-color="transparent"/>
    </radialGradient>
  </defs>
  <rect width="540" height="960" fill="transparent"/>
  <!-- Body silhouette -->
  <ellipse cx="270" cy="700" rx="160" ry="260" fill="url(#body)" opacity="0.8"/>
  <!-- Head -->
  <circle cx="265" cy="250" r="120" fill="{color1}" opacity="0.9"/>
  <!-- Face glow -->
  <circle cx="265" cy="240" r="90" fill="url(#face_glow)"/>
  <!-- Eyes -->
  {eye_svg}
  <!-- Mouth hint -->
  <line x1="240" y1="320" x2="290" y2="320" stroke="#fff" stroke-width="2" opacity="0.3"/>
  <!-- Label -->
  <text x="270" y="920" text-anchor="middle" fill="#888" font-size="18" font-family="sans-serif">{label} - {expression}</text>
</svg>'''

# ═══════════════════════════════════════════════════════════════════
# CARD ASSETS
# ═══════════════════════════════════════════════════════════════════

def make_card_back(theme="dark"):
    if theme == "dark":
        c1, c2, border = "#1a0a3e", "#3d1b6e", "#8844cc"
    else:
        c1, c2, border = "#1a3a2a", "#3d6b4e", "#6a9070"

    return f'''<svg xmlns="http://www.w3.org/2000/svg" width="300" height="500" viewBox="0 0 300 500">
  <defs>
    <linearGradient id="cbg" x1="0" y1="0" x2="1" y2="1">
      <stop offset="0%" stop-color="{c1}"/>
      <stop offset="100%" stop-color="{c2}"/>
    </linearGradient>
  </defs>
  <rect width="300" height="500" rx="15" fill="url(#cbg)" stroke="{border}" stroke-width="3"/>
  <rect x="20" y="20" width="260" height="460" rx="10" fill="none" stroke="{border}" stroke-width="1" opacity="0.5"/>
  <!-- Central symbol -->
  <circle cx="150" cy="250" r="60" fill="none" stroke="{border}" stroke-width="2" opacity="0.6"/>
  <circle cx="150" cy="250" r="40" fill="none" stroke="{border}" stroke-width="1.5" opacity="0.4"/>
  <circle cx="150" cy="250" r="20" fill="{border}" opacity="0.3"/>
  <!-- Corner symbols -->
  <circle cx="45" cy="45" r="12" fill="{border}" opacity="0.3"/>
  <circle cx="255" cy="45" r="12" fill="{border}" opacity="0.3"/>
  <circle cx="45" cy="455" r="12" fill="{border}" opacity="0.3"/>
  <circle cx="255" cy="455" r="12" fill="{border}" opacity="0.3"/>
  <!-- Star pattern -->
  <polygon points="150,180 160,220 200,225 170,250 180,290 150,265 120,290 130,250 100,225 140,220" fill="{border}" opacity="0.2"/>
</svg>'''

def make_tarot_card(card_id, card_name, number, color):
    return f'''<svg xmlns="http://www.w3.org/2000/svg" width="300" height="500" viewBox="0 0 300 500">
  <defs>
    <linearGradient id="cbg" x1="0" y1="0" x2="0" y2="1">
      <stop offset="0%" stop-color="#0d0d1a"/>
      <stop offset="100%" stop-color="#1a0a2e"/>
    </linearGradient>
  </defs>
  <rect width="300" height="500" rx="15" fill="url(#cbg)" stroke="{color}" stroke-width="2"/>
  <rect x="15" y="15" width="270" height="470" rx="10" fill="none" stroke="{color}" stroke-width="1" opacity="0.4"/>
  <!-- Number -->
  <text x="150" y="60" text-anchor="middle" fill="{color}" font-size="28" font-family="serif">{number}</text>
  <!-- Central art area -->
  <rect x="40" y="80" width="220" height="300" rx="8" fill="#111122" stroke="{color}" stroke-width="1" opacity="0.5"/>
  <text x="150" y="240" text-anchor="middle" fill="{color}" font-size="48" font-family="serif" opacity="0.4">{number}</text>
  <!-- Card name -->
  <text x="150" y="430" text-anchor="middle" fill="#ddd" font-size="22" font-family="serif">{card_name}</text>
  <!-- Decorative line -->
  <line x1="60" y1="450" x2="240" y2="450" stroke="{color}" stroke-width="1" opacity="0.3"/>
</svg>'''

# ═══════════════════════════════════════════════════════════════════
# UI ELEMENTS
# ═══════════════════════════════════════════════════════════════════

def make_dialogue_panel(theme="dark"):
    if theme == "dark":
        bg, border, accent = "rgba(10,5,20,0.9)", "#6a3090", "#8844cc"
    else:
        bg, border, accent = "rgba(10,25,15,0.9)", "#5a8060", "#6a9070"

    return f'''<svg xmlns="http://www.w3.org/2000/svg" width="1080" height="480" viewBox="0 0 1080 480">
  <rect width="1080" height="480" rx="20" fill="{bg}" stroke="{border}" stroke-width="2"/>
  <rect x="10" y="10" width="1060" height="460" rx="15" fill="none" stroke="{accent}" stroke-width="1" opacity="0.3"/>
  <!-- Speaker name area -->
  <rect x="30" y="20" width="300" height="50" rx="10" fill="{accent}" opacity="0.2"/>
</svg>'''

def make_button(text, theme="dark", width=400, height=80):
    if theme == "dark":
        bg1, bg2, border, text_color = "#2d1b4e", "#1a0a2e", "#8844cc", "#ddccff"
    else:
        bg1, bg2, border, text_color = "#2a4a3a", "#1a3a2a", "#6a9070", "#cceecc"

    return f'''<svg xmlns="http://www.w3.org/2000/svg" width="{width}" height="{height}" viewBox="0 0 {width} {height}">
  <defs>
    <linearGradient id="btn" x1="0" y1="0" x2="0" y2="1">
      <stop offset="0%" stop-color="{bg1}"/>
      <stop offset="100%" stop-color="{bg2}"/>
    </linearGradient>
  </defs>
  <rect width="{width}" height="{height}" rx="12" fill="url(#btn)" stroke="{border}" stroke-width="2"/>
  <text x="{width//2}" y="{height//2 + 8}" text-anchor="middle" fill="{text_color}" font-size="24" font-family="sans-serif">{text}</text>
</svg>'''

def make_choice_button(theme="dark"):
    if theme == "dark":
        bg, border, text_color = "rgba(30,15,50,0.8)", "#6a3090", "#ccbbee"
    else:
        bg, border, text_color = "rgba(20,40,25,0.8)", "#5a8060", "#bbddbb"

    return f'''<svg xmlns="http://www.w3.org/2000/svg" width="900" height="70" viewBox="0 0 900 70">
  <rect width="900" height="70" rx="10" fill="{bg}" stroke="{border}" stroke-width="1.5"/>
  <text x="30" y="44" fill="{text_color}" font-size="22" font-family="sans-serif">Choice text here</text>
</svg>'''

# ═══════════════════════════════════════════════════════════════════
# GENERATE ALL
# ═══════════════════════════════════════════════════════════════════

print("Generating Dark Fantasy backgrounds...")
for name, (c1, c2, c3, label, extras) in DARK_BACKGROUNDS.items():
    write_svg(f"Assets/_Project/Art/Backgrounds/{name}.svg", make_background(name, c1, c2, c3, label, extras))

print("\nGenerating Healing backgrounds...")
for name, (c1, c2, c3, label, extras) in HEAL_BACKGROUNDS.items():
    write_svg(f"Assets/_Project/Art/Backgrounds/heal_{name}.svg", make_background(name, c1, c2, c3, label, extras))

# Character portraits
print("\nGenerating character portraits...")
CHARACTERS = {
    "oracle":     ("#6a3090", "#3d1b6e"),
    "angel":      ("#cc8844", "#8a5522"),
    "keeper":     ("#4466aa", "#223366"),
    "wanderer":   ("#44aa66", "#226644"),
    "apprentice": ("#aa4466", "#662244"),
    "weaver":     ("#888888", "#444444"),
}
EXPRESSIONS = ["neutral", "happy", "sad", "angry", "surprised", "thoughtful", "loving", "fearful"]

for char_id, (c1, c2) in CHARACTERS.items():
    for expr in EXPRESSIONS:
        label = char_id.upper()
        write_svg(f"Assets/_Project/Art/Characters/{char_id}_neutral_{expr}.svg",
                  make_character_portrait(char_id, expr, c1, c2, label))

# Card assets
print("\nGenerating card assets...")
write_svg("Assets/_Project/Art/Cards/card_back_dark.svg", make_card_back("dark"))
write_svg("Assets/_Project/Art/Cards/card_back_heal.svg", make_card_back("heal"))

CARDS = [
    ("fool", "The Fool", "0", "#ffcc44"),
    ("magician", "The Magician", "I", "#ff4444"),
    ("high_priestess", "The High Priestess", "II", "#4488ff"),
    ("empress", "The Empress", "III", "#44cc44"),
    ("emperor", "The Emperor", "IV", "#cc4444"),
    ("hierophant", "The Hierophant", "V", "#8844cc"),
    ("lovers", "The Lovers", "VI", "#ff88aa"),
    ("chariot", "The Chariot", "VII", "#cc8844"),
    ("strength", "Strength", "VIII", "#ffaa44"),
    ("hermit", "The Hermit", "IX", "#6688aa"),
    ("wheel", "Wheel of Fortune", "X", "#44cccc"),
    ("justice", "Justice", "XI", "#cccc44"),
    ("hanged_man", "The Hanged Man", "XII", "#4466cc"),
    ("death", "Death", "XIII", "#222222"),
    ("temperance", "Temperance", "XIV", "#88aacc"),
    ("devil", "The Devil", "XV", "#cc2222"),
    ("tower", "The Tower", "XVI", "#ff4400"),
    ("star", "The Star", "XVII", "#aaccff"),
    ("moon", "The Moon", "XVIII", "#aaaacc"),
    ("sun", "The Sun", "XIX", "#ffdd44"),
    ("judgement", "Judgement", "XX", "#ff8844"),
    ("world", "The World", "XXI", "#44ff88"),
]

for card_id, name, number, color in CARDS:
    write_svg(f"Assets/_Project/Art/Cards/card_{card_id}.svg", make_tarot_card(card_id, name, number, color))

# UI elements
print("\nGenerating UI elements...")
write_svg("Assets/_Project/Art/UI/dialogue_panel_dark.svg", make_dialogue_panel("dark"))
write_svg("Assets/_Project/Art/UI/dialogue_panel_heal.svg", make_dialogue_panel("heal"))
write_svg("Assets/_Project/Art/UI/choice_button_dark.svg", make_choice_button("dark"))
write_svg("Assets/_Project/Art/UI/choice_button_heal.svg", make_choice_button("heal"))

for label in ["New Game", "Continue", "Settings", "Save", "Journal", "Spells", "Routes", "Daily Reading"]:
    safe = label.lower().replace(" ", "_")
    write_svg(f"Assets/_Project/Art/UI/btn_{safe}_dark.svg", make_button(label, "dark"))
    write_svg(f"Assets/_Project/Art/UI/btn_{safe}_heal.svg", make_button(label, "heal"))

write_svg("Assets/_Project/Art/UI/btn_confirm_18.svg", make_button("I am 18+", "dark", 500, 100))
write_svg("Assets/_Project/Art/UI/btn_deny_18.svg", make_button("I am under 18", "dark", 500, 100))

print(f"\nDone! Generated all placeholder SVG assets.")
