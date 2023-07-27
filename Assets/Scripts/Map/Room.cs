using System.Collections.Generic;

namespace CoolGameClub.Map
{
    public class Room
    {
        private bool _isDefeated;
        private bool _isSpawn;
        private bool _isBarroom;

        private List<Door> _doors;

        public Room(bool isSpawn = false, bool isBarroom = false) {
            _doors = new();
            _isSpawn = isSpawn;
            _isBarroom = isBarroom;
        }

        public void AddDoor(Door door) => _doors.Add(door);

        public void OnPlayerEnter() {
            if (_isDefeated) return;

            // TODO Lock room
            foreach (Door door in _doors) {
                door.Open(false);
            }

            // TODO Spawn enemies
        }

        public void OnDefeat() {
            _isDefeated = true;

            // TODO Unlock doors
            foreach (Door door in _doors) {
                door.Open(true);
            }

            // TODO Give upgrade if barroom
        }
    }
}
