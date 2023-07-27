using System.Collections.Generic;

namespace CoolGameClub.Map
{
    public class RoomTree
    {
        public RoomNode BaseRoomNode { get; private set; }

        public void AddBaseRoomNode(RoomNode roomNode) => BaseRoomNode = roomNode;

        public List<RoomNode> GetAllRoomNodes() => GetChildNodes(BaseRoomNode);

        public List<RoomNode> GetChildNodes(RoomNode node) {
            List<RoomNode> nodes = new();
            if (node.RoomLeft != null) foreach (RoomNode n in GetChildNodes(node.RoomLeft)) nodes.Add(n);
            if (node.RoomRight != null) foreach (RoomNode n in GetChildNodes(node.RoomRight)) nodes.Add(n);
            if (node.RoomUp != null) foreach (RoomNode n in GetChildNodes(node.RoomUp)) nodes.Add(n);
            if (node.RoomDown != null) foreach (RoomNode n in GetChildNodes(node.RoomDown)) nodes.Add(n);
            return nodes;
        }

        public class RoomNode
        {
            public RoomNode(Room room, RoomNode roomLeft = null, RoomNode roomRight = null, RoomNode roomUp = null, RoomNode roomDown = null) {
                Room = room;
                RoomLeft = roomLeft;
                RoomRight = roomRight;
                RoomUp = roomUp;
                RoomDown = roomDown;
            }

            public Room Room { private set; get; }

            public RoomNode RoomLeft { private set; get; }
            public RoomNode RoomRight { private set; get; }
            public RoomNode RoomUp { private set; get; }
            public RoomNode RoomDown { private set; get; }

            public void AddLeft(RoomNode roomNode) => RoomLeft = roomNode;
            public void AddRight(RoomNode roomNode) => RoomRight = roomNode;
            public void AddUp(RoomNode roomNode) => RoomUp = roomNode;
            public void AddDown(RoomNode roomNode) => RoomDown = roomNode;
        }
    }

}
