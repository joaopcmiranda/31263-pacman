using UnityEngine;

namespace Util
{
    public static class MovementUtil
    {
        public static Vector2 GetDirectionVector2(Direction direction)
        {
            return direction switch
            {
                Direction.UP => Vector2.up,
                Direction.RIGHT => Vector2.right,
                Direction.DOWN => Vector2.down,
                Direction.LEFT => Vector2.left,
                _ => Vector2.zero
            };
        }

        public static Vector2 GetTargetPosition(Vector2 currentPosition, Vector2 change)
        {
            return new Vector2(currentPosition.x + change.x, currentPosition.y - change.y);
        }

        public static Direction GetOppositeDirection(Direction direction)
        {
            return direction switch
            {
                Direction.UP => Direction.DOWN,
                Direction.RIGHT => Direction.LEFT,
                Direction.DOWN => Direction.UP,
                Direction.LEFT => Direction.RIGHT,
                _ => Direction.NONE
            };
        }
        
        public static Direction GetLeftDirection(Direction direction)
        {
            return direction switch
            {
                Direction.UP => Direction.LEFT,
                Direction.RIGHT => Direction.UP,
                Direction.DOWN => Direction.RIGHT,
                Direction.LEFT => Direction.DOWN,
                _ => Direction.NONE
            };
        }
        
        public static Direction GetRightDirection(Direction direction)
        {
            return direction switch
            {
                Direction.UP => Direction.RIGHT,
                Direction.RIGHT => Direction.DOWN,
                Direction.DOWN => Direction.LEFT,
                Direction.LEFT => Direction.UP,
                _ => Direction.NONE
            };
        }

        public static Direction FindFurthestDirectionFromPlayer(Vector2 ghostPosition, Vector2 playerPosition,
            Direction forbiddenDirection, Level1Manager levelManager)
        {
            var bestDirection = Direction.NONE;
            var bestDistance = float.MinValue;

            // iterate over all directions
            for (var i = 0; i < 4; i++)
            {
                var direction = (Direction)i;
                if (direction == forbiddenDirection || direction == Direction.NONE) continue;

                // get the vector for the direction
                var directionVector = GetDirectionVector2(direction);

                // get the target position
                var targetPosition = GetTargetPosition(ghostPosition, directionVector);

                if (!levelManager.IsTileWalkableForGhosts(targetPosition)) continue;

                // get the distance from the player
                var distanceFromPlayer = Vector2.Distance(targetPosition, playerPosition);


                if (distanceFromPlayer > bestDistance)
                {
                    bestDistance = distanceFromPlayer;
                    bestDirection = direction;
                }
            }

            return bestDirection;
        }

        public static Direction FindClosestDirectionToPlayer(Vector2 ghostPosition, Vector2 playerPosition,
            Direction forbiddenDirection, Level1Manager levelManager)
        {
            var bestDirection = Direction.NONE;
            var bestDistance = float.MaxValue;

            // iterate over all directions
            for (var i = 0; i < 4; i++)
            {
                var direction = (Direction)i;
                if (direction == forbiddenDirection || direction == Direction.NONE) continue;

                // get the vector for the direction
                var directionVector = GetDirectionVector2(direction);

                // get the target position
                var targetPosition = GetTargetPosition(ghostPosition, directionVector);

                if (!levelManager.IsTileWalkableForGhosts(targetPosition)) continue;

                // get the distance from the player
                var distanceFromPlayer = Vector2.Distance(targetPosition, playerPosition);


                if (distanceFromPlayer < bestDistance)
                {
                    bestDistance = distanceFromPlayer;
                    bestDirection = direction;
                }
            }

            return bestDirection;
        }

        public static Direction GetRandomDirection(Vector2 ghostPosition, Direction forbiddenDirection,
            Level1Manager levelManager)
        {
            var direction = (Direction)Random.Range(0, 4);
            while (
                direction == forbiddenDirection
                || direction == Direction.NONE
                || !levelManager.IsTileWalkableForGhosts(
                    GetTargetPosition(ghostPosition, GetDirectionVector2(direction))))
                direction = (Direction)Random.Range(0, 4);

            return direction;
        }

        public static Direction FindNextClockwisePosition(Vector2 position, Direction forwardDirection, Level1Manager levelManager)
        {
            // check left of ghost, then forward, then right, then backwards
            var leftDirection = GetLeftDirection(forwardDirection);
            var leftPosition = GetTargetPosition(position, GetDirectionVector2(leftDirection));
            if (levelManager.IsTileWalkableForGhosts(leftPosition))
                return leftDirection;
            
            var forwardPosition = GetTargetPosition(position, GetDirectionVector2(forwardDirection));
            if (levelManager.IsTileWalkableForGhosts(forwardPosition))
                return forwardDirection;
            
            var rightDirection = GetRightDirection(forwardDirection);
            var rightPosition = GetTargetPosition(position, GetDirectionVector2(rightDirection));
            if (levelManager.IsTileWalkableForGhosts(rightPosition))
                return rightDirection;
            
            var backwardsDirection = GetOppositeDirection(forwardDirection);
            var backwardsPosition = GetTargetPosition(position, GetDirectionVector2(backwardsDirection));
            if (levelManager.IsTileWalkableForGhosts(backwardsPosition))
                return backwardsDirection;
            
            return Direction.NONE;
        }
        
    }
}