# Hexagon-Hashing
Hexagon Hashing is a unique tool for managing thousands of objects in a game world. This tool optimizes object components and controls the functionality of various components within the player's field of view. The uniqueness of the algorithms lies in their use of a different approach to visibility checks based on Spatial Hashing, making them well-optimized and tailored to the desired functionality.

* This script disables unnecessary components when an object is out of the player's line of sight, allowing for flexible configuration of objects that require optimization, such as Animators or bot scripts. * While in the field of view, components are automatically enabled and resume operation.

* Thus, it simulates objects within the player's line of sight. The visibility zone utilizes Spatial Hashing techniques.
* Requires the use of the Mirror network library.
