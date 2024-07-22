# 5D Diplomacy With Multiverse Time Travel (Prototype)

This is the prototype, retained for sentimental reasons. None of the contents of this folder are necessary for running 5D Diplomacy.

Original video on the prototype: https://www.youtube.com/watch?v=2__NGeY5JUw

---

Based on [Diplomacy](https://en.wikipedia.org/wiki/Diplomacy_(game)) and [5D Chess With Multiverse Time Travel](https://en.wikipedia.org/wiki/5D_Chess_with_Multiverse_Time_Travel), 5D Diplomacy With Multiverse Time Travel is an (entirely un-serious) attempt to introduce multiverse and time travelling elements to the classic game of pure negotiation.

The current code takes the simplest non-trivial Diplomacy board and turns it into the starting point of time travel shenanigans.

### Disclaimer

The code is likely rather buggy, especially around retreats, and isn't resistant to badly formed inputs at all.

It's also horrendously ugly. I might fix that in the future, I might not. I consider this hackathon-esque in my approach, so please ignore my entirely undocumented, crazily formatted, paradigm-hopping mess. It works well enough for the demo, and that's all I need.

### Requirements

* Java 14 or higher
* 500 IQ

### How to Play

Compile the source code and run `processing.GameRunner` from the command line or similar.

You'll be prompted to enter orders. You must submit an order for every unit of either colour on an active board (shown as more opaque) and cannot submit orders for units on inactive boards.

Enter orders on separate lines and write `r` on a new line to finish submitting orders and resolve.

The format for orders is as follows:

* Hold: `x1,x2,x3`, where the `x1` is the column of the board on which the unit lies (left-most column is `0`), `x2` is the row of the board on which the unit lies (may be negative as the centre is `0`), and `x3` is one of `[0, 1, 2]` to specify the unit's location within a board. Here, `0` refers to the left region of the triangle, `1` to the top region and `2` to the right region.

* Move: `x1,x2,x3 m y1,y2,y3` where `x1,x2,x3` specifies the unit's location as above, and `y1,y2,y3` similarly specifes the unit's destination. Units can move anywhere on their own board or anywhere on boards one space away horizontally, vertically or diagonally.

* Support: `x1,x2,x3 s y1,y2,y3 z1,z2,z3` where `x1,x2,x3` is the supporting unit's location, `y1,y2,y3` is the supported unit's location and `z1,z2,z3` is the supported unit's destination. If `y1,y2,y3` and `z1,z2,z3` match, the order is a support to hold in place. The supported destination is subject to the same constraints as a move order, i.e. a unit can only support to places it could move.

If retreats are required, you'll be asked to enter orders again for each dislodged unit only. Disband orders have the same format as holds, and a retreat move has the same format as a regular move.