using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Ghosts
{
    // Tries to go left, then forward, then right, then backwards
    public class Blue4GhostController : GhostController
    {
        public override Vector2 initialPosition { get; } = new(15, 13);

        protected override Direction GetNextInput()
        {
            return MovementUtil.FindNextClockwisePosition(Position, LastMovementDirection, levelManager);
        }

        protected override List<Direction> GetPathOutOfBox()
        {
            // I needed to put him on the edge to make it easier for the path
            return new List<Direction>
            {
                Direction.LEFT, Direction.UP, Direction.UP, Direction.RIGHT, Direction.RIGHT, Direction.RIGHT,
                Direction.RIGHT, Direction.DOWN, Direction.DOWN, Direction.DOWN, Direction.RIGHT, Direction.RIGHT, Direction.RIGHT,
                Direction.DOWN
            };
        }
    }
}