using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Ghosts
{
    // Runs torwards the player
    public class Purple2GhostController : GhostController
    {
        protected override Direction GetNextInput()
        {
            var playerPosition = Player.GetPosition();

            var forbiddenDirection = MovementUtil.GetOppositeDirection(LastMovementDirection);

            return MovementUtil.FindClosestDirectionToPlayer(Position, playerPosition, forbiddenDirection, levelManager);
        }
        
        public override Vector2 initialPosition { get; } = new(12, 15);
        
        protected override List<Direction> GetPathOutOfBox()
        {
            return new List<Direction>
            {
                Direction.RIGHT, Direction.DOWN, Direction.DOWN
            };
        }
    }
}