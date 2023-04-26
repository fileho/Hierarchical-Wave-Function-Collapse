# Hierarchical Wave Function Collapse

Maxim Gumin’s Wave Function Collapse (WFC) is a quite recent
algorithm used for procedural content generation. The algorithm uses constraint
solving and local similarity to generate outputs. However, the algorithm struggles
to generate large complex outputs. We aim to generalize the original work to
make the algorithm hierarchical. It is a promising option that could make the
algorithm work in more challenging domains. We will analyze and discover many
improvements to make the hierarchical version applicable.

WFCs are structured into layers. On top of each WFC shape, we can run multiple different WFCs.
This algorithm provides us with much more controllability compared to a single WFC. It can also break
up the homogeny because we can have different areas with different characteristics.

## Setup & Usage
1) Download the project
2) Open it in Unity
3) Open World Map scene or the Dungeon scene
4) Experiment with the algorithm
    1) (Recommended) Run the application (enter play mode) and experiment with the generation
    2) It can also be used in the editor (open Hierarchical Controller and generate layers one by one
5) Try modifying input to some WFC

## Reference: 
- Unity WFC Package https://selfsame.itch.io/unitywfc
- WFC https://github.com/mxgmn/WaveFunctionCollapse
