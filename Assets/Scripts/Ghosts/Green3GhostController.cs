using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Ghosts
{
    // Goes anywhere randomly but backwards
    public class Green3GhostController: GhostController
    {
        protected override Direction GetNextInput()
        {
            var forbiddenDirection = MovementUtil.GetOppositeDirection(LastMovementDirection);

            return MovementUtil.GetRandomDirection(Position, forbiddenDirection, levelManager);
        }
        
        public override Vector2 initialPosition { get; } = new(15, 15);
        
        protected override List<Direction> GetPathOutOfBox()
        {
            return new List<Direction>
            {
                Direction.LEFT, Direction.DOWN, Direction.DOWN
            };
        }
    }
}