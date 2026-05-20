# Clock Face Theme - Screenshot Reference

This document maps the screenshot filenames in the `clock_face` folder to the implemented theme names and their RGB colors.

| Screenshot File | Theme Name | Enum Value | RGB Color | Theme Type |
|----------------|------------|------------|-----------|------------|
| `white_clock_face.png` | White | `ClockDialTheme.White` | `(245, 245, 245)` | Light |
| `turquoise_clock_face.png` | Turquoise | `ClockDialTheme.Turquoise` | `(168, 230, 230)` | Light |
| `glacier_clock_face.png` | Glacier | `ClockDialTheme.Glacier` | `(200, 210, 230)` | Light |
| `ocean_clock_face.png` | Ocean | `ClockDialTheme.Ocean` | `(30, 60, 100)` | Dark |
| `tennis_clock_face.png` | Tennis | `ClockDialTheme.Tennis` | `(50, 150, 80)` | Dark (Special) |
| `signal_blue_clock_face.png` | Signal Blue | `ClockDialTheme.SignalBlue` | `(42, 161, 225)` | Dark |
| `sky_blue_clock_face.png` | Sky Blue | `ClockDialTheme.SkyBlue` | `(196, 220, 232)` | Light |
| `beige_clock_face.png` | Beige | `ClockDialTheme.Beige` | `(212, 196, 168)` | Light |
| `cream_clock_face.png` | Cream | `ClockDialTheme.Cream` | `(240, 232, 216)` | Light |
| `lavender_clock_face.png` | Lavender | `ClockDialTheme.Lavender` | `(212, 200, 224)` | Light |
| `rose_clock_face.png` | Rose | `ClockDialTheme.Rose` | `(232, 180, 200)` | Light |
| `salmon_clock_face.png` | Salmon | `ClockDialTheme.Salmon` | `(232, 165, 132)` | Light |
| `yellow_clock_face.png` | Yellow | `ClockDialTheme.Yellow` | `(245, 184, 0)` | Light |
| `pistachio_clock_face.png` | Pistachio | `ClockDialTheme.Pistachio` | `(196, 212, 166)` | Light |
| `slate_clock_face.png` | Slate | `ClockDialTheme.Slate` | `(84, 84, 84)` | Dark |
| `noir_clock_face.png` | Noir | `ClockDialTheme.Noir` | `(43, 43, 43)` | Dark |

## Theme Type Descriptions

### Light Themes
- Use dark text and hands (approx. `(40, 40, 40)` to `(60, 60, 60)`)
- Suitable for daytime viewing
- Background colors are bright or pastel
- 11 themes total

### Dark Themes
- Use light text and hands (approx. `(200, 200, 200)` to `(220, 220, 220)`)
- Suitable for low-light viewing
- Background colors are deep or saturated
- 5 themes total

### Special Theme: Tennis
- Classified as a dark theme due to deep green background
- **Unique feature**: Uses two different number colors:
  - **Hour numbers (1-12)**: Yellow `(230, 220, 0)`
  - **Minute numbers (05-60)**: Cyan `(100, 200, 180)`
- Inspired by tennis court aesthetics

## Implementation Notes

1. All theme colors were extracted from the provided screenshots
2. RGB values are approximate and calibrated for visual consistency
3. Each theme maintains the same structural design (numbers, markers, hands)
4. The Tennis theme is the only one with multi-colored number display
5. In Night Mode, all themes use black background with user-selected lume color

## Settings Dialog Order

Themes appear in the settings dialog in the following order (left to right, top to bottom):

**Row 1:** White, Turquoise, Glacier, Ocean, Tennis, Signal Blue, Sky Blue, Beige  
**Row 2:** Cream, Lavender, Rose, Salmon, Yellow, Pistachio, Slate, Noir

This arrangement groups similar hues together and provides a logical color progression.
