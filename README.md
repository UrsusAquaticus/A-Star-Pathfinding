# A* Pathfinding
The scripts for an incomplete game I was experimenting with.
Last I left off, I was working on A* pathfinding performance, the next step was implementing inter-unit combat.
* A grid is randomly generated with either walkable or wall tiles.
* Each "unit" here is requesting a path to a random unoccupied/reserved walkable tile, then making it's way there following the provided path.
* When the player clicks, drags and releases, the units within the rectangle are be selected.
* Right clicking while units are selected will cause said units to request a path to the nearest unoccupied/reserved walkable tile to the clicked location.
* The units then stay at their final tile until a time has elapsed and they return to wandering randomly.

https://user-images.githubusercontent.com/64456163/144381009-a5bba6f2-3227-4d79-9abf-82db7e95d809.mp4
