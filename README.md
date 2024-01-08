# Hexagon Hashing

Hexagon Hashing is a versatile tool designed to efficiently manage objects in a game world, providing both general functionality and specialized interaction with Animator components. Hexagon Hashing drew inspiration from the sale of an optimization script for the Mirror network library. The script, which utilized a similar Spatial Hashing technique, served as the foundation for Hexagon Hashing's efficient visibility management.

## General Functionality

### Overview Video

https://github.com/LobosProger/Hexagon-Hashing/assets/78168123/84eb042d-76e8-4707-933a-517efea17993

- **Spatial Hashing Optimization:** Demonstrates the core functionality of Hexagon Hashing, utilizing Spatial Hashing for efficient management of objects based on player visibility. This ensures that only relevant objects are active, optimizing performance. On the example, you can see, how signaling lamps activating if those entering in the field of player visibility.

### Key Features
- **Dynamic Visibility Management:** Disables unnecessary components for objects outside the player's field of view, allowing for flexible configuration of objects requiring optimization (e.g., Animators, bot scripts).

- **Automatic Activation:** Components are automatically enabled and resume operation when within the player's field of view, creating a dynamic simulation of objects based on their visibility.

- **Spatial Hashing Techniques:** Efficiently identifies objects within the player's line of sight using Spatial Hashing, a powerful algorithm for quick visibility checks.

## Animator Component Interaction

- **Specialized Interaction with Animator Components:** Showcases Hexagon Hashing's capabilities with Animator components, providing activation and reconfiguration based on visibility.

- **Animator Activation:** Specifically configured for Animator components, the script enables and adjusts them when within the player's field of view.

- **State Adjustment:** For example, when a signaling lamp enters the player's field of view, the script not only activates the Animator but also adjusts it to display the current state, providing a seamless experience:

https://github.com/LobosProger/Hexagon-Hashing/assets/78168123/a7254ef4-99d7-4bcc-baec-25fdcbfd9f7c

Below you can see attachable components to gameobjects. In the first one, you can change time of rebuilding (time to check, what objects are entered in field of view) and changing the size of visibility by changing grid component values:

![Screenshot 2024-01-08 203557](https://github.com/LobosProger/Hexagon-Hashing/assets/78168123/eed7905f-3c3d-4719-96ef-4f646cbbf58d)

And the second need place to objects, which need to optimize. To its you can attach components of gameobject, which need to dynamically turn off:

![Screenshot 2024-01-08 203621](https://github.com/LobosProger/Hexagon-Hashing/assets/78168123/73ab28c6-dbf4-489a-8cbb-150debbabd94)


