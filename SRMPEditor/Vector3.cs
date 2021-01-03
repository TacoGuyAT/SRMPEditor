using System;

namespace SRMPEditor
{
    public class Vector3
    {
        public float x;
        public float y;
        public float z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        internal static float Distance(Vector3 pos, object zero)
        {
            throw new NotImplementedException();
        }
    }
}