using System.Collections.Generic;
using UnityEngine;

namespace Ghosts
{
    // Runs away from the player
    public class Red1GhostController : GhostController
    {
        protected override Direction GetNextInput()
        {
            return GetNextInputScared();
        }

        public override Vector2 initialPosition { get; } = new(12, 13);
        
        protected override List<Direction> GetPathOutOfBox()
        {
            return new List<Direction>
            {
                Direction.RIGHT, Direction.UP, Direction.UP
            };
        }
    }
}