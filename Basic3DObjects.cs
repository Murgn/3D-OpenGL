using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace My3DGame
{
    class Basic3DObjects
    {
        public class Obj3D
        {
            public int start_index;         // where it is in the index buffer
            public int triangle_count;      // how many triangles
            public Rectangle source_rect;   // section within texture to sample from
            public Texture2D tex;           // object texture
            public Vector3 rot;             // optional rotation
            public Vector3 pos;             // position
            public Matrix transform;        // world transform matrix (scale * rot * position_translation)

            // If position/rotation changes:
            public void UpdateTransform()
            {
                if (rot == Vector3.Zero) transform = Matrix.CreateTranslation(pos);
                else transform = Matrix.CreateFromYawPitchRoll(rot.Y, rot.X, rot.Z) * Matrix.CreateTranslation(pos);
            }

            public void UpdateTransformQuaternion()
            {
                if (rot == Vector3.Zero) transform = Matrix.CreateTranslation(pos);
                else
                {
                    Quaternion qR = Quaternion.CreateFromYawPitchRoll(rot.Y, rot.X, rot.Z);
                    transform = Matrix.CreateFromQuaternion(qR) * Matrix.CreateTranslation(pos);
                }
            }
        } // Obj3D

        // Geometry
        Matrix world;                                                   // for fixed geometry (like landscape mesh)
        public List<Obj3D> objex;                                       // list of 3D objects to render
        // static variables allow nested classes to access:
        static ContentManager Content;
        static IndexBuffer indexBuffer;                                 // holds and controls transfer of indices to hardware
        static VertexBuffer vertexBuffer;                               // holds and controls transfer of vertices to hardware
        static ushort[] indices;                                        // index list for index buffer
        static VertexPositionNormalTexture[] verts;                     // vertex list for assembling vertex buffer
        static int i_cnt, ibuf_start = 0;                               // for making texture-sort id's, index-buffer's current starting position (for adding new objects)
        static int v_cnt, vbuf_start = 0;                               // v_cnt is for making current object, total_verts keeps track of how many were already added (and can be used as starting index when adding new ones)
        static Dictionary<string, Texture2D> textures;                  // helps to store textures only once and get desired texture based on associated object's texture-file-name    

    }
}
