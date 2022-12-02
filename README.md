# Hierarchical Wave Function Collapse

Reference: 
- Unity WFC Package https://selfsame.itch.io/unitywfc
- WFC https://github.com/mxgmn/WaveFunctionCollapse

This implement a hierarchical version of the Wave Function Collapse algorithm.

It allows to run new WFCs on top of areas produced by WFC in previous layer.

# Examples

## World Map

Implement a simple world map using 3 layers.

1. Layout - generates ocean and continents
2. Biomes - adds islands; seperates continents into grasslands, forests and mountains
3. Details - final WFC for each biome
