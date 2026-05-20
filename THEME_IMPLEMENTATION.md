# Clock Face Theme Implementation

## Overview
This document describes the implementation of 16 distinct clock face themes for the Bauhaus Clock screensaver, based on the screenshots provided in the `clock_face` folder.

## Implemented Themes

### 1. **White** 
- Background: `#F5F5F5` (245, 245, 245)
- Text/Hands: Dark gray
- Light, minimal design

### 2. **Turquoise** 
- Background: `#A8E6E6` (168, 230, 230)
- Text/Hands: Dark gray
- Fresh, calming cyan-blue tone

### 3. **Glacier** 
- Background: `#C8D2E6` (200, 210, 230)
- Text/Hands: Dark gray
- Cool, icy blue-gray

### 4. **Ocean** 
- Background: `#1E3C64` (30, 60, 100)
- Text/Hands: Light gray/white
- Deep, dark blue (dark theme)

### 5. **Tennis** 
- Background: `#329650` (50, 150, 80)
- Hour numbers: Yellow `#E6DC00` (230, 220, 0)
- Minute numbers: Cyan `#64C8B4` (100, 200, 180)
- Hands: Light gray/white
- Green court-inspired (dark theme with special colored numbers)

### 6. **Signal Blue** 
- Background: `#2AA1E1` (42, 161, 225)
- Text/Hands: Light gray/white
- Bright, vibrant cyan (dark theme)

### 7. **Sky Blue** 
- Background: `#C4DCE8` (196, 220, 232)
- Text/Hands: Dark gray
- Soft, light sky blue

### 8. **Beige** 
- Background: `#D4C4A8` (212, 196, 168)
- Text/Hands: Dark gray
- Warm, neutral tan

### 9. **Cream** 
- Background: `#F0E8D8` (240, 232, 216)
- Text/Hands: Dark gray
- Soft, off-white cream

### 10. **Lavender** 
- Background: `#D4C8E0` (212, 200, 224)
- Text/Hands: Dark gray
- Gentle purple-gray

### 11. **Rose** 
- Background: `#E8B4C8` (232, 180, 200)
- Text/Hands: Dark gray
- Soft pink

### 12. **Salmon** 
- Background: `#E8A584` (232, 165, 132)
- Text/Hands: Dark gray
- Warm peach-salmon

### 13. **Yellow** 
- Background: `#F5B800` (245, 184, 0)
- Text/Hands: Dark brown/black
- Bright, vibrant yellow

### 14. **Pistachio** 
- Background: `#C4D4A6` (196, 212, 166)
- Text/Hands: Dark gray
- Light, muted green

### 15. **Slate** 
- Background: `#545454` (84, 84, 84)
- Text/Hands: Light gray/white
- Medium dark gray (dark theme)

### 16. **Noir** 
- Background: `#2B2B2B` (43, 43, 43)
- Text/Hands: Light gray/white
- Very dark charcoal (dark theme)

## Visual Design Elements

All themes share the following design elements based on the bauhausclock.com design:

### Numbers
- **Hour numbers (1-12)**: Bold, large font displayed at each hour position
- **Minute numbers (05, 10, 15...60)**: Smaller font at 5-minute intervals
- Font: Arial Bold for hours, Arial Regular for minutes

### Tick Marks
- **5-minute markers**: White/cream rounded rectangles with 3D gradient effect
  - Gradient from white (#FFFFFF) to light gray (#D2D2D2)
  - Subtle shadow for depth
  - Rounded corners (capsule shape)
  - Only displayed in day mode (not night mode)
- **Minute markers**: Thin lines between 5-minute markers

### Hands
- **Hour hand**: Shorter, wider rounded rectangle
- **Minute hand**: Longer, similar width to hour hand
- **Second hand**: Thin, longest hand
- All hands have:
  - Rounded rectangular (capsule) shape
  - Subtle shadow for 3D effect
  - Color matches theme (light for dark themes, dark for light themes)

### Center Hub
- Circular pin at the center
- Light color matching hands
- Subtle shadow and highlight for 3D effect

## Theme Classification

### Light Themes (Dark text on light background)
- White, Turquoise, Glacier, Sky Blue, Beige, Cream, Lavender, Rose, Salmon, Yellow, Pistachio

### Dark Themes (Light text on dark background)
- Ocean, Tennis, Signal Blue, Slate, Noir

### Special Theme
- **Tennis**: Has yellow hour numbers and cyan minute numbers instead of uniform coloring

## Code Changes

### Files Modified

1. **BauhausConfig.cs**
   - Extended `ClockDialTheme` enum from 5 to 16 themes

2. **BauhausClock.cs**
   - Added 11 new theme color definitions
   - Updated `GetBackgroundColor()` to handle all 16 themes
   - Added `IsDarkTheme()` method to determine text/hand color
   - Updated `GetTickColor()`, `GetNumberColor()`, `GetHandColor()` to use `IsDarkTheme()`
   - Enhanced `DrawNumbers()` to support special colors for Tennis theme
   - Completely redesigned `DrawTickMarks()` to render 5-minute markers as rounded rectangles
   - Added `DrawFiveMinuteMarker()` method for 3D rounded rectangle markers

3. **BauhausSettingsForm.cs**
   - Increased form size from 560x760 to 820x840
   - Updated dial panel size to accommodate 16 themes
   - Changed dial theme button array from 5 to 16 elements
   - Arranged themes in 2 rows of 8 buttons
   - Added labels and colors for all 11 new themes

## Technical Details

### 3D Effect Implementation
The 5-minute markers use a multi-layer approach:
1. **Shadow layer**: Slightly offset dark semi-transparent fill
2. **Main fill**: Linear gradient from white to light gray (vertical)
3. **Border**: Subtle gray outline for definition

### Tennis Theme Special Handling
```csharp
if (config.DialTheme == BauhausConfig.ClockDialTheme.Tennis && !IsNightMode())
{
    hourColor = Color.FromArgb(230, 220, 0);     // Yellow
    minuteColor = Color.FromArgb(100, 200, 180); // Cyan
}
```

### Dark Theme Detection
```csharp
private bool IsDarkTheme()
{
    return config.DialTheme == BauhausConfig.ClockDialTheme.Ocean ||
           config.DialTheme == BauhausConfig.ClockDialTheme.Tennis ||
           config.DialTheme == BauhausConfig.ClockDialTheme.Slate ||
           config.DialTheme == BauhausConfig.ClockDialTheme.Noir ||
           config.DialTheme == BauhausConfig.ClockDialTheme.SignalBlue;
}
```

## Night Mode
All themes support Night Mode, which:
- Overrides the background with pure black (#0A0A0A)
- Uses the user-selected lume color for all text and hands
- Uses simple tick marks instead of 3D rounded rectangles for performance

## User Interface
The settings dialog displays all 16 themes as circular preview buttons arranged in a 2×8 grid, showing:
- Theme background color
- Miniature clock markings
- Theme name label
- Selection highlight border

## Compatibility
- All themes work with all three movement types (Quartz, Mechanical, Digital)
- All themes work with both size options (Classic, Compact)
- All themes support night mode with customizable lume color
